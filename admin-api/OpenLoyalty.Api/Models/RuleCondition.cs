namespace OpenLoyalty.Api.Models
{
    /// <summary>
    /// Represents a single rule condition (field-operator-value).
    /// </summary>
    public class RuleCondition
    {
        /// <summary>
        /// Field name to evaluate (e.g., "min_order_amount", "CustomerTier")
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Comparison operator (e.g., "gt", "eq", "contains", "in")
        /// </summary>
        public string Operator { get; set; } = string.Empty;

        /// <summary>
        /// Value to compare against (can be string, number, array, etc.)
        /// </summary>
        public object? Value { get; set; }
    }
}
