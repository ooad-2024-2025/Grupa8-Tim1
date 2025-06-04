using System;
using System.Collections.Generic;
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
    [Authorize(Roles = "Administrator, Korisnik")]
    public class StatistikeNapretkaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatistikeNapretkaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StatistikeNapretka
        public async Task<IActionResult> Index()
        {
            var statistike = await _context.StatistikeNapretka
                .Include(s => s.Korisnik)
                .ToListAsync();
            return View(statistike);
        }

        // GET: StatistikeNapretka/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var statistika = await _context.StatistikeNapretka
                .Include(s => s.Korisnik)
                .FirstOrDefaultAsync(m => m.IdZapisa == id);

            if (statistika == null)
                return NotFound();

            return View(statistika);
        }

        // GET: StatistikeNapretka/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            var korisnici = _context.Korisnik
                .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                .ToList();
            ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika", "PunoIme");
            return View();
        }

        // POST: StatistikeNapretka/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("IdZapisa,Datum,Tezina,Bmi,KalorijskiUnos,IdKorisnika")] StatistikeNapretka statistikeNapretka)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statistikeNapretka);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Statistika napretka je uspješno dodana.";
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
            ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika", "PunoIme", statistikeNapretka.IdKorisnika);
            return View(statistikeNapretka);
        }

        // GET: StatistikeNapretka/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var statistika = await _context.StatistikeNapretka.FindAsync(id);
            if (statistika == null)
                return NotFound();

            var korisnici = _context.Korisnik
                .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                .ToList();
            ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika", "PunoIme", statistika.IdKorisnika);
            return View(statistika);
        }

        // POST: StatistikeNapretka/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("IdZapisa,Datum,Tezina,Bmi,KalorijskiUnos,IdKorisnika")] StatistikeNapretka statistikeNapretka)
        {
            if (id != statistikeNapretka.IdZapisa)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statistikeNapretka);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Statistika napretka je uspješno ažurirana.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatistikeNapretkaExists(statistikeNapretka.IdZapisa))
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
            ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika", "PunoIme", statistikeNapretka.IdKorisnika);
            return View(statistikeNapretka);
        }

        // GET: StatistikeNapretka/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var statistika = await _context.StatistikeNapretka
                .Include(s => s.Korisnik)
                .FirstOrDefaultAsync(m => m.IdZapisa == id);

            if (statistika == null)
                return NotFound();

            return View(statistika);
        }

        // POST: StatistikeNapretka/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statistika = await _context.StatistikeNapretka.FindAsync(id);
            if (statistika != null)
            {
                _context.StatistikeNapretka.Remove(statistika);
                TempData["DeleteMessage"] = "Statistika napretka je uspješno obrisana.";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatistikeNapretkaExists(int id)
        {
            return _context.StatistikeNapretka.Any(e => e.IdZapisa == id);
        }
    }
}