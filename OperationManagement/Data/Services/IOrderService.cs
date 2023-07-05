using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface IOrderService:IEntityBaseRepository<Order>
    {
        public int GetNumberOfAllOrders();
    }
}
