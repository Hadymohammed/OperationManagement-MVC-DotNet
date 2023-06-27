using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public interface IProcessStatusService:IEntityBaseRepository<ProcessStatus>
    {
        public List<ProcessStatus> GetByProcessId(int processId);
    }
}
