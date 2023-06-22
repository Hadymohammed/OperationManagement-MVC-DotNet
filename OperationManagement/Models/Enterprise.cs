using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class Enterprise
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string? LogoURL { get; set; }
        public List<Staff>? Staff { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Component>? Components { get; set; }
        public List<Measurement>? Measurements { get; set; }
        public List<Process>? Processes { get; set; }
        public List<Specification>? Specifications { get; set; }
        public List<Customer>? Customers { get; set; }
        public List<DeliveryLocation>? DeliveryLocations { get; set; }
    }
}
