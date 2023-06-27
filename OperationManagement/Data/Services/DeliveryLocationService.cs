using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class DeliveryLocationService:EntityBaseRepository<DeliveryLocation>,IDeliveryLocationService
    {
        readonly AppDBContext _context;
        public DeliveryLocationService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
