using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OperationManagement.Data.Base;

namespace OperationManagement.Models
{
    public class SpecificationOption: IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, ForeignKey("SpecificationId")]
        public int SpecificationId { get; set; }
        public Specification? Specification { get; set; }
        public List<ProductSpecification>? Products { get; set; }

    }
}
