using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mayka.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace mayka.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private IHostingEnvironment _env;

        public ProductsController(AppDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = await _context.Products.Include(m => m.Category).ToListAsync();
            return View(appDbContext);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mProduct = await _context.Products
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mProduct == null)
            {
                return NotFound();
            }

            return View(mProduct);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MProduct mProduct, IFormFile image, IFormFile imageBack)
        {
            string pathServer = Path.Combine(_env.WebRootPath, "Images");
            if (!Directory.Exists(pathServer))
            {
                Directory.CreateDirectory(pathServer);
            }
            if (image != null)
            {
                string imageName = DateTime.Now.Day + DateTime.Now.Minute +DateTime.Now.Second + DateTime.Now.Hour + image.FileName;
                string imagePath = Path.Combine(pathServer, imageName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                mProduct.Img = imageName;
            }
            if (imageBack != null)
            {
                string imageName = DateTime.Now.Day + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Hour + imageBack.FileName;
                string imagePath = Path.Combine(pathServer, imageName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageBack.CopyToAsync(fileStream);
                }
                mProduct.ImgBack = imageName;
            }
            _context.Add(mProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mProduct = await _context.Products.FindAsync(id);
            if (mProduct == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", mProduct.CategoryId);
            return View(mProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MProduct mProduct, IFormFile image, IFormFile imageBack)
        {
            if (id != mProduct.Id)
            {
                return NotFound();
            }

            try
            {
                string pathServer = Path.Combine(_env.WebRootPath, "Images");
                if (!Directory.Exists(pathServer))
                {
                    Directory.CreateDirectory(pathServer);
                }
                if (image != null)
                {
                    string imageName = DateTime.Now.Day + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Hour + image.FileName;
                    string imagePath = Path.Combine(pathServer, imageName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    mProduct.Img = imageName;
                }
                if (imageBack != null)
                {
                    string imageName = DateTime.Now.Day + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Hour + imageBack.FileName;
                    string imagePath = Path.Combine(pathServer, imageName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageBack.CopyToAsync(fileStream);
                    }
                    mProduct.ImgBack = imageName;
                }
                _context.Update(mProduct);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MProductExists(mProduct.Id))
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

            var mProduct = await _context.Products
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mProduct == null)
            {
                return NotFound();
            }

            return View(mProduct);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mProduct = await _context.Products.FindAsync(id);
            _context.Products.Remove(mProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
