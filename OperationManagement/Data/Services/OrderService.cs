using Microsoft.EntityFrameworkCore;
using OperationManagement.Data.Base;
using OperationManagement.Data.Common;
using OperationManagement.Models;
using System.Net.Mail;

namespace OperationManagement.Data.Services
{
    public class OrderService:EntityBaseRepository<Order>,IOrderService
    {
        private readonly AppDBContext _context;
        private readonly IProductService _productService;
        private readonly IAttachmentService _attachmentService;
        public OrderService(AppDBContext context,
            IProductService productService,
            IAttachmentService attachmentService) : base(context)
        {
            _context = context;
            _productService = productService;
            _attachmentService = attachmentService;
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
    
        public async Task<bool> DeleteCompleteOrder(int orderId)
        {
            var order = await GetByIdAsync(orderId, o=>o.Products,o=>o.Attachments);
            try
            {
                var products = order.Products.ToList();
                foreach(var product in products)
                {
                    await _productService.DeleteCompleteProduct(product.Id);
                }
                var attachements = order.Attachments.ToList();
                foreach(var attachment in attachements)
                {
                    if (FilesManagement.DeleteFile(attachment.FileURL))
                    {
                        await _attachmentService.DeleteAsync(attachment.Id);
                    }
                }
                await DeleteAsync(order.Id);
                return true;
            }
            catch(Exception err)
            {
                return false;
            }
        }
    }
}
