using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string EnterpriseOrderNumber { get; set; }
        [AllowNull]
        public string? Address { get; set; }
        [AllowNull]
        public string? GPS_URL { get; set; }
        [Required]
        public DateTime ContractDate { get; set; }
        [AllowNull]
        public DateTime? PlannedStartDate { get; set; }
        [AllowNull]
        public DateTime? PlannedEndDate { get; set; }
        [AllowNull]
        public DateTime? ActualStartDate { get; set; }
        [AllowNull]
        public DateTime? ActualEndDate { get; set; }
        [AllowNull]
        public DateTime? HandOverDate { get; set; }
        [AllowNull,DefaultValue(false)]
        public bool? IsHandOver { get; set; }
        [Required,ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        [Required]
        public int DeliveryLocationId { get; set; }
        public DeliveryLocation? DeliveryLocation { get; set; }
        public List<Product>? Products { get; set; }
        public List<Attachment>? Attachments { get; set; }
    }
}
