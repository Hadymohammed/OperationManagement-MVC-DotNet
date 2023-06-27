using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class SpecificationOptionService:EntityBaseRepository<SpecificationOption>,ISpecificationOptionService
    {
        private readonly AppDBContext _context;
        public SpecificationOptionService(AppDBContext context):base(context)
        {
            _context = context;
        }

        public List<SpecificationOption> GetBySpecificationId(int specificationId)
        {
            return _context.SpecificationOptions.Where(s => s.SpecificationId == specificationId).ToList();
        }
    }
}
