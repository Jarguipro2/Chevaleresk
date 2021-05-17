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
        public ActionResult Index(string sortOrder, string searchString, string filterByType, string filterByReview, string filterByPriceMin, string filterByPriceMax)
        {
            //For view hyperlinks
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) || sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "type_desc" : "Type";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";
            ViewBag.ReviewSearch = filterByReview == "0" ? 0 : 0;

            //Fetching items
            var items = from i in db.Items
                        select i;
            //Filter if searching
            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(i => i.Name.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(filterByType))
            {
                items = items.Where(i => i.Items_Type.Name.Contains(filterByType));
            }
            if (!String.IsNullOrEmpty(filterByPriceMin))
            {
                int minPrice = int.Parse(filterByPriceMin);
                items = items.Where(i => i.Price >= minPrice);
            }
            if (!String.IsNullOrEmpty(filterByPriceMax))
            { 
                int maxPrice = int.Parse(filterByPriceMax);
                items = items.Where(i => i.Price <= maxPrice);
            }
            if (!String.IsNullOrEmpty(filterByReview))
            {
                int reviewFilter = int.Parse(filterByReview);
                if (reviewFilter != 0)
                {
                    items = items.Where(i => Math.Floor(i.Items_Reviews.Average(r => r.Star)) == reviewFilter);
                }
            }
            
               
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(i => i.Name);
                    break;
                case "Name":
                    items = items.OrderBy(i => i.Name);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(i => i.Price);
                    break;
                case "Price":
                    items = items.OrderBy(i => i.Price); 
                    break;
                default:
                    items = items.OrderBy(i => i.Name);
                    break;
            }

            return View(items.ToList());
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
            int reviewsCount = item.Items_Reviews.Count;
            if (reviewsCount != 0)
            {
                ViewBag.HasReview = true;
                
                ViewBag.FiveStarReviewPercentage = Math.Floor(((double)item.Items_Reviews.Count(i => i.Star == 5)) / reviewsCount * 100);
                if (double.IsNaN(ViewBag.FiveStarReviewPercentage))
                    ViewBag.FiveStarReviewPercentage = 0;

                ViewBag.FourStarReviewPercentage = Math.Floor(((double)item.Items_Reviews.Count(i => i.Star == 4)) / reviewsCount * 100);
                if (double.IsNaN(ViewBag.FourStarReviewPercentage))
                    ViewBag.FourStarReviewPercentage = 0;

                ViewBag.ThreeStarReviewPercentage = Math.Floor(((double)item.Items_Reviews.Count(i => i.Star == 3)) / reviewsCount * 100);
                if (double.IsNaN(ViewBag.ThreeStarReviewPercentage))
                    ViewBag.ThreeStarReviewPercentage = 0;

                ViewBag.TwoStarReviewPercentage = Math.Floor(((double)item.Items_Reviews.Count(i => i.Star == 2)) / reviewsCount * 100);
                if (double.IsNaN(ViewBag.TwoStarReviewPercentage))
                    ViewBag.TwoStarReviewPercentage = 0;

                ViewBag.OneStarReviewPercentage = Math.Floor(((double)item.Items_Reviews.Count(i => i.Star == 1)) / reviewsCount * 100);
                if (double.IsNaN(ViewBag.OneStarReviewPercentage))
                    ViewBag.OneStarReviewPercentage = 0;
            }
            else
                ViewBag.HasReview = false;
                
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
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult RemoveReview(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Items_Reviews review = db.Items_Reviews.Find(id);
            if (review == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if(OnlineUsers.CurrentUserIsAdmin() || review.IdPlayer == OnlineUsers.CurrentUser.Id)
            {
                db.Items_Reviews.Remove(review);
                db.SaveChanges();
            }
            return RedirectToAction("Details/" + review.IdObject, "Items");
        }


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

            var inventory = db.User_Inventory.Where(c => c.IdObject == item.IdObject).FirstOrDefault();

            if (inventory == null)
            {
                item.RemoveAvatar();
                db.Items.Remove(item);
            }
            else
            {
                item.StockQuantity = 0;
            }

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
