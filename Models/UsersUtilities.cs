using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace EFA_DEMO.Models
{
    public static class OnlineUsers
    {
        public static List<UserView> Users
        {
            get
            {
                if (HttpRuntime.Cache["OnLineUsers"] == null)
                    HttpRuntime.Cache["OnLineUsers"] = new List<UserView>();
                return (List<UserView>)HttpRuntime.Cache["OnLineUsers"];
            }
        }

        public static UserView CurrentUser
        {
            get
            {
                try
                {
                        RefreshSessionFromDatabase();
                    
                    return (UserView)HttpContext.Current.Session["User"];
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    HttpContext.Current.Session.Timeout = 6;
                    HttpContext.Current.Session["User"] = value;
                }
                else
                {
                    if (HttpContext.Current != null)
                        HttpContext.Current.Session.Abandon();
                }
            }
        }
        public static DateTime LastUpdate
        {
            get
            {
                if (HttpRuntime.Cache["OnLineUsersUpdate"] == null)
                    HttpRuntime.Cache["OnLineUsersUpdate"] = new DateTime(0);
                return (DateTime)HttpRuntime.Cache["OnLineUsersUpdate"];
            }
            set
            {
                HttpRuntime.Cache["OnLineUsersUpdate"] = value;
            }
        }
        public static void AddSessionUser(UserView user)
        {
            Users.Add(user);
            using (DBEntities2 DB = new DBEntities2())
            {
                DB.AddUserLog(user);
            }
            LastUpdate = DateTime.Now;
            CurrentUser = user;
        }
        public static void RemoveSessionUser()
        {
            if (Users != null)
            {
                Users.Remove(CurrentUser);
                CurrentUser = null;
                LastUpdate = DateTime.Now;
            }
        }
        public static UserView GetSessionUser()
        {
            return CurrentUser;
        }

        public static UserView Find(int userId)
        {
            return Users.Where(u => u.Id == userId).FirstOrDefault();
        }

        public static bool NeedUpdate(DateTime date)
        {
            return DateTime.Compare(date, LastUpdate) < 0;
        }
        public static bool CurrentUserIsAdmin()
        {
            RefreshSessionFromDatabase();
            UserView user = CurrentUser;
            if (user != null)
                return user.Admin;
            return false;
        }
        public static bool IsOnLine(int userId)
        {
            foreach (UserView user in Users)
            {
                if (user.Id == userId)
                    return true;
            }
            return false;
        }
        public static void RefreshSessionFromDatabase()
        {
            int id;
            try
            {
                if ((UserView)HttpContext.Current.Session["User"] != null)
                {
                    id = ((UserView)HttpContext.Current.Session["User"]).Id;
                    using (DBEntities2 DB = new DBEntities2())
                    {
                        CurrentUser = DB.Users.Find(id).ToUserView();
                    }
                }
            }
            catch(Exception)
            {
                return;
            }
            

        }
    }
}