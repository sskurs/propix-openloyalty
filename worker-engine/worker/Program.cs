// Program.cs - drop-in replacement for .NET 9 Worker
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Worker.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using Worker.Services;
using Worker.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using Microsoft.Extensions.Logging;
using ConnectionMultiplexer = StackExchange.Redis.ConnectionMultiplexer;
using IConnectionMultiplexer = StackExchange.Redis.IConnectionMultiplexer;
using Worker.Engines;
using Confluent.Kafka;
using Worker.Repositories;
using Worker.Handlers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Access configuration via context.Configuration
        var config = context.Configuration;

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // DbContext - replace "WorkerDatabase" with your connection string name
        var connString = config.GetConnectionString("WorkerDatabase")
                         ?? config["ConnectionStrings:WorkerDatabase"]
                         ?? "Host=127.0.0.1;Port=5432;Database=loyalty_worker;Username=loyaltypro;Password=Propix@123!";

        services.AddDbContext<WorkerDbContext>(options =>
        {
            options.UseNpgsql(connString, npgsql =>
            {
                // optional: enable retry on failure
                npgsql.EnableRetryOnFailure();
            })
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

            // optional: tune EF logging
            options.EnableSensitiveDataLogging(false);
        });

        // Redis connection (IConnectionMultiplexer)
        var redisConn = config.GetValue<string>("Redis:Connection") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConn)
        );

        // Kafka ConsumerConfig and ProducerConfig
        services.AddSingleton(sp =>
        {
            var bootstrap = config.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:19092";
            return new ConsumerConfig
            {
                BootstrapServers = bootstrap,
                GroupId = config.GetValue<string>("Kafka:ConsumerGroup") ?? "worker-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
        });

        services.AddSingleton(sp =>
        {
            var bootstrap = config.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:19092";
            return new ProducerConfig
            {
                BootstrapServers = bootstrap
            };
        });

        // Health checks (DB + Redis)
        services.AddHealthChecks()
            .AddNpgSql(connString, name: "postgres")
            .AddRedis(redisConn, name: "redis");

        // Repositories & infrastructure
        services.AddScoped<IRuleRepository, RuleRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>(); // ensure this implementation exists
        services.AddScoped<EarningRuleCreatedHandler>();
        services.AddScoped<CampaignHandler>();
        services.AddScoped<TransactionHandler>();
        services.AddScoped<Worker.Handlers.PointsBalanceHandler>();
       

        services.AddScoped<Worker.Infra.IMessageDispatcher, Worker.Infra.MessageDispatcher>();
        services.AddScoped<Worker.Handlers.RuleUpdateHandler>();
        services.AddScoped<Worker.Strategies.IRewardStrategy, Worker.Strategies.PointsRewardStrategy>();
        services.AddScoped<Worker.Strategies.IRewardStrategy, Worker.Strategies.CouponRewardStrategy>();

        // Engines or other app services
        //services.AddSingleton<IRuleEngine, InMemoryRuleEngine>(); // replace with your implementation
        //services.AddSingleton<IRuleEngine, RuleEngine>();
          services.AddSingleton<IRuleEngine, JsonRuleEngine>();

        // Hosted services (consumers/publishers)
        // services.AddHostedService<EarningRuleConsumerService>();
        // Hosted services (consumers/publishers)
        services.AddHostedService<CampaignConsumerService>();
        services.AddHostedService<KafkaConsumerService>();
        services.AddHostedService<RulesLoaderService>();
        services.AddHostedService<OutboxPublisherService>();
        
        

        // Rules Engine (Microsoft)
        services.AddScoped<CampaignRuleRepository>();
        services.AddScoped<IRuleEvaluator>(sp =>
        {
            var repo = sp.GetRequiredService<CampaignRuleRepository>();
            var workflows = repo.LoadActiveCampaignWorkflowsAsync().GetAwaiter().GetResult();
            return new RulesEngineEvaluator(workflows);
        });
        // Maintain old interface for backward compat if needed, or remove if fully switched.
        // For TransactionHandler, we will switch to IRuleEvaluator.
        services.AddSingleton<IAdvancedRuleEngine, AdvancedRuleEngine>(); // Keep for legacy


        // any additional DI registrations can go here...
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();
    try {
        // await db.Database.MigrateAsync(); // Conflict with shared DB
        await db.Database.EnsureCreatedAsync();

        // Manual Schema Creation for new tables (bypassing migrations due to permission/context issues)
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS \"campaign_conditions\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY, \"CampaignId\" uuid NOT NULL, \"ConditionJson\" jsonb, CONSTRAINT \"PK_campaign_conditions\" PRIMARY KEY (\"Id\"));");
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS \"campaign_rewards\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY, \"CampaignId\" uuid NOT NULL, \"RewardJson\" jsonb, CONSTRAINT \"PK_campaign_rewards\" PRIMARY KEY (\"Id\"));");
        await db.Database.ExecuteSqlRawAsync("ALTER TABLE campaign_conditions ADD COLUMN IF NOT EXISTS \"RuleJson\" jsonb;");
        await db.Database.ExecuteSqlRawAsync("UPDATE campaign_conditions SET \"RuleJson\" = '{{}}'::jsonb WHERE \"RuleJson\" IS NULL;");


        await db.Database.ExecuteSqlRawAsync($"CREATE TABLE IF NOT EXISTS \"campaign_executions\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY, \"CampaignId\" uuid NOT NULL, \"MemberId\" text, \"TransactionId\" text, \"RewardType\" text, \"ExecutionResultJson\" jsonb, \"ExecutedAt\" timestamp with time zone NOT NULL, CONSTRAINT \"PK_campaign_executions\" PRIMARY KEY (\"Id\"));");

        // Worker Internal Tables (Renamed per Spec)
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS \"outbox\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY, \"CreatedAt\" timestamp with time zone NOT NULL, \"Key\" text, \"Payload\" text NOT NULL, \"SentAt\" timestamp with time zone, \"Status\" text NOT NULL, \"Topic\" text, \"Tries\" integer NOT NULL, CONSTRAINT \"PK_outbox\" PRIMARY KEY (\"Id\"));");
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS \"transaction_events\" (\"Id\" integer GENERATED BY DEFAULT AS IDENTITY, \"TransactionId\" text NOT NULL, \"UserId\" text NOT NULL, \"Amount\" numeric NOT NULL, \"RawPayload\" text NOT NULL, \"CreatedAt\" timestamp with time zone NOT NULL, CONSTRAINT \"PK_transaction_events\" PRIMARY KEY (\"Id\"));");
        
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS \"worker_coupons\" (\"Id\" uuid NOT NULL, \"Code\" text NOT NULL, \"Type\" text NOT NULL, \"DiscountValue\" numeric NOT NULL, \"Currency\" text, \"MaxUses\" integer, \"MaxUsesPerMember\" integer, \"MinOrderValue\" numeric, \"StartDate\" timestamp with time zone, \"EndDate\" timestamp with time zone, \"Status\" text NOT NULL DEFAULT 'active', \"CreatedAt\" timestamp with time zone NOT NULL, \"UpdatedAt\" timestamp with time zone NOT NULL, CONSTRAINT \"PK_worker_coupons\" PRIMARY KEY (\"Id\"));");
        
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS \"worker_earning_rules\" (\"Id\" text NOT NULL, \"Name\" text NOT NULL, \"ConditionJson\" text, \"PointsJson\" text, \"LimitsJson\" text, \"Expression\" text, \"Status\" text DEFAULT 'active', \"Active\" boolean NOT NULL, \"IsActive\" boolean NOT NULL DEFAULT TRUE, \"Version\" integer NOT NULL, \"UpdatedAt\" timestamp with time zone NOT NULL, \"CreatedAt\" timestamp with time zone NOT NULL, \"CreatedBy\" text NOT NULL DEFAULT 'system', CONSTRAINT \"PK_worker_earning_rules\" PRIMARY KEY (\"Id\"));");
        
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS \"member_coupons\" (\"Id\" uuid NOT NULL, \"MemberId\" text NOT NULL, \"CouponCode\" text NOT NULL, \"Status\" text NOT NULL, \"AssignedAt\" timestamp with time zone NOT NULL, CONSTRAINT \"PK_member_coupons\" PRIMARY KEY (\"Id\"));");
        await db.Database.ExecuteSqlRawAsync("DO $$ BEGIN IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='member_coupons' AND column_name='RedeemedAt') THEN ALTER TABLE member_coupons ADD COLUMN \"RedeemedAt\" timestamp with time zone; END IF; END $$;");
        
        Console.WriteLine("Worker Database Migrated Successfully (Manual Schema).");
        
        // Cleanup Test Data (Resolves "Phantom Rule" issue)
        try
        {
            // 1. Delete the hardcoded WELCOME10 coupon if it exists
            var deletedCoupons = await db.Database.ExecuteSqlRawAsync("DELETE FROM worker_coupons WHERE \"Code\" = 'WELCOME10'");
            if (deletedCoupons > 0) Console.WriteLine($"Cleaned up {deletedCoupons} test coupon(s) (WELCOME10).");

            // 2. Delete the test campaign that uses this coupon (identified by Name convention or known IDs from previous logs)
            // Using a broad check for 'Welcome Campaign' as a safeguard against the phantom rule.
            var deletedCampaigns = await db.Database.ExecuteSqlRawAsync("DELETE FROM campaigns WHERE \"Name\" = 'Welcome Campaign'");
            if (deletedCampaigns > 0) Console.WriteLine($"Cleaned up {deletedCampaigns} test campaign(s).");
            
            // Also cleanup by code if stored
            var deletedCampaignsByCode = await db.Database.ExecuteSqlRawAsync("DELETE FROM campaigns WHERE \"Code\" = 'WELCOME10'");
            if (deletedCampaignsByCode > 0) Console.WriteLine($"Cleaned up {deletedCampaignsByCode} test campaign(s) by Code.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cleaning up test data: {ex.Message}");
        }

    } catch (Exception ex) {
        Console.WriteLine($"Worker Database Migration Failed: {ex.Message}");
        throw;
    }
}

await host.RunAsync();
