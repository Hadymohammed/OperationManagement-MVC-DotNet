using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Data.Services;
using OperationManagement.Data.Static;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    [Authorize(Roles = UserRoles.User)]
    public class CustomersController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly ICustomerContactService _customerContactService;
        public CustomersController(AppDBContext context,
            UserManager<ApplicationUser> userManager,
            ICustomerService customerService,
            ICustomerContactService customerContactService)
        {
            _context = context;
            _userManager = userManager;
            _customerService = customerService;
            _customerContactService = customerContactService;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _customerService.GetAllAsync(c=>c.Enterprise));
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetByIdAsync((int)id, c => c.Enterprise,c=>c.Contacts);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        //[Authorize(Roles =UserRoles.User)]
        public async Task<IActionResult> CreateAsync()
        {
            //var user = await _userManager.GetUserAsync(User);
            var customer = new Customer
            {
                Contacts = new List<CustomerContact>(),
                EnterpriseId = 1//(int)user.EnterpriseId
            };
            return View(customer);
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,NationalId,Nationality,Gender,BirthDate,EnterpriseId")] Customer customer, CustomerContact[] Contacts)
        {
            if (ModelState.IsValid)
            {
                
                await _customerService.AddAsync(customer);
                foreach(var contact in Contacts)
                {
                    await _customerContactService.AddAsync(new CustomerContact()
                    {
                        CustomerId = customer.Id,
                        Type = contact.Type,
                        Value = contact.Value
                    });
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetByIdAsync((int)id,c=>c.Enterprise,c=>c.Contacts);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,NationalId,Nationality,Gender,BirthDate,EnterpriseId,Contacts")] Customer customer, CustomerContact[] Contacts)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _customerService.UpdateAsync(customer.Id,customer);
                    customer.Contacts = _customerContactService.getByCustomerId(customer.Id);
                    customer.Contacts.Clear();
                    foreach (var contact in Contacts)
                    {
                        await _customerContactService.AddAsync(new CustomerContact()
                        {
                            CustomerId = customer.Id,
                            Type = contact.Type,
                            Value = contact.Value
                        });
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetByIdAsync((int)id, c => c.Enterprise,c=>c.Contacts);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'AppDBContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
