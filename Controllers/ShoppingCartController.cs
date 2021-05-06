using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EFA_DEMO.Models;

namespace EFA_DEMO.Controllers
{
    public class ShoppingCartController : Controller
    {
        private DBEntities2 db = new DBEntities2();

        [UserAccess]
        public ActionResult Add(int? id, int quantityReceive)
        {
            Item item = db.Items.Find(id);

            IDictionary<Item, int> DicItem = new Dictionary<Item, int>(new CartDictComparer());

            if (Session["cart"] != null)
            {
                DicItem = (Dictionary<Item, int>)Session["cart"];
            }
            if (DicItem.ContainsKey(item) && DicItem[item] <= 0)
            {
                ViewBag.ObjetNonValide = "L'item n'est plus en stock.";
                DicItem[item] = 0;
            }
            if (quantityReceive > item.StockQuantity)
            {
                DicItem[item] = item.StockQuantity;
                Session["error"] = " Votre panier à été mis à jour vers notre stock le plus récent";
            }
            else
            {
                if (DicItem.ContainsKey(item))
                    DicItem[item] = quantityReceive;
                else
                    DicItem.Add(item, quantityReceive);
            }

            Session["cart"] = DicItem;
            Session["count"] = Convert.ToInt32(Session["count"]) + 1;

            return RedirectToAction("Index", "Items");
        }
        [UserAccess]
        public ActionResult Remove(int? id)
        {
            Item item = db.Items.Find(id);

            var DicItem = (Dictionary<Item, int>)Session["cart"];

            DicItem.Remove(item);
            Session["cart"] = DicItem;
            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
            return RedirectToAction("ShoppingCart", "ShoppingCart");
        }
        [UserAccess]
        public ActionResult ShoppingCart()
        {
            var currentPlayer = db.Users.Find(OnlineUsers.CurrentUser.Id);

            ViewBag.currentMoneyPlayer = currentPlayer.Money;
            ViewBag.itemsSession = 0;
           
            double soldeTotal = 0;
          
            var panierJoueur = (Dictionary<Item, int>)Session["cart"];

            if (panierJoueur != null)
            {
                foreach (var items in panierJoueur)
                {
                    var idObject = db.Items.Find(items.Key.IdObject);

                    soldeTotal += items.Key.Price * items.Value;

                    if (idObject.StockQuantity < items.Value)
                        ViewBag.ObjetNonValide += " " + items.Key.Name + " ";

                    ViewBag.itemsSession += items.Value;
                }
            }

            if (soldeTotal > currentPlayer.Money)
                ViewBag.NotEnoughMoney = "Vous n'avez pas assez d'argent, pour compléter la transaction";
            else
                ViewBag.NotEnoughMoney = "";

            return View((Dictionary<Item, int>)Session["cart"]);
        }

        [UserAccess]
        public ActionResult BuyCart()
        {
            double soldeTotal = 0;
            bool panierValide = true;
            
            var currentPlayer = db.Users.Find(OnlineUsers.CurrentUser.Id);

            if ((Dictionary<Item, int>)Session["cart"] != null)
            {
                var panierJoueur = (Dictionary<Item, int>)Session["cart"];

                foreach (var item in panierJoueur)
                {
                    if (db.Items.Find(item.Key.IdObject).StockQuantity < item.Value)
                        panierValide = false;

                    soldeTotal += item.Value * item.Key.Price;
                }

                if (currentPlayer.Money - (int)soldeTotal >= 0 && panierValide)
                {
                    foreach (var itemsReceive in panierJoueur)
                    {
                        var itemDB = db.Items.Find(itemsReceive.Key.IdObject);
                
                        itemDB.StockQuantity -= itemsReceive.Value;

                        if (db.UserHasItem(currentPlayer, itemsReceive.Key))
                        {
                            var invIdStatus = db.User_Inventory.FirstOrDefault(m => m.IdObject == itemsReceive.Key.IdObject && m.IdPlayer == currentPlayer.IdPlayer);
                            invIdStatus.Quantity += itemsReceive.Value;
                        }
                        else
                        {
                            User_Inventory inventaire = new User_Inventory();

                            inventaire.IdPlayer = currentPlayer.IdPlayer;
                            inventaire.IdObject = itemsReceive.Key.IdObject;
                            inventaire.Quantity = itemsReceive.Value;

                            db.User_Inventory.Add(inventaire);
                        }
                    }

                    currentPlayer.Money -= (int)soldeTotal;
                    panierJoueur.Clear();

                    db.SaveChanges();
                }
            }

            return RedirectToAction("ShoppingCart", "ShoppingCart");
        }

        public ActionResult UpdateItemQuantity(int id, int quantity)
        {
            var cart = Session["cart"] as Dictionary<Item, int>;
            var item = cart.FirstOrDefault(itemCart => itemCart.Key.IdObject == id);

            if (quantity <= item.Key.StockQuantity && quantity > 0)
            {
                cart[item.Key] = quantity;
                Session["error"] = "";
            }
            else
            {
                cart[item.Key] = item.Key.StockQuantity;
                Session["error"] = " Votre panier à été mis à jour vers notre stock le plus récent";
            }

            Session["cart"] = cart;
            return RedirectToAction("ShoppingCart");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
