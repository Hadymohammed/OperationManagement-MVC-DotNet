using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface ISpecificationOptionService:IEntityBaseRepository<SpecificationOption>
    {
        public List<SpecificationOption> GetBySpecificationId(int specificationId);
    }
}
