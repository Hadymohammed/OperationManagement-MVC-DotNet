using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface IOrderService:IEntityBaseRepository<Order>
    {
        public int GetNumberOfAllOrders();
        public Order GetCompleteOrder(int orderId);
        public Task<int> UpdateProgress(int orderId);
        public Task<bool> DeleteCompleteOrder(int orderId);


    }
}
