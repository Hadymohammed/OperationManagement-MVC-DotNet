using OperationManagement.Data.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class ProductSpecification: IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [AllowNull, DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        [AllowNull]
        public string? Remark { get; set; }
        [Required,ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required,ForeignKey("SpecificationId")]
        public int SpecificationId { get; set; }
        public Specification? Specification { get; set; }
        [Required, ForeignKey("OptionId")]
        public int OptionId { get; set; }
        public SpecificationOption? Option { get; set; }
        [Required, ForeignKey("StatusId")]
        public int StatusId { get; set; }
        public SpecificationStatus? Status { get; set; }




    }
}
