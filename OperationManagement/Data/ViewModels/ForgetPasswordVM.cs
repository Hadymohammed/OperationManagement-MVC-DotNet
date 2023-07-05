using System.ComponentModel.DataAnnotations;

namespace OperationManagement.Data.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required,EmailAddress]
        public string Email { get; set; }
    }
}
