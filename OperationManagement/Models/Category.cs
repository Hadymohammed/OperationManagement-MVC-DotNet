using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationManagement.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required,ForeignKey("EnterpriseId")]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public List<Product>? Products { get; set; }
    }
}
