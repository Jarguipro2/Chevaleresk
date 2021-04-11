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
            Item item = db.Items.Find(id);

            if (Session["cart"] == null)
            {
                List<Item> li = new List<Item>();

                li.Add(item);
                Session["cart"] = li;
                ViewBag.cart = li.Count();
                
                Session["count"] = 1;
            }
            else
            {
                List<Item> li = (List<Item>)Session["cart"];
              
                li.Add(item);
                Session["cart"] = li;
                ViewBag.cart = li.Count();

                Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                
            }
            return RedirectToAction("Myorder", "AddToCart");
        }
        public ActionResult Remove(int? id)
        {
            Item item = db.Items.Find(id);

            List<Item> li = (List<Item>)Session["cart"];
            li.RemoveAll(x => x.IdObject == item.IdObject);
            Session["cart"] = li;
            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
            return RedirectToAction("Myorder", "AddToCart");
        }

        public ActionResult Myorder()
        {
            int nombreItemsSession = 0;

            if ((List<Item>)Session["cart"] != null)
            {
                foreach (var item in (List<Item>)Session["cart"])
                    nombreItemsSession++;
            }

            ViewBag.cart = nombreItemsSession;

            return View((List<Item>)Session["cart"]);
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
