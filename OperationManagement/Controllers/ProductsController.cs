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
using OperationManagement.Data.ViewModels;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
    [Authorize(Roles = UserRoles.User)]
    public class ProductsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IProductService _productService;
        private readonly ISpecificationService _specificationService;
        private readonly IProcessService _processService;
        private readonly IMeasurementService _measurementService;
        private readonly IComponentService _componentService;
        private readonly IProductComponentService _productComponentService;
        private readonly IProductMeasurementService _productMeasurementService;
        private readonly IProductProcessService _productProcessService;
        private readonly IProductSpecificationService _productSpecificationService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(AppDBContext context,
            IProductService productService,
            ISpecificationService specificationService,
            IProcessService processService,
            IMeasurementService measurementService,
            IComponentService componentService,
            IProductComponentService productComponentService,
            IProductMeasurementService productMeasurementService,
            IProductProcessService productProcessService,
            IProductSpecificationService productSpecificationService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _productService = productService;
            _specificationService = specificationService;
            _processService = processService;
            _measurementService = measurementService;
            _componentService = componentService;
            _productComponentService = productComponentService;
            _productMeasurementService = productMeasurementService;
            _productProcessService = productProcessService;
            _productSpecificationService = productSpecificationService;
            _orderService = orderService;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? orderId,int? categoryId,int? processId,int? processCategoryId)
        {

            var all = await _productService.GetAllAsync(p => p.Category, p => p.Order,p=>p.Order.Customer,p=>p.Processes);
            var user = await _userManager.GetUserAsync(User);
            #region ViewList
            ViewData["OrderId"] = new SelectList(_context.Orders.Include(o => o.Customer)
                .Where(o => o.Customer.EnterpriseId == user.EnterpriseId), "Id", "EnterpriseOrderNumber");

            // Create a new SelectList with the custom option and assign it to the OrderId ViewData
            var orderIdList = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "0" }
            };
            orderIdList.AddRange((SelectList)ViewData["OrderId"]);
            ViewData["OrderId"] = new SelectList(orderIdList, "Value", "Text", (orderId != null ? orderId : 0));

            ViewData["CategoryId"] = new SelectList(_context.Categories
                .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name");

            // Create a new SelectList with the custom option and assign it to the CategoryId ViewData
            var categoryIdList = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "0" }
            };
            categoryIdList.AddRange((SelectList)ViewData["CategoryId"]);
            ViewData["CategoryId"] = new SelectList(categoryIdList, "Value", "Text", (categoryId != null ? categoryId : 0));

            ViewData["ProcessId"] = new SelectList(_context.Processes
                .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name");

            // Create a new SelectList with the custom option and assign it to the ProcessId ViewData
            var processIdList = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "0" }
            };
            processIdList.AddRange((SelectList)ViewData["ProcessId"]);
            ViewData["ProcessId"] = new SelectList(processIdList, "Value", "Text", (processId != null ? processId : 0));

            ViewData["ProcessCategoryId"] = new SelectList(_context.ProcessCategories
                .Where(o => o.EnterpriseId == user.EnterpriseId), "Id", "Name");

            // Create a new SelectList with the custom option and assign it to the CategoryId ViewData
            var processCategoryIdList = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "0" }
            };
            processCategoryIdList.AddRange((SelectList)ViewData["ProcessCategoryId"]);
            ViewData["ProcessCategoryId"] = new SelectList(processCategoryIdList, "Value", "Text", (processCategoryId != null ? processCategoryId : 0));

            #endregion
            /*Search*/
            if (orderId != null)
            {
                all = all.Where(p => p.OrderId == orderId);
            }
            if (categoryId != null)
            {
                all = all.Where(p => p.CategoryId == categoryId);
            }
            if(processId != null)
            {
                all = all.Where(p => p.Processes.Any(pr => pr.ProcessId == processId));
            }
            if (processCategoryId != null)
            {
                all = all.Where(p => p.Processes.Any(pr => pr.Process?.CategoryId == processCategoryId));
            }
            return View(all.Where(p=>p.Order.Customer.EnterpriseId==user.EnterpriseId));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = _productService.getCompleteProductById((int)id);
            if (product == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (product.Order.Customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create(int?orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(e=>e.EnterpriseId==user.EnterpriseId), "Id", "Name");
            ViewData["OrderId"] = new SelectList(_context.Orders.Include(o => o.Customer)
                    .Where(o => o.Customer.EnterpriseId == user.EnterpriseId), "Id", "EnterpriseOrderNumber",(orderId!=null ? orderId:null));

            var allSpecs = await _specificationService.GetAllAsync(s => s.Statuses, s => s.Options);
            var allComps = await _componentService.GetAllAsync(c => c.Photos);
            var allProcess = await _processService.GetAllAsync(p => p.Statuses);
            var allMeags = await _measurementService.GetAllAsync();
            return View(new CreateProductVM
            {
                Specifications=allSpecs.Where(s=>s.EnterpriseId==user.EnterpriseId),
                Components= allComps.Where(c=>c.EnterpriseId==user.EnterpriseId),
                Processes= allProcess.Where(p=>p.EnterpriseId==user.EnterpriseId),
                Measurements = allMeags.Where(m=> m.EnterpriseId==user.EnterpriseId),
            });
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (ModelState.IsValid)
            {
                var order = await _orderService.GetByIdAsync(vm.Product.OrderId,o=>o.Customer);
                if (order.Customer.EnterpriseId != user.EnterpriseId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _productService.AddAsync(vm.Product);
                //add measurements
                if (vm.ProductMeasurements != null)
                {
                    foreach(var m in vm.ProductMeasurements)
                    {
                        if (m.Value != null)
                        {
                            await _productMeasurementService.AddAsync(new ProductMeasurement()
                            {
                                ProductId = vm.Product.Id,
                                MeasurementId = m.MeasurementId,
                                Value = m.Value,
                                Unit = m.Unit
                            });
                        }
                    }
                }
                //add components
                if (vm.ProductComponents != null)
                {
                    foreach(var comp in vm.ProductComponents)
                    {
                        if (comp.Item1)
                        {
                            await _productComponentService.AddAsync(new ProductComponent() { 
                                ProductId=vm.Product.Id,
                                ComponentId= comp.Item2.ComponentId,
                                Quantity=comp.Item2.Quantity
                            });
                        }
                    }
                }
                //add specifications
                if (vm.ProductSpecifications != null)
                {
                    foreach(var spec in vm.ProductSpecifications)
                    {
                        if (spec.Item1)
                        {
                            await _productSpecificationService.AddAsync(new ProductSpecification()
                            {
                                ProductId = vm.Product.Id,
                                SpecificationId = spec.Item2.SpecificationId,
                                StatusId=spec.Item2.StatusId,
                                OptionId = spec.Item2.OptionId,
                                Remark = spec.Item2.Remark,
                                Date = spec.Item2.Date
                            });
                        }
                    }
                }
                //add process
                if (vm.ProductProcesses != null)
                {
                    foreach(var process in vm.ProductProcesses)
                    {
                        if (process.Item1)
                        {
                            var diffDays = 0;
                            if (process.Item2.StartDate != null && process.Item2.EndDate != null)
                            {
                                TimeSpan timeDiff = (TimeSpan)(process.Item2.EndDate - process.Item2.StartDate);
                                diffDays = timeDiff.Days;
                            }
                            await _productProcessService.AddAsync(new ProductProcess()
                            {
                                ProductId = vm.Product.Id,
                                ProcessId = process.Item2.ProcessId,
                                StatusId = process.Item2.StatusId,
                                Comment = process.Item2.Comment,
                                StartDate = process.Item2.StartDate,
                                EndDate = process.Item2.EndDate,
                                EstimatedDuration = diffDays
                            });
                        }
                    }
                }
                await _orderService.UpdateProgress(order.Id);
                return RedirectToAction("Details", new {Id=vm.Product.Id});
            }
            var allSpecs = await _specificationService.GetAllAsync(s => s.Statuses, s => s.Options);
            var allComps = await _componentService.GetAllAsync(c => c.Photos);
            var allProcess = await _processService.GetAllAsync(p => p.Statuses);
            var allMeags = await _measurementService.GetAllAsync();
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(e => e.EnterpriseId == user.EnterpriseId), "Id", "Name", vm.Product.CategoryId);
            ViewData["OrderId"] = new SelectList(_context.Orders.Include(o => o.Customer)
                    .Where(o => o.Customer.EnterpriseId == user.EnterpriseId), "Id", "EnterpriseOrderNumber", vm.Product.OrderId);
            vm.Specifications = allSpecs.Where(s=>s.EnterpriseId==user.EnterpriseId);
            vm.Components = allComps.Where(s => s.EnterpriseId == user.EnterpriseId);
            vm.Processes = allProcess.Where(s => s.EnterpriseId == user.EnterpriseId);
            vm.Measurements = allMeags.Where(s => s.EnterpriseId == user.EnterpriseId);
            return View(vm);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = _productService.getCompleteProductById((int)id);
            if (product == null)
            {
                return NotFound();
            }
            if (product.Order.Customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var AllComponents =await _componentService.GetAllAsync(c=>c.Photos);
            var AllSpecifications =await _specificationService.GetAllAsync(s=>s.Statuses,s=>s.Options);
            var AllMeasurements =await _measurementService.GetAllAsync();
            var AllProcess =await _processService.GetAllAsync(p=>p.Statuses);
            AllComponents = AllComponents.Where(p => p.EnterpriseId == (int)user.EnterpriseId);
            AllSpecifications = AllSpecifications.Where(p => p.EnterpriseId == (int)user.EnterpriseId);
            AllMeasurements = AllMeasurements.Where(p => p.EnterpriseId == (int)user.EnterpriseId);
            AllProcess = AllProcess.Where(p => p.EnterpriseId == (int)user.EnterpriseId);
            var vm = new CreateProductVM()
            {
                CategoryId = product.CategoryId,
                Product = product,
                Measurements = AllMeasurements,
                Specifications = AllSpecifications,
                Processes = AllProcess,
                Components = AllComponents,
                ProductComponents = new List<TupleVM<bool,ProductComponent>>(),
                ProductMeasurements=new List<ProductMeasurement>(),
                ProductProcesses=new List<TupleVM<bool,ProductProcess>>(),
                ProductSpecifications=new List<TupleVM<bool,ProductSpecification>>()
            };
            foreach (var comp in AllComponents)
            {
                if (product.Components.Any(c => c.ComponentId == comp.Id))
                {
                    vm.ProductComponents.Add(new TupleVM<bool, ProductComponent>()
                    {
                        Item1 = true,
                        Item2 = _productComponentService.GetComponentInProduct(comp.Id,product.Id),
                    });
                }
                else
                {
                    vm.ProductComponents.Add(new TupleVM<bool, ProductComponent>()
                    {
                        Item1 = false,
                        Item2 = new ProductComponent(),
                    });
                }
            }
            foreach (var spec in AllSpecifications)
            {
                if (product.Specifications.Any(c => c.SpecificationId == spec.Id))
                {
                    vm.ProductSpecifications.Add(new TupleVM<bool, ProductSpecification>()
                    {
                        Item1 = true,
                        Item2 = _productSpecificationService.GetSpecificationInProduct(spec.Id, product.Id),
                    });
                }
                else
                {
                    vm.ProductSpecifications.Add(new TupleVM<bool, ProductSpecification>()
                    {
                        Item1 = false,
                        Item2 = new ProductSpecification()
                    });

                }
            }
            foreach (var proc in AllProcess)
            {
                if (product.Processes.Any(c => c.ProcessId == proc.Id))
                {
                    vm.ProductProcesses.Add(new TupleVM<bool, ProductProcess>()
                    {
                        Item1 = true,
                        Item2 = _productProcessService.GetProcessInProduct(proc.Id, product.Id),
                    });
                }
                else
                {
                    vm.ProductProcesses.Add(new TupleVM<bool, ProductProcess>()
                    {
                        Item1 = false,
                        Item2 = new ProductProcess(),
                    });

                }

            }
            foreach (var meag in AllMeasurements)
            {
                if (product.Measurements.Any(c => c.MeasurementId == meag.Id))
                {
                    vm.ProductMeasurements.Add(_productMeasurementService.GetMeasurementInProduct(meag.Id, product.Id));
                }
                else
                {
                    vm.ProductMeasurements.Add(new ProductMeasurement());
                }

            }
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(e => e.EnterpriseId == user.EnterpriseId), "Id", "Name", product.CategoryId);
            ViewData["OrderId"] = new SelectList(_context.Orders.Include(o=>o.Customer).Where(e => e.Customer.EnterpriseId == user.EnterpriseId), "Id", "EnterpriseOrderNumber", product.OrderId);
            return View(vm);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkI7778/8d=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,CreateProductVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _orderService.GetByIdAsync(vm.Product.OrderId,o=>o.Customer);
            if (id != vm.Product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(order.Customer.EnterpriseId!=user.EnterpriseId)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    await _productService.UpdateAsync(id, vm.Product);
                    /*Update details*/
                    var product = _productService.getCompleteProductById(vm.Product.Id);
                    //add measurements
                    if (vm.ProductMeasurements != null)
                    {
                        foreach (var m in vm.ProductMeasurements)
                        {
                            var meag = product.Measurements.Where(me => me.MeasurementId == m.MeasurementId).FirstOrDefault();
                            if(meag!=null)
                            {
                                if (m.Value != null)
                                {
                                    meag.Value = m.Value;
                                    meag.Unit = m.Unit;
                                    await _productMeasurementService.UpdateAsync(meag.Id, meag);
                                }
                                else
                                {
                                    await _productMeasurementService.DeleteAsync(meag.Id);
                                }
                            }
                            else
                            {
                                if (m.Value != null)
                                {
                                    m.ProductId = product.Id;
                                    await _productMeasurementService.AddAsync(m);
                                }
                            }
                        }
                    }
                    //add components
                    if (vm.ProductComponents != null)
                    {
                        foreach (var comp in vm.ProductComponents)
                        {
                            var component = product.Components.Where(c => c.ComponentId == comp.Item2.ComponentId).FirstOrDefault();
                            if (comp.Item1)
                            {
                                if (component!=null)
                                {
                                    component.Quantity = comp.Item2.Quantity;
                                    component.Unit = comp.Item2.Unit;
                                    await _productComponentService.UpdateAsync(component.Id, component);
                                }
                                else
                                {
                                    await _productComponentService.AddAsync(new ProductComponent()
                                    {
                                        ProductId = vm.Product.Id,
                                        ComponentId = comp.Item2.ComponentId,
                                        Quantity = comp.Item2.Quantity,
                                        Unit = comp.Item2.Unit
                                    });
                                }
                            }
                            else
                            {
                                if (component != null)
                                {
                                    await _productComponentService.DeleteAsync(component.Id);
                                }
                            }
                        }
                    }
                    //add specifications
                    if (vm.ProductSpecifications != null)
                    {
                        foreach (var spec in vm.ProductSpecifications)
                        {
                            var specification = product.Specifications.Where(s => s.SpecificationId == spec.Item2.SpecificationId).FirstOrDefault();
                            if (spec.Item1)
                            {
                                if (specification!=null)
                                {
                                    specification.Remark = spec.Item2.Remark;
                                    specification.OptionId = spec.Item2.OptionId;
                                    specification.StatusId = spec.Item2.StatusId;
                                    specification.Date = spec.Item2.Date;
                                    await _productSpecificationService.UpdateAsync(specification.Id, specification);
                                }
                                else
                                {
                                    await _productSpecificationService.AddAsync(new ProductSpecification()
                                    {
                                        ProductId = vm.Product.Id,
                                        SpecificationId = spec.Item2.SpecificationId,
                                        StatusId = spec.Item2.StatusId,
                                        OptionId = spec.Item2.OptionId,
                                        Remark = spec.Item2.Remark,
                                        Date = spec.Item2.Date
                                    });
                                }
                            }
                            else
                            {
                                if (specification != null)
                                {
                                    await _productSpecificationService.DeleteAsync(specification.Id);
                                }
                            }
                        }
                    }
                    //add process
                    if (vm.ProductProcesses != null)
                    {
                        foreach (var process in vm.ProductProcesses)
                        {
                            var proc = product.Processes.Where(p => p.ProcessId == process.Item2.ProcessId).FirstOrDefault();
                            var diffDays = 0;
                            if (process.Item2.StartDate != null && process.Item2.EndDate != null)
                            {
                                TimeSpan timeDiff = (TimeSpan)(process.Item2.EndDate - process.Item2.StartDate);
                                diffDays = timeDiff.Days;
                            }
                            process.Item2.EstimatedDuration = diffDays;
                            if (process.Item1)
                            {
                                if (proc!=null)
                                {
                                    proc.StartDate = process.Item2.StartDate;
                                    proc.EndDate = process.Item2.EndDate;
                                    proc.Comment = process.Item2.Comment;
                                    proc.EstimatedDuration = process.Item2.EstimatedDuration;
                                    proc.StatusId = process.Item2.StatusId;
                                    await _productProcessService.UpdateAsync(proc.Id, proc);
                                }
                                else
                                {
                                    await _productProcessService.AddAsync(new ProductProcess()
                                    {
                                        ProductId = vm.Product.Id,
                                        ProcessId = process.Item2.ProcessId,
                                        StatusId = process.Item2.StatusId,
                                        Comment = process.Item2.Comment,
                                        StartDate = process.Item2.StartDate,
                                        EndDate = process.Item2.EndDate,
                                        EstimatedDuration = process.Item2.EstimatedDuration
                                    });
                                }
                            }
                            else
                            {
                                if (proc != null)
                                {
                                    await _productProcessService.DeleteAsync(proc.Id);
                                }
                            }
                        }
                    }
                    await _orderService.UpdateProgress(order.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(vm.Product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { Id = vm.Product.Id });
            }
            return NotFound();
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _productService.GetByIdAsync((int)id, p => p.Category, p => p.Order);
            var order = await _orderService.GetByIdAsync(product.OrderId, p => p.Customer);
            if (product == null)
            {
                return NotFound();
            }
            if (order.Customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (_context.Products == null)
            {
                return Problem("Entity set 'AppDBContext.Products'  is null.");
            }
            var product = _productService.getCompleteProductById(id);
            if (product != null)
            {
                int orderId = product.OrderId;
                await _productService.DeleteCompleteProduct(product.Id);
                await _orderService.UpdateProgress(orderId);
            }
            if (product.Order.Customer.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteMeasurement(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var meas = await _productMeasurementService.GetByIdAsync(id, m => m.Measurement);
            if (meas.Measurement.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var productId = meas.ProductId;
            await _productMeasurementService.DeleteAsync(meas.Id);
            return RedirectToAction("Details", new { id = productId });
        }
        [HttpGet]
        public async Task<IActionResult> DeleteSpecification(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var spec = await _productSpecificationService.GetByIdAsync(id, m => m.Specification);
            if (spec.Specification.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var productId = spec.ProductId;
            await _productSpecificationService.DeleteAsync(spec.Id);
            return RedirectToAction("Details", new { id = productId });
        }
        [HttpGet]
        public async Task<IActionResult> DeleteProcess(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var process = await _productProcessService.GetByIdAsync(id, m => m.Process);
            if (process.Process.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var productId = process.ProductId;
            await _productProcessService.DeleteAsync(process.Id);
            return RedirectToAction("Details", new { id = productId });
        }
        [HttpGet]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var comp = await _productComponentService.GetByIdAsync(id, m => m.Component);
            if (comp.Component.EnterpriseId != user.EnterpriseId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var productId = comp.ProductId;
            await _productComponentService.DeleteAsync(comp.Id);
            return RedirectToAction("Details", new { id = productId });
        }
        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
