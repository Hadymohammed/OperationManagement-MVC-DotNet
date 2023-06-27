using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ComponentService:EntityBaseRepository<Component>,IComponentService
    {
        private readonly AppDBContext _context;
        public ComponentService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
