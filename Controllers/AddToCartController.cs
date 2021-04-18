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
    public class AddToCartController : Controller
    {
        private DBEntities2 db = new DBEntities2();

        public ActionResult Add(int? id)
        {
            int compteurItems = 0;

            Item item = db.Items.Find(id);
            var itemStock = db.Items.Find(id);

            IDictionary<Item, int> DicItem = new Dictionary<Item, int>(new CartDictComparer());

            if (Session["cart"] == null)
            {
                DicItem.Add(item, 1);

                Session["cart"] = DicItem;
                ViewBag.cart = DicItem.Count();
                
                Session["count"] = 1;
            }
            else
            {
                DicItem = (Dictionary<Item, int>)Session["cart"];
                if (DicItem.ContainsKey(item))
                {
                    if(DicItem[item] > item.StockQuantity)
                    {
                        DicItem[item] = item.StockQuantity;
                        ViewBag.ObjetNonValide = " Votre Panier à été mis à jour vers notre stock le plus récent";
                    }
                    else
                    {
                        DicItem[item] += 1;
                    }
                }
                else
                {
                    if(item.StockQuantity <= 0)
                        ViewBag.ObjetNonValide = "L'item n'est plus en stock.";
                    else
                        DicItem.Add(item, 1);
                }
                
                //foreach (var valueItem in DicItem)
                //{
                //    compteurItems += DicItem[valueItem.Key];
                //}

                Session["cart"] = DicItem;
                ViewBag.cart = compteurItems;
                Session["count"] = Convert.ToInt32(Session["count"]) + 1;
            }
            return RedirectToAction("Myorder", "AddToCart");
        }
        public ActionResult Remove(int? id)
        {
            Item item = db.Items.Find(id);

            var DicItem = (Dictionary<Item, int>)Session["cart"];

            DicItem.Remove(item);
            Session["cart"] = DicItem;
            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
            return RedirectToAction("Myorder", "AddToCart");
        }
        [UserAccess]
        public ActionResult Myorder()
        {
            var currentPlayer = db.Users.Find(OnlineUsers.CurrentUser.Id);

            ViewBag.currentMoneyPlayer = currentPlayer.Money;

            double soldeTotal = 0;
          
            var query = (Dictionary<Item, int>)Session["cart"];

            if (query != null)
            {
                foreach (var items in query)
                {
                    var idObject = db.Items.Find(items.Key.IdObject);

                    soldeTotal += items.Key.Price * items.Value;

                    if (idObject.StockQuantity < items.Value)
                        ViewBag.ObjetNonValide += " " + items.Key.Name + " ";
                }
            }

            if (soldeTotal >= currentPlayer.Money)
                ViewBag.NotEnoughMoney = "Vous n'avez pas assez d'argent, pour compléter la transaction";
            else
                ViewBag.NotEnoughMoney = "";

            return View((Dictionary<Item, int>)Session["cart"]);
        }

        public ActionResult BuyCart()
        {
            double soldeTotal = 0;
            
            var currentPlayer = db.Users.Find(OnlineUsers.CurrentUser.Id);

            if ((Dictionary<Item, int>)Session["cart"] != null)
            {
                var query = (Dictionary<Item, int>)Session["cart"];

                foreach (var itemCost in query)
                    soldeTotal += itemCost.Value * itemCost.Key.Price;

                foreach (var itemsReceive in query)
                {
                    var itemDB = db.Items.Find(itemsReceive.Key.IdObject);

                    if (itemDB.StockQuantity >= itemsReceive.Value && currentPlayer.Money - (int)soldeTotal >= 0)
                    {
                        itemDB.StockQuantity -= itemsReceive.Value;
                        currentPlayer.Money -= (int)soldeTotal;

                        var invIdStatus = db.User_Inventory.SingleOrDefault(m => m.IdObject == itemsReceive.Key.IdObject);
                        var idPlayerStatus = db.User_Inventory.SingleOrDefault(m => m.IdPlayer == currentPlayer.IdPlayer);

                        if (idPlayerStatus != null && invIdStatus != null)
                        {
                            idPlayerStatus.Quantity += itemsReceive.Value;
                        }
                        else
                        {
                            User_Inventory inventaire = new User_Inventory();

                            inventaire.IdPlayer = currentPlayer.IdPlayer;
                            inventaire.IdObject = itemsReceive.Key.IdObject;
                            inventaire.Quantity = itemsReceive.Value;

                            db.User_Inventory.Add(inventaire);
                        }

                        db.SaveChanges();
                    }
                }

                query.Clear();
            }

            return RedirectToAction("MyOrder", "AddToCart");
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
