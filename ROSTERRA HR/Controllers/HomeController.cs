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
            if (Session["Role"] != null && string.Equals(Session["Role"].ToString(), "HR", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return RedirectToAction("Login", "Account");
        }
    }
}