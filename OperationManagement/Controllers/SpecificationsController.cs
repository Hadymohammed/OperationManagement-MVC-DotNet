using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using OperationManagement.Data;
using OperationManagement.Data.Services;
using OperationManagement.Data.Static;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    [Authorize(Roles = UserRoles.User)]
    public class SpecificationsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ISpecificationService _specificationService;
        private readonly ISpecificationOptionService _optionSerivce;
        private readonly ISpecificationStatusService _statusService;
        private readonly UserManager<ApplicationUser> _userManager;
        public SpecificationsController(AppDBContext context,
            ISpecificationService specificationService,
            ISpecificationOptionService optionSerivce,
            ISpecificationStatusService statusService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _specificationService = specificationService;
            _optionSerivce = optionSerivce;
            _statusService = statusService;
            _userManager = userManager;
        }

        // GET: Specifications
        public async Task<IActionResult> Index()
        {
            var all = await _specificationService.GetAllAsync(s => s.Enterprise, s=>s.Category);
            var user = await _userManager.GetUserAsync(User);
            
            return View(all.Where(s=>s.EnterpriseId==user.EnterpriseId));
        }

        // GET: Specifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Specifications == null)
            {
                return NotFound();
            }

            var specification = await _specificationService.GetByIdAsync((int)id, s => s.Enterprise,s=>s.Category, s => s.Statuses, s => s.Options);
                
            if (specification == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(specification);
        }

        // GET: Specifications/Create
        public async Task<IActionResult> Create(int? CategoryId)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["CategoryId"] = new SelectList(_context.SpecificationCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",CategoryId);
            return View(new Specification()
            {
                EnterpriseId=(int)user.EnterpriseId,
                Options=new List<SpecificationOption>(),
                Statuses=new List<SpecificationStatus>(),
            });
        }

        // POST: Specifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EnterpriseId,CategoryId")] Specification specification, SpecificationOption[] Options, SpecificationStatus[] Statuses)
        {
            var user = await _userManager.GetUserAsync(User);
            if (Options == null)
            {
                ModelState.AddModelError(String.Empty, "Options can't be empty.");
            }
            if (Statuses == null)
            {
                ModelState.AddModelError(String.Empty, "Status can't be empty.");
            }
            if (ModelState.IsValid)
            {
                if (specification.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _specificationService.AddAsync(specification);
                foreach(var option in Options)
                {
                    await _optionSerivce.AddAsync(new SpecificationOption()
                    {
                        SpecificationId = specification.Id,
                        Name = option.Name
                    });
                }
                bool DoneStatusFlag = false;

                foreach (var status in Statuses)
                {
                    await _statusService.AddAsync(new SpecificationStatus()
                    {
                        SpecificationId = specification.Id,
                        Name = status.Name
                    });
                    if (status.Name == Consts.DoneStatus)
                    {
                        DoneStatusFlag = true;
                    }
                }
                if (!DoneStatusFlag)
                {
                    await _statusService.AddAsync(new SpecificationStatus()
                    {
                        SpecificationId = specification.Id,
                        Name = Consts.DoneStatus
                    });
                }
                return RedirectToAction(nameof(Index));
            }
            specification.Statuses = Statuses.ToList();
            specification.Options = Options.ToList();
            ViewData["CategoryId"] = new SelectList(_context.SpecificationCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",specification.CategoryId);
            return View(specification);
        }

        // GET: Specifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Specifications == null)
            {
                return NotFound();
            }

            var specification = await _specificationService.GetByIdAsync((int)id, s => s.Enterprise,s=>s.Category, s => s.Statuses, s => s.Options);
            if (specification == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            ViewData["CategoryId"] = new SelectList(_context.SpecificationCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",specification.CategoryId);
            return View(specification);
        }

        // POST: Specifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnterpriseId,CategoryId")] Specification specification, SpecificationOption[] Options, SpecificationStatus[] Statuses)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id != specification.Id)
            {
                return NotFound();
            }
            if (Options == null)
            {
                ModelState.AddModelError(String.Empty, "Options can't be empty.");
            }
            else if (Options.Count() == 0)
            {
                ModelState.AddModelError(String.Empty, "Options can't be empty.");
            }
            if (Statuses == null)
            {
                ModelState.AddModelError(String.Empty, "Status can't be empty.");
            }
            else if (Statuses.Count() == 0)
            {
                ModelState.AddModelError(String.Empty, "Status can't be empty.");
            }
            if (ModelState.IsValid)
            {
                if (specification.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
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
                    bool DoneStatusFlag = false;

                    foreach (var status in Statuses)
                    {
                        await _statusService.AddAsync(new SpecificationStatus()
                        {
                            SpecificationId = specification.Id,
                            Name = status.Name
                        });
                        if (status.Name == Consts.DoneStatus)
                        {
                            DoneStatusFlag = true;
                        }
                    }
                    if(!DoneStatusFlag)
                    {
                        await _statusService.AddAsync(new SpecificationStatus()
                        {
                            SpecificationId = specification.Id,
                            Name = Consts.DoneStatus
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
            specification.Statuses = Statuses.ToList();
            specification.Options = Options.ToList();
            ViewData["CategoryId"] = new SelectList(_context.SpecificationCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",specification.CategoryId);
            return View(specification);
        }

        // GET: Specifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Specifications == null)
            {
                return NotFound();
            }

            var specification = await _specificationService.GetByIdAsync((int)id,s=>s.Category,s=>s.Enterprise);
            if (specification == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
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
                var user = await _userManager.GetUserAsync(User);
                if (specification.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
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
