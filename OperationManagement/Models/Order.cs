using OperationManagement.Data.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class Order:IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required,DisplayName("Enterprise Order Number")]
        public string EnterpriseOrderNumber { get; set; }
        [AllowNull]
        public string? Address { get; set; }
        [AllowNull]
        public string? GPS_URL { get; set; }
        [Required, DataType(DataType.Date),DisplayName("Contract Date")]
        public DateTime ContractDate { get; set; }
        [AllowNull, DataType(DataType.Date), DisplayName("Planned Start Date")]
        public DateTime? PlannedStartDate { get; set; }
        [AllowNull, DataType(DataType.Date), DisplayName("Planned End Date")]
        public DateTime? PlannedEndDate { get; set; }
        [AllowNull, DataType(DataType.Date), DisplayName("Actual Start Date")]
        public DateTime? ActualStartDate { get; set; }
        [AllowNull, DataType(DataType.Date), DisplayName("Actual End Date")]
        public DateTime? ActualEndDate { get; set; }
        [AllowNull, DataType(DataType.Date), DisplayName("Hand Over Date")]
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
