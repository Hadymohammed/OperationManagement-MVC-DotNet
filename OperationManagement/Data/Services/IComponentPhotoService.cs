using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface IComponentPhotoService:IEntityBaseRepository<ComponentPhoto>
    {
        public List<ComponentPhoto> GetByComponentId(int cId);

    }
}
