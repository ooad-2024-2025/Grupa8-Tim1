using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiShape.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace OptiShape.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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

                // Your email configuration
                var fromAddress = new MailAddress("your-email@example.com", "OptiShape App");
                var toAddress = new MailAddress("ooooaadd1@gmail.com", "Admin");

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",  // Using Gmail SMTP server
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
                    TempData["Success"] = "Zahtjev je uspješno poslan. O?ekujte odgovor u narednih nekoliko dana.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email: {ex.Message}");
                TempData["Error"] = $"Greška prilikom slanja zahtjeva: {ex.Message}";
            }

            return RedirectToAction("StudentApplication");
        }
    }
}
