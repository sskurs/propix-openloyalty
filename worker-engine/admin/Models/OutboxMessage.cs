using System;
namespace Admin.Models {
  public class OutboxMessage {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Topic { get; set; } = "";
    public string Payload { get; set; } = "";
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}

