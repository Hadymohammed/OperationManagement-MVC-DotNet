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
using OperationManagement.Data.ViewModels;
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
                //return redirect to SpecificationCategory/details
                return RedirectToAction("Details", new { id = specification.Id });
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
        public async Task<IActionResult> Edit(int id,Specification specification)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id != specification.Id)
            {
                return NotFound();
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
                //return redirect to Specification/details
                return RedirectToAction("Details", new { id = specification.Id });
            }
            ViewData["CategoryId"] = new SelectList(_context.SpecificationCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",specification.CategoryId);
            return View(specification);
        }
        //GET: Specifications/EditOptions/5
        public async Task<IActionResult> EditOption(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var option = await _optionSerivce.GetByIdAsync((int)id,o=>o.Specification);
            if (option == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (option.Specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(option);
        }
        //POST: Specifications/EditOptions/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOption(int id,[Bind("Id","Name","SpecificationId")] SpecificationOption option)
        {
            if (id != option.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _optionSerivce.UpdateAsync(option.Id, option);
                //return redirect to SpecificationCategory/details
                return RedirectToAction("Edit", new { id = option.SpecificationId });
            }
            return View(option);
        }
        //GET: Specifications/EditStatuses/5
        public async Task<IActionResult> EditStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _statusService.GetByIdAsync((int)id,s=>s.Specification);
            if (status == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (status.Specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if(status.Name==Consts.DoneStatus){
                ModelState.AddModelError(String.Empty, "Done status can't be edited.");
                var specification=await _specificationService.GetByIdAsync(status.SpecificationId,s=>s.Enterprise,s=>s.Category,s=>s.Statuses,s=>s.Options);
                ViewData["CategoryId"] = new SelectList(_context.SpecificationCategories
                .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",specification.CategoryId);
                //Return redirect to Specification/Edit
                return View("Edit",specification);
            }
            return View(status);
        }
        //POST: Specifications/EditStatuses/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id,[Bind("Id","Name","SpecificationId")] SpecificationStatus status)
        {
            if (id != status.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _statusService.UpdateAsync(status.Id, status);
                //return redirect to SpecificationCategory/details
                return RedirectToAction("Edit", new { id = status.SpecificationId });
            }
            return View(status);
        }
        //GET: Specifications/CreateOption/5
        public async Task<IActionResult> CreateOption(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var specification = await _specificationService.GetByIdAsync((int)id,s=>s.Enterprise,s=>s.Category);
            if (specification == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(new SpecificationOption()
            {
                SpecificationId=specification.Id,
                Specification = specification
            });
        }
        //POST: Specifications/CreateOption/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOption([Bind("Name","SpecificationId")] SpecificationOption option)
        {
            if (ModelState.IsValid)
            {
                await _optionSerivce.AddAsync(new SpecificationOption()
                {
                    SpecificationId = option.SpecificationId,
                    Name = option.Name
                });
                //return redirect to SpecificationCategory/details
                return RedirectToAction("Edit", new { id = option.SpecificationId });
            }
            var specification = await _specificationService.GetByIdAsync(option.SpecificationId);
            option.Specification = specification;
            return View(option);
        }
        //GET: Specifications/CreateStatus/5
        public async Task<IActionResult> CreateStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var specification = await _specificationService.GetByIdAsync((int)id,s=>s.Enterprise,s=>s.Category);
            if (specification == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(new SpecificationStatus()
            {
                SpecificationId=specification.Id,
                Specification = specification
            });
        }
        //POST: Specifications/CreateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStatus([Bind("Name","SpecificationId")] SpecificationStatus status)
        {
            if (ModelState.IsValid)
            {
                await _statusService.AddAsync(new SpecificationStatus()
                {
                    SpecificationId = status.SpecificationId,
                    Name = status.Name
                });
                //return redirect to SpecificationCategory/details
                return RedirectToAction("Edit", new { id = status.SpecificationId });
            }
            var specification = await _specificationService.GetByIdAsync(status.SpecificationId);
            status.Specification = specification;
            return View(status);
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
            var specification = await _specificationService.GetByIdAsync((int)id, s => s.Enterprise, s => s.Statuses, s => s.Options,s=>s.Products);
            if(specification == null){
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            //specification can't be deleted if it is used in a product
            if(specification.Products.Count>0){
                ModelState.AddModelError(String.Empty, "Specification can't be deleted because it is used in a product.");
                //Return redirect to Specification/Delete
                return View("Delete",specification);
            }
            await _specificationService.DeleteAsync(specification.Id);
            
           //return redirect to SpecificationCategory/details
            return RedirectToAction("Details", "SpecificationCategories", new { id = specification.CategoryId });
        }
        // Delete SpecificationOption
        [HttpGet]
        public async Task<IActionResult> DeleteOption(int id)
        {
            var option = await _optionSerivce.GetByIdAsync(id,o=>o.Products,o=>o.Specification);
            if (option == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (option.Specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            //status can't be deleted if it is used in a product
            if(option.Products.Count>0){
                ModelState.AddModelError(String.Empty, "Option can't be deleted because it is used in a product.");
                var specification=await _specificationService.GetByIdAsync(option.SpecificationId,s=>s.Enterprise,s=>s.Category,s=>s.Statuses,s=>s.Options);
                ViewData["CategoryId"] = new SelectList(_context.SpecificationCategories
                .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",specification.CategoryId);
                //Return redirect to Specification/Edit
                return View("Edit",specification);
            }

            await _optionSerivce.DeleteAsync(option.Id);
            return RedirectToAction("Edit", new { id = option.SpecificationId });
        }
        // Delete SpecificationStatus
        [HttpGet]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _statusService.GetByIdAsync(id,s=>s.Specification,s=>s.Products);
            if (status == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
           if (status.Specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            //status can't be deleted if it is DoneStatus
            if(status.Name==Consts.DoneStatus){
                ModelState.AddModelError(String.Empty, "Done status can't be deleted.");
                var specification=await _specificationService.GetByIdAsync(status.SpecificationId,s=>s.Enterprise,s=>s.Category,s=>s.Statuses,s=>s.Options);
                ViewData["CategoryId"] = new SelectList(_context.SpecificationCategories
                .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",specification.CategoryId);
                //Return redirect to Specification/Edit
                return View("Edit",specification);
            }
            //status can't be deleted if it is used in a product
            if(status.Products.Count>0){
                ModelState.AddModelError(String.Empty, "Status can't be deleted because it is used in a product.");
                var specification=await _specificationService.GetByIdAsync(status.SpecificationId,s=>s.Enterprise,s=>s.Category,s=>s.Statuses,s=>s.Options);
                ViewData["CategoryId"] = new SelectList(_context.SpecificationCategories
                .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",specification.CategoryId);
                
                //Return redirect to Specification/Edit
                return View("Edit",specification);
            }
            await _statusService.DeleteAsync(status.Id);
            return RedirectToAction("Edit", new { id = status.SpecificationId });
        }
        private bool SpecificationExists(int id)
        {
          return (_context.Specifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
