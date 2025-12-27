using System;

namespace Worker.Models
{
    public class TransactionEvent
    {
        public int Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string RawPayload { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
