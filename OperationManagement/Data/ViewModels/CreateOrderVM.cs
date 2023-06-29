using Microsoft.Build.Framework;
using OperationManagement.Models;

namespace OperationManagement.Data.ViewModels
{
    public class CreateOrderVM
    {
        [Required]
        public Order Order { get; set; }
        public IEnumerable<Customer>? Customers { get; set; }
        public IEnumerable<DeliveryLocation>? DeliveryLocations { get; set; }
        public List<string>? Titles { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }
}
