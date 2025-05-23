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
            var applicationDbContext = _context.StatistikeNapretka.Include(s => s.Korisnik);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StatistikeNapretka/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistikeNapretka = await _context.StatistikeNapretka
                .Include(s => s.Korisnik)
                .FirstOrDefaultAsync(m => m.IdZapisa == id);
            if (statistikeNapretka == null)
            {
                return NotFound();
            }

            return View(statistikeNapretka);
        }

        // GET: StatistikeNapretka/Create
        public IActionResult Create()
        {
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika");
            return View();
        }

        // POST: StatistikeNapretka/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdZapisa,Datum,Tezina,Bmi,KalorijskiUnos,IdKorisnika")] StatistikeNapretka statistikeNapretka)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statistikeNapretka);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", statistikeNapretka.IdKorisnika);
            return View(statistikeNapretka);
        }

        // GET: StatistikeNapretka/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistikeNapretka = await _context.StatistikeNapretka.FindAsync(id);
            if (statistikeNapretka == null)
            {
                return NotFound();
            }
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", statistikeNapretka.IdKorisnika);
            return View(statistikeNapretka);
        }

        // POST: StatistikeNapretka/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdZapisa,Datum,Tezina,Bmi,KalorijskiUnos,IdKorisnika")] StatistikeNapretka statistikeNapretka)
        {
            if (id != statistikeNapretka.IdZapisa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statistikeNapretka);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatistikeNapretkaExists(statistikeNapretka.IdZapisa))
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
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", statistikeNapretka.IdKorisnika);
            return View(statistikeNapretka);
        }

        // GET: StatistikeNapretka/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistikeNapretka = await _context.StatistikeNapretka
                .Include(s => s.Korisnik)
                .FirstOrDefaultAsync(m => m.IdZapisa == id);
            if (statistikeNapretka == null)
            {
                return NotFound();
            }

            return View(statistikeNapretka);
        }

        // POST: StatistikeNapretka/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statistikeNapretka = await _context.StatistikeNapretka.FindAsync(id);
            if (statistikeNapretka != null)
            {
                _context.StatistikeNapretka.Remove(statistikeNapretka);
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
