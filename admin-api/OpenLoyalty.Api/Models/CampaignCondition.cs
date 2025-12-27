using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoyalty.Api.Models
{
    [Table("campaign_conditions")]
    public class CampaignCondition
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CampaignId { get; set; }
        public required string ConditionType { get; set; }
        public required string Operator { get; set; }
        public required string Value { get; set; } // Store as JSON string

        /// <summary>
        /// JSON representation of complex nested rule structure (RuleGroup).
        /// If null, fall back to simple ConditionType/Operator/Value format.
        /// </summary>
        [Column(TypeName = "jsonb")]
        public string? RuleGroupJson { get; set; }

        /// <summary>
        /// JSON representation of the UI editor state, used to re-populate the Rule Builder.
        /// </summary>
        [Column(TypeName = "jsonb")]
        public string? RuleEditorStateJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CampaignId")]
        public Campaign? Campaign { get; set; }
    }
}
