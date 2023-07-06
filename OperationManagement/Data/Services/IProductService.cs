using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface IProductService:IEntityBaseRepository<Product>
    {
        public List<Product> getByOrderId(int orderId);
        public Product getCompleteProductById(int productId);
        public Task<bool> DeleteCompleteProduct(int productId);
        public Task<int> UpdateProgressAsync(int productId);

    }
}
