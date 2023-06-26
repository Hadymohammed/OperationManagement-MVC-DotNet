using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class CustomerContactService:EntityBaseRepository<CustomerContact>,ICustomerContactService
    {
        readonly AppDBContext _context;
        public CustomerContactService(AppDBContext context):base(context)
        {
            _context = context;
        }

        public List<CustomerContact> getByCustomerId(int customerId)
        {
            return _context.CustomerContacts.Where(c => c.CustomerId == customerId).ToList();
        }
    }
}
