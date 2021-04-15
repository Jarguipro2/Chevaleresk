using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Chevaleresk.Models
{
    public class PlayerView
    {
        public int IdPlayer { get; set; }

        [Required(ErrorMessage ="Nom d'utilisateur requis")]
        [Display(Name ="Nom d'utilisateur")]
        public string Username { get; set; }

        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mot de passe requis")]
        [Display(Name = "Nouveau mot de passe")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Le mot de passe doit être confirmé")]
        [Display(Name ="Confirmation mot de passe")]
        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage ="La confirmation ne correspond pas")]
        public string Confirmation { get; set; }

        [Required(ErrorMessage = "Nom complet requis")]
        [Display(Name = "Nom complet")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Adresse courriel requis")]
        [Display(Name = "Adresse courriel")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public int Money { get; set; }
        public bool Admin { get; set; }

        public Players ToPlayer()
        {
            return new Players()
            {
                IdPlayer = this.IdPlayer,
                Username = this.Username,
                FullName = this.FullName,
                Password = this.Password,
                Email = this.Email,
                Admin = this.Admin

            };
        }
    }
    public static class MyExtension
    {
        public static PlayerView ToPlayerView(this Players player)
        {
            return new PlayerView()
            {
                IdPlayer = player.IdPlayer,
                Username = player.Username,
                FullName = player.FullName,
                Password = player.Password,
                Admin = player.Admin,
                Money = player.Money
            };
        }
        public static bool UsernameExist(this ChevalereskDBEntities2 DB,string username)
        {
            Players player = DB.Players.Where(p => p.Username == username).FirstOrDefault();
            return (player != null);
        }
        public static PlayerView AddPlayer(this ChevalereskDBEntities2 DB, PlayerView playerView)
        {
            Players player = playerView.ToPlayer();
            player = DB.Players.Add(player);
            DB.SaveChanges();
            return player.ToPlayerView();
        }
        public static bool UpdatePlayer(this ChevalereskDBEntities2 DB, PlayerView playerView)
        {
            Players playerToUpdate = playerView.ToPlayer();
            DB.Entry(playerToUpdate).State = EntityState.Modified;
            DB.SaveChanges();
            return true;
        }
        public static bool RemovePlayer(this ChevalereskDBEntities2 DB, PlayerView playerView)
        {
            Players playerToDelete = playerView.ToPlayer();
            DB.Players.Remove(playerToDelete);
            DB.SaveChanges();
            return true;
        }
        public static Players FindByUserName(this ChevalereskDBEntities2 DB,string username)
        {
            Players player = DB.Players.Where(u => u.Username == username).FirstOrDefault();
            return player;
        }
        public static void AddItem(this ChevalereskDBEntities2 DB, Items item)
        {
            DB.Items.Add(item);
            DB.SaveChanges();
        }
    }
    public static class OnlinePlayers
    {
        public static List<Players> Players
        {
            get
            {
                if (HttpRuntime.Cache["OnlinePlayers"] == null)
                    HttpRuntime.Cache["OnlinePlayers"] = new List<Players>();
                return (List <Players>) HttpRuntime.Cache["OnlinePlayers"];
            }
        }

        public static DateTime LastUpdate { get; private set; }

        public static void RemoveSessionPlayer()
        {
            if(HttpRuntime.Cache["OnlinePlayers"] != null)
            {
                ((List<Players>)HttpRuntime.Cache["OnlinePlayers"]).Remove(GetSessionPlayer());
            }
        }
        public static Players GetSessionPlayer()
        {
            try
            {
                return (Players)HttpContext.Current.Session["Player"];
            }
            catch
            {
                return null;
            }
        }
        public static void AddSessionPlayer(Players player)
        {
            Players.Add(player);
            LastUpdate = DateTime.Now;
            HttpContext.Current.Session["Player"] = player;
            HttpContext.Current.Session.Timeout = 30;
        }
    }
}