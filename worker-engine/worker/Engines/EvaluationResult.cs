using System.Collections.Generic;

namespace Worker.Engines
{
    public class EvaluationResult
    {
        public bool Matched { get; set; }
        public List<RuleActionResult> Actions { get; } = new();
    }

    public class RuleActionResult
    {
        public string ActionType { get; set; } = string.Empty;
        public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
    }
}
