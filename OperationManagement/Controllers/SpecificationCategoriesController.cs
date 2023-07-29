using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OperationManagement.Data;
using OperationManagement.Data.Services;
using OperationManagement.Models;
using Microsoft.AspNetCore.Authorization;
using OperationManagement.Data.Static;

namespace OperationManagement.Controllers
{
    [Authorize(Roles = UserRoles.User)]
    public class SpecificationCategoriesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ISpecificationCategoryService _specificationCategoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SpecificationCategoriesController(AppDBContext context,
            ISpecificationCategoryService specificationCategoryService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _specificationCategoryService = specificationCategoryService;
            _userManager = userManager;
        }

        // GET: SpecificationCategories
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var Categories = await _specificationCategoryService.GetAllAsync();
            return View(Categories.Where(s=>s.EnterpriseId==user.EnterpriseId).ToList());
        }

        // GET: SpecificationCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SpecificationCategories == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var specificationCategory = await _specificationCategoryService.GetByIdAsync(id.Value,s=>s.Enterprise,s=>s.Specifications);
        
            if (specificationCategory == null)
            {
                return NotFound();
            }
            if(specificationCategory.EnterpriseId!=user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(specificationCategory);
        }

        // GET: SpecificationCategories/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(new SpecificationCategory(){
                EnterpriseId = (int)user.EnterpriseId
            });
        }

        // POST: SpecificationCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EnterpriseId")] SpecificationCategory specificationCategory)
        {
            if (ModelState.IsValid)
            {
                specificationCategory.NoramlizedName = _userManager.NormalizeName(specificationCategory.Name);
                await _specificationCategoryService.AddAsync(specificationCategory);
                return RedirectToAction(nameof(Index));
            }
            return View(specificationCategory);
        }

        // GET: SpecificationCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SpecificationCategories == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var specificationCategory = await _specificationCategoryService.GetByIdAsync(id.Value,s=>s.Enterprise);
            if (specificationCategory == null)
            {
                return NotFound();
            }
            if (specificationCategory.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(specificationCategory);
        }

        // POST: SpecificationCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnterpriseId")] SpecificationCategory specificationCategory)
        {
            if (id != specificationCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                try
                {
                    if (specificationCategory.EnterpriseId != user.EnterpriseId)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    specificationCategory.NoramlizedName = _userManager.NormalizeName(specificationCategory.Name);
                    await _specificationCategoryService.UpdateAsync(specificationCategory.Id,specificationCategory);
                    
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
            return View(specificationCategory);
        }

        // GET: SpecificationCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SpecificationCategories == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var specificationCategory = await _specificationCategoryService.GetByIdAsync(id.Value,s=>s.Enterprise);

            if (specificationCategory == null)
            {
                return NotFound();
            }
            if (specificationCategory.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
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
            var user = await _userManager.GetUserAsync(User);
            var specificationCategory = await _specificationCategoryService.GetByIdAsync(id,s=>s.Enterprise);
            if (specificationCategory != null)
            {
                if (specificationCategory.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _specificationCategoryService.DeleteAsync(specificationCategory.Id);
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SpecificationCategoryExists(int id)
        {
          return (_context.SpecificationCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
