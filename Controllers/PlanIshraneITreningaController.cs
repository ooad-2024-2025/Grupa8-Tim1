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
    [Authorize(Roles = "Administrator, Korisnik, Trener")]
    public class PlanIshraneITreningaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanIshraneITreningaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PlanIshraneITreninga
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PlanIshraneTreninga.Include(p => p.Korisnik);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PlanIshraneITreninga/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planIshraneTreninga = await _context.PlanIshraneTreninga
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(m => m.IdPlana == id);
            if (planIshraneTreninga == null)
            {
                return NotFound();
            }

            return View(planIshraneTreninga);
        }

        // GET: PlanIshraneITreninga/Create
        [Authorize(Roles = "Administrator, Trener")]
        public IActionResult Create()
        {
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika");
            return View();
        }

        // POST: PlanIshraneITreninga/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Trener")]
        public async Task<IActionResult> Create([Bind("IdPlana,DatumKreiranja,Kalorije,Protein,Ugljikohidrati,Masti,IdKorisnika")] PlanIshraneTreninga planIshraneTreninga)
        {
            if (ModelState.IsValid)
            {
                _context.Add(planIshraneTreninga);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", planIshraneTreninga.IdKorisnika);
            return View(planIshraneTreninga);
        }

        // GET: PlanIshraneITreninga/Edit/5
        [Authorize(Roles = "Administrator, Trener")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planIshraneTreninga = await _context.PlanIshraneTreninga.FindAsync(id);
            if (planIshraneTreninga == null)
            {
                return NotFound();
            }
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", planIshraneTreninga.IdKorisnika);
            return View(planIshraneTreninga);
        }

        // POST: PlanIshraneITreninga/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Trener")]
        public async Task<IActionResult> Edit(int id, [Bind("IdPlana,DatumKreiranja,Kalorije,Protein,Ugljikohidrati,Masti,IdKorisnika")] PlanIshraneTreninga planIshraneTreninga)
        {
            if (id != planIshraneTreninga.IdPlana)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planIshraneTreninga);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanIshraneTreningaExists(planIshraneTreninga.IdPlana))
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
            ViewData["IdKorisnika"] = new SelectList(_context.Korisnik, "IdKorisnika", "IdKorisnika", planIshraneTreninga.IdKorisnika);
            return View(planIshraneTreninga);
        }

        // GET: PlanIshraneITreninga/Delete/5
        [Authorize(Roles = "Administrator, Trener")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planIshraneTreninga = await _context.PlanIshraneTreninga
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(m => m.IdPlana == id);
            if (planIshraneTreninga == null)
            {
                return NotFound();
            }

            return View(planIshraneTreninga);
        }

        // POST: PlanIshraneITreninga/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Trener")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planIshraneTreninga = await _context.PlanIshraneTreninga.FindAsync(id);
            if (planIshraneTreninga != null)
            {
                _context.PlanIshraneTreninga.Remove(planIshraneTreninga);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanIshraneTreningaExists(int id)
        {
            return _context.PlanIshraneTreninga.Any(e => e.IdPlana == id);
        }
    }
}