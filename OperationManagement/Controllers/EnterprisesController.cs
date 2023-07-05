using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Data.Static;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    [Authorize(Roles =UserRoles.User)]
    public class EnterprisesController : Controller
    {
        private readonly AppDBContext _context;

        public EnterprisesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Enterprises
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Enterprises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enterprises == null)
            {
                return NotFound();
            }

            var enterprise = await _context.Enterprises.FindAsync(id);
            if (enterprise == null)
            {
                return NotFound();
            }
            return View(enterprise);
        }

        private bool EnterpriseExists(int id)
        {
          return (_context.Enterprises?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
