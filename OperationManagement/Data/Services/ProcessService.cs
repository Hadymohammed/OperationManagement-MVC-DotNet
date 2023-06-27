using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ProcessService:EntityBaseRepository<Process>,IProcessService
    {
        private readonly AppDBContext _context;
        public ProcessService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
