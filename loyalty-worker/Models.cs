using System;
using System.ComponentModel.DataAnnotations;

public class User {
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public decimal TotalSpent { get; set; } = 0m;
    public int OrdersCount { get; set; } = 0;
    public DateTime? LastPurchaseAt { get; set; }
    public int Balance { get; set; } = 0;
}

public class Transaction {
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public decimal GrossValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class EarningRule {
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string ConditionJson { get; set; } = "";
    public string PointsJson { get; set; } = "";
    public bool Active { get; set; } = true;
}

public class EarningRuleUsage {
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RuleId { get; set; } = "";
    public string UserId { get; set; } = "";
    public int Points { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
