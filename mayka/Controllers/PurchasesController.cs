using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mayka.Models;
using Microsoft.AspNetCore.Authorization;

namespace mayka.Controllers
{
    [Authorize]
    public class PurchasesController : Controller
    {
        private readonly AppDbContext _context;

        public PurchasesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Purchases.Include(m => m.Client);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mPurchase = await _context.Purchases
                .Include(m => m.Client)
                .Include(m => m.PurProducts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mPurchase == null)
            {
                return NotFound();
            }

            //List<MProduct> productsPurch = new List<MProduct>();
            //foreach (var item in mPurchase.PurProducts)
            //{
            //    var mProducts = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
            //    productsPurch.Add(mProducts);
            //}

            //if (productsPurch == null || productsPurch.Count == 0)
            //{
            //    ViewBag.ProductsPurch = null;
            //}
            //else
            //{
            //    ViewBag.ProductsPurch = productsPurch;
            //}
            return View(mPurchase);
        }

        //public IActionResult Create()
        //{
        //    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Id");
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(MPurchase mPurchase)
        //{
        //    _context.Add(mPurchase);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mPurchase = await _context.Purchases.FindAsync(id);
            if (mPurchase == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Id", mPurchase.ClientId);
            return View(mPurchase);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MPurchase mPurchase)
        {
            if (id != mPurchase.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(mPurchase);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MPurchaseExists(mPurchase.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mPurchase = await _context.Purchases
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mPurchase == null)
            {
                return NotFound();
            }

            return View(mPurchase);
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mPurchase = await _context.Purchases.FindAsync(id);
            _context.Purchases.Remove(mPurchase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MPurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}
