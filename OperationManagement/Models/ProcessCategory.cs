using Microsoft.Build.Framework;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class ProcessCategory
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string? NoramlizedName { get; set; }
        public List<Process>? Processes { get; set; }
    }
}
