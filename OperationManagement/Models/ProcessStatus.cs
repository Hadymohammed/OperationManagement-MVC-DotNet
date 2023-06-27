using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OperationManagement.Data.Base;

namespace OperationManagement.Models
{
    public class ProcessStatus: IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, ForeignKey("ProcessId")]
        public int ProcessId { get; set; }
        public Process? Process { get; set; }
        public List<ProductProcess>? Products { get; set; }
    }
}
