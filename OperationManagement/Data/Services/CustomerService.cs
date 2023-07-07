using OperationManagement.Data.Base;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class CustomerService:EntityBaseRepository<Customer>,ICustomerService
    {
        readonly AppDBContext _context;
        readonly IOrderService _orderService;
        public CustomerService(AppDBContext context,
            IOrderService orderService):base(context)
        {
            _context = context;
            _orderService = orderService;
        }
        public int GetNumberOfAllCustomers()
        {
            return _context.Customers.Count();
        }
        public async Task<bool> DeleteCompleteCustomer(int customerId)
        {
            try
            {
                var customer = await GetByIdAsync(customerId, c => c.Orders);
                var orders = customer.Orders.ToList();
                foreach (var order in orders)
                {
                    await _orderService.DeleteCompleteOrder(order.Id);
                }
                await DeleteAsync(customer.Id);
                return true;
            }catch(Exception err)
            {
                return false;
            }
        } 
    }
}
