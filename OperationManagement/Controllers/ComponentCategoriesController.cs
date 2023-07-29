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
    public class ComponentCategoriesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IComponentCategoryService _categoryService;
        private readonly IComponentService _componentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ComponentCategoriesController(AppDBContext context,
            IComponentCategoryService componentCategoryService,
            IComponentService componentService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _categoryService = componentCategoryService;
            _componentService = componentService;
            _userManager = userManager;
        }

        // GET: ComponentCategories
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var categories = await _categoryService.GetAllAsync();
            return View(categories.Where(c=>c.EnterpriseId==user.EnterpriseId));
        }

        // GET: ComponentCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ComponentCategories == null)
            {
                return NotFound();
            }

            var componentCategory = await _categoryService.GetByIdAsync((int)id);
            var allcomponents = await _componentService.GetAllAsync(c=>c.Photos);
            allcomponents = allcomponents.Where(c => c.CategoryId == componentCategory.Id);
            componentCategory.Components = allcomponents.ToList();
            if (componentCategory == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (componentCategory.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(componentCategory);
        }

        // GET: ComponentCategories/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(new ComponentCategory()
            {
                EnterpriseId=(int)user.EnterpriseId
            });
        }

        // POST: ComponentCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EnterpriseId")] ComponentCategory componentCategory)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                if (componentCategory.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _categoryService.AddAsync(componentCategory);
                return RedirectToAction(nameof(Index));
            }
            return View(componentCategory);
        }

        // GET: ComponentCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ComponentCategories == null)
            {
                return NotFound();
            }

            var componentCategory = await _categoryService.GetByIdAsync((int)id);
            if (componentCategory == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (componentCategory.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(componentCategory);
        }

        // POST: ComponentCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnterpriseId")] ComponentCategory componentCategory)
        {
            if (id != componentCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (componentCategory.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                try
                {
                    await _categoryService.UpdateAsync(componentCategory.Id,componentCategory);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComponentCategoryExists(componentCategory.Id))
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
            return View(componentCategory);
        }

        // GET: ComponentCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ComponentCategories == null)
            {
                return NotFound();
            }

            var componentCategory = await _categoryService.GetByIdAsync((int)id);
            if (componentCategory == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (componentCategory.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(componentCategory);
        }

        // POST: ComponentCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var componentCategory = await _categoryService.GetByIdAsync(id);
            if (componentCategory != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (componentCategory.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _categoryService.DeleteAsync(componentCategory.Id);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool ComponentCategoryExists(int id)
        {
          return (_context.ComponentCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
