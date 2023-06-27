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
    public class MeasurementsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IMeasurementService _measurementSerivce;
        public MeasurementsController(AppDBContext context,
            IMeasurementService measurementService)
        {
            _context = context;
            _measurementSerivce = measurementService;
        }

        // GET: Measurements
        public async Task<IActionResult> Index()
        {
            return View(await _measurementSerivce.GetAllAsync(m=>m.Enterprise));
        }

        // GET: Measurements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Measurements == null)
            {
                return NotFound();
            }

            var measurement = await _measurementSerivce.GetByIdAsync((int)id, m => m.Enterprise);
            if (measurement == null)
            {
                return NotFound();
            }

            return View(measurement);
        }

        // GET: Measurements/Create
        public IActionResult Create()
        {
            return View(new Measurement()
            {
                EnterpriseId=1
            });
        }

        // POST: Measurements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EnterpriseId")] Measurement measurement)
        {
            if (ModelState.IsValid)
            {
                await _measurementSerivce.AddAsync(measurement);
                return RedirectToAction(nameof(Index));
            }
            return View(measurement);
        }

        // GET: Measurements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Measurements == null)
            {
                return NotFound();
            }

            var measurement = await _measurementSerivce.GetByIdAsync((int)id, m => m.Enterprise);
            if (measurement == null)
            {
                return NotFound();
            }
            return View(measurement);
        }

        // POST: Measurements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnterpriseId")] Measurement measurement)
        {
            if (id != measurement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _measurementSerivce.UpdateAsync(id, measurement);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeasurementExists(measurement.Id))
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
            return View(measurement);
        }

        // GET: Measurements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Measurements == null)
            {
                return NotFound();
            }

            var measurement = await _measurementSerivce.GetByIdAsync((int)id, m => m.Enterprise);
            if (measurement == null)
            {
                return NotFound();
            }

            return View(measurement);
        }

        // POST: Measurements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Measurements == null)
            {
                return Problem("Entity set 'AppDBContext.Measurements'  is null.");
            }
            var measurement = await _measurementSerivce.GetByIdAsync(id, m => m.Enterprise);
            if (measurement != null)
            {
                await _measurementSerivce.DeleteAsync(measurement.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MeasurementExists(int id)
        {
          return (_context.Measurements?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
