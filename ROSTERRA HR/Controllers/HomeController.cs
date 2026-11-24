using System;
using System.Web.Mvc;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Default entry controller. Directs traffic to Login or Dashboard.
    /// </summary>
    public class HomeController : Controller
    {
        // GET: / or /Home/Index
        public ActionResult Index()
        {
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

            return RedirectToAction("Login", "Account");
        }
    }
}