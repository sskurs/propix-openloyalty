using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Customer.Api.Data;
using Confluent.Kafka;
using System.Text.Json.Serialization;

namespace OpenLoyalty.Customer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<LoyaltyDbContext>(options =>
            {
                var conn = builder.Configuration.GetConnectionString("OpenloyaltyDatabase");
                options.UseNpgsql(conn)
                       .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

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

            builder.Services.AddOpenApi();

            // Kafka producer config for events
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
            };
            builder.Services.AddSingleton(producerConfig);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseCors("AllowAll");


            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                // Automatically apply migrations in development
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<LoyaltyDbContext>();
                    // Note: Ensure migrations exist or use EnsureCreated() if not using EF migrations for this project
                    db.Database.EnsureCreated(); 
                    
                    // Manual Schema Updates for robustness
                    db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS member_coupons (" +
                        "\"Id\" uuid NOT NULL, " +
                        "\"MemberId\" text NOT NULL, " +
                        "\"CouponCode\" text NOT NULL, " +
                        "\"Status\" text NOT NULL, " +
                        "\"AssignedAt\" timestamp with time zone NOT NULL, " +
                        "\"RedeemedAt\" timestamp with time zone, " +
                        "CONSTRAINT \"PK_member_coupons\" PRIMARY KEY (\"Id\"));");

                    db.Database.ExecuteSqlRaw("ALTER TABLE campaigns ADD COLUMN IF NOT EXISTS \"ConditionsJson\" text;");
                    db.Database.ExecuteSqlRaw("ALTER TABLE campaigns ADD COLUMN IF NOT EXISTS \"RewardsJson\" text;");
                }
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
