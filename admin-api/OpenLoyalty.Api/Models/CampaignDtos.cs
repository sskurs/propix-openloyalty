using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenLoyalty.Api.Models
{
    public class CreateCampaignDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; } // e.g. bonus-points, points-multiplier, cashback

        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int Priority { get; set; } = 0;
        public bool IsStackable { get; set; } = true;
        public int? MaxTotalRewards { get; set; }
        public int? MaxPerCustomer { get; set; }
        public byte[]? RowVersion { get; set; }

        public List<CampaignConditionDto>? Conditions { get; set; }
        
        /// <summary>
        /// Complex nested rule structure. If provided, this takes precedence over simple Conditions list.
        /// Supports dual-purpose format: { editorState, rulesEngine }
        /// </summary>
        public object? RuleGroup { get; set; }
        public object? RuleEditorState { get; set; }
        
        public List<CampaignRewardDto>? Rewards { get; set; }
    }

    public class CampaignConditionDto
    {
        public string? Type { get; set; }
        public string? Operator { get; set; }
        public object? Value { get; set; } 
        public object? RuleEditorState { get; set; }
    }

    public class CampaignRewardDto
    {
        public string? Type { get; set; }
        public object? Value { get; set; }
    }

    public class CampaignListItemDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Status { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int EnrollmentCount { get; set; } // Placeholder
    }
}
