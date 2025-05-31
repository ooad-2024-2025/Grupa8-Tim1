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
    public class TerminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TerminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Termin
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Termin.Include(t => t.Korisnik);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Termin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _context.Termin
                .Include(t => t.Korisnik)
                .FirstOrDefaultAsync(m => m.IdTermina == id);
            if (termin == null)
            {
                return NotFound();
            }

            return View(termin);
        }

        // GET: Termin/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika");
            return View();
        }

        // POST: Termin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("IdTermina,Datum,IdKorisnika")] Termin termin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(termin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", termin.IdKorisnika);
            return View(termin);
        }

        // GET: Termin/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _context.Termin.FindAsync(id);
            if (termin == null)
            {
                return NotFound();
            }
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", termin.IdKorisnika);
            return View(termin);
        }

        // POST: Termin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("IdTermina,Datum,IdKorisnika")] Termin termin)
        {
            if (id != termin.IdTermina)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(termin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerminExists(termin.IdTermina))
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
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", termin.IdKorisnika);
            return View(termin);
        }

        // GET: Termin/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _context.Termin
                .Include(t => t.Korisnik)
                .FirstOrDefaultAsync(m => m.IdTermina == id);
            if (termin == null)
            {
                return NotFound();
            }

            return View(termin);
        }

        // POST: Termin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var termin = await _context.Termin.FindAsync(id);
            if (termin != null)
            {
                _context.Termin.Remove(termin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TerminExists(int id)
        {
            return _context.Termin.Any(e => e.IdTermina == id);
        }
    }
}