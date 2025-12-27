using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Worker.Models

{
    [Table("outbox")]
    public class OutboxMessage
    {
        [Key]
        public long Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [MaxLength(255)]
        public string? Key { get; set; }

        // store JSON payload
        public string Payload { get; set; } = "{}";

        public DateTimeOffset? SentAt { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "pending";

        [MaxLength(200)]
        public string? Topic { get; set; }

        public int Tries { get; set; } = 0;
    }
}
