using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Data.Common;
using OperationManagement.Data.Services;
using OperationManagement.Data.Static;
using OperationManagement.Data.ViewModels;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    [Authorize(Roles = UserRoles.User)]
    public class OrdersController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IDeliveryLocationService _deliveryLocationService;
        private readonly IAttachmentService _attachmentService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProductService _productService;
        public OrdersController(AppDBContext context,
            IOrderService orderService,
            ICustomerService customerService,
            IDeliveryLocationService deliveryLocationService,
            IAttachmentService attachmentService,
            IProductService productService,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _orderService = orderService;
            _customerService = customerService;
            _deliveryLocationService = deliveryLocationService;
            _attachmentService = attachmentService;
            _webHostEnvironment = webHostEnvironment;
            _productService=productService;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _orderService.GetAllAsync(o=>o.Customer,o=>o.DeliveryLocation));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetByIdAsync((int)id, o => o.Customer, o => o.DeliveryLocation, o => o.Attachments);
            if (order == null)
            {
                return NotFound();
            }
            order = _context.Orders
                .Where(o => o.Id == order.Id)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Category)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Components)
                            .ThenInclude(c => c.Component)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Measurements)
                            .ThenInclude(m => m.Measurement)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Processes)
                            .ThenInclude(pr => pr.Process)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Specifications)
                            .ThenInclude(s => s.Specification)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Specifications)
                            .ThenInclude(s => s.Option)
                    .Include(o => o.Products)
                        .ThenInclude(p => p.Specifications)
                            .ThenInclude(s => s.Status)
                    .FirstOrDefault();
            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CreateOrderVM()
            {
                Customers = await _customerService.GetAllAsync(),
                DeliveryLocations = await _deliveryLocationService.GetAllAsync()
            };
            return View(vm);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderVM OrderVM)
        {
            if (ModelState.IsValid)
            {
                await _orderService.AddAsync(OrderVM.Order);
                for(int i = 0;OrderVM.Attachments!=null&&OrderVM.Titles!=null&& i < OrderVM.Attachments.Count; i++)
                {
                    var filePath = await FilesManagement.SaveOrderAttachement(OrderVM.Attachments[i], OrderVM.Order.EnterpriseOrderNumber, OrderVM.Titles[i]);
                    if (filePath != null)
                    {
                        await _attachmentService.AddAsync(new Attachment()
                        {
                            OrderId=OrderVM.Order.Id,
                            FileURL=filePath,
                            Title = OrderVM.Titles[i]
                        });
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            OrderVM.Customers = await _customerService.GetAllAsync();
            OrderVM.DeliveryLocations = await _deliveryLocationService.GetAllAsync();
            return View(OrderVM);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetByIdAsync((int)id, o => o.DeliveryLocation, o => o.Customer, o => o.Attachments);
            if (order == null)
            {
                return NotFound();
            }
            var VM = new CreateOrderVM()
            {
                Order = order,
                Customers = await _customerService.GetAllAsync(),
                DeliveryLocations = await _deliveryLocationService.GetAllAsync()
            };
           return View(VM);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateOrderVM OrderVM)
        {
            if (id != OrderVM.Order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (OrderVM.Order.HandOverDate != null)
                        OrderVM.Order.IsHandOver = true;
                    await _orderService.UpdateAsync(OrderVM.Order.Id, OrderVM.Order);
                    for (int i = 0; OrderVM.Attachments != null && OrderVM.Titles != null && i < OrderVM.Attachments.Count; i++)
                    {
                        var filePath = await FilesManagement.SaveOrderAttachement(OrderVM.Attachments[i], OrderVM.Order.EnterpriseOrderNumber, OrderVM.Titles[i]);
                        if (filePath != null)
                        {
                            await _attachmentService.AddAsync(new Attachment()
                            {
                                OrderId = OrderVM.Order.Id,
                                FileURL = filePath,
                                Title = OrderVM.Titles[i]
                            });
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(OrderVM.Order.Id))
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

            var VM = new CreateOrderVM()
            {
                Order = await _orderService.GetByIdAsync(id,o=>o.Customer,o=>o.Attachments,o=>o.DeliveryLocation),
                Customers = await _customerService.GetAllAsync(),
                DeliveryLocations = await _deliveryLocationService.GetAllAsync()
            };
            return View(VM);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetByIdAsync((int)id, o => o.Customer, o => o.DeliveryLocation);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'AppDBContext.Orders'  is null.");
            }
            var order = await _orderService.GetByIdAsync((int)id);
            if (order != null)
            {
                await _orderService.DeleteAsync(order.Id);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var attachment = await _attachmentService.GetByIdAsync(id);
            if (attachment == null)
                return NotFound();

            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath,"wwwroot/" ,attachment.FileURL);

            if (System.IO.File.Exists(filePath))
            {
                // Read the file contents
                var fileContent = System.IO.File.ReadAllBytes(filePath);

                // Determine the file's MIME type
                var mimeType = "application/octet-stream";
                var fileExtension = Path.GetExtension(filePath);
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    mimeType = GetMimeType(fileExtension);
                }
                // Return the file as a download
                return File(fileContent, mimeType, attachment.Title+fileExtension);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var attachment = await _attachmentService.GetByIdAsync(id);
            if (attachment == null)
                return NotFound();
            if (FilesManagement.DeleteFile(attachment.FileURL))
            {
                await _attachmentService.DeleteAsync(attachment.Id);
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction("Details", new { id = attachment.OrderId });
        }
        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private string GetMimeType(string fileExtension)
        {
            // You can customize this method to provide MIME types for specific file extensions
            // Here, a simple mapping is used for some common file extensions

            switch (fileExtension.ToLower())
            {
                case ".txt":
                    return "text/plain";
                case ".pdf":
                    return "application/pdf";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
