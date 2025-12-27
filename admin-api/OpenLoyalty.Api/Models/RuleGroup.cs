using System.Collections.Generic;

namespace OpenLoyalty.Api.Models
{
    /// <summary>
    /// Represents a group of rule conditions with a combinator (AND/OR logic).
    /// Supports nested groups for complex rule structures.
    /// </summary>
    public class RuleGroup
    {
        /// <summary>
        /// Logical combinator for this group: "AND" or "OR"
        /// </summary>
        public string Combinator { get; set; } = "AND";

        /// <summary>
        /// List of individual conditions in this group
        /// </summary>
        public List<RuleCondition> Conditions { get; set; } = new List<RuleCondition>();

        /// <summary>
        /// Nested rule groups for complex logic
        /// </summary>
        public List<RuleGroup> Groups { get; set; } = new List<RuleGroup>();
    }
}
