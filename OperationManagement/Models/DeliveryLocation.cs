using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using OperationManagement.Data.Base;

namespace OperationManagement.Models
{
    public class DeliveryLocation:IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string? GPS_URL { get; set; }
        [Required, ForeignKey("EnterpriseId")]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
