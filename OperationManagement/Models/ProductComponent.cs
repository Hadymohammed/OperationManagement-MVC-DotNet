using OperationManagement.Data.Base;
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
        [AllowNull,DefaultValue(0)]
        public int Quantity { get; set; }
        [Required, ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required,ForeignKey("ComponentId")]
        public int ComponentId { get; set; }
        public Component? Component { get; set; }
    }
}
