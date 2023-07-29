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
using OperationManagement.Data.Common;
using OperationManagement.Data.Services;
using OperationManagement.Data.Static;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    [Authorize(Roles = UserRoles.User)]
    public class ComponentsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IComponentService _componentService;
        private readonly IComponentPhotoService _PhotoService;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IComponentCategoryService _categoryService;
        public ComponentsController(AppDBContext context,
            IComponentService componentService,
            IComponentPhotoService photoService,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            IComponentCategoryService categoryService)
        {
            _context = context;
            _componentService = componentService;
            _PhotoService = photoService;
            _env = env;
            _userManager = userManager;
            _categoryService = categoryService;

        }

        // GET: Components
        public async Task<IActionResult> Index()
        {
            var all = await _componentService.GetAllAsync(c => c.Photos,c=>c.Category);
            var user = await _userManager.GetUserAsync(User);
            
            return View(all.Where(c=>c.EnterpriseId==user.EnterpriseId));
        }

        // GET: Components/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Components == null)
            {
                return NotFound();
            }


            var component = await _componentService.GetByIdAsync((int)id, c => c.Photos,c=>c.Category);
            if (component == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (component.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(component);
        }

        // GET: Components/Create
        public async Task<IActionResult> Create(int? CategoryId)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["CategoryId"] = new SelectList(_context.ComponentCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",CategoryId);

            return View(new Component()
            {
                EnterpriseId=(int)user.EnterpriseId
            });
        }

        // POST: Components/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Supplier,EnterpriseId,CategoryId")] Component component,List<IFormFile> Photos)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (component.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _componentService.AddAsync(component);
                int cnt = 0;
                if (Photos.Any())
                {
                    foreach(var photo in Photos)
                    {
                        cnt++;
                        var filePath = await FilesManagement.ComponentPhoto(photo, "Component", component.Id, cnt);
                        if (filePath != null)
                        {
                            await _PhotoService.AddAsync(new ComponentPhoto()
                            {
                                ComponentId=component.Id,
                                PhotoURL=filePath
                            });
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.ComponentCategories
            .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name");
            component = await _componentService.GetByIdAsync((int)component.Id, c => c.Photos, c => c.Category);
            return View(component);
        }

        // GET: Components/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Components == null)
            {
                return NotFound();
            }

            var component = await _componentService.GetByIdAsync((int)id, c => c.Photos, c => c.Category);
            if (component == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (component.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            ViewData["CategoryId"] = new SelectList(_context.ComponentCategories
            .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",component.CategoryId);
            return View(component);
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Supplier,EnterpriseId,CategoryId")] Component component, List<IFormFile> photos)
        {
            if (id != component.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                try
                {
                    if (component.EnterpriseId != user.EnterpriseId)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    await _componentService.UpdateAsync(component.Id,component);
                    var imgs = _PhotoService.GetByComponentId(component.Id);
                    int cnt = ((imgs!=null&&imgs.Any())?imgs.LastOrDefault().Id:0);
                    if (photos != null && photos.Any())
                    {
                        foreach (var photo in photos)
                        {
                            cnt++;
                            var filePath = await FilesManagement.ComponentPhoto(photo, "Component", component.Id, cnt);
                            if (filePath != null)
                            {
                                await _PhotoService.AddAsync(new ComponentPhoto()
                                {
                                    ComponentId = component.Id,
                                    PhotoURL = filePath
                                });
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComponentExists(component.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.ComponentCategories
               .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name");
            component = await _componentService.GetByIdAsync((int)component.Id, c => c.Photos, c => c.Category);
            return View(component);
        }

        // GET: Components/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Components == null)
            {
                return NotFound();
            }

            var component = await _componentService.GetByIdAsync((int)id, c => c.Photos, c => c.Category);
            if (component == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (component.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(component);
        }

        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Components == null)
            {
                return Problem("Entity set 'AppDBContext.Components'  is null.");
            }
            var component = await _componentService.GetByIdAsync((int)id, c => c.Photos);

            if (component == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (component.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            foreach(var photo in component.Photos)
            {
                FilesManagement.DeleteFile(photo.PhotoURL);
            }
            await _componentService.DeleteAsync(component.Id);
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<ActionResult> DeletePhoto(int PhotoId)
        {
            var photo = await _PhotoService.GetByIdAsync(PhotoId);
            var componentId = photo.ComponentId;
            var component = await _componentService.GetByIdAsync(componentId, h => h.Photos);

            if (component.Photos.Count() <= 1)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (component.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            string fileName = photo.PhotoURL;
            string rootPath = _env.WebRootPath;
            string filePath = Path.Combine(rootPath, fileName);
            bool isDeleted = FilesManagement.DeleteFile(filePath);
            if (isDeleted)
            {
                await _PhotoService.DeleteAsync(photo.Id);
                return RedirectToAction("Edit", new { Id = component.Id });
            }
            return NotFound();
        }
        private bool ComponentExists(int id)
        {
          return (_context.Components?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
