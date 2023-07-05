using OperationManagement.Models;

namespace OperationManagement.Data.ViewModels
{
    public class AdminIndexVM
    {
        public List<Enterprise> Enterprises { get; set; }
        public List<ApplicationUser> PendingUsers { get; set; }
        public int NumberOfCustomers { get; set; }
        public int NumberOfOrders { get; set; }
    }
}
