using Microsoft.Build.Framework;
using OperationManagement.Models;

namespace OperationManagement.Data.ViewModels
{
    public class ReviewJoinRequestVM
    {
        public ApplicationUser? Staff { get; set; }
        public Enterprise? Enterprise { get; set; }
        [Required]
        public string Messege { get; set; }
        [Required]
        public int EnterpriseId { get; set; }
        [Required]
        public string Action { get; set; }
    }
}
