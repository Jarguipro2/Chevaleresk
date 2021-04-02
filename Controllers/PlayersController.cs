using Chevaleresk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Chevaleresk.Controllers.Authorization;

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
        [PlayersAccess]
        public ActionResult Index()
        {
            return View();
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
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginView loginView)
        {
            if (ModelState.IsValid)
            {
                Players playerFound = DB.FindByUserName(loginView.UserName);
                if(playerFound != null)
                {
                    if(playerFound.Password != loginView.Password)
                    {
                        ModelState.AddModelError("Password", "Mot de passe incorrect");
                        return View(loginView);
                    }
                }
                else
                {
                    ModelState.AddModelError("UserName", "Ce nom d'utilisateur n'existe pas");
                    return View(loginView);
                }
                OnlinePlayers.AddSessionPlayer(playerFound);
            }
            return RedirectToAction("Index");
        }
        public ActionResult Logout()
        {
            OnlinePlayers.RemoveSessionPlayer();
            return RedirectToAction("Login");
        }
    }
}