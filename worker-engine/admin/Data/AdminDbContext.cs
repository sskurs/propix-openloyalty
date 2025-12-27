using Microsoft.EntityFrameworkCore;
namespace Admin.Data {
  public class AdminDbContext : DbContext {
    public AdminDbContext(DbContextOptions<AdminDbContext> opts): base(opts) {}
    public DbSet<Admin.Models.OutboxMessage> Outbox => Set<Admin.Models.OutboxMessage>();
  }
}

