using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class CategoryService:EntityBaseRepository<Category>,ICategoryService
    {
        readonly AppDBContext _context;
        public CategoryService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
