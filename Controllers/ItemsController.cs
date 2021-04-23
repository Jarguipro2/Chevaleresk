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
            var tempView = db.Items.Where(x => x.Name.Contains(sortOrder) || sortOrder == null);
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_asc" : "";
            if (sortOrder == "name_asc")
            {
                ViewBag.NameSortParm = "name_desc";
            }

            ViewBag.PriceSortParm = sortOrder == "price" ? "price" : "price";
            if (sortOrder == "price")
            {
                ViewBag.PriceSortParm = "price_desc";
            }

            ViewBag.TypeSortParm = "type_arme";
            if (sortOrder == "type_arme")
            {
                ViewBag.TypeSortParm = "type_armure";
            }
            else if (sortOrder == "type_armure")
            {
                ViewBag.TypeSortParm = "type_potion";
            }
            else if (sortOrder == "type_potion")
            {
                ViewBag.TypeSortParm = "type_ressource";
            }

            var TempItems = from i in db.Items
                            select i;
            switch (sortOrder)
            {
                case "name_desc":
                    TempItems = TempItems.OrderByDescending(i => i.Name);
                    break;
                case "name_asc":
                    TempItems = TempItems.OrderBy(i => i.Name);
                    break;
                case "price":
                    TempItems = TempItems.OrderBy(i => i.Price);
                    break;
                case "price_desc":
                    TempItems = TempItems.OrderByDescending(i => i.Price);
                    break;
                case "type_arme":
                    return View(db.Items.Where(x => x.Items_Type.Name.Contains("arme")).ToList());
                case "type_armure":
                    return View(db.Items.Where(x => x.Items_Type.Name.Contains("armure")).ToList());
                case "type_potion":
                    return View(db.Items.Where(x => x.Items_Type.Name.Contains("potion")).ToList());
                case "type_ressource":
                    return View(db.Items.Where(x => x.Items_Type.Name.Contains("ressource")).ToList());
                default:
                    return View(db.Items.Where(x => x.Name.Contains(sortOrder) || sortOrder == null).ToList());
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

        [AdminAccess]
        public ActionResult Create()
        {
            Item item = new Item();
            return View(item);
        }

        [AdminAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Item item)
        {
            item.SaveAvatar();
            if (ModelState.IsValid)
            {
                db.AddItem(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // GET: Items/Edit/5
        [AdminAccess]
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
        [AdminAccess]
        public ActionResult Edit(Item item)
        {
            db.UpdateItem(item);
            if (ModelState.IsValid)
            {
                //db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // GET: Items/Delete/5
        [AdminAccess]
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
