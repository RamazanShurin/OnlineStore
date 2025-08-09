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
    public class ClientsController : Controller
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mClient = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mClient == null)
            {
                return NotFound();
            }

            return View(mClient);
        }

        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(MClient mClient)
        //{
        //    _context.Add(mClient);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mClient = await _context.Clients.FindAsync(id);
            if (mClient == null)
            {
                return NotFound();
            }
            return View(mClient);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MClient mClient)
        {
            if (id != mClient.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(mClient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MClientExists(mClient.Id))
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

            var mClient = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mClient == null)
            {
                return NotFound();
            }

            return View(mClient);
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mClient = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(mClient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
