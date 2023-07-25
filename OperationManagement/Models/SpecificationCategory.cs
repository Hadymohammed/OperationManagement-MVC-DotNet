using Microsoft.Build.Framework;
using OperationManagement.Data.Base;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class SpecificationCategory:IEntityBase
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string? NoramlizedName { get; set; }
        public List<Specification>? Specifications { get; set; }
        [Required]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
    }
}
