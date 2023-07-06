using OperationManagement.Data.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class Product:IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required,DisplayName("Emterprise Code")]
        public string EnterpriseCode { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string? Description { get; set; }
        [Required]
        public float Price { get; set; }
        [AllowNull,DefaultValue(1)]
        public int Quantity { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [AllowNull,DefaultValue(0)]
        public int? Progress { get; set; }
        [AllowNull,DefaultValue(false)]
        public bool IsCompleted { get; set; }
        public Category? Category { get; set; }
        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public List<ProductComponent>? Components { get; set; }
        public List<ProductMeasurement>? Measurements { get; set; }
        public List<ProductProcess>? Processes { get; set; }
        public List<ProductSpecification>? Specifications { get; set; }
    }
}
