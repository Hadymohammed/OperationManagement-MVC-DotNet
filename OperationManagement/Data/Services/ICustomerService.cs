using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface ICustomerService:IEntityBaseRepository<Customer>
    {
        public int GetNumberOfAllCustomers();
        public Task<bool> DeleteCompleteCustomer(int customerId);

    }
}
