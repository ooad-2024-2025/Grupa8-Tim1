﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OptiShape.Data;
using OptiShape.Models;

namespace OptiShape.Controllers
{
    [Authorize(Roles = "Administrator, Korisnik")]
    public class StatistikeNapretkaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatistikeNapretkaController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: StatistikeNapretka
        public async Task<IActionResult> Index()
        {
            // For admins, show all records
            if (User.IsInRole("Administrator"))
            {
                var statistike = await _context.StatistikeNapretka
                    .Include(s => s.Korisnik)
                    .ToListAsync();
                return View(statistike);
            }
            else // For regular users, show only their own records
            {
                // Get current logged in user's email
                var userEmail = User.Identity.Name;

                // Find the corresponding Korisnik record
                var korisnik = await _context.Korisnik
                    .FirstOrDefaultAsync(k => k.Email == userEmail);

                if (korisnik == null)
                    return NotFound("Korisnik nije pronađen.");

                var statistike = await _context.StatistikeNapretka
                    .Include(s => s.Korisnik)
                    .Where(s => s.IdKorisnika == korisnik.IdKorisnika)
                    .ToListAsync();

                return View(statistike);
            }
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

            // For regular users, check if this record belongs to them
            if (!User.IsInRole("Administrator"))
            {
                var userEmail = User.Identity.Name;
                var korisnik = await _context.Korisnik
                    .FirstOrDefaultAsync(k => k.Email == userEmail);

                if (korisnik == null || statistika.IdKorisnika != korisnik.IdKorisnika)
                    return Forbid();
            }

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
        public async Task<IActionResult> Create([Bind("IdZapisa,Datum,Tezina,KalorijskiUnos,IdKorisnika")] StatistikeNapretka statistikeNapretka)
        {
            // Pronađi korisnika i uzmi visinu
            var korisnik = await _context.Korisnik.FirstOrDefaultAsync(k => k.IdKorisnika == statistikeNapretka.IdKorisnika);

            if (korisnik == null)
            {
                ModelState.AddModelError("IdKorisnika", "Korisnik nije pronađen.");
            }
            else
            {
                // BMI = tezina (kg) / (visina (m))^2
                var visinaMetri = korisnik.Visina / 100.0;
                if (visinaMetri > 0)
                {
                    statistikeNapretka.Bmi = Math.Round(statistikeNapretka.Tezina / (visinaMetri * visinaMetri), 2);
                }
                else
                {
                    ModelState.AddModelError("IdKorisnika", "Visina korisnika nije validna.");
                }
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("IdZapisa,Datum,Tezina,KalorijskiUnos,IdKorisnika")] StatistikeNapretka statistikeNapretka)
        {
            if (id != statistikeNapretka.IdZapisa)
                return NotFound();

            // Pronađi korisnika i uzmi visinu
            var korisnik = await _context.Korisnik.FirstOrDefaultAsync(k => k.IdKorisnika == statistikeNapretka.IdKorisnika);

            if (korisnik == null)
            {
                ModelState.AddModelError("IdKorisnika", "Korisnik nije pronađen.");
            }
            else
            {
                var visinaMetri = korisnik.Visina / 100.0;
                if (visinaMetri > 0)
                {
                    statistikeNapretka.Bmi = Math.Round(statistikeNapretka.Tezina / (visinaMetri * visinaMetri), 2);
                }
                else
                {
                    ModelState.AddModelError("IdKorisnika", "Visina korisnika nije validna.");
                }
            }

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