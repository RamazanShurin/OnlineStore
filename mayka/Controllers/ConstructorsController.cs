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
    public class ConstructorsController : Controller
    {
        private readonly AppDbContext _context;

        public ConstructorsController(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.Constructors.ToListAsync());
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mConstructor = await _context.Constructors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mConstructor == null)
            {
                return NotFound();
            }
            
            var listPhotos = await _context.Photos.Where(x => x.ConstructorId == id).ToListAsync();
            ViewBag.Photos = listPhotos;

            return View(mConstructor);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mConstructor = await _context.Constructors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mConstructor == null)
            {
                return NotFound();
            }

            return View(mConstructor);
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mConstructor = await _context.Constructors.FindAsync(id);
            _context.Constructors.Remove(mConstructor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MConstructorExists(int id)
        {
            return _context.Constructors.Any(e => e.Id == id);
        }
    }
}
