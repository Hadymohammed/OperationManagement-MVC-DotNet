using Microsoft.Build.Framework;
using OperationManagement.Data.Base;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class ProcessCategory:IEntityBase
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string? NoramlizedName { get; set; }
        public List<Process>? Processes { get; set; }
        [Required]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
    }
}
