using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data.Base;
using OperationManagement.Data.Static;
using OperationManagement.Models;

namespace OperationManagement.Data.Services
{
    public class ProductService:EntityBaseRepository<Product>,IProductService
    {
        private readonly AppDBContext _context;
        private readonly IProductSpecificationService _productSpecificationService;
        private readonly IProductMeasurementService _productMeasurementService;
        private readonly IProductComponentService _productComponentService;
        private readonly IProductProcessService _productProcessService;
        public ProductService(AppDBContext context,
            IProductSpecificationService productSpecificationService,
            IProductMeasurementService productMeasurementService,
            IProductComponentService productComponentService,
            IProductProcessService productProcessService):base(context)
        {
            _context = context;
            _productSpecificationService = productSpecificationService;
            _productMeasurementService = productMeasurementService;
            _productComponentService = productComponentService;
            _productProcessService = productProcessService;
        }
        public List<Product> getByOrderId(int orderId)
        {
            return _context.Products.Where(p => p.OrderId == orderId).ToList();
        }
        public Product getCompleteProductById(int productId)
        {
            return _context.Products
                    .Where(p => p.Id == productId)
                    .Include(p => p.Category)
                    .Include(p => p.Components)
                        .ThenInclude(c => c.Component)
                    .Include(p => p.Measurements)
                        .ThenInclude(m => m.Measurement)
                    .Include(p => p.Processes)
                        .ThenInclude(pr => pr.Process)
                            .ThenInclude(process=>process.Statuses)
                    .Include(p => p.Specifications)
                        .ThenInclude(s => s.Specification)
                            .ThenInclude(spec => spec.Options)
                    .Include(p => p.Specifications)
                        .ThenInclude(s => s.Status)
                    .Include(p=>p.Order)
                        .ThenInclude(o=>o.Customer)
                    .SingleOrDefault();
        }
        public async Task<bool> DeleteCompleteProduct(int productId)
        {
            var Product = await GetByIdAsync(productId, p => p.Measurements, p => p.Processes, p => p.Components, p => p.Specifications);
            if (Product == null)
                return false;
            try
            {
                //delete specifications
                var SpecToDelete = Product.Specifications.ToList();
                foreach (var s in SpecToDelete)
                {
                    await _productSpecificationService.DeleteAsync(s.Id);
                }
                //delete components
                var compToDelete = Product.Components.ToList();
                foreach (var c in compToDelete)
                {
                    await _productComponentService.DeleteAsync(c.Id);
                }
                //delete measurements
                var meagToDelete = Product.Measurements.ToList();
                foreach (var m in meagToDelete)
                {
                    await _productMeasurementService.DeleteAsync(m.Id);
                }
                //delete processes
                var procToDelete = Product.Processes.ToList();
                foreach(var p in procToDelete)
                {
                    await _productProcessService.DeleteAsync(p.Id);
                }
                //delete product
                await DeleteAsync(Product.Id);
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
            return false;
        }
        public async Task<int> UpdateProgressAsync(int productId)
        {
            var product = await GetByIdAsync(productId,p=>p.Processes);
            product.Processes = _productProcessService.GetByProductId(productId);
            if (product.Processes == null)
            {
                product.Progress = 0;
            }
            else if (product.Processes.Count()==0)
            {
                product.Progress = 0;
            }
            else
            {
                float totalDone = 0;
                foreach (var process in product.Processes)
                {
                    if (process.Status.Name == Consts.DoneStatus)
                    {
                        totalDone++;
                    }
                }
                product.Progress = (int)(totalDone / product.Processes.Count() * 100);
                if (totalDone == product.Processes.Count())
                {
                    product.IsCompleted = true;
                }
            }
            await UpdateAsync(product.Id, product);
            return (int)product.Progress;
        }
    }
}
