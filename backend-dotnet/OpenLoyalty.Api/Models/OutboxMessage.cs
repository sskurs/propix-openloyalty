using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoyalty.Api.Models
{
    [Table("outbox")]
    public class OutboxMessage
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Topic { get; set; } = "";
        public string? Key { get; set; }
        public string Payload { get; set; } = "";
        public string Status { get; set; } = "pending"; // pending, sending, sent, failed
        public int Tries { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
    }
}