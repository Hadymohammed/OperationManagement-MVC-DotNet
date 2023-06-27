using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class SpecificationStatusService:EntityBaseRepository<SpecificationStatus>,ISpecificationStatusService
    {
        private readonly AppDBContext _context;
        public SpecificationStatusService(AppDBContext context):base(context)
        {
            _context = context;
        }

        public List<SpecificationStatus> GetBySpecificationId(int specificationId)
        {
            return _context.SpecificationStatuses.Where(s => s.SpecificationId == specificationId).ToList();
        }
    }
}
