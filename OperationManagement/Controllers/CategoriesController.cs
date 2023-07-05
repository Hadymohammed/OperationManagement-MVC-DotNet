using System;
using System.Collections.Generic;
using System.Data;
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
    public class CategoriesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        public CategoriesController(AppDBContext context,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var all = await _categoryService.GetAllAsync(c => c.Enterprise);
            return View(all.Where(c=>c.EnterpriseId==user.EnterpriseId));
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var category = await _categoryService.GetByIdAsync((int)id, c => c.Enterprise);
            if (category == null)
            {
                return NotFound();
            }
            if (user.EnterpriseId != category.Id)
            {
                return RedirectToAction("ActionDenied", "Account");
            }

            return View(category);
        }

        // GET: Categories/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(new Category()
            {
                EnterpriseId= (int)user.EnterpriseId
            });
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EnterpriseId")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetByIdAsync((int)id, c => c.Enterprise);
            if (category == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (category.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnterpriseId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            
            if (ModelState.IsValid)
            {
                if (user.EnterpriseId != category.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                try
                {
                    await _categoryService.UpdateAsync(category.Id, category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetByIdAsync((int)id, c => c.Enterprise);
            if (category == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (category.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'AppDBContext.Categories'  is null.");
            }
            var category = await _categoryService.GetByIdAsync((int)id, c => c.Enterprise);
            var user = await _userManager.GetUserAsync(User);
            if (category.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (category != null)
            {
                await _categoryService.DeleteAsync(category.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
