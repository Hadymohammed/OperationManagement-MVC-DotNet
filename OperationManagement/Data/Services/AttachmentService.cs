using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class AttachmentService:EntityBaseRepository<Attachment>,IAttachmentService
    {
        private readonly AppDBContext _context;
        public AttachmentService(AppDBContext context):base(context)
        {
            _context = context;
        }
    }
}
