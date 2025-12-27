using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenLoyalty.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PosController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public PosController(LoyaltyDbContext context)
        {
            _context = context;
        }

        // POST: api/pos/transaction
        [HttpPost("transaction")]
        public async Task<IActionResult> SubmitTransaction([FromBody] JsonElement transactionPayload)
        {
            // We accept raw JSON to be flexible with the POS schema
            if (transactionPayload.ValueKind == JsonValueKind.Undefined)
            {
                return BadRequest("Invalid JSON payload");
            }

            // Extract Transaction ID for the Key (optional, but good practice)
            string key = Guid.NewGuid().ToString();
            if (transactionPayload.TryGetProperty("transactionId", out var txIdProp) || 
                transactionPayload.TryGetProperty("transaction_id", out txIdProp))
            {
                key = txIdProp.GetString() ?? key;
            }

            var outboxMessage = new OutboxMessage
            {
                Topic = "transaction.created",
                Key = key,
                Payload = JsonSerializer.Serialize(new { type = "transaction.created", payload = transactionPayload }),
                CreatedAt = DateTime.UtcNow,
                Status = "pending",
                Tries = 0
            };

            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Transaction received and queued", transactionId = key });
        }
    }
}
