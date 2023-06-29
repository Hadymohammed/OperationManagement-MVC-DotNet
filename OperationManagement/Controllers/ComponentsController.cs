﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Data.Common;
using OperationManagement.Data.Services;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    public class ComponentsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IComponentService _componentService;
        private readonly IComponentPhotoService _PhotoService;
        private readonly IWebHostEnvironment _env;
        public ComponentsController(AppDBContext context,
            IComponentService componentService,
            IComponentPhotoService photoService,
            IWebHostEnvironment env)
        {
            _context = context;
            _componentService = componentService;
            _PhotoService = photoService;
            _env = env;
        }

        // GET: Components
        public async Task<IActionResult> Index()
        {
            return View(await _componentService.GetAllAsync(c=>c.Photos));
        }

        // GET: Components/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Components == null)
            {
                return NotFound();
            }

            var component = await _componentService.GetByIdAsync((int)id, c => c.Photos);
            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }

        // GET: Components/Create
        public IActionResult Create()
        {
            return View(new Component()
            {
                EnterpriseId=1
            });
        }

        // POST: Components/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Suppler,EnterpriseId")] Component component,List<IFormFile> Photos)
        {
            if (ModelState.IsValid)
            {
                if (Photos == null)
                {
                    ModelState.AddModelError("Photos", "At least one photo required.");
                    return View(component);
                }
                if (Photos.Count == 0)
                {
                    ModelState.AddModelError("Photos", "At least one photo required.");
                    return View(component);
                }
                await _componentService.AddAsync(component);
                int cnt = 0;
                foreach(var photo in Photos)
                {
                    cnt++;
                    var filePath = await FilesManagement.ComponentPhoto(photo, component.Name, component.Id, cnt);
                    if (filePath != null)
                    {
                        await _PhotoService.AddAsync(new ComponentPhoto()
                        {
                            ComponentId=component.Id,
                            PhotoURL=filePath
                        });
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(component);
        }

        // GET: Components/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Components == null)
            {
                return NotFound();
            }

            var component = await _componentService.GetByIdAsync((int)id, c => c.Photos);
            if (component == null)
            {
                return NotFound();
            }
            return View(component);
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Suppler,EnterpriseId")] Component component, List<IFormFile> photos)
        {
            if (id != component.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _componentService.UpdateAsync(component.Id,component);
                    var imgs = await _PhotoService.GetAllAsync();
                    int cnt = imgs.LastOrDefault().Id;
                    foreach (var photo in photos)
                    {
                        cnt++;
                        var filePath = await FilesManagement.ComponentPhoto(photo, component.Name, component.Id, cnt);
                        if (filePath != null)
                        {
                            await _PhotoService.AddAsync(new ComponentPhoto()
                            {
                                ComponentId = component.Id,
                                PhotoURL = filePath
                            });
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
            return View(component);
        }

        // GET: Components/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Components == null)
            {
                return NotFound();
            }

            var component = await _componentService.GetByIdAsync((int)id, c => c.Photos);
            if (component == null)
            {
                return NotFound();
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
            if (component != null)
            {
                await _componentService.DeleteAsync(component.Id);
            }
            
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