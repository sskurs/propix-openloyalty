using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((ctx, lc) => lc.WriteTo.Console())
    .ConfigureAppConfiguration(cfg =>
    {
        cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((ctx, services) =>
    {
        var config = ctx.Configuration;
        services.AddDbContext<WorkerDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("Postgres")));

        services.AddSingleton<IEventBus, KafkaEventBus>();
        services.AddSingleton<ISnapshotProvider, RedisSnapshotProvider>();
        services.AddScoped<IRuleEngine, JsonRuleEngine>();
        services.AddScoped<IPointsService, PointsService>();
        services.AddHostedService<LoyaltyEventWorker>();

        var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();
        db.Database.EnsureCreated();
    })
    .Build();

await host.RunAsync();
