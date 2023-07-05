using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Data.Services;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    [Authorize(Roles = UserRoles.User)]
    public class DeliveryLocationsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IDeliveryLocationService _deliveryLocationService;

        public DeliveryLocationsController(AppDBContext context,
            IDeliveryLocationService deliveryLocationService)
        {
            _context = context;
            _deliveryLocationService = deliveryLocationService;
        }

        // GET: DeliveryLocations
        public async Task<IActionResult> Index()
        {
            return View(await _deliveryLocationService.GetAllAsync(d=>d.Enterprise));
        }

        // GET: DeliveryLocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DeliveryLocations == null)
            {
                return NotFound();
            }

            var deliveryLocation = await _deliveryLocationService.GetByIdAsync((int)id, m => m.Enterprise);
            if (deliveryLocation == null)
            {
                return NotFound();
            }

            return View(deliveryLocation);
        }

        // GET: DeliveryLocations/Create
        public IActionResult Create()
        {
            return View(new DeliveryLocation()
            {
                EnterpriseId=1
            });
        }

        // POST: DeliveryLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,GPS_URL,EnterpriseId")] DeliveryLocation deliveryLocation)
        {
            if (ModelState.IsValid)
            {
                await _deliveryLocationService.AddAsync(deliveryLocation);
                return RedirectToAction(nameof(Index));
            }
            return View(deliveryLocation);
        }

        // GET: DeliveryLocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DeliveryLocations == null)
            {
                return NotFound();
            }

            var deliveryLocation = await _deliveryLocationService.GetByIdAsync((int)id, m => m.Enterprise);
            if (deliveryLocation == null)
            {
                return NotFound();
            }
            return View(deliveryLocation);
        }

        // POST: DeliveryLocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,GPS_URL,EnterpriseId")] DeliveryLocation deliveryLocation)
        {
            if (id != deliveryLocation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _deliveryLocationService.UpdateAsync(deliveryLocation.Id, deliveryLocation);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryLocationExists(deliveryLocation.Id))
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
            return View(deliveryLocation);
        }

        // GET: DeliveryLocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DeliveryLocations == null)
            {
                return NotFound();
            }

            var deliveryLocation = await _deliveryLocationService.GetByIdAsync((int)id, m => m.Enterprise);
            if (deliveryLocation == null)
            {
                return NotFound();
            }

            return View(deliveryLocation);
        }

        // POST: DeliveryLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DeliveryLocations == null)
            {
                return Problem("Entity set 'AppDBContext.DeliveryLocations'  is null.");
            }
            var deliveryLocation = await _deliveryLocationService.GetByIdAsync((int)id, m => m.Enterprise);
            if (deliveryLocation != null)
            {
                await _deliveryLocationService.DeleteAsync(deliveryLocation.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryLocationExists(int id)
        {
          return (_context.DeliveryLocations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
