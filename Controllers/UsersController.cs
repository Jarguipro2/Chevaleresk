using Chevaleresk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chevaleresk.Controllers
{
    public class PlayersController : Controller
    {
        private ChevalereskDBEntities2 DB = new ChevalereskDBEntities2();
        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Subscribe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Subscribe(PlayerView playerView)
        {
            if (ModelState.IsValid)
            {
                if (DB.UsernameExist(playerView.Username))
                {
                    ModelState.AddModelError("Username", "Ce nom d'utilisateur existe déjà");
                    return View(playerView);
                }
                playerView.Password = playerView.NewPassword;
                playerView.Money = 0;
                playerView.Admin = false;
                DB.AddPlayer(playerView);
            }
            return View(playerView);
        }
    }
}