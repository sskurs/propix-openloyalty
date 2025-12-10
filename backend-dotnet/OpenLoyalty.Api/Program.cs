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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = "Server=localhost;Port=3306;Database=openloyalty;User=root;Password=samsung";
//builder.Services.AddDbContext<LoyaltyDbContext>(options =>
  //  options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDbContext<LoyaltyDbContext>(options =>
{
    var conn =  builder.Configuration.GetConnectionString("OpenloyaltyDatabase");
    options.UseMySql(conn, ServerVersion.AutoDetect(conn));
});
builder.Services.AddScoped<PointsEngine>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
    q.UseMicrosoftDependencyInjectionJobFactory();

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

// Add Confluent.Kafka producer config
var producerConfig = new ProducerConfig
{
    BootstrapServers = builder.Configuration["Kafka:BootstrapServers"], // Corrected: Used builder.Configuration
};
builder.Services.AddSingleton(producerConfig);

// Outbox repository registration
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

// Register the background service
builder.Services.AddHostedService<OpenLoyalty.Api.Services.OutboxPublisherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Apply CORS policy first
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
