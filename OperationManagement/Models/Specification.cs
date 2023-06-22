using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OperationManagement.Models
{
    public class Specification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, ForeignKey("EnterpriseId")]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public List<SpecificationStatus>? Statuses { get; set; }
        public List<SpecificationOption>? Options { get; set; }
        public List<ProductSpecification>? Products { get; set; }

    }
}
