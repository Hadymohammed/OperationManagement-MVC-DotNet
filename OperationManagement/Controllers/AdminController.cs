using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Data;
using OperationManagement.Data.Common;
using OperationManagement.Data.Services;
using OperationManagement.Data.Static;
using OperationManagement.Data.ViewModels;
using OperationManagement.Models;
using System.IdentityModel.Tokens.Jwt;

namespace OperationManagement.Controllers
{
    [Authorize(Roles =UserRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IEnterpriseService _enterpriseService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderSerivce;


        public AdminController(IEnterpriseService enterpriseService,
            ICustomerService customerService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager,
            AppDBContext context)
        {
            _enterpriseService = enterpriseService;
            _customerService = customerService;
            _orderSerivce = orderService;
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var allEnterprises = await _enterpriseService.GetAllAsync();

            return View(new AdminIndexVM()
            {
                Enterprises = allEnterprises.Where(e => e.Accepted == true).ToList(),
                PendingUsers = _userManager.Users.Where(u => u.Registered == false).ToList(),
                NumberOfCustomers = _customerService.GetNumberOfAllCustomers(),
                NumberOfOrders = _orderSerivce.GetNumberOfAllOrders()
            });
        }
        [HttpGet]
        public async Task<IActionResult> ReviewJoinRequest(string id)
        {
            var staff = await _userManager.FindByIdAsync(id);
            if (staff == null)
                return NotFound();
            var roles = await _userManager.GetRolesAsync(staff);

            if ((bool)staff.Registered||roles.FirstOrDefault()!=UserRoles.User)
                return NotFound();
            staff.Enterprise = await _enterpriseService.GetByIdAsync((int)staff.EnterpriseId);
            if (staff.Enterprise == null)
                return NotFound();

            return View(new ReviewJoinRequestVM()
            {
                Staff=staff,
                Enterprise=staff.Enterprise,
                EnterpriseId= (int)staff.EnterpriseId,
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinRequestAction(ReviewJoinRequestVM vm)
        {
            
            if (!ModelState.IsValid)
            {
                vm.Enterprise = await _enterpriseService.GetByIdAsync(vm.EnterpriseId, s => s.Staff);
                vm.Staff = vm.Enterprise.Staff.FirstOrDefault();
                return View(vm);
            }
            if (vm.Action == "Accept")
            {
                var enterprise = await _enterpriseService.GetByIdAsync(vm.EnterpriseId, s => s.Staff);
                if (enterprise == null)
                {
                    return NotFound();
                }
                enterprise.Accepted = true;
                await _enterpriseService.UpdateAsync(enterprise.Id, enterprise);
                var token=JWTHelper.GenerateJwtToken(enterprise.Staff.FirstOrDefault().Email, enterprise.Staff.FirstOrDefault().Id);
                var Token = new Token
                {
                    token = token
                };
                await _context.Tokens.AddAsync(Token);
                _context.SaveChanges();
                var UId = Token.Id;
                string oneTimeAddStaffLink = Url.Action("Register", "Account", new { UID = UId, Role = UserRoles.User, token = Token.token }, Request.Scheme);
                EmailHelper.SendEmailAddStaff(enterprise.Staff.FirstOrDefault().Email, oneTimeAddStaffLink,enterprise.Name);
            }
            else if (vm.Action == "Reject")
            {
                var enterprise = await _enterpriseService.GetByIdAsync(vm.EnterpriseId,s=>s.Staff);
                if (enterprise == null)
                {
                    return NotFound();
                }
                EmailHelper.SendEnterpriseRejection(enterprise.Staff.FirstOrDefault().Email, vm.Messege, enterprise.Name);
                await _userManager.DeleteAsync(enterprise.Staff.FirstOrDefault());
                await _enterpriseService.DeleteAsync(enterprise.Id);
            }
            return RedirectToAction("Index");
        }
    }
}
