using System.ComponentModel.DataAnnotations;

namespace OperationManagement.Data.ViewModels
{
    public class OTPVM
    {
        public string? Email { get; set; }
        [Required,MaxLength(6),MinLength(6)]
        public string OTP { get; set; }
    }
}
