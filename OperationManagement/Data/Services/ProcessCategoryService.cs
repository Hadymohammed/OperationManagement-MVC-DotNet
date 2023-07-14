using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ProcessCategoryService:EntityBaseRepository<ProcessCategory>,IProcessCategoryService
    {
        private readonly AppDBContext _context;
        public ProcessCategoryService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
