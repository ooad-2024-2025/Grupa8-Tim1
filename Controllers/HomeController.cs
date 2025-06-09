using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiShape.Data;
using OptiShape.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace OptiShape.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult StudentApplication()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendStudentRequest()
        {
            try
            {
                var userName = User.Identity?.Name;
                _logger.LogInformation($"Sending student application email for user: {userName}");

                var fromAddress = new MailAddress("ooooaadd1@gmail.com", "OptiShape App");
                var toAddress = new MailAddress("ooooaadd1@gmail.com", "Admin");

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("ooooaadd1@gmail.com", "xtkc ssht bihg mxzz")
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "Zahtjev za studentski status",
                    Body = $"<h3>Zahtjev za studentski status</h3>" +
                           $"<p><strong>Korisnik:</strong> {userName}</p>" +
                           $"<p>Korisnik je podnio zahtjev za studentski status putem OptiShape aplikacije.</p>" +
                           $"<p>Datum i vrijeme zahtjeva: {DateTime.Now}</p>",
                    IsBodyHtml = true
                })
                {
                    await Task.Run(() => smtp.Send(message));
                    _logger.LogInformation("Email successfully sent");
                    TempData["Success"] = "Zahtjev je uspješno poslan. Očekujte odgovor u narednih nekoliko dana.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email: {ex.Message}");
                TempData["Error"] = $"Greška prilikom slanja zahtjeva: {ex.Message}";
            }

            return RedirectToAction("StudentApplication");
        }

        [Authorize]
        public async Task<IActionResult> IzborTrenera()
        {
            var userEmail = User.Identity?.Name;
            var korisnik = await _db.Korisnik.FirstOrDefaultAsync(k => k.Email == userEmail);

            if (korisnik == null || korisnik.IdTrenera != null || User.IsInRole("Administrator") || User.IsInRole("Trener"))
            {
                return RedirectToAction("Dashboard");
            }

            var trenerIds = await _db.Korisnik
                .Where(k => k.IdTrenera != null)
                .Select(k => k.IdTrenera.Value)
                .Distinct()
                .ToListAsync();

            var treneri = await _db.Korisnik
                .Where(k => trenerIds.Contains(k.IdKorisnika))
                .ToListAsync();

            return View(treneri);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> OdaberiTrenera(int trenerId)
        {
            var userEmail = User.Identity?.Name;
            var korisnik = await _db.Korisnik.FirstOrDefaultAsync(k => k.Email == userEmail);

            // Provjera validnosti korisnika i trenera
            if (korisnik == null || korisnik.IdTrenera != null)
            {
                return RedirectToAction("Dashboard");
            }

            var trener = await _db.Korisnik.FirstOrDefaultAsync(k => k.IdKorisnika == trenerId);
            if (trener == null)
            {
                TempData["Error"] = "Odabrani trener ne postoji.";
                return RedirectToAction("IzborTrenera");
            }

            korisnik.IdTrenera = trenerId;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Uspješno ste odabrali trenera!";
            return RedirectToAction("Dashboard");
        }
    }
}