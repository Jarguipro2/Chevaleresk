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
                    DicItem[item] += 1;
                else
                    DicItem.Add(item, 1);

                foreach (var valueItem in DicItem)
                {
                    compteurItems += DicItem[valueItem.Key];
                }

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

        public ActionResult Myorder()
        {
            // --- Version si le user essaie de force l'entré ALPHA 0.01 ---
            //var currentPlayer = null;

            //if (OnlineUsers.CurrentUser != null)
            //{
            //    currentPlayer = db.Users.Find(OnlineUsers.CurrentUser.Id);
            //}
            // ------------------------------------------------------------

            //Dictionary<Item, int> openWith =
            //    new Dictionary<Item, int>();

            var currentPlayer = db.Users.Find(OnlineUsers.CurrentUser.Id);

            ViewBag.currentMoneyPlayer = currentPlayer.Money;

            int nombreItemsSession = 0;
            double soldeTotal = 0;

            if ((Dictionary<Item, int>)Session["cart"] != null)
            {
                var query = (Dictionary<Item, int>)Session["cart"];

              
            }
            if (soldeTotal >= currentPlayer.Money)
            {
                ViewBag.NotEnoughMoney = "Vous n'avez pas assez d'argent, pour compléter la transaction";
            }

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

                        //db.UpdateUser(currentPlayer.ToUserView());
                        db.Entry(itemDB).State = EntityState.Modified;

                        var invIdStatus = db.User_Inventory.SingleOrDefault(m => m.IdPlayer == currentPlayer.IdPlayer);
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


        // GET: AddToCart
        public ActionResult Index()
        {
            var items = db.Items.Include(i => i.Items_Type);
            return View(items.ToList());
        }

        // GET: AddToCart/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: AddToCart/Create
        public ActionResult Create()
        {
            ViewBag.IdType = new SelectList(db.Items_Type, "IdType", "Name");
            return View();
        }

        // POST: AddToCart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdObject,Name,StockQuantity,Price,PictureGUID,IdType")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdType = new SelectList(db.Items_Type, "IdType", "Name", item.IdType);
            return View(item);
        }

        // GET: AddToCart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdType = new SelectList(db.Items_Type, "IdType", "Name", item.IdType);
            return View(item);
        }

        // POST: AddToCart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdObject,Name,StockQuantity,Price,PictureGUID,IdType")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdType = new SelectList(db.Items_Type, "IdType", "Name", item.IdType);
            return View(item);
        }

        // GET: AddToCart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: AddToCart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
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
