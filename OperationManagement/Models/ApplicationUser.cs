using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using OperationManagement.Data.Base;

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
        [AllowNull, DefaultValue(false)]
        public bool? Registered { get; set; }
        [AllowNull, ForeignKey("EnterpriseId")]
        public int? EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public List<Token>? Tokens { get; set; }
    }
}
