using System;
using System.ComponentModel.DataAnnotations;

namespace OpenLoyalty.Api.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
