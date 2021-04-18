using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EFA_DEMO.Models;

namespace EFA_DEMO.Controllers
{
    public class ItemsController : Controller
    {
        private DBEntities2 db = new DBEntities2();

        // GET: Items
        public ActionResult Index(string sortOrder)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_asc" : "";
            ViewBag.PriceSortParm = sortOrder == "price" ? "price_asc" : "price";
            var TempItems = from i in db.Items
                            select i;
            switch (sortOrder)
            {
                case "name_desc":
                    TempItems = TempItems.OrderByDescending(s => s.Name);
                    break;
                case "price_asc":
                    TempItems = TempItems.OrderBy(s => s.Price);
                    break;
                case "price":
                    TempItems = TempItems.OrderBy(s => s.Price);
                    break;
                default:
                    TempItems = TempItems.OrderBy(s => s.Name);
                    break;
            }

            List<Item> items = db.Items.ToList();
            return View(TempItems.ToList());
        }

        // GET: Items/Details/5
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
            if (OnlineUsers.CurrentUser != null)
                ViewBag.currentUserOwnThisItem = db.UserHasItem(OnlineUsers.CurrentUser.ToUser(), item);
            else
                ViewBag.currentUserOwnThisItem = false;
            TempData["item"] = item;
            return View(item);
        }
        [UserAccess, HttpPost]
        public ActionResult Details([Bind(Include ="Star, Review")] Items_Reviews items_Review)
        {
            Item item = TempData["item"] as Item;
            items_Review.IdObject = item.IdObject;
            items_Review.IdPlayer = OnlineUsers.CurrentUser.Id;
            if (ModelState.IsValid)
            {
                db.Items_Reviews.Add(items_Review);
                db.SaveChanges();
            }

            return RedirectToAction("Details", "Items", item.IdObject);
        }
        // GET: Items/Create
        public ActionResult Create()
        {
            Item item = new Item();
            //var dictionnary = db.Items_Type.GroupBy(it => it.IdType).ToDictionary(g => g.Key, g => g.ToList());
            //item.Items_Type = dictionnary;
            return View(item);
        }

        // POST: Items/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Item item)
        {
            item.SaveAvatar();
            System.Diagnostics.Debug.WriteLine(item.PictureGUID);
            if (ModelState.IsValid)
            {
                db.AddItem(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // GET: Items/Edit/5
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
            return View(item);
        }

        // POST: Items/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
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
            return View(item);
        }

        // GET: Items/Delete/5
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

        // POST: Items/Delete/5
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
