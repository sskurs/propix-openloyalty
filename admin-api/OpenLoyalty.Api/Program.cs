using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Services;
using System.Text.Json.Serialization;
using Quartz;
using OpenLoyalty.Api.Jobs;
using OpenLoyalty.Api.Infrastructure;
using OpenLoyalty.Api.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// CORRECTED: Use Npgsql for PostgreSQL connection
builder.Services.AddDbContext<LoyaltyDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("OpenloyaltyDatabase");
    options.UseNpgsql(conn)
           .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

builder.Services.AddScoped<PointsEngine>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddQuartz(q =>
{
    // Use DI for job factory so job constructor dependencies are injected


    // Register the job
    var jobKey = new JobKey("rule-scheduler-job");
    q.AddJob<RuleSchedulerJob>(opts => opts.WithIdentity(jobKey));

    // Trigger: run every 1 minute (simple schedule)
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("rule-scheduler-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(1)
            .RepeatForever()
        )
    );
});

// Register Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Add Confluent.Kafka producer and consumer config
var producerConfig = new ProducerConfig
{
    BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
};
builder.Services.AddSingleton(producerConfig);

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
    GroupId = "api-activity-log-group",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false
};
builder.Services.AddSingleton(consumerConfig);

// Outbox repository registration
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

// Register the background services
builder.Services.AddHostedService<OpenLoyalty.Api.Services.OutboxPublisherService>();
builder.Services.AddHostedService<OpenLoyalty.Api.Services.ActivityLogConsumerService>();
builder.Services.AddHostedService<OpenLoyalty.Api.Services.CouponSyncConsumerService>();
builder.Services.AddHostedService<OpenLoyalty.Api.Services.PointsSyncConsumerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Apply CORS policy first
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    // Automatically apply migrations in development
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<LoyaltyDbContext>();
        db.Database.Migrate();

        // Seed some initial rewards if empty
        if (!db.Rewards.Any())
        {
            var tierId = new Guid("11111111-1111-1111-1111-111111111111"); // Bronze
            db.Rewards.AddRange(
                new Reward { Id = Guid.NewGuid(), Code = "REW-01", Name = "Premium Coffee Voucher", CostPoints = 50, Type = "vouchers", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, MinTierId = tierId },
                new Reward { Id = Guid.NewGuid(), Code = "REW-02", Name = "Airport Lounge Access", CostPoints = 500, Type = "experiences", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, MinTierId = tierId },
                new Reward { Id = Guid.NewGuid(), Code = "REW-03", Name = "10% Shopping Discount", CostPoints = 100, Type = "discounts", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, MinTierId = tierId }
            );
        }

        if (!db.ActivityLogs.Any())
        {
            db.ActivityLogs.AddRange(
                new ActivityLog { Id = Guid.NewGuid(), Type = "System", Description = "Loyalty system initialized and online.", CreatedAt = DateTime.UtcNow },
                new ActivityLog { Id = Guid.NewGuid(), Type = "Campaign", Description = "Welcome rewards campaign activated.", CreatedAt = DateTime.UtcNow.AddMinutes(-5) }
            );
        }
        
        db.SaveChanges();

        // Seed or Update Earning Rules
        var baseRule = db.EarningRules.FirstOrDefault(r => r.Name == "Base Earn Rule");
        if (baseRule == null)
        {
            db.EarningRules.Add(new EarningRule
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Base Earn Rule",
                Description = "Earn 10 points for every transaction",
                Category = "Standard",
                EventKey = "transaction.created",
                Status = "DRAFT", // Trigger sync
                Type = "earning",
                Priority = 1,
                Version = 1,
                ActivateAt = DateTime.UtcNow.AddMinutes(-10),
                ConditionJson = "[{\"Field\":\"amount\",\"Operator\":\"gt\",\"Value\":0}]",
                PointsJson = "[{\"Type\":\"addPoints\",\"Parameters\":{\"points\":10}}]",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            // Force update to DRAFT to trigger scheduler if needed
            baseRule.Status = "DRAFT";
            baseRule.ActivateAt = DateTime.UtcNow.AddMinutes(-10);
            baseRule.ConditionJson = "[{\"Field\":\"amount\",\"Operator\":\"gt\",\"Value\":0}]";
            baseRule.PointsJson = "[{\"Type\":\"addPoints\",\"Parameters\":{\"points\":10}}]";
            db.EarningRules.Update(baseRule);
        }
        db.SaveChanges();
        
        // Manual Fix for MemberCoupons Schema (String IDs vs Guids)
        try 
        {
             // Drop incorrect table if exists (from old migration)
             db.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS \"member_coupon\"");
             
             // Ensure correct table exists
             db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""member_coupons"" (
                    ""Id"" uuid NOT NULL,
                    ""MemberId"" text NOT NULL,
                    ""CouponCode"" text NOT NULL,
                    ""Status"" text NOT NULL DEFAULT 'active',
                    ""AssignedAt"" timestamp with time zone NOT NULL,
                    ""RedeemedAt"" timestamp with time zone,
                    CONSTRAINT ""PK_member_coupons"" PRIMARY KEY (""Id"")
                );
             ");
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"Error fixing MemberCoupons schema: {ex.Message}");
        }

        // Manual Fix for CampaignExecutions Schema (Sync with Worker)
        try
        {
             db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""campaign_executions"" (
                    ""Id"" uuid NOT NULL,
                    ""RuleId"" text NOT NULL,
                    ""TransactionId"" text,
                    ""MemberId"" uuid NOT NULL,
                    ""ExecutedAt"" timestamp with time zone NOT NULL,
                    ""RewardType"" text,
                    ""RewardValue"" numeric,
                    CONSTRAINT ""PK_campaign_executions"" PRIMARY KEY (""Id"")
                );
             ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fixing CampaignExecutions schema: {ex.Message}");
        }

        // Manual Fix for Seed User ExternalId
        try
        {
            db.Database.ExecuteSqlRaw("UPDATE member SET \"ExternalId\" = 'CUST-1001' WHERE \"Email\" = 'user@example.com' AND \"ExternalId\" IS NULL");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating seed user ExternalId: {ex.Message}");
        }
    }
}

app.UseAuthorization();

app.MapControllers();

app.Run();
