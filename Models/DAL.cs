﻿using EFA_DEMO.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

public static class DBEntitiesExtensionsMethods
{
    public static UserView ToUserView(this User user)
    {
        return new UserView()
        {
            Id = user.IdPlayer,
            AvatarId = user.AvatarId,
            Username = user.Username,
            FullName = user.FullName,
            Password = user.Password,
            Admin = user.Admin
        };
    }
    public static bool UserNameExist(this DBEntities2 DB, string userName)
    {
        User user = DB.Users.Where(u => u.Username == userName).FirstOrDefault();
        return (user != null);
    }
    public static User FindByUserName(this DBEntities2 DB, string userName)
    {
        User user = DB.Users.Where(u => u.Username == userName).FirstOrDefault();
        return user;
    }
    public static UserView AddUser(this DBEntities2 DB, UserView userView)
    {
        userView.SaveAvatar();
        User user = userView.ToUser();
        user = DB.Users.Add(user);
        DB.SaveChanges();
        return user.ToUserView();
    }
    public static bool UpdateUser(this DBEntities2 DB, UserView userView)
    {
        userView.SaveAvatar();
        User userToUpdate = DB.Users.Find(userView.Id);
        userView.CopyToUser(userToUpdate);
        DB.Entry(userToUpdate).State = EntityState.Modified;
        DB.SaveChanges();
        return true;
    }
    public static bool RemoveUser(this DBEntities2 DB, UserView userView)
    {
        userView.RemoveAvatar();
        User userToDelete = DB.Users.Find(userView.Id);
        DB.Users.Remove(userToDelete);
        DB.SaveChanges();
        return true;
    }

    public static Log AddUserLog(this DBEntities2 DB, UserView userView)
    {
        Log log = new Log();
        log.IdPlayer = userView.Id;
        log.LoginDate = DateTime.Now;
        DB.Logs.Add(log);
        DB.SaveChanges();
        return log;
    }
}