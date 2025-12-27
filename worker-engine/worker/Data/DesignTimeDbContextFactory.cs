using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Worker.Data;

namespace Worker.Data
{
    // EF will look for IDesignTimeDbContextFactory implementations at design time.
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WorkerDbContext>
    {
        public WorkerDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<WorkerDbContext>();

            // Replace with your local connection string for design-time migrations.
            // Use host.docker.internal when running from inside containers; locally use localhost.
            var conn = "Host=localhost;Port=5432;Database=loyalty_worker;Username=postgres;Password=Propix@123";

            builder.UseNpgsql(conn, npgsqlOptions => {
                // optional: configure migrations assembly if needed
                // npgsqlOptions.MigrationsAssembly(typeof(WorkerDbContext).Assembly.FullName);
            });

            return new WorkerDbContext(builder.Options);
        }
    }
}
