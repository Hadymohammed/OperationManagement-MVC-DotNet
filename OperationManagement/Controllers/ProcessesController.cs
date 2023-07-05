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
        private readonly UserManager<ApplicationUser> _userManager;

        public ProcessesController(AppDBContext context,
            IProcessService processService,
            IProcessStatusService processStatusService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _processService = processService;
            _processStatusService = processStatusService;
            _userManager = userManager;
        }

        // GET: Processes
        public async Task<IActionResult> Index()
        {
            var all = await _processService.GetAllAsync();
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

            var process = await _processService.GetByIdAsync((int)id, p => p.Enterprise, p => p.Statuses);
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
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
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
        public async Task<IActionResult> Create([Bind("Name,EnterpriseId")] Process process, ProcessStatus[] statuses)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (process.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _processService.AddAsync(process);
                foreach(var status in statuses)
                {
                    await _processStatusService.AddAsync(new ProcessStatus()
                    {
                        ProcessId = process.Id,
                        Name = status.Name,
                    });
                }
                return RedirectToAction(nameof(Index));
            }
            return View(process);
        }

        // GET: Processes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Processes == null)
            {
                return NotFound();
            }

            var process = await _processService.GetByIdAsync((int)id, p => p.Enterprise, p => p.Statuses);
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

        // POST: Processes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EnterpriseId")] Process process, ProcessStatus[] statuses)
        {
            if (id != process.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (process.EnterpriseId != user.EnterpriseId)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    await _processService.UpdateAsync(process.Id, process);
                    process.Statuses = _processStatusService.GetByProcessId(process.Id);
                    process.Statuses.Clear();
                    foreach (var status in statuses)
                    {
                        await _processStatusService.AddAsync(new ProcessStatus()
                        {
                            ProcessId = process.Id,
                            Name = status.Name,
                        });
                    }
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(process);
        }

        // GET: Processes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Processes == null)
            {
                return NotFound();
            }

            var process = await _processService.GetByIdAsync((int)id, p => p.Enterprise, p => p.Statuses);
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
            var process = await _processService.GetByIdAsync(id);
            if (process != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (process.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _processService.DeleteAsync(process.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProcessExists(int id)
        {
          return (_context.Processes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
