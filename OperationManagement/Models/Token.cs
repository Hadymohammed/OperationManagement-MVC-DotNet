using System.ComponentModel.DataAnnotations;

namespace OperationManagement.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string token { get; set; }
    }
}
