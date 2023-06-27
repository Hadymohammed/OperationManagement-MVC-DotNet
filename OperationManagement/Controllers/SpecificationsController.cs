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
    public class SpecificationsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ISpecificationService _specificationService;
        private readonly ISpecificationOptionService _optionSerivce;
        private readonly ISpecificationStatusService _statusService;
        public SpecificationsController(AppDBContext context,
            ISpecificationService specificationService,
            ISpecificationOptionService optionSerivce,
            ISpecificationStatusService statusService)
        {
            _context = context;
            _specificationService = specificationService;
            _optionSerivce = optionSerivce;
            _statusService = statusService;
        }

        // GET: Specifications
        public async Task<IActionResult> Index()
        {
            return View(await _specificationService.GetAllAsync(s=>s.Enterprise));
        }

        // GET: Specifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Specifications == null)
            {
                return NotFound();
            }

            var specification = await _specificationService.GetByIdAsync((int)id, s => s.Enterprise, s => s.Statuses, s => s.Options);
                
            if (specification == null)
            {
                return NotFound();
            }

            return View(specification);
        }

        // GET: Specifications/Create
        public IActionResult Create()
        {
            return View(new Specification()
            {
                EnterpriseId=1,
                Options=new List<SpecificationOption>(),
                Statuses=new List<SpecificationStatus>(),
            });
        }

        // POST: Specifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EnterpriseId")] Specification specification, SpecificationOption[] Options, SpecificationStatus[] Statuses)
        {
            if (ModelState.IsValid)
            {
                await _specificationService.AddAsync(specification);
                foreach(var option in Options)
                {
                    await _optionSerivce.AddAsync(new SpecificationOption()
                    {
                        SpecificationId = specification.Id,
                        Name = option.Name
                    });
                }
                foreach(var status in Statuses)
                {
                    await _statusService.AddAsync(new SpecificationStatus()
                    {
                        SpecificationId = specification.Id,
                        Name = status.Name
                    });
                }
                return RedirectToAction(nameof(Index));
            }
            return View(specification);
        }

        // GET: Specifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Specifications == null)
            {
                return NotFound();
            }

            var specification = await _specificationService.GetByIdAsync((int)id, s => s.Enterprise, s => s.Statuses, s => s.Options);
            if (specification == null)
            {
                return NotFound();
            }
            return View(specification);
        }

        // POST: Specifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnterpriseId")] Specification specification, SpecificationOption[] Options, SpecificationStatus[] Statuses)
        {
            if (id != specification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _specificationService.UpdateAsync(specification.Id, specification);
                    specification.Statuses = _statusService.GetBySpecificationId(specification.Id);
                    specification.Options = _optionSerivce.GetBySpecificationId(specification.Id);

                    specification.Statuses.Clear();
                    specification.Options.Clear();
                    foreach (var option in Options)
                    {
                        await _optionSerivce.AddAsync(new SpecificationOption()
                        {
                            SpecificationId = specification.Id,
                            Name = option.Name
                        });
                    }
                    foreach (var status in Statuses)
                    {
                        await _statusService.AddAsync(new SpecificationStatus()
                        {
                            SpecificationId = specification.Id,
                            Name = status.Name
                        });
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecificationExists(specification.Id))
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
            return View(specification);
        }

        // GET: Specifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Specifications == null)
            {
                return NotFound();
            }

            var specification = await _context.Specifications
                .Include(s => s.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specification == null)
            {
                return NotFound();
            }

            return View(specification);
        }

        // POST: Specifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Specifications == null)
            {
                return Problem("Entity set 'AppDBContext.Specifications'  is null.");
            }
            var specification = await _specificationService.GetByIdAsync((int)id, s => s.Enterprise, s => s.Statuses, s => s.Options);
            if (specification != null)
            {
                await _specificationService.DeleteAsync(specification.Id);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool SpecificationExists(int id)
        {
          return (_context.Specifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
