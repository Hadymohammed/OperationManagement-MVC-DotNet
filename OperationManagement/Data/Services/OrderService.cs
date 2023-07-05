using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class OrderService:EntityBaseRepository<Order>,IOrderService
    {
        private readonly AppDBContext _context;
        public OrderService(AppDBContext context):base(context)
        {
            _context = context;
        }
        public int GetNumberOfAllOrders()
        {
            return _context.Orders.Count();
        }
    }
}
