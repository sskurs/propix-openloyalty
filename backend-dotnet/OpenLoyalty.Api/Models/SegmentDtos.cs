using System;
using System.Collections.Generic;

namespace OpenLoyalty.Api.Models
{
    // Used when creating a new segment from the frontend.
    public class CreateSegmentDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } // "DYNAMIC" or "STATIC"

        // The rule builder's state will be sent as a JSON object.
        public object Conditions { get; set; }
    }

    // Used for displaying the list of segments in the admin UI.
    public class SegmentListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Members { get; set; }
        public decimal AvgTransactionValue { get; set; }
        public double AvgNumTransactions { get; set; }
        public decimal AvgSpending { get; set; }
        public string CreatedOn { get; set; }
        public string Status { get; set; }
    }
}
