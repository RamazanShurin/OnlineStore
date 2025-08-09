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
    public class CommentsController : Controller
    {
        private readonly AppDbContext _context;
        private IHostingEnvironment _env;

        public CommentsController(AppDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Comments.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mComment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mComment == null)
            {
                return NotFound();
            }

            return View(mComment);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MComment mComment, IFormFile image)
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
                mComment.Img = imageName;
            }
            _context.Add(mComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mComment = await _context.Comments.FindAsync(id);
            if (mComment == null)
            {
                return NotFound();
            }
            return View(mComment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MComment mComment, IFormFile image)
        {
            if (id != mComment.Id)
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
                    mComment.Img = imageName;
                }
                _context.Update(mComment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MCommentExists(mComment.Id))
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

            var mComment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mComment == null)
            {
                return NotFound();
            }

            return View(mComment);
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mComment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(mComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MCommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
