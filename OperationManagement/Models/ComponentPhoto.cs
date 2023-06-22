using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationManagement.Models
{
    public class ComponentPhoto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string PhotoURL { get; set; }
        [Required,ForeignKey("ComponentId")]
        public int ComponentId { get; set; }
        public Component? Component { get; set; }

    }
}
