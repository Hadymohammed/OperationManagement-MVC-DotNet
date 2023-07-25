using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationManagement.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string token { get; set; }
        [Required,ForeignKey("userId")]
        public string userId { get; set; }
        public ApplicationUser? user { get; set; }
    }
}
