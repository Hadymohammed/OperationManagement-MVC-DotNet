using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ProductMeasurementService:EntityBaseRepository<ProductMeasurement>,IProductMeasurementService
    {
        private readonly AppDBContext _context;
        public ProductMeasurementService(AppDBContext appDBContext):base(appDBContext)
        {
               _context = appDBContext;
        }
        public ProductMeasurement GetMeasurementInProduct(int meagId, int productId)
        {
            return _context.ProductMeasurements.Where(c => c.ProductId == productId && c.MeasurementId == meagId).FirstOrDefault();
        }
    }
}
