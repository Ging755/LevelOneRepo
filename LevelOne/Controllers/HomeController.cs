using LevelOne.Models;
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
            if(count < 3)
            {
                return View(Items.ToList());
            }
            else{
                Random rand = new Random();
                var number = rand.Next((count-3) + 1);
                Items = Items.Skip(number).AsQueryable();
            }
            return View(Items.ToList());
        }

        public async Task<ActionResult> ItemList(int? page, string search, string priceorder, int? categoryid)
        {
            return View();
        }
    }
}