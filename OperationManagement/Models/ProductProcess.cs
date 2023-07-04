using OperationManagement.Data.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OperationManagement.Models
{
    public class ProductProcess: IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [AllowNull]
        public string? Comment { get; set; }
        [AllowNull,DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [AllowNull, DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        [AllowNull]
        public int? EstimatedDuration { get; set; }
        [Required,ForeignKey("ProcessId")]
        public int ProcessId { get; set; }
        public Process? Process { get; set; }
        [Required, ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required, ForeignKey("StatusId")]
        public int StatusId { get; set; }
        public ProcessStatus? Status { get; set; }


    }
}
