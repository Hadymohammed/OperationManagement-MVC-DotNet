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
    [Authorize(Roles =UserRoles.User)]
    public class ProcessCategoriesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IProcessCategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProcessCategoriesController(AppDBContext context,
            IProcessCategoryService processCategoryService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _categoryService = processCategoryService;
            _userManager = userManager;
        }

        // GET: ProcessCategories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            var user = await _userManager.GetUserAsync(User);

            return View(categories.Where(c=>c.EnterpriseId==(int)user.EnterpriseId));
        }

        // GET: ProcessCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _categoryService.GetByIdAsync((int)id, c => c.Processes);
            var user = await _userManager.GetUserAsync(User);
            if (user.EnterpriseId!=category.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: ProcessCategories/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(new ProcessCategory()
            {
                EnterpriseId= (int)user.EnterpriseId
            });
        }

        // POST: ProcessCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EnterpriseId")] ProcessCategory processCategory)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user.EnterpriseId != processCategory.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                processCategory.NoramlizedName = _userManager.NormalizeName(processCategory.Name);
                await _categoryService.AddAsync(processCategory);
                return RedirectToAction(nameof(Index));
            }
            return View(processCategory);
        }

        // GET: ProcessCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProcessCategories == null)
            {
                return NotFound();
            }

            var processCategory = await _categoryService.GetByIdAsync((int)id);
            var user = await _userManager.GetUserAsync(User);
            if (user.EnterpriseId != processCategory.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (processCategory == null)
            {
                return NotFound();
            }
            return View(processCategory);
        }

        // POST: ProcessCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnterpriseId")] ProcessCategory processCategory)
        {
            if (id != processCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user.EnterpriseId != processCategory.EnterpriseId)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    processCategory.NoramlizedName = _userManager.NormalizeName(processCategory.Name);
                    await _categoryService.UpdateAsync(id,processCategory);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcessCategoryExists(processCategory.Id))
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
            return View(processCategory);
        }

        // GET: ProcessCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProcessCategories == null)
            {
                return NotFound();
            }

            var processCategory = await _categoryService.GetByIdAsync((int)id);
            if (processCategory == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user.EnterpriseId != processCategory.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(processCategory);
        }

        // POST: ProcessCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProcessCategories == null)
            {
                return Problem("Entity set 'AppDBContext.ProcessCategories'  is null.");
            }
            var processCategory = await _categoryService.GetByIdAsync(id);
            if (processCategory != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user.EnterpriseId != processCategory.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _categoryService.DeleteAsync(processCategory.Id);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool ProcessCategoryExists(int id)
        {
          return (_context.ProcessCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
