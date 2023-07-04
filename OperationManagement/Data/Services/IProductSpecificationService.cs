using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface IProductSpecificationService:IEntityBaseRepository<ProductSpecification>
    {
        public ProductSpecification GetSpecificationInProduct(int specId, int productId);
    }
}
