using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OperationManagement.Data.ViewModels
{
    public class JoinRequestVM
    {
        [Required,DisplayName("Enterprise Name")]
        public string EnterpriseName { get; set; }
        [Required]
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Must be a name")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Must be a name")]
        public string LastName { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
