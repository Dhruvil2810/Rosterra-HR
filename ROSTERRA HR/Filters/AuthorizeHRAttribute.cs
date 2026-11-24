using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ROSTERRA_HR.Filters
{
    /// <summary>
    /// Restricts action or controller execution to authenticated users with the 'HR' role.
    /// Verifies session state and redirects unauthenticated requests to the Login page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizeHRAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null || httpContext.Session == null)
            {
                return false;
            }

            var username = httpContext.Session["Username"] as string;
            var role = httpContext.Session["Role"] as string;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                return false;
            }

            return string.Equals(role, "HR", StringComparison.OrdinalIgnoreCase);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Account/Login");
        }
    }
}
