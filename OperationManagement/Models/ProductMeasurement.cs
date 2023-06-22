using OperationManagement.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationManagement.Models
{
    public class ProductMeasurement
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public float Value { get; set; }
        [Required]
        public MeasurementUnit Unit { get; set; }
        [Required, ForeignKey("MeasurementId")]
        public int MeasurementId { get; set; }
        public Measurement? Measurement { get; set; }
        [Required, ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
