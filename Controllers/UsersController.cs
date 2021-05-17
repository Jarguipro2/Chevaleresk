using EFA_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EFA_DEMO.Controllers
{
    public class UsersController : Controller
    {
        private DBEntities2 DB = new DBEntities2();

        private DateTime OnLineUsersLastUpdate
        {
            get
            {
                if (Session["OnLineUsersUpdate"] == null)
                    Session["OnLineUsersUpdate"] = new DateTime(0);
                return (DateTime)Session["OnLineUsersUpdate"];
            }
            set
            {
                Session["OnLineUsersUpdate"] = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }

        [UserAccess]
        public ActionResult Index()
        {
            OnLineUsersLastUpdate = new DateTime(0); // forcer le rafraichissement
            ViewBag.UserFullName = OnlineUsers.CurrentUser.FullName;
            return View();
        }

        [UserAccess]
        public ActionResult OnlineUsersList()
        {
            if (OnlineUsers.NeedUpdate(OnLineUsersLastUpdate))
            {
                OnLineUsersLastUpdate = DateTime.Now;
                return PartialView(OnlineUsers.Users);
            }
            return null;
        }

        public ActionResult Subscribe()
        {
            return View(new UserView());
        }
        [HttpPost]
        public ActionResult Subscribe(UserView userView)
        {
            if (ModelState.IsValid)
            {
                if (DB.UserNameExist(userView.Username))
                {
                    ModelState.AddModelError("Username", "Ce nom d'usager existe déjà.");
                    return View(userView);
                }
                if (DB.EmailExist(userView.Email))
                {
                    ModelState.AddModelError("Email", "Cet email existe déjà.");
                    return View(userView);
                }
                userView.Password = userView.NewPassword;
                DB.AddUser(userView);
                return RedirectToAction("Login");
            }
            return View(userView);
        }
        [AdminAccess]
        public ActionResult List()
        {
            return View(DB.Users.ToList());
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
                User userFound = DB.FindByUserName(loginView.Username);
                if (userFound != null)
                {
                    if (userFound.Password != loginView.Password)
                    {
                        ModelState.AddModelError("Password", "Mot de passe incorrect");
                        return View(loginView);
                    }
                }
                else
                {
                    ModelState.AddModelError("Username", "Ce non d'usager n'existe pas");
                    return View(loginView);
                }
                OnlineUsers.AddSessionUser(userFound.ToUserView());

                /*
                /// Code test pour contater l'effacement en casdade des logs
                userFound.ToUserView().RemoveAvatar();
                DB.Users.Remove(userFound);
                DB.SaveChanges();
                return null; */
            }
            return RedirectToAction("Index", "items");
        }

        public ActionResult Logout()
        {
            OnlineUsers.RemoveSessionUser();
            return RedirectToAction("Login");
        }

        [UserAccess]
        public ActionResult Profil()
        {
            UserView userView = DB.Users.Find(OnlineUsers.CurrentUser.Id).ToUserView();
            ViewBag.PasswordChangeToken = Guid.NewGuid().ToString().Substring(0,8);
            return View(userView);
        }
        [HttpGet, AdminAccess, RequireRouteValues(new[] { "id" })]
        public ActionResult Profil(int? id)
        {
            User user = DB.Users.Find(id);
            UserView userView;
            if (user != null)
                userView = user.ToUserView();
            else
            {
                Response.StatusCode = 404;
                return null;
            }

            ViewBag.PasswordChangeToken = Guid.NewGuid().ToString().Substring(0, 8);
            return View(userView);
        }
        [HttpPost, UserAccess]
        public ActionResult Profil(UserView userview)
        {
            string id = "";
            if (ModelState.IsValid)
            {
                string PasswordChangeToken = (string)Request["PasswordChangeToken"];
                User user = DB.Users.Find(userview.Id);
                if(user == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                 if(!OnlineUsers.CurrentUserIsAdmin() && user.IdPlayer != OnlineUsers.CurrentUser.Id)
                {
                    Response.StatusCode = 403;
                    return null;
                }
                if (userview.NewPassword.Equals(PasswordChangeToken))
                {
                    userview.Password = user.Password;
                }
                else
                {
                    userview.Password = userview.NewPassword;
                }
                userview.Money += user.Money;
                if (!OnlineUsers.CurrentUserIsAdmin())
                {
                    userview.Money = user.Money;
                    userview.Admin = user.Admin;
                }
                
                else
                    id = userview.Id.ToString();
                DB.UpdateUser(userview);

                //userview.CopyToUserView(OnlineUsers.CurrentUser);
                //OnlineUsers.LastUpdate = DateTime.Now;
            }
            
            return  RedirectToAction("Profil/" + id);
        }

        [UserAccess]
        public ActionResult UserLogs()
        {
            User user = DB.Users.Find(OnlineUsers.CurrentUser.Id);
            List<Log> logs = user.Logs.OrderByDescending(l => l.LoginDate).ToList();
            return View(logs);
        }

        [AdminAccess]
        public ActionResult AllUsersLogs()
        {
            OnLineUsersLastUpdate = new DateTime(0); // forcer le rafraichissement
            return View();
        }

        [AdminAccess]
        public ActionResult UsersLogs()
        {
            if (OnlineUsers.NeedUpdate(OnLineUsersLastUpdate))
            {
                OnLineUsersLastUpdate = DateTime.Now;
               
                List<Log> logs = DB.Logs.OrderByDescending(l => l.LoginDate).ToList();
                return PartialView(logs);
            }
            return null;
        }
        [HttpGet, ActionName("Inventory"), UserAccess]
        public ActionResult Inventory(int? id)
        {
            if(OnlineUsers.CurrentUser.Id != id && !OnlineUsers.CurrentUserIsAdmin())
            {
                Response.StatusCode = 403;
                return null;
            }
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            User user = DB.Users.Find(id);
            if(user == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            User_Inventory user_Inventory = new User_Inventory();
            user_Inventory.ItemsQuantities = DB.UserItemsQuantities(user);
            user_Inventory.User = user;
            
            return View(user_Inventory);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}