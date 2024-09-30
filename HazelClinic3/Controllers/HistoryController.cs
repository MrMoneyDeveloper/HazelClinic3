using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HazelClinic3.Models;
using System.Net.Mail;

namespace HazelClinic3.Controllers
{
    public class HistoryController : Controller
    {
        private DataContext db = new DataContext();

        
        public ActionResult AddHistory()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHistory(History history)
        {
            if (ModelState.IsValid)
            {
                
                var appointment = db.AppTbl.FirstOrDefault(a => a.AppointmentId == history.AppointmentId);
                if (appointment == null)
                {
                    ModelState.AddModelError("AppointmentId", "Appointment ID not found");
                    return View(history);
                }

                
                if (appointment.IdNumber != history.IdNumber)
                {
                    ModelState.AddModelError("IdNumber", "ID Number does not match the appointment");
                    return View(history);
                }

               
                history.Timestamp = DateTime.Now;
                db.Histories.Add(history);
                db.SaveChanges();
                return RedirectToAction("Index", "Appointments");
            }

            return View(history);
        }

        public ActionResult ViewAll()
        {
            var historyList = db.Histories.ToList();
            return View(historyList);
        }



        public ActionResult ViewHistory()
        {
            var historyList = db.Histories.ToList();
            return View(historyList);
        }

        
        [HttpPost]
        public ActionResult SendReport(int appointmentId, string report)
        {
            try
            {
               
                var appointment = db.AppTbl.FirstOrDefault(a => a.AppointmentId == appointmentId);
                if (appointment == null)
                {
                    return Json(new { success = false, message = "Appointment not found" });
                }

                string email = appointment.Email; 

                
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda"),
                    EnableSsl = true,
                };

                
                var mailMessage = new MailMessage("spcadurbanza@gmail.com", email)
                {
                    Subject = "Appointment Report from SPCA Durban",
                    IsBodyHtml = true 
                };

                
                string logoUrl = "https://i.postimg.cc/qRMDx9MV/Screenshot-12.png"; 
                string body = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        width: 80%;
                        margin: 0 auto;
                    }}
                    .header {{
                        text-align: center;
                        margin-bottom: 20px;
                    }}
                    .logo {{
                        max-width: 200px;
                    }}
                    .content {{
                        padding: 20px;
                        background-color: #f9f9f9;
                        border-radius: 5px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <img src='{logoUrl}' alt='SPCA Durban Logo' class='logo' />
                    </div>
                    <div class='content'>
                        <h2>Appointment Report:</h2>
                        <p><strong>Business Name:</strong> SPCA Durban</p>
                        <p><strong>Appointment ID:</strong> {appointmentId}</p>
                        <p><strong>Report Details:</strong> {report}</p>
                        <p><strong>Timestamp:</strong> {DateTime.Now}</p>
                    </div>
                </div>
            </body>
            </html>
        ";

                mailMessage.Body = body;

              
                smtpClient.Send(mailMessage);

                TempData["SuccessMessage"] = "Email sent successfully";
                return RedirectToAction("ViewHistory");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to send email: " + ex.Message;
                return RedirectToAction("ViewHistory");
            }
        }

        
        [HttpPost]
        public ActionResult DeleteReport(int reportId)
        {
            try
            {
                var report = db.Histories.Find(reportId);
                if (report == null)
                {
                    return HttpNotFound();
                }

                db.Histories.Remove(report);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Report deleted successfully";
                return RedirectToAction("ViewHistory");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete report: " + ex.Message;
                return RedirectToAction("ViewHistory");
            }
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

