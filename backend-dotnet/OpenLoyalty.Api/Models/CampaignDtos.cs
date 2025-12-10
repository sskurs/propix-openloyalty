using System;

namespace OpenLoyalty.Api.Models
{
    public class CreateCampaignDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }

    public class CampaignListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int EnrollmentCount { get; set; } // Placeholder
    }
}
