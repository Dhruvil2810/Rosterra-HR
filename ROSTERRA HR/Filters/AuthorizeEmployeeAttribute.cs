using System;
using System.Web;
using System.Web.Mvc;

namespace ROSTERRA_HR.Filters
{
    /// <summary>
    /// Restricts access strictly to authenticated users with the 'Employee' role.
    /// Blocks unauthorized or unauthenticated requests and redirects to Login.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizeEmployeeAttribute : AuthorizeAttribute
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

            return string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Account/Login");
        }
    }
}
