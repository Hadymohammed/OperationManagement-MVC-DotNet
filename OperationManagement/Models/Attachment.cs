using OperationManagement.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace OperationManagement.Models
{
    public class Attachment:IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string FileURL { get; set; }
        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
