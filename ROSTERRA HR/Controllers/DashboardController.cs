using System.Web.Mvc;
using ROSTERRA_HR.Filters;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Serves the HR Dashboard. Protected by AuthorizeHR filter.
    /// Direct URL access without HR authentication is blocked.
    /// </summary>
    [AuthorizeHR]
    public class DashboardController : Controller
    {
        // GET: /Dashboard
        // GET: /Dashboard/Index
        public ActionResult Index()
        {
            ViewBag.Username = Session["Username"] ?? "HR Admin";
            return View();
        }
    }
}