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
using System.IO;
using PagedList;

namespace LevelOne.Controllers
{
    [Authorize(Users = "admin@gmail.com")]
    public class ItemsController : Controller
    {
        private WebShopContext db = new WebShopContext();

        // GET: Items
        public async Task<ActionResult> ItemsList(int? page, string search, string priceorder, string discount)
        {
            page = page ?? 1;
            ViewBag.search = search;
            ViewBag.priceorder = priceorder;
            ViewBag.discount = discount;
            var items = db.Items.AsQueryable();
            IPagedList<Item> model;
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(x => x.Name.ToUpper().Contains(search.ToUpper())).AsQueryable();
            }
            if (!string.IsNullOrEmpty(discount))
            {
                if (discount == "true")
                {
                    items = items.Where(x => x.Discount == true).AsQueryable();
                }
                else
                {
                    items = items.Where(x => x.Discount == false).AsQueryable();
                }
            }
            if (!string.IsNullOrEmpty(priceorder))
            {
                switch (priceorder.ToUpper())
                {
                    case "ASCENDING":
                        items = items.OrderBy(x => x.Price).AsQueryable();
                        break;
                    case "DESCENDING":
                        items = items.OrderByDescending(x => x.Price).AsQueryable();
                        break;
                    default:
                        model = items.OrderByDescending(x => x.Id).ToPagedList((int)page, 3);
                        break;
                }
                model = items.ToPagedList((int)page, 6);
            }
            else
            {
                model = items.OrderByDescending(x => x.Id).ToPagedList((int)page, 3);
            }
            return View(new StaticPagedList<Item>(model, model.GetMetaData()));
        }

        // GET: Items/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Price,Discount,CategoryId")] Item item, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                item = InsertImage(item, image);
                db.Items.Add(item);
                await db.SaveChangesAsync();
                return RedirectToAction("ItemsList");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Price,Discount,CategoryId")] Item item, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    DeleteImage(item);
                    item = InsertImage(item, image);
                }
                db.Entry(item).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("ItemsList");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Item item = await db.Items.FindAsync(id);
            DeleteImage(item);
            db.Items.Remove(item);
            await db.SaveChangesAsync();
            return RedirectToAction("ItemsList");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public Item InsertImage(Item item, HttpPostedFileBase image)
        {
            if (image != null)
            {
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(image.FileName);
                var Extension = Path.GetExtension(image.FileName);
                item.ImgUrl = nameWithoutExtension + DateTime.Now.ToString("MMddyyHmmss") + Extension;
                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/"), item.ImgUrl);
                image.SaveAs(path);
            }
            else
            {
                item.ImgUrl = "defaultItem.png";
            }
            return item;
        }

        public void DeleteImage(Item item)
        {
            var path = System.Web.HttpContext.Current.Request.MapPath("~/Images/VehicleMakesImages/" + item.ImgUrl);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
