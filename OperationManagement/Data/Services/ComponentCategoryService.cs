using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ComponentCategoryService:EntityBaseRepository<ComponentCategory>,IComponentCategoryService
    {
        private readonly AppDBContext _context;
        public ComponentCategoryService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
