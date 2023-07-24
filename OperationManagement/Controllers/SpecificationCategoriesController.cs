using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    public class SpecificationCategoriesController : Controller
    {
        private readonly AppDBContext _context;

        public SpecificationCategoriesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: SpecificationCategories
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.SpecificationCategories.Include(s => s.Enterprise);
            return View(await appDBContext.ToListAsync());
        }

        // GET: SpecificationCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SpecificationCategories == null)
            {
                return NotFound();
            }

            var specificationCategory = await _context.SpecificationCategories
                .Include(s => s.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specificationCategory == null)
            {
                return NotFound();
            }

            return View(specificationCategory);
        }

        // GET: SpecificationCategories/Create
        public IActionResult Create()
        {
            ViewData["EnterpriseId"] = new SelectList(_context.Enterprises, "Id", "Name");
            return View();
        }

        // POST: SpecificationCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NoramlizedName,EnterpriseId")] SpecificationCategory specificationCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specificationCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EnterpriseId"] = new SelectList(_context.Enterprises, "Id", "Name", specificationCategory.EnterpriseId);
            return View(specificationCategory);
        }

        // GET: SpecificationCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SpecificationCategories == null)
            {
                return NotFound();
            }

            var specificationCategory = await _context.SpecificationCategories.FindAsync(id);
            if (specificationCategory == null)
            {
                return NotFound();
            }
            ViewData["EnterpriseId"] = new SelectList(_context.Enterprises, "Id", "Name", specificationCategory.EnterpriseId);
            return View(specificationCategory);
        }

        // POST: SpecificationCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NoramlizedName,EnterpriseId")] SpecificationCategory specificationCategory)
        {
            if (id != specificationCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specificationCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecificationCategoryExists(specificationCategory.Id))
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
            ViewData["EnterpriseId"] = new SelectList(_context.Enterprises, "Id", "Name", specificationCategory.EnterpriseId);
            return View(specificationCategory);
        }

        // GET: SpecificationCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SpecificationCategories == null)
            {
                return NotFound();
            }

            var specificationCategory = await _context.SpecificationCategories
                .Include(s => s.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specificationCategory == null)
            {
                return NotFound();
            }

            return View(specificationCategory);
        }

        // POST: SpecificationCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SpecificationCategories == null)
            {
                return Problem("Entity set 'AppDBContext.SpecificationCategories'  is null.");
            }
            var specificationCategory = await _context.SpecificationCategories.FindAsync(id);
            if (specificationCategory != null)
            {
                _context.SpecificationCategories.Remove(specificationCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecificationCategoryExists(int id)
        {
          return (_context.SpecificationCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
