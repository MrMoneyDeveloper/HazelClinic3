using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using HazelClinic3.Models;

namespace HazelClinic3.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendReport(string email, string report)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email address is required." });
            }

            // Email settings
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda"),
                EnableSsl = true,
            };

            try
            {
                var mailMessage = new MailMessage("spcadurbanza@gmail.com", email)
                {
                    Subject = "Appointment Report from SPCA Durban",
                    Body = report,
                    IsBodyHtml = false // Change to true if the body contains HTML
                };

                smtpClient.Send(mailMessage);

                return Json(new { success = true, message = "Report sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
