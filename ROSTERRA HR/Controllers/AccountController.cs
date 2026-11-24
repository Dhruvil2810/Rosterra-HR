using System;
using System.Web.Mvc;
using System.Web.Security;
using ROSTERRA_HR.Models;
using ROSTERRA_HR.Repositories;
using ROSTERRA_HR.Security;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Handles authentication workflows: Login and Logout.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;

        public AccountController()
        {
            _userRepository = new UserRepository();
        }

        // GET: /Account
        // GET: /Account/Index
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // If already logged in, redirect to the appropriate dashboard
            if (Session["Role"] != null)
            {
                string role = Session["Role"].ToString();
                if (string.Equals(role, "HR", StringComparison.OrdinalIgnoreCase))
                {
                    return Redirect("~/Dashboard");
                }
                if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase))
                {
                    return Redirect("~/EmployeeDashboard");
                }
            }

            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = _userRepository.GetByUsername(model.Username);

                if (user == null || !user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    return View(model);
                }

                bool isPasswordValid = PasswordHasher.VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt);
                if (!isPasswordValid)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    return View(model);
                }

                // Update last login timestamp
                _userRepository.UpdateLastLogin(user.UserId);

                // Initialize authentication ticket and session state
                FormsAuthentication.SetAuthCookie(user.Username, false);
                Session["UserId"] = user.UserId;
                Session["Username"] = user.Username;
                Session["Role"] = user.Role;

                // Role-based post-login redirection
                if (string.Equals(user.Role, "HR", StringComparison.OrdinalIgnoreCase))
                {
                    return Redirect("~/Dashboard");
                }
                else if (string.Equals(user.Role, "Employee", StringComparison.OrdinalIgnoreCase))
                {
                    return Redirect("~/EmployeeDashboard");
                }

                return Redirect("~/Account/Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred during login. " + ex.Message);
                return View(model);
            }
        }

        // GET or POST: /Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            // Expire ASP.NET session cookie
            if (Response.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.UtcNow.AddMonths(-20);
            }

            return Redirect("~/Account/Login");
        }
    }
}
