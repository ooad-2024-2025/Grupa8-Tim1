using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OptiShape.Data;
using OptiShape.Models;

namespace OptiShape.Controllers
{
    public class PlacanjeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlacanjeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Placanje
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Placanje.Include(p => p.Korisnik);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Placanje/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placanje = await _context.Placanje
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(m => m.IdPlacanja == id);
            if (placanje == null)
            {
                return NotFound();
            }

            return View(placanje);
        }

        // GET: Placanje/Create
        public IActionResult Create()
        {
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika");
            return View();
        }

        // POST: Placanje/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPlacanja,DatumPlacanja,Iznos,PopustPrimijenjen,NacinPlacanja,IdKorisnika")] Placanje placanje)
        {
            if (ModelState.IsValid)
            {
                _context.Add(placanje);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", placanje.IdKorisnika);
            return View(placanje);
        }

        // GET: Placanje/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placanje = await _context.Placanje.FindAsync(id);
            if (placanje == null)
            {
                return NotFound();
            }
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", placanje.IdKorisnika);
            return View(placanje);
        }

        // POST: Placanje/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPlacanja,DatumPlacanja,Iznos,PopustPrimijenjen,NacinPlacanja,IdKorisnika")] Placanje placanje)
        {
            if (id != placanje.IdPlacanja)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(placanje);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlacanjeExists(placanje.IdPlacanja))
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
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", placanje.IdKorisnika);
            return View(placanje);
        }

        // GET: Placanje/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placanje = await _context.Placanje
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(m => m.IdPlacanja == id);
            if (placanje == null)
            {
                return NotFound();
            }

            return View(placanje);
        }

        // POST: Placanje/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var placanje = await _context.Placanje.FindAsync(id);
            if (placanje != null)
            {
                _context.Placanje.Remove(placanje);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlacanjeExists(int id)
        {
            return _context.Placanje.Any(e => e.IdPlacanja == id);
        }
    }
}
