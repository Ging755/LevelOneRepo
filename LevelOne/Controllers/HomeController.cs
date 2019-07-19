﻿using LevelOne.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LevelOne.Controllers
{
    public class HomeController : Controller
    {
        WebShopContext db = new WebShopContext();
        // GET: Home
        public async Task<ActionResult> Index()
        {
            ViewBag.categories = db.Categories.ToList();
            var Items = db.Items.Where(x => x.Discount == true).AsQueryable();
            var count = Items.Count();
            if (count < 6)
            {
                return View(Items.ToList());
            }
            else
            {
                Random rand = new Random();
                var number = rand.Next(1, (count - 6) + 1);
                Items = Items.OrderByDescending(x => x.Id).Skip(number).Take(6).AsQueryable();
            }
            return View(Items.ToList());
        }

        public async Task<ActionResult> ItemList(int? page, string search, string priceorder, int categoryid)
        {
            page = page ?? 1;
            ViewBag.oave = page;
            ViewBag.search = search;
            ViewBag.priceorder = priceorder;
            ViewBag.categoryid = categoryid;
            var items = db.Items.Where(x => x.CategoryId == categoryid);
            IPagedList<Item> model;
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(x => x.Name.ToUpper().Contains(search.ToUpper())).AsQueryable();
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
                        model = items.OrderByDescending(x => x.Id).ToPagedList((int)page, 6);
                        break;
                }
                model = items.ToPagedList((int)page, 6);
            }
            else
            {
                model = items.OrderByDescending(x => x.Id).ToPagedList((int)page, 6);
            }
            return View(new StaticPagedList<Item>(model, model.GetMetaData()));
        }

        public async Task<ActionResult> AddToCart(int? id, int? page, string search, string priceorder, int? categoryid)
        {
            Item item = db.Items.Where(x => x.Id == id).FirstOrDefault();
            List<Item> ItemList = new List<Item>();
            if (Session["products"] == null)
            {
                ItemList.Add(item);
                Session["products"] = ItemList;
            }
            else
            {
                ItemList = Session["products"] as List<Item>;
                ItemList.Add(item);
                Session["products"] = ItemList;
            }
            if(categoryid == null)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("ItemList", new { page = page, search = search, priceorder = priceorder, categoryid = categoryid });
        }

        public async Task<ActionResult> CategoryPicker()
        {
            var categories = db.Categories.ToList();
            return PartialView("_CategoryPicker", categories);
        }

        public async Task<ActionResult> Cart()
        {
            var items = Session["products"] as List<Item>;
            if(items != null){
                decimal totalprice = 0;
                foreach (var x in items)
                {
                    totalprice += x.Price;
                }
                ViewBag.totalprice = totalprice;
            }
            return View(items);
        }
        
        public async Task<ActionResult> RemoveFromCart(int? id)
        {
            var items = Session["products"] as List<Item>;
            items.Remove(items.Where(x => x.Id == id).FirstOrDefault());
            Session["products"] = items;
            return RedirectToAction("Cart");
        }
    }
}