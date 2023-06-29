using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Data.Services;
using OperationManagement.Data.ViewModels;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IDeliveryLocationService _deliveryLocationService;
        public OrdersController(AppDBContext context,
            IOrderService orderService,
            ICustomerService customerService,
            IDeliveryLocationService deliveryLocationService)
        {
            _context = context;
            _orderService = orderService;
            _customerService = customerService;
            _deliveryLocationService = deliveryLocationService;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.Orders.Include(o => o.Customer).Include(o => o.DeliveryLocation);
            return View(await appDBContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.DeliveryLocation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CreateOrderVM()
            {
                Order = new Order(),
                Customers = await _customerService.GetAllAsync(),
                DeliveryLocations = await _deliveryLocationService.GetAllAsync()
            };
            return View(vm);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderVM OrderVM)
        {
            if (ModelState.IsValid)
            {
                await _orderService.AddAsync(OrderVM.Order);
                return RedirectToAction(nameof(Index));
            }
            OrderVM.Customers = await _customerService.GetAllAsync();
            OrderVM.DeliveryLocations = await _deliveryLocationService.GetAllAsync();
            return View(OrderVM);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var VM = new CreateOrderVM()
            {
                Order = order,
                Customers = await _customerService.GetAllAsync(),
                DeliveryLocations = await _deliveryLocationService.GetAllAsync()
            };
           return View(VM);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EnterpriseOrderNumber,Address,GPS_URL,ContractDate,PlannedStartDate,PlannedEndDate,ActualStartDate,ActualEndDate,HandOverDate,IsHandOver,CustomerId,DeliveryLocationId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (order.HandOverDate != null)
                        order.IsHandOver = true;
                    await _orderService.UpdateAsync(order.Id, order);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var VM = new CreateOrderVM()
            {
                Order = order,
                Customers = await _customerService.GetAllAsync(),
                DeliveryLocations = await _deliveryLocationService.GetAllAsync()
            };
            return View(VM);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetByIdAsync((int)id, o => o.Customer, o => o.DeliveryLocation);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'AppDBContext.Orders'  is null.");
            }
            var order = await _orderService.GetByIdAsync((int)id);
            if (order != null)
            {
                await _orderService.DeleteAsync(order.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
