using Microsoft.Build.Framework;
using OperationManagement.Models;

namespace OperationManagement.Data.ViewModels
{
    public class CreateOrderVM
    {
        [Required]
        public Order Order { get; set; }
        [Required]
        public int EnterpriseId { get; set; }
        public Customer? Customer { get; set; }
        public IEnumerable<DeliveryLocation>? DeliveryLocations { get; set; }
        public List<string>? Titles { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }
}
