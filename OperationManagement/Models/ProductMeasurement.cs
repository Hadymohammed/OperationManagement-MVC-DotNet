using OperationManagement.Data.Base;
using OperationManagement.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class ProductMeasurement: IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [AllowNull]
        public float? Value { get; set; }
        [AllowNull]
        public MeasurementUnit? Unit { get; set; }
        [Required, ForeignKey("MeasurementId")]
        public int MeasurementId { get; set; }
        public Measurement? Measurement { get; set; }
        [Required, ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
