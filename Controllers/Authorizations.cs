using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFA_DEMO.Models;

namespace EFA_DEMO.Controllers
{
    public class UserAccess : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (OnlineUsers.CurrentUser != null)
                return true;
            else
            {
                OnlineUsers.RemoveSessionUser();
                httpContext.Response.Redirect("/Users/Login");
            }
            return base.AuthorizeCore(httpContext);
        }
    }
    public class AdminAccess : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            UserView sessionUser = OnlineUsers.CurrentUser;
            if (sessionUser != null && sessionUser.Admin)
                return true;
            else
            {
                OnlineUsers.RemoveSessionUser();
                httpContext.Response.Redirect("/Users/Login");
            }
            return base.AuthorizeCore(httpContext);
        }
    }
}