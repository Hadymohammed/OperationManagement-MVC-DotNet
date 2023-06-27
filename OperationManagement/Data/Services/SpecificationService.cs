using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class SpecificationService:EntityBaseRepository<Specification>,ISpecificationService
    {
        private readonly AppDBContext _context;
        public SpecificationService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
