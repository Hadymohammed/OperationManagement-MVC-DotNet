using OperationManagement.Data.Base;
using OperationManagement.Data.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class ProductComponent: IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required,DefaultValue(0), Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
        [Required]
        public MeasurementUnit Unit { get; set; }
        [Required, ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required,ForeignKey("ComponentId")]
        public int ComponentId { get; set; }
        public Component? Component { get; set; }
    }
}
