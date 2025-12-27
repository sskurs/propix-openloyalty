using System;

namespace OpenLoyalty.Customer.Api.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public required string Topic { get; set; }
        public required string Payload { get; set; }
        public required string MessageType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
