using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Data.Services;
using OperationManagement.Data.Static;
using OperationManagement.Models;
using NPOI;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Component = OperationManagement.Models.Component;

namespace OperationManagement.Controllers
{
    [Authorize(Roles = UserRoles.User)]
    public class CustomersController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly ICustomerContactService _customerContactService;
        private readonly IOrderService _orderService;
        public CustomersController(AppDBContext context,
            UserManager<ApplicationUser> userManager,
            ICustomerService customerService,
            ICustomerContactService customerContactService,
            IOrderService orderService)
        {
            _context = context;
            _userManager = userManager;
            _customerService = customerService;
            _customerContactService = customerContactService;
            _orderService = orderService;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string? email, string? name, string? phone)
        {
            var all = await _customerService.GetAllAsync(c => c.Enterprise);
            var user = await _userManager.GetUserAsync(User);
            all = all.Where(c => c.EnterpriseId == user.EnterpriseId);
            if (!string.IsNullOrEmpty(email))
            {
                var Nemail = _userManager.NormalizeEmail(email);
                all = all.Where(c => c.NormalizedEmail?.Contains(Nemail) ?? false);
                ViewBag.Email = email;
            }
            if (!string.IsNullOrEmpty(name))
            {
                var Nname = _userManager.NormalizeName(name);
                all = all.Where(c => c.NormalizedName.Contains(Nname));
                ViewBag.Name = name;
            }
            if (!string.IsNullOrEmpty(phone))
            {
                all = all.Where(c => c.Phone.Contains(phone));
                ViewBag.Phone = phone;
            }
            return View(all);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetByIdAsync((int)id, c => c.Enterprise, c => c.Contacts, c => c.Orders);
            if (customer == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(customer);
        }

        // GET: Customers/Create
        public async Task<IActionResult> CreateAsync()
        {

            var user = await _userManager.GetUserAsync(User);
            var customer = new Customer
            {
                Contacts = new List<CustomerContact>(),
                EnterpriseId = (int)user.EnterpriseId
            };
            return View(customer);
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Phone,Name,Email,NationalId,Nationality,Gender,BirthDate,EnterpriseId")] Customer customer, CustomerContact[] Contacts)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (customer.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                if (await PhoneNumberExists(customer.Phone, customer.EnterpriseId, null))
                {
                    customer.Contacts = new List<CustomerContact>();
                    ModelState.AddModelError("Phone", "This Phone number is registered already.");
                    return View(customer);
                }
                customer.NormalizedName = _userManager.NormalizeName(customer.Name);
                customer.NormalizedEmail = _userManager.NormalizeEmail(customer.Email);
                await _customerService.AddAsync(customer);
                foreach (var contact in Contacts)
                {
                    await _customerContactService.AddAsync(new CustomerContact()
                    {
                        CustomerId = customer.Id,
                        Type = contact.Type,
                        Value = contact.Value
                    });
                }
                return RedirectToAction(nameof(Index));
            }
            customer.Contacts = new List<CustomerContact>();
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetByIdAsync((int)id, c => c.Enterprise, c => c.Contacts);
            if (customer == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Phone,Name,Email,NationalId,Nationality,Gender,BirthDate,EnterpriseId,Contacts")] Customer customer, CustomerContact[] Contacts)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (customer.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                if (await PhoneNumberExists(customer.Phone, customer.EnterpriseId, customer.Id))
                {
                    //customer.Contacts = new List<CustomerContact>();
                    ModelState.AddModelError("Phone", "This Phone number is registered already.");
                    return View(customer);
                }
                try
                {
                    customer.NormalizedName = _userManager.NormalizeName(customer.Name);
                    customer.NormalizedEmail = _userManager.NormalizeEmail(customer.Email);
                    await _customerService.UpdateAsync(customer.Id, customer);
                    customer.Contacts = _customerContactService.getByCustomerId(customer.Id);
                    customer.Contacts.Clear();
                    foreach (var contact in Contacts)
                    {
                        await _customerContactService.AddAsync(new CustomerContact()
                        {
                            CustomerId = customer.Id,
                            Type = contact.Type,
                            Value = contact.Value
                        });
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            customer.Contacts = new List<CustomerContact>();
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetByIdAsync((int)id, c => c.Enterprise, c => c.Contacts);

            if (customer == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'AppDBContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);

            if (customer != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (customer.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                await _customerService.DeleteCompleteCustomer(customer.Id);
            }

            return RedirectToAction(nameof(Index));
        }
        //export to excel
        public async Task<IActionResult> ExportExcel(int id)
        {
            var customer = await _customerService.GetByIdAsync(id, c => c.Enterprise);
            if (customer == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            customer.Orders = _orderService.GetOrdersByCustomerId(customer.Id);
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Main Info");
            // print setup
            sheet.PrintSetup.PaperSize = (short)PaperSize.A4;
            sheet.PrintSetup.Landscape = true;
            sheet.PrintSetup.FitWidth = 1;
            sheet.PrintSetup.FitHeight = 0;


            // Create a cell style with borders
            ICellStyle borderStyle = workbook.CreateCellStyle();
            borderStyle.BorderTop = BorderStyle.Thin;
            borderStyle.BorderBottom = BorderStyle.Thin;
            borderStyle.BorderLeft = BorderStyle.Thin;
            borderStyle.BorderRight = BorderStyle.Thin;

            //write customer info 
            int rowCounter = 0;
            int column = 0;
            //write enterprise info
            IRow row = sheet.CreateRow(rowCounter++);
            row.CreateCell(0).SetCellValue("Enterprise Info");
            row = sheet.CreateRow(rowCounter++);

            row.CreateCell(0).SetCellValue("Enterprise " + nameof(customer.Enterprise.Name));
            row.CreateCell(1).SetCellValue(customer.Enterprise.Name);

            row = sheet.CreateRow(rowCounter++);
            row.CreateCell(0).SetCellValue("Export Date");
            row.CreateCell(1).SetCellValue(DateTime.Now.ToString("dd/MM/yyyy"));



            #region Customer Info
            row = sheet.CreateRow(rowCounter++);
            row.CreateCell(0).SetCellValue("Customer Info");

            row = sheet.CreateRow(rowCounter++);
            row.CreateCell(0).SetCellValue(nameof(customer.Name));
            row.CreateCell(1).SetCellValue(customer.Name);

            row = sheet.CreateRow(rowCounter++);
            row.CreateCell(0).SetCellValue(nameof(customer.Phone));
            row.CreateCell(1).SetCellValue(customer.Phone);

            row = sheet.CreateRow(rowCounter++);
            row.CreateCell(0).SetCellValue(nameof(customer.Email));
            row.CreateCell(1).SetCellValue(customer.Email);

            row = sheet.CreateRow(rowCounter++);
            row.CreateCell(0).SetCellValue(nameof(customer.NationalId));
            row.CreateCell(1).SetCellValue(customer.NationalId);
            #endregion

            sheet.AutoSizeColumn(0);
            sheet.AutoSizeColumn(1);
            foreach (var order in customer.Orders)
            {
                //order code
                var orderCode = order.EnterpriseOrderNumber.ToString();
                //max sheet name is 31 char
                orderCode = orderCode.Length > 20 ? orderCode.Substring(0, 20) : orderCode;
                //sheet for each order
                sheet = workbook.CreateSheet($"Order {orderCode}");
                // print setup
                sheet.PrintSetup.PaperSize = (short)PaperSize.A4;
                sheet.PrintSetup.Landscape = true;
                sheet.PrintSetup.FitWidth = 1;
                sheet.PrintSetup.FitHeight = 0;

                rowCounter = 0;
                column = 0;
                //Order Info
                #region Order Info
                row = sheet.CreateRow(rowCounter++);

                row.CreateCell(0).SetCellValue(nameof(order.EnterpriseOrderNumber));
                row.CreateCell(1).SetCellValue(order.EnterpriseOrderNumber);

                row = sheet.CreateRow(rowCounter++);
                row.CreateCell(0).SetCellValue(nameof(order.Address));
                row.CreateCell(1).SetCellValue(order.Address);

                row = sheet.CreateRow(rowCounter++);
                row.CreateCell(0).SetCellValue(nameof(order.ContractDate));
                row.CreateCell(1).SetCellValue(order.ContractDate.ToShortDateString());

                row = sheet.CreateRow(rowCounter++);
                row.CreateCell(0).SetCellValue(nameof(order.DeliveryLocation));
                row.CreateCell(1).SetCellValue(order.DeliveryLocation?.Name);

                //add blank row
                row = sheet.CreateRow(rowCounter++);

                #endregion
                row = sheet.CreateRow(rowCounter++);
                row.CreateCell(0).SetCellValue("Products");
                foreach (var product in order.Products)
                {
                    column = 0;
                    //Product Info
                    #region Product Info
                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(0).SetCellValue("Product " + nameof(product.Name));
                    row.CreateCell(1).SetCellValue(product.Name);

                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(0).SetCellValue(nameof(product.Category));
                    row.CreateCell(1).SetCellValue(product.Category?.Name);

                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(0).SetCellValue(nameof(product.Quantity));
                    row.CreateCell(1).SetCellValue(product.Quantity.ToString());

                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(0).SetCellValue(nameof(product.Price));
                    row.CreateCell(1).SetCellValue(product.Price.ToString());

                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(0).SetCellValue(nameof(product.Progress));
                    row.CreateCell(1).SetCellValue(product.Progress.ToString() + "%");
                    #endregion

                    //Product Specifications
                    #region Product Specifications
                    column = 0;
                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(0).SetCellValue("Specifications");
                    row = sheet.CreateRow(rowCounter++);

                    row.CreateCell(column).SetCellValue(nameof(Specification));
                    row.GetCell(column++).CellStyle = borderStyle;

                    row.CreateCell(column).SetCellValue(nameof(ProductSpecification.Option));
                    row.GetCell(column++).CellStyle = borderStyle;

                    row.CreateCell(column).SetCellValue(nameof(ProductSpecification.Status));
                    row.GetCell(column++).CellStyle = borderStyle;

                    foreach (var specification in product.Specifications)
                    {
                        column = 0;
                        row = sheet.CreateRow(rowCounter++);
                        row.CreateCell(column).SetCellValue(specification.Specification?.Name);
                        row.GetCell(column++).CellStyle = borderStyle;

                        row.CreateCell(column).SetCellValue(specification.Option?.Name);
                        row.GetCell(column++).CellStyle = borderStyle;

                        row.CreateCell(column).SetCellValue(specification.Status?.Name);
                        row.GetCell(column++).CellStyle = borderStyle;
                    }
                    #endregion
                    //Product Processes
                    #region Product processes
                    column = 0;
                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(0).SetCellValue("Processes");
                    row = sheet.CreateRow(rowCounter++);

                    row.CreateCell(column).SetCellValue(nameof(Process));
                    row.GetCell(column++).CellStyle = borderStyle;

                    row.CreateCell(column).SetCellValue(nameof(ProductProcess.Status));
                    row.GetCell(column++).CellStyle = borderStyle;

                    row.CreateCell(column).SetCellValue(nameof(ProductProcess.StartDate));
                    row.GetCell(column++).CellStyle = borderStyle;

                    row.CreateCell(column).SetCellValue(nameof(ProductProcess.EndDate));
                    row.GetCell(column++).CellStyle = borderStyle;

                    foreach (var process in product.Processes)
                    {
                        column = 0;
                        row = sheet.CreateRow(rowCounter++);
                        row.CreateCell(column).SetCellValue(process.Process?.Name);
                        row.GetCell(column++).CellStyle = borderStyle;

                        row.CreateCell(column).SetCellValue(process.Status?.Name);
                        row.GetCell(column++).CellStyle = borderStyle;

                        row.CreateCell(column).SetCellValue(process.StartDate?.ToShortDateString());
                        row.GetCell(column++).CellStyle = borderStyle;

                        row.CreateCell(column).SetCellValue(process.EndDate?.Date.ToShortDateString());
                        row.GetCell(column++).CellStyle = borderStyle;
                    }
                    #endregion
                    //Product Measurements
                    #region  Product Measurements
                    column = 0;
                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(0).SetCellValue("Measurements");
                    row = sheet.CreateRow(rowCounter++);

                    row.CreateCell(column).SetCellValue(nameof(ProductMeasurement.Measurement));
                    row.GetCell(column++).CellStyle = borderStyle;

                    row.CreateCell(column).SetCellValue(nameof(ProductMeasurement.Value));
                    row.GetCell(column++).CellStyle = borderStyle;

                    foreach (var measurement in product.Measurements)
                    {
                        column = 0;
                        row = sheet.CreateRow(rowCounter++);
                        row.CreateCell(column).SetCellValue(measurement.Measurement?.Name);
                        row.GetCell(column++).CellStyle = borderStyle;

                        row.CreateCell(column).SetCellValue(measurement.Value.ToString());
                        row.GetCell(column++).CellStyle = borderStyle;
                    }
                    #endregion
                    //Product Components
                    #region Product Components
                    column = 0;
                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(0).SetCellValue("Components");
                    row = sheet.CreateRow(rowCounter++);
                    row.CreateCell(column).SetCellValue(nameof(ProductComponent.Component));
                    row.GetCell(column++).CellStyle = borderStyle;

                    row.CreateCell(column).SetCellValue(nameof(ProductComponent.Quantity));
                    row.GetCell(column++).CellStyle = borderStyle;

                    foreach (var component in product.Components)
                    {
                        column = 0;
                        row = sheet.CreateRow(rowCounter++);
                        row.CreateCell(column).SetCellValue(component.Component?.Name);
                        row.GetCell(column++).CellStyle = borderStyle;

                        row.CreateCell(column).SetCellValue(component.Quantity.ToString());
                        row.GetCell(column++).CellStyle = borderStyle;
                    }
                    #endregion
                    row = sheet.CreateRow(rowCounter++);
                    //make column width fit content automatically
                    for (int i = 0; i < 10; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }

                }
            }
            using (var stream = new MemoryStream())
            {
                workbook.Write(stream);
                string excelName = $"{customer.Name}.xlsx";
                Response.ContentType = "application/vnd.ms-excel";
                Response.Headers.Add("content-disposition", $"attachment; filename={excelName}");
                return File(stream.GetBuffer(), "application/vnd.ms-excel", excelName);
            }
        }
        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private async Task<bool> PhoneNumberExists(string phone, int enterpriseId, int? customerId)
        {
            return (_context.Customers?.Any(e => e.EnterpriseId == enterpriseId
            && e.Phone == phone
            && (customerId == null ? true : e.Id != customerId))
            ).GetValueOrDefault();

        }
    }
}
