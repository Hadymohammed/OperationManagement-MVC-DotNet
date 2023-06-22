using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class Staff
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required,Phone]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        [AllowNull]
        public string? ProfilePictureURL { get; set; }
        [Required,ForeignKey("EnterpriseId")]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
    }
}
