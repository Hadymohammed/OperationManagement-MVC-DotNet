using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OperationManagement.Data.Base;

namespace OperationManagement.Models
{
    public class Specification:IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, ForeignKey("EnterpriseId")]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
        [Required, ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public SpecificationCategory? Category { get; set; }
        public List<SpecificationStatus>? Statuses { get; set; }
        public List<SpecificationOption>? Options { get; set; }
        public List<ProductSpecification>? Products { get; set; }

    }
}
