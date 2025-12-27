using System.Text.Json;

namespace Worker.Models
{
    /// <summary>
    /// Represents a group of rule conditions with AND/OR logic.
    /// Matches the frontend RuleGroup structure.
    /// </summary>
    public class RuleGroupModel
    {
        public string Combinator { get; set; } = "AND"; // "AND" or "OR"
        public List<RuleConditionModel> Conditions { get; set; } = new();
        public List<RuleGroupModel> Groups { get; set; } = new();
    }

    /// <summary>
    /// Represents a single rule condition (field-operator-value).
    /// Matches the frontend RuleCondition structure.
    /// </summary>
    public class RuleConditionModel
    {
        public string Field { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public object? Value { get; set; }
    }
}
