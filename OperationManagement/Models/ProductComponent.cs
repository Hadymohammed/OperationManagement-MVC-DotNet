using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationManagement.Models
{
    public class ProductComponent
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required, ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required,ForeignKey("ComponentId")]
        public int ComponentId { get; set; }
        public Component? Component { get; set; }
    }
}
