using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface IProductComponentService:IEntityBaseRepository<ProductComponent>
    {
        public ProductComponent GetComponentInProduct(int componentId, int productId);

    }
}
