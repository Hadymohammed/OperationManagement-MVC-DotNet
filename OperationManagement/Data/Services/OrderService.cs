using Microsoft.EntityFrameworkCore;
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
        public Order GetCompleteOrder(int orderId)
        {
           return _context.Orders
                .Where(o => o.Id == orderId)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Category)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Components)
                            .ThenInclude(c => c.Component)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Measurements)
                            .ThenInclude(m => m.Measurement)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Processes)
                            .ThenInclude(pr => pr.Process)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Specifications)
                            .ThenInclude(s => s.Specification)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Specifications)
                            .ThenInclude(s => s.Option)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Specifications)
                            .ThenInclude(s => s.Status)
                    .FirstOrDefault();
        }
    }
}
