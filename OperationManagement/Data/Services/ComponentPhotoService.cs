using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ComponentPhotoService:EntityBaseRepository<ComponentPhoto>,IComponentPhotoService
    {
        private readonly AppDBContext _context;
        public ComponentPhotoService(AppDBContext context):base(context)
        {
            _context = context;
        }
        public List<ComponentPhoto> GetByComponentId(int cId)
        {
            return _context.ComponentPhotos.Where(c => c.ComponentId == cId).ToList();
        }
    }
}
