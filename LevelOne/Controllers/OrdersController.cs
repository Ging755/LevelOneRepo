using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LevelOne.Models;

namespace LevelOne.Controllers
{
    public class OrdersController : Controller
    {
        private WebShopContext db = new WebShopContext();

        // GET: Orders
        [Authorize(Users = "admin@gmail.com")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Orders.ToListAsync());
        }

        // GET: Orders/Details/5
        [Authorize(Users = "admin@gmail.com")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,RecipientName,RecipientSurname,RecipientAdress,RecipientZipCode,RecipientEmail,RecipientPhone,OrderReciveTime,OrderSent")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderedItems = Session["products"] as IEnumerable<Item>;
                order.OrderSent = false;
                order.OrderReciveTime = DateTime.Now.ToString();
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return PartialView("_OrderSent");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Users = "admin@gmail.com")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@gmail.com")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,RecipientName,RecipientSurname,RecipientAdress,RecipientZipCode,RecipientEmail,RecipientPhone,OrderReciveTime,OrderSent")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderReciveTime = db.Orders.Find(order.Id).OrderReciveTime;
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Users = "admin@gmail.com")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@gmail.com")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
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
