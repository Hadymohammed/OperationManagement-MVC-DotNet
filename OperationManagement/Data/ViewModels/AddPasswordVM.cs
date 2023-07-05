using System.ComponentModel.DataAnnotations;

namespace OperationManagement.Data.ViewModels
{
    public class AddPasswordVM
    {
        public string? FirstName { get; set; }
        [Required]
        public string StaffId { get; set; }
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
