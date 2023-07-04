using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Data.Services;
using OperationManagement.Data.ViewModels;
using OperationManagement.Models;

namespace OperationManagement.Controllers
{
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
            IOrderService orderService)
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
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _productService.GetAllAsync(p=>p.Category,p=>p.Order));
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

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create(int?orderId)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            if (orderId != null)
            {
                var order = await _orderService.GetByIdAsync((int)orderId);
                ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "EnterpriseOrderNumber",order.Id);
            }
            else
            {
                ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "EnterpriseOrderNumber");
            }
            return View(new CreateProductVM
            {
                Specifications=await _specificationService.GetAllAsync(s=>s.Statuses,s=>s.Options),
                Components=await _componentService.GetAllAsync(c=>c.Photos),
                Processes=await _processService.GetAllAsync(p=>p.Statuses),
                Measurements=await _measurementService.GetAllAsync()
            });
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVM vm)
        {
            if (ModelState.IsValid)
            {
                await _productService.AddAsync(vm.Product);
                //add measurements
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
                //add components
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
                //add specifications
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
                //add process
                foreach(var process in vm.ProductProcesses)
                {
                    if (process.Item1)
                    {
                        await _productProcessService.AddAsync(new ProductProcess()
                        {
                            ProductId = vm.Product.Id,
                            ProcessId = process.Item2.ProcessId,
                            StatusId = process.Item2.StatusId,
                            Comment = process.Item2.Comment,
                            StartDate = process.Item2.StartDate,
                            EndDate = process.Item2.EndDate
                        });
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", vm.Product.CategoryId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "EnterpriseOrderNumber", vm.Product.OrderId);
            vm.Specifications = await _specificationService.GetAllAsync(s => s.Statuses, s => s.Options);
            vm.Components = await _componentService.GetAllAsync(c => c.Photos);
            vm.Processes = await _processService.GetAllAsync(p => p.Statuses);
            vm.Measurements = await _measurementService.GetAllAsync();
            return View(vm);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            var AllComponents =await _componentService.GetAllAsync(c=>c.Photos);
            var AllSpecifications =await _specificationService.GetAllAsync(s=>s.Statuses,s=>s.Options);
            var AllMeasurements =await _measurementService.GetAllAsync();
            var AllProcess =await _processService.GetAllAsync(p=>p.Statuses);
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
            foreach(var comp in AllComponents)
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "EnterpriseOrderNumber", product.OrderId);
            return View(vm);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,CreateProductVM vm)
        {
            if (id != vm.Product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateAsync(id, vm.Product);
                    /*Update details*/
                    var product = _productService.getCompleteProductById(vm.Product.Id);
                    //add measurements
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
                    //add components
                    foreach (var comp in vm.ProductComponents)
                    {
                        var component = product.Components.Where(c => c.ComponentId == comp.Item2.ComponentId).FirstOrDefault();
                        if (comp.Item1)
                        {
                            if (component!=null)
                            {
                                component.Quantity = comp.Item2.Quantity;
                                await _productComponentService.UpdateAsync(component.Id, component);
                            }
                            else
                            {
                                await _productComponentService.AddAsync(new ProductComponent()
                                {
                                    ProductId = vm.Product.Id,
                                    ComponentId = comp.Item2.ComponentId,
                                    Quantity = comp.Item2.Quantity
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
                    //add specifications
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
                    //add process
                    foreach (var process in vm.ProductProcesses)
                    {
                        var proc = product.Processes.Where(p => p.ProcessId == process.Item2.ProcessId).FirstOrDefault();
                        if (process.Item1)
                        {
                            if (proc!=null)
                            {
                                proc.StartDate = process.Item2.StartDate;
                                proc.EndDate = process.Item2.EndDate;
                                proc.Comment = process.Item2.Comment;
                                proc.EstimatedDuration = process.Item2.EstimatedDuration;

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
                                    EndDate = process.Item2.EndDate
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
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _productService.GetByIdAsync((int)id, p => p.Category, p => p.Order);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'AppDBContext.Products'  is null.");
            }
            var product = _productService.getCompleteProductById(id);
            if (product != null)
            {
                await _productService.DeleteCompleteProduct(product.Id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
