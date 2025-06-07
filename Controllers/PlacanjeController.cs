using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using OptiShape.Data;
using OptiShape.Models;

namespace OptiShape.Controllers
{
    [Authorize(Roles = "Administrator")] // Removed "Korisnik" role from authorization
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
            var placanja = await _context.Placanje
                .Include(p => p.Korisnik)
                .ToListAsync();
            return View(placanja);
        }

        // GET: Placanje/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var placanje = await _context.Placanje
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(m => m.IdPlacanja == id);

            if (placanje == null)
                return NotFound();

            return View(placanje);
        }

        // GET: Placanje/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            var korisnici = _context.Korisnik
                .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                .ToList();
            ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika", "PunoIme");
            return View();
        }

        // POST: Placanje/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("IdPlacanja,DatumPlacanja,Iznos,PopustPrimijenjen,NacinPlacanja,IdKorisnika")] Placanje placanje)
        {
            if (ModelState.IsValid)
            {
                _context.Add(placanje);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Plaćanje je uspješno dodano.";
                return RedirectToAction(nameof(Index));
            }

            // Prikaz grešaka u konzoli (debug)
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Greška za {entry.Key}: {error.ErrorMessage}");
                }
            }

            var korisnici = _context.Korisnik
                .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                .ToList();
            ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika", "PunoIme", placanje.IdKorisnika);
            return View(placanje);
        }

        // GET: Placanje/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var placanje = await _context.Placanje.FindAsync(id);
            if (placanje == null)
                return NotFound();

            var korisnici = _context.Korisnik
                .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                .ToList();
            ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika", "PunoIme", placanje.IdKorisnika);
            return View(placanje);
        }

        // POST: Placanje/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("IdPlacanja,DatumPlacanja,Iznos,PopustPrimijenjen,NacinPlacanja,IdKorisnika")] Placanje placanje)
        {
            if (id != placanje.IdPlacanja)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(placanje);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Plaćanje je uspješno ažurirano.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlacanjeExists(placanje.IdPlacanja))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Prikaz grešaka u konzoli (debug)
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Greška za {entry.Key}: {error.ErrorMessage}");
                }
            }

            var korisnici = _context.Korisnik
                .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                .ToList();
            ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika", "PunoIme", placanje.IdKorisnika);
            return View(placanje);
        }

        // GET: Placanje/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var placanje = await _context.Placanje
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(m => m.IdPlacanja == id);

            if (placanje == null)
                return NotFound();

            return View(placanje);
        }

        // POST: Placanje/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var placanje = await _context.Placanje.FindAsync(id);
            if (placanje != null)
            {
                _context.Placanje.Remove(placanje);
                TempData["SuccessMessage"] = "Plaćanje je uspješno obrisano.";
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