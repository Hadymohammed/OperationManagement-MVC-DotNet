using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using OperationManagement.Data.Base;

namespace OperationManagement.Models
{
    public class Component:IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string Supplier { get; set; }
        [Required, ForeignKey("EnterpriseId")]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public List<ComponentPhoto>? Photos { get; set; }
        public List<ProductComponent>? Products { get;set; }
    }
}
