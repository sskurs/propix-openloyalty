using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worker.Models
{
    [Table("campaign_conditions")]
    public class CampaignConditionEntity
    {
        [Key]
        public long Id { get; set; }

        public Guid CampaignId { get; set; }

        [Column(TypeName = "jsonb")] // PostgreSql JSONB
        public string ConditionJson { get; set; } = "{}";

        [Column(TypeName = "jsonb")]
        public string RuleJson { get; set; } = "{}";

        [Column("RuleGroupJson", TypeName = "jsonb")]
        public string? RuleGroupJson { get; set; } // MS RulesEngine JSON
    }
}
