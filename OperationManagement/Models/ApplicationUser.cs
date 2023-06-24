using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [AllowNull]
        public string? ProfilePictureURL { get; set; }
        [AllowNull, ForeignKey("EnterpriseId")]
        public int? EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
    }
}
