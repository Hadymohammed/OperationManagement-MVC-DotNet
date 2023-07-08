using Microsoft.Build.Framework;
using OperationManagement.Models;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Data.ViewModels
{
    public class ReviewJoinRequestVM
    {
        public ApplicationUser? Staff { get; set; }
        public Enterprise? Enterprise { get; set; }
        [AllowNull]
        public string? Messege { get; set; }
        [Required]
        public int EnterpriseId { get; set; }
        [Required]
        public string Action { get; set; }
    }
}
