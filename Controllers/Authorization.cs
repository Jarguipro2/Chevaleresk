using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chevaleresk.Models;

namespace Chevaleresk.Controllers
{
    public class Authorization
    {
        public class PlayersAccess : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                if (OnlinePlayers.GetSessionPlayer() != null)
                    return true;
                else
                    httpContext.Response.Redirect("/Players/Login");
                return base.AuthorizeCore(httpContext);
            }
        }
        public class AdminAccess : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                Players sessionPlayer = OnlinePlayers.GetSessionPlayer();
                if(sessionPlayer != null)
                    if (sessionPlayer.Admin)
                        return true;
                    else
                        httpContext.Response.Redirect("/Players/Login");
                return base.AuthorizeCore(httpContext);
            }
        }
    }
}