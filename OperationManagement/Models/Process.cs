using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OperationManagement.Data.Base;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class Process:IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, ForeignKey("EnterpriseId")]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
        [AllowNull,ForeignKey("CategoryId")]
        public int? CategoryId { get; set; }
        public ProcessCategory? Category { get; set; }
        public List<ProcessStatus>? Statuses { get; set; }
        public List<ProductProcess>? Products { get; set; }
    }
}
