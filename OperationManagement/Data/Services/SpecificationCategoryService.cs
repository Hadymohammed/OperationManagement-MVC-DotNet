using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class SpecificationCategoryService:EntityBaseRepository<SpecificationCategory>,ISpecificationCategoryService
    {
        private readonly AppDBContext _context;
        public SpecificationCategoryService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
