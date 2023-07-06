using Microsoft.EntityFrameworkCore;
using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class OrderService:EntityBaseRepository<Order>,IOrderService
    {
        private readonly AppDBContext _context;
        private readonly IProductService _productService;
        public OrderService(AppDBContext context,
            IProductService productService):base(context)
        {
            _context = context;
            _productService = productService;
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
        public async Task<int> UpdateProgress(int orderId)
        {
            var order = await GetByIdAsync(orderId,o=>o.Products);
            if (order.Products == null)
            {
                order.Progress = 0;
            }
            else if (order.Products.Count() == 0)
            {
                order.Progress = 0;
            }
            else
            {
                float totalDone = 0;
                foreach(var product in order.Products)
                {
                    await _productService.UpdateProgressAsync(product.Id);
                    if (product.IsCompleted)
                        totalDone++;
                }
                order.Progress = (int)(totalDone / order.Products.Count() * 100);
            }
            await UpdateAsync(order.Id, order);
            return (int)order.Progress;
        }
    }
}
