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
    [Authorize(Roles = UserRoles.User)]
    public class ProcessesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IProcessService _processService;
        private readonly IProcessStatusService _processStatusService;
        private readonly IProcessCategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProcessesController(AppDBContext context,
            IProcessService processService,
            IProcessStatusService processStatusService,
            UserManager<ApplicationUser> userManager,
            IProcessCategoryService processCategoryService)
        {
            _context = context;
            _processService = processService;
            _processStatusService = processStatusService;
            _userManager = userManager;
            _categoryService = processCategoryService;
        }

        // GET: Processes
        public async Task<IActionResult> Index()
        {
            var all = await _processService.GetAllAsync(p=>p.Category);
            var user = await _userManager.GetUserAsync(User);
            
            return View(all.Where(p=>p.EnterpriseId==user.EnterpriseId));
        }

        // GET: Processes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Processes == null)
            {
                return NotFound();
            }

            var process = await _processService.GetByIdAsync((int)id, p => p.Enterprise, p => p.Statuses,p=>p.Category);
            if (process == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (process.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(process);
        }

        // GET: Processes/Create
        public async Task<IActionResult> Create(int? CategoryId)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["CategoryId"] = new SelectList(_context.ProcessCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name",CategoryId);

            return View(new Process()
            {
                EnterpriseId=(int)user.EnterpriseId,
                Statuses=new List<ProcessStatus>()
            });
        }

        // POST: Processes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EnterpriseId,CategoryId")] Process process, ProcessStatus[] statuses)
        {
            var user = await _userManager.GetUserAsync(User);
            if (statuses == null)
            {
                ModelState.AddModelError(String.Empty, "Status Can't be empty.");
            }
            else if (statuses.Count() == 0)
            {
                ModelState.AddModelError(String.Empty, "Status Can't be empty.");
            }
            if (ModelState.IsValid)
            {
                if (process.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _processService.AddAsync(process);
                bool DoneStatusFlag = false;
                foreach (var status in statuses)
                {
                    await _processStatusService.AddAsync(new ProcessStatus()
                    {
                        ProcessId = process.Id,
                        Name = status.Name,
                    });
                    if (status.Name == Consts.DoneStatus)
                    {
                        DoneStatusFlag = true;
                    }
                }
                if (!DoneStatusFlag)
                {
                    await _processStatusService.AddAsync(new ProcessStatus()
                    {
                        ProcessId = process.Id,
                        Name = Consts.DoneStatus
                    });
                }
                //return redirect to ProcessCategories/details
                return RedirectToAction("Details", "ProcessCategories", new { id = process.CategoryId });
            }
            process.Statuses = statuses.ToList();
            ViewData["CategoryId"] = new SelectList(_context.ProcessCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name");

            return View(process);
        }

        // GET: Processes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Processes == null)
            {
                return NotFound();
            }

            var process = await _processService.GetByIdAsync((int)id, p => p.Enterprise, p => p.Statuses, p => p.Category);
            if (process == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (process.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewData["CategoryId"] = new SelectList(_context.ProcessCategories
               .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name");

            return View(process);
        }

        // POST: Processes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnterpriseId,CategoryId")] Process process)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id != process.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (process.EnterpriseId != user.EnterpriseId)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    await _processService.UpdateAsync(process.Id, process);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcessExists(process.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return redirect to ProcessCategories/details
                return RedirectToAction("Details", "ProcessCategories", new { id = process.CategoryId });
            }
            ViewData["CategoryId"] = new SelectList(_context.ProcessCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name");
            return View(process);
        }

        // GET: Processes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Processes == null)
            {
                return NotFound();
            }

            var process = await _processService.GetByIdAsync((int)id, p => p.Enterprise, p => p.Statuses,p=>p.Products);
            if (process == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (process.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(process);
        }

        // POST: Processes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Processes == null)
            {
                return Problem("Entity set 'AppDBContext.Processes'  is null.");
            }
            var process = await _processService.GetByIdAsync(id,p=>p.Products);
            if(process==null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (process.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            //process can't be deleted if it has products
            if (process.Products.Count > 0)
            {
                ModelState.AddModelError(String.Empty, "Process can't be deleted if it has products.");
                return View(process);
            }
            await _processService.DeleteAsync(process.Id);
            //return redirect to ProcessCategories/details
            return RedirectToAction("Details", "ProcessCategories", new { id = process.CategoryId });
        }
        // GET: Processes/CreateStatus
        public async Task<IActionResult> CreateStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var process = await _processService.GetByIdAsync((int)id);
            if (process == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (process.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(new ProcessStatus()
            {
                ProcessId = process.Id,
                Process = process
            });
        }
        // POST: Processes/CreateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStatus([Bind("Name,ProcessId")] ProcessStatus status)
        {
            if (ModelState.IsValid)
            {
                await _processStatusService.AddAsync(status);
                //return redirect to ProcessCategories/details
                return RedirectToAction("Edit", new { id = status.ProcessId });
            }
            status.Process = await _processService.GetByIdAsync(status.ProcessId);
            return View(status);
        }
        // GET: Processes/EditStatus/5
        public async Task<IActionResult> EditStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _processStatusService.GetByIdAsync((int)id, s => s.Process);
            if (status == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (status.Process.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            //Done status can't be edited
            if (status.Name == Consts.DoneStatus)
            {
                ModelState.AddModelError(String.Empty, "Done status can't be edited.");
                var process = await _processService.GetByIdAsync(status.ProcessId, p => p.Enterprise, p => p.Statuses, p => p.Category);
                return View("Edit", process);
            }
            return View(status);
        }
        // POST: Processes/EditStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id, [Bind("Id,Name,ProcessId")] ProcessStatus status)
        {
            if (id != status.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _processStatusService.UpdateAsync(status.Id, status);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcessExists(status.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return redirect to ProcessCategories/details
                return RedirectToAction("Edit", new { id = status.ProcessId });
            }
            status.Process = await _processService.GetByIdAsync(status.ProcessId);
            return View(status);
        }
        
        //Delete Status
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _processStatusService.GetByIdAsync(id,s=>s.Process,s=>s.Products);
            
            if (status == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (status.Process.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewData["CategoryId"] = new SelectList(_context.ProcessCategories
   .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name");
             //Done status can't be deleted
            if (status.Name == Consts.DoneStatus)
            {
                ModelState.AddModelError(String.Empty, "Done status can't be deleted.");
                var process = await _processService.GetByIdAsync(status.ProcessId, p => p.Enterprise, p => p.Statuses, p => p.Category);
                return View("Edit", process);
            }
            //status can't be deleted if it has products
            if (status.Products.Count > 0)
            {
                ModelState.AddModelError(String.Empty, "Status can't be deleted if it has products.");
                var process = await _processService.GetByIdAsync(status.ProcessId, p => p.Enterprise, p => p.Statuses, p => p.Category);
                
                return View("Edit",process);
            }
           
            await _processStatusService.DeleteAsync(status.Id);
            //return redirect to ProcessCategories/details
            return RedirectToAction("Edit", new { id = status.ProcessId });
        }
        private bool ProcessExists(int id)
        {
          return (_context.Processes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
