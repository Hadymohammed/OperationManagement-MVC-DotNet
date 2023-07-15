using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OperationManagement.Data;
using OperationManagement.Data.Common;
using OperationManagement.Data.Services;
using OperationManagement.Data.Static;
using OperationManagement.Data.ViewModels;
using OperationManagement.Models;
using static System.Net.WebRequestMethods;

namespace OperationManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDBContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEnterpriseService _enterpriseService;
        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEnterpriseService enterpriseService,
            AppDBContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _enterpriseService = enterpriseService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);   
                if (user == null)
                {
                    ModelState.AddModelError("Email", "This email not registered on our system.");
                    return View(vm);
                }
                if (!(bool)user.Registered)
                {
                    ModelState.AddModelError("Email", "This email not Accepted or complete registration yet.");
                    return View(vm);

                }
                var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (await _userManager.IsInRoleAsync(user, UserRoles.User))
                    {
                        return RedirectToAction("Index", "Customers");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                }
            }
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult JoinRequest()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinRequestAsync(JoinRequestVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var isEmailValid = await PresirvedEmail(vm.Email);
            if (isEmailValid)
            {
                ModelState.AddModelError("Email", "This email already has an account.");
                return View(vm);
            }
            SessionHelper.saveObject(HttpContext, SessionHelper.JoinKey, vm);
            return RedirectToAction("EmailOTP", new OTPVM()
            {
                Email = vm.Email
            });
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if(user == null)
            {
                ModelState.AddModelError("Email", "This email not registered on our system.");
                return View(vm.Email);
            }
            SessionHelper.saveObject(HttpContext, SessionHelper.ForgetPasswordKey, vm.Email);
            return RedirectToAction("ForgetPasswordOTP", new OTPVM()
            {
                Email = vm.Email
            });
            return View();
        }
        [HttpGet]
        public IActionResult ForgetPasswordOTP(OTPVM vm)
        {
            if (!OTPServices.HaveOTP(HttpContext))
            {
                OTPServices.SendEmailOTP(HttpContext, vm.Email);
            }
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPasswordOTPAsync(OTPVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            if (OTPServices.VerifyOTP(HttpContext, vm) == false)
            {
                ModelState.AddModelError("OTP", "InValid Code");
                return View(vm);
            }
            var email = SessionHelper.getObject<string>(HttpContext, SessionHelper.ForgetPasswordKey);
            if (email == null)
                return NotFound();
            var staff = await _userManager.FindByEmailAsync(email);

            return RedirectToAction("AddPassword",new AddPasswordVM()
            {
                FirstName=staff.FirstName,
                StaffId=staff.Id
            });

        }
        [HttpGet]
        public IActionResult AddPassword(AddPasswordVM vm)
        {
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPasswordAsync(AddPasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var email = SessionHelper.getObject<string>(HttpContext, SessionHelper.ForgetPasswordKey);
            if (email == null)
                return NotFound();
            var staff = await _userManager.FindByEmailAsync(email);
            staff.PasswordHash = _userManager.PasswordHasher.HashPassword(staff, vm.Password);
            var tokensToDelete = _context.Tokens.Where(t => t.userId == staff.Id);
            _context.Tokens.RemoveRange(tokensToDelete);
            _context.SaveChanges();
            await _userManager.UpdateAsync(staff);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult EmailOTP(OTPVM vm)
        {
            if (!OTPServices.HaveOTP(HttpContext))
            {
                OTPServices.SendEmailOTP(HttpContext, vm.Email);
            }
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailOTPAsync(OTPVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            if (OTPServices.VerifyOTP(HttpContext, vm) == false)
            {
                ModelState.AddModelError("OTP", "InValid Code");
                return View(vm);
            }
            var request = SessionHelper.getObject<JoinRequestVM>(HttpContext, SessionHelper.JoinKey);
            if (request == null)
                return NotFound();
            try
            {
                var enterprise = new Enterprise()
                {
                    Name = request.EnterpriseName,
                    Description = request.Description,
                    LogoURL = Consts.profileImgUrl,
                    Accepted = false
                };
                await _enterpriseService.AddAsync(enterprise);
                var staff = new ApplicationUser()
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserName=request.Email,
                    PasswordHash = RandomPassword.GenerateRandomPassword(12),
                    EmailConfirmed = true,
                    ProfilePictureURL = Consts.profileImgUrl,
                    Registered = false,
                    EnterpriseId = enterprise.Id
                };
                await _userManager.CreateAsync(staff);
                await _userManager.AddToRoleAsync(staff,UserRoles.User);
                return RedirectToAction("JoinSuccess");
            }
            catch (Exception err)
            {
                return Problem("Service internal error, contact us on operatobusiness@gmail.com");
            }
        }
        [HttpGet]
        public IActionResult JoinSuccess()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Register(int TID, string Role, string token)
        {
            if (_context.Tokens.Where(t => t.Id == TID).FirstOrDefault() == null || JWTHelper.ValidateToken(token) == null)
            {
                return NotFound();
            }
            var UserIdStr = JWTHelper.ValidateToken(token);
            var staff=await _userManager.FindByIdAsync(UserIdStr);
            SessionHelper.saveObject(HttpContext, SessionHelper.TokenKey, new Token()
            {
                Id=TID,
                token=token
            });
            return View(new AddPasswordVM()
            {
                FirstName=staff.FirstName,
                StaffId=staff.Id
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddPasswordVM vm)
        {
            var staff = await _userManager.FindByIdAsync(vm.StaffId);
            if (staff == null)
            {
                ModelState.AddModelError("Password", "No such user please contact us.");
                return View(vm);
            }
            var token = SessionHelper.getObject<Token>(HttpContext, SessionHelper.TokenKey);
            var tokenDB = _context.Tokens.Where(t => t.Id == token.Id).FirstOrDefault();
            if(tokenDB == null)
            {
                ModelState.AddModelError("Password", "Your link is not valid please contact us if this the first time you use it.");
                return View(vm);
            }
            _context.Tokens.Remove(tokenDB);
            _context.SaveChanges();
            staff.PasswordHash=_userManager.PasswordHasher.HashPassword(staff, vm.Password);
            staff.Registered = true;
            await _userManager.UpdateAsync(staff);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> ResendForgetPasswordOTP()
        {
            var email = SessionHelper.getObject<string>(HttpContext, SessionHelper.ForgetPasswordKey);
            if (email == null)
                return NotFound();
            OTPServices.SendEmailOTP(HttpContext, email);
            return RedirectToAction("ForgetPasswordOTP", new OTPVM()
            {
                Email = email
            });
        }
        [HttpGet]
        public async Task<IActionResult> ResendEmailOTP()
        {
            var request = SessionHelper.getObject<JoinRequestVM>(HttpContext, SessionHelper.JoinKey);
            if (request == null)
                return NotFound();

            OTPServices.SendEmailOTP(HttpContext, request.Email);
            return RedirectToAction("EmailOTP", new OTPVM()
            {
                Email = request.Email
            });
        }
        private async Task<bool> PresirvedEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}
