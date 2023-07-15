using OperationManagement.Data.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class Enterprise:IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string? LogoURL { get; set; }
        [AllowNull,DefaultValue(false)]
        public bool? Accepted { get; set; }
        [AllowNull]
        public string? Description { get; set; }
        public List<ApplicationUser>? Staff { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Component>? Components { get; set; }
        public List<Measurement>? Measurements { get; set; }
        public List<Process>? Processes { get; set; }
        public List<ProcessCategory>? ProcessCategories { get; set; }
        public List<ComponentCategory>? ComponentCategories { get; set; }
        public List<Specification>? Specifications { get; set; }
        public List<Customer>? Customers { get; set; }
        public List<DeliveryLocation>? DeliveryLocations { get; set; }
    }
}
