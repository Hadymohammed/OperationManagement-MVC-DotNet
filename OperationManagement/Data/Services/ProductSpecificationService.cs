using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ProductSpecificationService:EntityBaseRepository<ProductSpecification>,IProductSpecificationService
    {
        private readonly AppDBContext _context;
        public ProductSpecificationService(AppDBContext context):base(context)
        {
            _context = context;
        }
        public ProductSpecification GetSpecificationInProduct(int specId, int productId)
        {
            return _context.ProductSpecifications.Where(c => c.ProductId == productId && c.SpecificationId == specId).FirstOrDefault();
        }
    }
}
