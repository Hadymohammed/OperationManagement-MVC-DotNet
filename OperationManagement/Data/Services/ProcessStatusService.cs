using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ProcessStatusService:EntityBaseRepository<ProcessStatus>,IProcessStatusService
    {
        private readonly AppDBContext _context;
        public ProcessStatusService(AppDBContext context) : base(context)
        {
            _context = context;
        }

        public List<ProcessStatus> GetByProcessId(int processId)
        {
            return _context.ProcessStatuses.Where(p => p.ProcessId == processId).ToList();
        }
    }
}
