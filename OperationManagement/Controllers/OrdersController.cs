using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(AppDBContext context,
            IOrderService orderService,
            ICustomerService customerService,
            IDeliveryLocationService deliveryLocationService,
            IAttachmentService attachmentService,
            IProductService productService,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _orderService = orderService;
            _customerService = customerService;
            _deliveryLocationService = deliveryLocationService;
            _attachmentService = attachmentService;
            _webHostEnvironment = webHostEnvironment;
            _productService=productService;
            _userManager=userManager;
        }

        // GET: Orders
        /*public async Task<IActionResult> Index()
        {
            var all = await _orderService.GetAllAsync(o => o.Customer, o => o.DeliveryLocation);
            var user = await _userManager.GetUserAsync(User);
            return View(all.Where(o=>o.Customer.EnterpriseId==user.EnterpriseId));
        }*/

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
            var user = await _userManager.GetUserAsync(User);
            if (order.Customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            order = _orderService.GetCompleteOrder(order.Id);
            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create(int CustomerId)
        {
            var customer = await _customerService.GetByIdAsync(CustomerId);
            if (customer == null)
            {
                return NotFound();
            }
            var allLocations = await _deliveryLocationService.GetAllAsync();
            var user = await _userManager.GetUserAsync(User);

            
            var vm = new CreateOrderVM()
            {
                Customer = customer ,
                EnterpriseId = customer.EnterpriseId,
                DeliveryLocations = allLocations.Where(l=>l.EnterpriseId==user.EnterpriseId)
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
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (EnterpriseOrderNumberExists(OrderVM.Order.EnterpriseOrderNumber, OrderVM.EnterpriseId,null))
                {
                    ModelState.AddModelError("Order.EnterpriseOrderNumber", $"The enterprise code {OrderVM.Order.EnterpriseOrderNumber} already used.");
                    var Locations = await _deliveryLocationService.GetAllAsync();
                    var model = new CreateOrderVM()
                    {
                        Order = OrderVM.Order,
                        Customer = await _customerService.GetByIdAsync(OrderVM.Order.CustomerId),
                        DeliveryLocations = Locations.Where(l => l.EnterpriseId == user.EnterpriseId)
                    };
                    return View(model);
                }
                var customer = await _customerService.GetByIdAsync(OrderVM.Order.CustomerId);
                if (customer.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _orderService.AddAsync(OrderVM.Order);
                for(int i = 0;OrderVM.Attachments!=null&&OrderVM.Titles!=null&& i < OrderVM.Attachments.Count; i++)
                {
                    var filePath = await FilesManagement.SaveOrderAttachement(OrderVM.Attachments[i], OrderVM.Order.EnterpriseOrderNumber, "Attachement");
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
                return RedirectToAction("Details", new {Id=OrderVM.Order.Id});
            }
            var allLocations = await _deliveryLocationService.GetAllAsync();
            OrderVM.Customer = await _customerService.GetByIdAsync(OrderVM.Order.CustomerId);
            OrderVM.DeliveryLocations = allLocations.Where(l=>l.EnterpriseId==user.EnterpriseId);
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
            var allLocations = await _deliveryLocationService.GetAllAsync();

            var user = await _userManager.GetUserAsync(User);
            if (order.Customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var customer = await _customerService.GetByIdAsync(order.CustomerId);
            var VM = new CreateOrderVM()
            {
                Order = order,
                Customer = customer,
                EnterpriseId = customer.EnterpriseId,
                DeliveryLocations = allLocations.Where(l=>l.EnterpriseId==user.EnterpriseId)
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
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (EnterpriseOrderNumberExists(OrderVM.Order.EnterpriseOrderNumber,OrderVM.EnterpriseId,OrderVM.Order.Id))
                {
                    ModelState.AddModelError("Order.EnterpriseOrderNumber", $"The enterprise code {OrderVM.Order.EnterpriseOrderNumber} already used.");
                    var Locations = await _deliveryLocationService.GetAllAsync();
                    var dbOrder = await _orderService.GetByIdAsync(id, o => o.Customer, o => o.Attachments, o => o.DeliveryLocation);
                    var model = new CreateOrderVM()
                    {
                        Order = dbOrder,
                        Customer = await _customerService.GetByIdAsync(dbOrder.CustomerId),
                        DeliveryLocations = Locations.Where(l => l.EnterpriseId == user.EnterpriseId)
                    };
                    return View(model);
                }
                try
                    {
                    var customer = await _customerService.GetByIdAsync(OrderVM.Order.CustomerId);
                    if (customer.EnterpriseId != user.EnterpriseId)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    if (OrderVM.Order.HandOverDate != null)
                        OrderVM.Order.IsHandOver = true;
                    OrderVM.Order.CustomerId = customer.Id;
                    await _orderService.UpdateAsync(OrderVM.Order.Id, OrderVM.Order);
                    for (int i = 0; OrderVM.Attachments != null && OrderVM.Titles != null && i < OrderVM.Attachments.Count; i++)
                    {
                        var filePath = await FilesManagement.SaveOrderAttachement(OrderVM.Attachments[i], OrderVM.Order.EnterpriseOrderNumber, "Attachement");
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
                return RedirectToAction("Details", new {Id=OrderVM.Order.Id});
            }
            var AllLocations = await _deliveryLocationService.GetAllAsync();
            var order = await _orderService.GetByIdAsync(id, o => o.Customer, o => o.Attachments, o => o.DeliveryLocation);
            var VM = new CreateOrderVM()
            {
                Order =order,
                Customer =await _customerService.GetByIdAsync(order.CustomerId),
                DeliveryLocations = AllLocations.Where(l=>l.EnterpriseId==user.EnterpriseId) 
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
            var user = await _userManager.GetUserAsync(User);
            if (order.Customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
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
            var order = await _orderService.GetByIdAsync((int)id,c=>c.Customer);
            var customerId = order.CustomerId;
            if (order != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (order.Customer.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _orderService.DeleteCompleteOrder(order.Id);
            }
            return RedirectToAction("Details", "Customers", new {id=customerId});
        }
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var attachment = await _attachmentService.GetByIdAsync(id);
            if (attachment == null)
                return NotFound();
            var order = await _orderService.GetByIdAsync(attachment.OrderId, o => o.Customer);
            var user = await _userManager.GetUserAsync(User);
            if (order.Customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
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
            var order = await _orderService.GetByIdAsync(attachment.OrderId, o => o.Customer);
            var user = await _userManager.GetUserAsync(User);
            if (order.Customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
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
        private bool EnterpriseOrderNumberExists(string code,int enterpriseId, int? orderId)
        {
            return (_context.Orders?.Any(e =>e.Customer.EnterpriseId==enterpriseId
            && e.EnterpriseOrderNumber == code
            && (orderId==null?true:e.Id!=orderId))).GetValueOrDefault();
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
