using OperationManagement.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationManagement.Models
{
    public class CustomerContact
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public ContactType Type { get; set; }
        [Required]
        public string Value { get; set; }
        [Required,ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }    
    }
}
