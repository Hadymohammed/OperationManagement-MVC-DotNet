using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ProductComponentService:EntityBaseRepository<ProductComponent>,IProductComponentService
    {
        private readonly AppDBContext _context;
        public ProductComponentService(AppDBContext context):base(context)
        {
            _context = context;
        }
        public ProductComponent GetComponentInProduct(int componentId,int productId)
        {
            return _context.ProductComponents.Where(c => c.ProductId == productId && c.ComponentId == componentId).FirstOrDefault();
        }
    }
}
