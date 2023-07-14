using OperationManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace OperationManagement.Data.ViewModels
{
    public class CreateProductVM
    {
        [Required]
        public int CategoryId { get; set; }
        public Product Product { get; set; }
        public List<ProductMeasurement>? ProductMeasurements { get; set; }
        public List<TupleVM<bool, ProductSpecification>>? ProductSpecifications { get; set; }
        public List<TupleVM<bool, ProductProcess>>? ProductProcesses { get; set; }
        public List<TupleVM<bool, ProductComponent>>? ProductComponents { get; set; }
        public IEnumerable<Specification>? Specifications { get; set; }
        public IEnumerable<Process>? Processes { get; set; }
        public IEnumerable<ProcessCategory>? ProcessCategories { get; set; }
        public IEnumerable<Measurement>? Measurements { get; set; }
        public IEnumerable<Component>? Components { get; set; }
    }
}
