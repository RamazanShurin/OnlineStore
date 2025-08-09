using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mayka.Helpers;
using mayka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mayka.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                return View();
            }
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Product.Price);
            
            return View();
        }

        public IActionResult Buy(string id, string size)
        {
            MProduct productModel = _context.Products.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { Product = productModel, Size = size });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                int count = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart").Count();
                SessionHelper.SetCartCount(HttpContext.Session, count);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

                cart.Add(new Item { Product = productModel, Size = size });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                int count = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart").Count();
                SessionHelper.SetCartCount(HttpContext.Session, count);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(string id, string size)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

            int index = isExist(id, size);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            int count = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart").Count();
            SessionHelper.SetCartCount(HttpContext.Session, count);
            return RedirectToAction("Index");
        }

        public IActionResult Double(string id, string size)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            MProduct product = _context.Products.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
            Item item = new Item() { Product = product, Size = size };
            cart.Add(item);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            int count = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart").Count();
            SessionHelper.SetCartCount(HttpContext.Session, count);
            return RedirectToAction("Index");
        }

        private int isExist(string id, string size)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id == Convert.ToInt32(id) && cart[i].Size == size)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}