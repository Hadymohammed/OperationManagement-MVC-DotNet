using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class EnterpriseService:EntityBaseRepository<Enterprise>,IEnterpriseService
    {
        private readonly AppDBContext _context;
        public EnterpriseService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
