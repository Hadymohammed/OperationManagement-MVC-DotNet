using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface ISpecificationStatusService : IEntityBaseRepository<SpecificationStatus>
    {
        public  List<SpecificationStatus> GetBySpecificationId(int specificationId);
    }
}
