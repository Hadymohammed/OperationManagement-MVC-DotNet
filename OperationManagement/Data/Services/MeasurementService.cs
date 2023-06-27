using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class MeasurementService:EntityBaseRepository<Measurement>,IMeasurementService
    {
        readonly AppDBContext _context;
        public MeasurementService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
