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
    }
}
