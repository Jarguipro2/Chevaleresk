using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Chevaleresk.Models;

namespace Chevaleresk.Controllers
{
    public class AddToCartController : Controller
    {
        private ChevalereskDBEntities2 db = new ChevalereskDBEntities2();

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
            Items items = db.Items.Find(id);
            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
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
        public ActionResult Create([Bind(Include = "IdObject,Name,StockQuantity,Price,PictureGUID,IdType")] Items items)
        {
            if (ModelState.IsValid)
            {
                db.Items.Add(items);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdType = new SelectList(db.Items_Type, "IdType", "Name", items.IdType);
            return View(items);
        }

        // GET: AddToCart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items items = db.Items.Find(id);
            if (items == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdType = new SelectList(db.Items_Type, "IdType", "Name", items.IdType);
            return View(items);
        }

        // POST: AddToCart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdObject,Name,StockQuantity,Price,PictureGUID,IdType")] Items items)
        {
            if (ModelState.IsValid)
            {
                db.Entry(items).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdType = new SelectList(db.Items_Type, "IdType", "Name", items.IdType);
            return View(items);
        }

        // GET: AddToCart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items items = db.Items.Find(id);
            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        // POST: AddToCart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Items items = db.Items.Find(id);
            db.Items.Remove(items);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Add(Items mo)
        {
            if (Session["cart"] == null)
            {
                List<Items> li = new List<Items>();

                li.Add(mo);
                Session["cart"] = li;
                ViewBag.cart = li.Count();

                Session["count"] = 1;
            }
            else
            {
                List<Items> li = (List<Items>)Session["cart"];
                li.Add(mo);
                Session["cart"] = li;
                ViewBag.cart = li.Count();
                Session["count"] = Convert.ToInt32(Session["count"]) + 1;

            }
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Myorder()
        {
            if ((List<Items>)Session["cart"] != null)
                return View((List<Items>)Session["cart"]);

            else
                return View();
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
