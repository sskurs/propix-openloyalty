using System.Threading.Tasks;
using OpenLoyalty.Api.Models;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;


namespace OpenLoyalty.Api.Repository
{
    public interface IOutboxRepository
    {
        Task EnqueueAsync(OutboxMessage msg);
    }

    public class OutboxRepository : IOutboxRepository
    {
        private readonly LoyaltyDbContext _db;
        public OutboxRepository(LoyaltyDbContext db) => _db = db;

        public async Task EnqueueAsync(OutboxMessage msg)
        {
           // _db.Outbox.Add(msg);
            await _db.SaveChangesAsync();
        }
    }
}
