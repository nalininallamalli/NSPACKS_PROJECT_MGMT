﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using HackathonPMA.Models;
using System.Collections.Generic;
using PagedList;
using System.Data.Entity;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace HackathonPMA.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private Entities db = new Entities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if(User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //ToAdd: start
        public ActionResult shMapping()
        {
            //if (Convert.ToString(TempData["pid"]) == "")
            //{
            //    return RedirectToAction("Index", "Projects");
            //}
            //TempData["pid"] = TempData["pid"];
            TempData["project"] = TempData["project"];
            TempData["fundsMapping"] = TempData["fundsMapping"];
            ViewBag.hdnUsr = TempData["hdnUsr"];
            ViewBag.hdnRid = TempData["hdnRid"];
            ViewBag.IsParent = TempData["isParent"];
            TempData["hdnFunds"] = TempData["hdnFunds"];
            TempData["isEdit"] = TempData["isEdit"];
            var Db = new ApplicationDbContext();
            if (Convert.ToString(TempData["isEdit"]) == "1" && Convert.ToString(TempData["hdnUsr"]) == "")
            {
                var z = "";

                Project project = (Project)TempData["project"];
                var ep = from s in db.EmployeeProjects
                         where s.ProjectId == project.Id
                         select s;
                var usr = "";
                foreach (EmployeeProject e in ep)
                {
                    usr += "#" + e.EmployeeId;
                   
                    //var z1 = db.AspNetUsers.Where(item => item.Id == "0f3d796a-af0d-445c-9f5c-c4ad7624df45").Select(x => x.AspNetRoles.FirstOrDefault().Id).ToList();
                    var z1 = db.AspNetUsers.Where(item => item.Id ==e.EmployeeId).Select(x => x.AspNetRoles.FirstOrDefault().Id).ToList();
                    z = z1[0];
                }
                ViewBag.hdnRid = z;
                ViewBag.hdnUsr = usr;
                TempData["hdnUsr"] = usr;
                TempData["hdnRid"] = z;
            }
            var model = new RegisterViewModel();
            var list = Db.Roles.OrderBy(r => r.Name).Where(adm => adm.Name != "Admin").ToList()
                .Select(rr => new SelectListItem { Value = rr.Id.ToString(), Text = rr.Name })
                .ToList();
            model.Roles = (IEnumerable<SelectListItem>)list;
            TempData["project"] = TempData["project"];
            TempData["isEdit"] = TempData["isEdit"];
            TempData["isParent"] = TempData["isParent"];
            TempData["fundsMapping"] = TempData["fundsMapping"];            
            TempData["hdnFunds"] = TempData["hdnFunds"];
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult shMapping(string hdnUsr,string hdnRid, string btnAction)
        {
            if (btnAction == "Cancel")
            {
                TempData["project"] = null;
                TempData["hdnUsr"] = null;
                TempData["fundsMapping"] = null;
                TempData["isEdit"] = null;
                return RedirectToAction("Index", "Projects");
            }
            TempData["project"] = TempData["project"];
            TempData["isEdit"] = TempData["isEdit"];
            TempData["isParent"] = TempData["isParent"];
            TempData["fundsMapping"] = TempData["fundsMapping"];
            TempData["hdnUsr"] = hdnUsr;
            TempData["hdnRid"] = hdnRid;
            TempData["hdnFunds"] = TempData["hdnFunds"];
            Project project = (Project)TempData["project"];

            if (btnAction == "Back")
            {
                if (Convert.ToString(TempData["isEdit"]) == "1")
                {
                    return RedirectToAction("Edit", "Projects", new { id = project.Id });
                }
                return RedirectToAction("Create", "Projects");
            }
            if (btnAction == "Next")
            {
                TempData["project"] = TempData["project"];
                return RedirectToAction("fundsMapping", "Funds");
            }

            //ToDo chk for page
            
            //db.Projects.Add(project);
            //db.SaveChanges();
            ////ToAdd: start
            //int id = project.Id;
            //var Db = new ApplicationDbContext();
            //foreach (string s in hdnUsr.Split('#'))
            //{
            //    if (s != null && s != "")
            //    {
            //        var cnt = 0;
            //        if (db.EmployeeProjects.Count() > 0)
            //            cnt = db.EmployeeProjects.Max(x => x.Id);

            //        EmployeeProject ep = new EmployeeProject();
            //        ep.Id = cnt + 1;
            //        ep.EmployeeId = s;
            //        ep.ProjectId = Convert.ToInt32(id);

            //        db.EmployeeProjects.Add(ep);

            //        db.SaveChanges();
            //    }
            //}

            //Project project = (Project)TempData["project"];

            if (Convert.ToString(TempData["isEdit"]) == "1")
            {
                Project p = db.Projects.Find(project.Id);
                p.Name = project.Name;
                p.Description = project.Description;
                p.StartDate = project.StartDate;
                p.EndDate = project.EndDate;
                p.City = project.City;
                p.Location = project.Location;
                p.Category = project.Category;
                p.ModifiedOn = DateTime.Now;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                db.Projects.Add(project);
                db.SaveChanges();
            }
            //ToAdd: start
            int id = project.Id;
            var Db = new ApplicationDbContext();
            if (Convert.ToString(TempData["isEdit"]) == "1")
            {

                var ep = from s in db.EmployeeProjects
                         where s.ProjectId == id
                         select s;

                foreach (EmployeeProject e in ep)
                {
                    db.EmployeeProjects.Remove(e);
                }
            }
            foreach (string s in hdnUsr.Split('#'))
            {
                if (s != null && s != "")
                {
                    var cnt = 0;
                    if (db.EmployeeProjects.Count() > 0)
                        cnt = db.EmployeeProjects.Max(x => x.Id);

                    EmployeeProject ep = new EmployeeProject();
                    ep.Id = cnt + 1;
                    ep.EmployeeId = s;
                    ep.ProjectId = Convert.ToInt32(id);

                    db.EmployeeProjects.Add(ep);

                    db.SaveChanges();
                }
            }
            TempData["project"] = null;
            TempData["hdnUsr"] = null;
            TempData["fundsMapping"] = null;
            TempData["hdnUsr"] = null;
            TempData["hdnRid"] = null;
            TempData["isEdit"] = null;
            TempData["hdnFunds"] = null;
            TempData["isParent"] = null;
           
            return RedirectToAction("Index", "projects");
        }

        [HttpPost]
        public ActionResult LoadUsers(string roleid)
        {
            var users = UserManager.Users.Where(item => item.Roles.FirstOrDefault().RoleId == roleid).Select(x => x);
            return Json(users.ToList());
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/ListUsers
        public ActionResult ListUsers(string sortBy, string currentFilter, string searchBy, int? page)
        {
            ViewBag.FirstNameSort = string.IsNullOrEmpty(sortBy) ? "FirstName desc" : "";
            ViewBag.LastNameSort = sortBy == "LastName" ? "LastName desc" : "LastName";
            ViewBag.CitySort = sortBy == "City" ? "City desc" : "City";
            ViewBag.StateSort = sortBy == "State" ? "State desc" : "State";
            ViewBag.UserNameSort = sortBy == "UserName" ? "UserName desc" : "UserName";
            ViewBag.GenderSort = sortBy == "Gender" ? "Gender desc" : "Gender";

            if (searchBy != null)
            {
                page = 1;
            }
            else
            {
                searchBy = currentFilter;
            }

            ViewBag.CurrentFilter = searchBy;

            var users = from s in UserManager.Users
                           select s;

            if (!String.IsNullOrEmpty(searchBy))
            {
                users = users.Where(s => s.FirstName.Contains(searchBy)
                                       || s.LastName.Contains(searchBy)
                                       || s.City.Contains(searchBy)
                                       || s.State.Contains(searchBy)
                                       || s.UserName.Contains(searchBy)
                                       || s.Gender.ToString().Contains(searchBy));
            }

            switch (sortBy)
            {
                case "FirstName desc":
                    users = users.OrderByDescending(s => s.FirstName);
                    break;
                case "FirstName":
                    users = users.OrderBy(s => s.FirstName);
                    break;
                case "LastName desc":
                    users = users.OrderByDescending(s => s.LastName);
                    break;
                case "LastName":
                    users = users.OrderBy(s => s.LastName);
                    break;
                case "City desc":
                    users = users.OrderByDescending(s => s.City);
                    break;
                case "City":
                    users = users.OrderBy(s => s.City);
                    break;
                case "State":
                    users = users.OrderBy(s => s.State);
                    break;
                case "State desc":
                    users = users.OrderByDescending(s => s.State);
                    break;
                case "UserName":
                    users = users.OrderBy(s => s.UserName);
                    break;
                case "UserName desc":
                    users = users.OrderByDescending(s => s.UserName);
                    break;
                case "Gender":
                    users = users.OrderBy(s => s.Gender);
                    break;
                case "Gender desc":
                    users = users.OrderByDescending(s => s.Gender);
                    break;
                default:
                    users = users.OrderBy(s => s.FirstName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            List<UserDetailsModel> modelList = new List<UserDetailsModel>();
            foreach (var user in users.ToList())
            {
                UserDetailsModel model = new UserDetailsModel();
                model.User = user;
                model.Roles = UserManager.GetRoles(user.Id);
                modelList.Add(model);
            }
            return View(modelList.ToPagedList(pageNumber, pageSize));
            //return View(users.ToPagedList(pageNumber, pageSize));
           // return View(users.ToList());
        }

        // GET: /Account/DetailtUser
        public ActionResult DetailUser(string id)
        {
            var model = new UserDetailsModel
            {
                Id = id,
                User = UserManager.FindById(id),
                Roles = UserManager.GetRoles(id)

            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string id)
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            EditUserViewModel model = new EditUserViewModel();

            var userRoles = UserManager.GetRoles(user.Id);
            var list = Db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem 
            {
                Selected = userRoles.Contains(rr.Name),
                Value = rr.Name.ToString(), 
                Text = rr.Name }).ToList();

            model.user = user;
            model.Roles = (IEnumerable<SelectListItem>)list;
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(string id, EditUserViewModel model)
        {
            var Db = new ApplicationDbContext();
            var list = Db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            model.Roles = (IEnumerable<SelectListItem>)list;

            if (ModelState.IsValid)
            {
                var user = Db.Users.First(u => u.Id == id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.FirstName = model.user.FirstName;
                user.LastName = model.user.LastName;
                user.Email = model.user.Email;
                user.UserName = model.user.Email;
                user.Gender = model.user.Gender;
                user.PhoneNumber = model.user.PhoneNumber;
                user.City = model.user.City;
                user.State = model.user.State;
                user.Country = model.user.Country;
                user.Zip = model.user.Zip;
                user.AddressLine = model.user.AddressLine;
                user.Salary = model.user.Salary;
                Db.Entry(user).State = System.Data.Entity.EntityState.Modified;

                var Roles = UserManager.GetRoles(id);
                if(!Roles.First().Equals(model.Role))
                { 
                    await UserManager.AddToRoleAsync(user.Id, model.Role);
                    await UserManager.RemoveFromRoleAsync(user.Id, Roles.First());
                }

                await Db.SaveChangesAsync();
                return RedirectToAction("ListUsers");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Account/DeleteUser/
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUser(string id = null)
        {
            var model = new UserDetailsModel
            {
                Id = id,
                User = UserManager.FindById(id),
                Roles = UserManager.GetRoles(id)

            };
            if (model.User == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Account/DeleteUser
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete_User(string id)
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.Id == id);
            Db.Users.Remove(user);
            await Db.SaveChangesAsync();
            return RedirectToAction("ListUsers");
        }

        //
        // GET: /Account/Register
        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            var Db = new ApplicationDbContext();
            var model = new RegisterViewModel();
            var list = Db.Roles.OrderBy(r => r.Name).ToList().Select(rr =>new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            model.Roles = (IEnumerable<SelectListItem>)list;
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var Db = new ApplicationDbContext();            
            var list = Db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            model.Roles = (IEnumerable<SelectListItem>)list;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Gender = model.Gender,
                PhoneNumber = model.PhoneNumber, City = model.City, State = model.State, Country = model.Country, Zip = model.Zip, AddressLine = model.AddressLine, Salary = model.Salary};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Role);
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //return RedirectToAction("Index", "Home");
                    return RedirectToAction("ListUsers");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        public ActionResult Report(string id)
        {
            LocalReport lr = new LocalReport();

            string path = Path.Combine(Server.MapPath("~/Reports"), "StakeHolders.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }

            List<AspNetUser> cm = new List<AspNetUser>();

            using (Entities es = new Entities())
            {
                cm = es.AspNetUsers.ToList();
            }

            ReportDataSource rd = new ReportDataSource("StakeHoldersDataSet", cm);
            lr.DataSources.Add(rd);

            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            return File(renderedBytes, mimeType);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}