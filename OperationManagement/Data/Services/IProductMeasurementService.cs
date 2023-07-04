using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface IProductMeasurementService : IEntityBaseRepository<ProductMeasurement>
    {
        public ProductMeasurement GetMeasurementInProduct(int meagId, int productId);
    }
}
