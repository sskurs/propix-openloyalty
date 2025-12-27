using System.Collections.Generic;

namespace Worker.Models
{
    public class RuleAction
    {
        public string ActionType { get; set; } = string.Empty;
        public Dictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
    }
}
