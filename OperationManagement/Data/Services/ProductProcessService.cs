using Microsoft.EntityFrameworkCore;
using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ProductProcessService:EntityBaseRepository<ProductProcess>,IProductProcessService
    {
        private readonly AppDBContext _context;
        public ProductProcessService(AppDBContext context):base(context)
        {
            _context = context;
        }
        public ProductProcess GetProcessInProduct(int processId, int productId)
        {
            return _context.ProductProcesses.Where(c => c.ProductId == productId && c.ProcessId == processId).FirstOrDefault();
        }
        public List<ProductProcess> GetByProductId(int productId)
        {
            return _context.ProductProcesses.Where(p => p.ProductId == productId).Include(p => p.Process.Statuses).ToList();
        }

    }
}
