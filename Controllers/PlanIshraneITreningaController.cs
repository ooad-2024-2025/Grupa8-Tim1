﻿using System;
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
        public async Task<IActionResult> Create()
        {
            // Provjera uloge i filtriranje korisnika u skladu s tim
            if (User.IsInRole("Trener"))
            {
                // Dohvati email trenutnog trenera
                var trenerEmail = User.Identity.Name;

                // Pronađi trenera u tablici Korisnik
                var trener = await _context.Korisnik
                    .FirstOrDefaultAsync(k => k.Email == trenerEmail);

                if (trener != null)
                {
                    // Dohvati samo korisnike ovog trenera
                    var korisniciTrenera = await _context.Korisnik
                        .Where(k => k.IdTrenera == trener.IdKorisnika)
                        .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                        .ToListAsync();

                    // Kreiraj SelectList s pripremljenim punim imenom
                    ViewData["IdKorisnika"] = new SelectList(korisniciTrenera, "IdKorisnika", "PunoIme");
                }
                else
                {
                    // Fallback ako nije pronađen trener
                    ViewData["IdKorisnika"] = new SelectList(new List<object>(), "IdKorisnika", "PunoIme");
                }
            }
            else // Administrator
            {
                // Administratori vide sve korisnike s punim imenom
                var korisnici = await _context.Korisnik
                    .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                    .ToListAsync();
                ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika", "PunoIme");
            }

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
                TempData["SuccessMessage"] = "Plan je uspješno dodat.";
                return RedirectToAction(nameof(Index));
            }

            



            // DEBUG: Ispis svih grešaka iz ModelState ako forma nije validna
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Greška za {entry.Key}: {error.ErrorMessage}");
                }
            }



            // Ponovo postavi SelectList s pravilnim formatiranjem ako validacija ne prođe
            if (User.IsInRole("Trener"))
            {
                var trenerEmail = User.Identity.Name;
                var trener = await _context.Korisnik
                    .FirstOrDefaultAsync(k => k.Email == trenerEmail);

                if (trener != null)
                {
                    var korisniciTrenera = await _context.Korisnik
                        .Where(k => k.IdTrenera == trener.IdKorisnika)
                        .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                        .ToListAsync();

                    ViewData["IdKorisnika"] = new SelectList(korisniciTrenera, "IdKorisnika",
                        "PunoIme", planIshraneTreninga.IdKorisnika);
                }
                else
                {
                    ViewData["IdKorisnika"] = new SelectList(new List<object>(), "IdKorisnika", "PunoIme", planIshraneTreninga.IdKorisnika);
                }
            }
            else
            {
                var korisnici = await _context.Korisnik
                    .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                    .ToListAsync();
                ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika",
                    "PunoIme", planIshraneTreninga.IdKorisnika);
            }

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

            // Postavi SelectList na isti način kao i u Create akciji
            if (User.IsInRole("Trener"))
            {
                var trenerEmail = User.Identity.Name;
                var trener = await _context.Korisnik
                    .FirstOrDefaultAsync(k => k.Email == trenerEmail);

                if (trener != null)
                {
                    var korisniciTrenera = await _context.Korisnik
                        .Where(k => k.IdTrenera == trener.IdKorisnika)
                        .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                        .ToListAsync();

                    ViewData["IdKorisnika"] = new SelectList(korisniciTrenera, "IdKorisnika",
                        "PunoIme", planIshraneTreninga.IdKorisnika);
                }
                else
                {
                    ViewData["IdKorisnika"] = new SelectList(new List<object>(), "IdKorisnika", "PunoIme", planIshraneTreninga.IdKorisnika);
                }
            }
            else
            {
                var korisnici = await _context.Korisnik
                    .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                    .ToListAsync();
                ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika",
                    "PunoIme", planIshraneTreninga.IdKorisnika);
            }

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

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"❌ Greška u polju '{entry.Key}': {error.ErrorMessage}");
                    }
                }
            }

           
                 // dodana ova 2

            // Ponovno postavi SelectList ako validacija ne uspije
            if (User.IsInRole("Trener"))
            {
                var trenerEmail = User.Identity.Name;
                var trener = await _context.Korisnik
                    .FirstOrDefaultAsync(k => k.Email == trenerEmail);

                if (trener != null)
                {
                    var korisniciTrenera = await _context.Korisnik
                        .Where(k => k.IdTrenera == trener.IdKorisnika)
                        .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                        .ToListAsync();

                    ViewData["IdKorisnika"] = new SelectList(korisniciTrenera, "IdKorisnika",
                        "PunoIme", planIshraneTreninga.IdKorisnika);
                }
                else
                {
                    ViewData["IdKorisnika"] = new SelectList(new List<object>(), "IdKorisnika", "PunoIme", planIshraneTreninga.IdKorisnika);
                }
            }
            else
            {
                var korisnici = await _context.Korisnik
                    .Select(k => new { k.IdKorisnika, PunoIme = k.Ime + " " + k.Prezime })
                    .ToListAsync();
                ViewData["IdKorisnika"] = new SelectList(korisnici, "IdKorisnika",
                    "PunoIme", planIshraneTreninga.IdKorisnika);
            }

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