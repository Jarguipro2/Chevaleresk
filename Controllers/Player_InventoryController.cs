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
    public class Player_InventoryController : Controller
    {
        private ChevalereskDBEntities2 db = new ChevalereskDBEntities2();

        // GET: Player_Inventory
        public ActionResult Index()
        {
            var player_Inventory = db.Player_Inventory.Include(p => p.Items).Include(p => p.Players);
            return View(player_Inventory.ToList());
        }

        // GET: Player_Inventory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player_Inventory player_Inventory = db.Player_Inventory.Find(id);
            if (player_Inventory == null)
            {
                return HttpNotFound();
            }
            return View(player_Inventory);
        }

        // GET: Player_Inventory/Create
        public ActionResult Create()
        {
            ViewBag.IdObject = new SelectList(db.Items, "IdObject", "Name");
            ViewBag.IdPlayer = new SelectList(db.Players, "IdPlayer", "Username");
            return View();
        }

        // POST: Player_Inventory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPlayer,IdObject,Quantity")] Player_Inventory player_Inventory)
        {
            if (ModelState.IsValid)
            {
                db.Player_Inventory.Add(player_Inventory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdObject = new SelectList(db.Items, "IdObject", "Name", player_Inventory.IdObject);
            ViewBag.IdPlayer = new SelectList(db.Players, "IdPlayer", "Username", player_Inventory.IdPlayer);
            return View(player_Inventory);
        }

        // GET: Player_Inventory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player_Inventory player_Inventory = db.Player_Inventory.Find(id);
            if (player_Inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdObject = new SelectList(db.Items, "IdObject", "Name", player_Inventory.IdObject);
            ViewBag.IdPlayer = new SelectList(db.Players, "IdPlayer", "Username", player_Inventory.IdPlayer);
            return View(player_Inventory);
        }

        // POST: Player_Inventory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPlayer,IdObject,Quantity")] Player_Inventory player_Inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player_Inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdObject = new SelectList(db.Items, "IdObject", "Name", player_Inventory.IdObject);
            ViewBag.IdPlayer = new SelectList(db.Players, "IdPlayer", "Username", player_Inventory.IdPlayer);
            return View(player_Inventory);
        }

        // GET: Player_Inventory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player_Inventory player_Inventory = db.Player_Inventory.Find(id);
            if (player_Inventory == null)
            {
                return HttpNotFound();
            }
            return View(player_Inventory);
        }

        // POST: Player_Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Player_Inventory player_Inventory = db.Player_Inventory.Find(id);
            db.Player_Inventory.Remove(player_Inventory);
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
