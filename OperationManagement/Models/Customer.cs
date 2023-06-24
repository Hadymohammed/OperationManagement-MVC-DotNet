using OperationManagement.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required, RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$"), Display(Name = "Full name")]
        public string Name { get; set; }
        [AllowNull,EmailAddress]
        public string Email { get; set; }
        [AllowNull]
        public string? NationalId { get; set; }
        [AllowNull]
        public Nationality? Nationality { get; set; }
        [AllowNull]
        public Gender? Gender { get; set; }
        [AllowNull,DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        [Required]
        public int EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public List<CustomerContact>? Contacts { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
