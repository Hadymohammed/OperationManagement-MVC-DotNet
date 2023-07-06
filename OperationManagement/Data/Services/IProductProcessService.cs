using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface IProductProcessService:IEntityBaseRepository<ProductProcess>
    {
        public ProductProcess GetProcessInProduct(int processId, int productId);
        public List<ProductProcess> GetByProductId(int productId);
    }
}
