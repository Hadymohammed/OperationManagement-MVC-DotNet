using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class CustomerService:EntityBaseRepository<Customer>,ICustomerService
    {
        readonly AppDBContext _context;
        public CustomerService(AppDBContext context):base(context)
        {
            _context = context;
        }
        public int GetNumberOfAllCustomers()
        {
            return _context.Customers.Count();
        }
    }
}
