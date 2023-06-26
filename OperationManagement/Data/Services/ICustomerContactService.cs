using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface ICustomerContactService:IEntityBaseRepository<CustomerContact>
    {
        public List<CustomerContact> getByCustomerId(int customerId);
    }
}
