using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using HazelClinic3.Models;
using System.IO;
using Rotativa;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mail;
using Stripe.Checkout;
using Stripe;

namespace HazelClinic3.Controllers
{
    public class AppointmentsController : Controller
    {
        private DataContext db = new DataContext();
        
        private readonly AppointmentService _appointmentService;

        public AppointmentsController(DataContext context, AppointmentService appointmentService)
        {
            db = context;
            _appointmentService = appointmentService;
        }

        public DateTime AppointmentTime { get; private set; }
        public DateTime AppointmentDate { get; private set; }


        public ActionResult Index()
        {
            return View(db.AppTbl.ToList());
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.AppTbl.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }


        public ActionResult Create()
        {

            DateTime currentDate = DateTime.Today;  // Start checking from today's date
            var availableTimes = _appointmentService.GetAvailableTimes(currentDate);  // Attempt to fetch available times for today

            // If no times are available today, find the next available day
            while (!availableTimes.Any())
            {
                currentDate = currentDate.AddDays(1);  // Move to the next day
                availableTimes = _appointmentService.GetAvailableTimes(currentDate);  // Fetch available times for the new date
                var model1 = new Appointment { AppoinmentDate = currentDate };
                ViewBag.AvailableTimes = new SelectList(availableTimes, "Value", "Text");

                // Return the view with the model
                return View(model1);
            }

            // Create the model for the view with the date set to today or the next available day
            var model = new Appointment { AppoinmentDate = currentDate };

            // Pass the available times to the view using ViewBag
            ViewBag.AvailableTimes = new SelectList(availableTimes, "Value", "Text");

            // Return the view with the model
            return View(model);
        }

        public ActionResult UpdateDate(DateTime date)
        {
            var availableTimes = _appointmentService.GetAvailableTimes(date);
            if (!availableTimes.Any())
            {
                date = GetNextAvailableDate(date); // Adjust this method to correctly find the next day
                availableTimes = _appointmentService.GetAvailableTimes(date);
            }

            var model = new Appointment { AppoinmentDate = date, AppointmentTime = date };
            return View("Create", model); // Assuming "AppointmentForm" is your view name
        }




      
        private DateTime GetNextAvailableDate(DateTime start)
        {
            DateTime nextDate = start.AddDays(1); // Start checking from the day after 'start'

           
            int maxDaysToCheck = 365; // For example, no more than one year ahead
            int daysChecked = 0;
            while (daysChecked < maxDaysToCheck)
            {
                var timesAvailable = _appointmentService.GetAvailableTimes(nextDate);
                if (timesAvailable.Any())
                    return nextDate;

                nextDate = nextDate.AddDays(1);
                daysChecked++;
            }

            throw new Exception("No available slots found within the next year."); // Or handle this scenario appropriately

        }


        /*public ActionResult FetchAvailableTimes(DateTime date)
        {
            var times = _appointmentService.GetAvailableTimes(date);
            ViewBag.AvailableTimes = new SelectList(times, "Value", "Text");
            return PartialView("_AvailableTimesSelectList", times);
        }*/






        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppoinmentId,FullName,Email,PhoneNumber,AnimalName,AnimalType,AppointmentType,PetSpecies,ConsultationType,Corona,DPV,Rabies,Clostridial,Leptospirosis,Brucellosis,AppoinmentDate,AppointmentTime,AppType,VaccineCost,ConsultationCost,TotalFee,IdNumber")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                string ID = (string)Session["ID"];
                string userEmail = (string)Session["GlobalEmail"];
                string userName = (string)Session["GlobalPetName"];
                //bool isAppointmentAvailable = IsAppointmentAvailable(appointment.AppoinmentDate, appointment.AppointmentTime);
                var currentUser = db.Users2.FirstOrDefault(u => u.Email == userEmail);

                if (currentUser != null)
                {
                   
                    appointment.IdNumber = currentUser.IDnum;
                    appointment.Email = currentUser.Email;
                    appointment.FullName = currentUser.Firstname + currentUser.Lastname;
                    appointment.AnimalName = currentUser.Username;
                    appointment.PhoneNumber = currentUser.Mobile;
                }

                var availableTimes = _appointmentService.GetAvailableTimes(appointment.AppoinmentDate);
                bool isAppointmentAvailable = availableTimes.Any(t => t.Value == appointment.AppointmentTime.ToString("HH:mm"));

                if (!isAppointmentAvailable)
                {
                    ModelState.AddModelError("AppointmentTime", "The selected time is not available for the chosen date.");
                    ViewBag.AvailableTimes = new SelectList(availableTimes, "Value", "Text");
                    return View(appointment);
                }
                else
                {
                    appointment.VaccineCost = appointment.CalcVaccineCost();
                    appointment.ConsultationCost = appointment.CalcConsultationCost();
                    appointment.AppType = appointment.CalcBasicAppType();
                    

                    if (!string.IsNullOrEmpty(appointment.PromoCode))
                    {
                        // Search for the promo code in the Ratings table
                        var rating = db.Ratings.FirstOrDefault(r => r.promocode == appointment.PromoCode);

                        if (rating != null)
                        {
                            // Apply the promo code and delete it from Ratings table
                            appointment.PromoCode = rating.promocode;
                            rating.promocode = null;

                            // Apply a 5% discount on the total fee
                            appointment.TotalFee = appointment.CalcTotalFeePromo();
                            TempData["AppointmentConfirm"] = appointment;

                            db.AppTbl.Add(appointment);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        appointment.TotalFee = appointment.CalcTotalFee();
                        TempData["AppointmentConfirm"] = appointment;

                        db.AppTbl.Add(appointment);
                        db.SaveChanges();
                    }



                    var returnUrl = Url.Action("Confirmation", "Appointments", new { appointmentId = appointment.AppointmentId }, protocol: Request.UrlReferrer.Scheme);

                    StripeConfiguration.ApiKey = "sk_test_51P1F4HKgNOMzGBDh6junKYp3kCPW3zvipfkiuPzCd2TAYjBgx72OR2OoyalAJ9lmLOSPMuVYQhCbOyWTQZO0M4tG000sJOeAP2"; // Replace with your actual Stripe secret key

                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                        LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "zar",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Appointment Fee",
                            },
                            UnitAmount = (long)appointment.TotalFee * 100,
                        },
                        Quantity = 1,
                    },
                },
                        Mode = "payment",
                        SuccessUrl = returnUrl,
                        CancelUrl = returnUrl,
                    };

                    var service = new SessionService();
                    var session = service.Create(options);

                    return Redirect(session.Url);
                }
            }

            //var availableTimesForCurrentDate = GetAvailableTimesForDate(appointment.AppoinmentDate);
            //ViewBag.AvailableTimes = new SelectList(availableTimesForCurrentDate);
            TempData["AppointmentConfirm"] = appointment;
            return View(appointment);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Confirmation(int appointmentId)
        {
            Appointment appointment = TempData["AppointmentConfirm"] as Appointment;
            if (appointment == null)
            {
                appointment = db.AppTbl.Find(appointmentId);
            }

            if (appointment == null)
            {
                return RedirectToAction("Create");
            }

            byte[] pdfBytes = GenerateInvoicePdf(appointment);
            using (var stream = new MemoryStream(pdfBytes))
            {
                SendEmailWithAttachment(appointment.Email, "Invoice.pdf", "Invoice for your appointment is attached.", stream);
            }

            return View(appointment);
        }


        public bool IsAppointmentAvailable(DateTime? appointmentDate, DateTime appointmentTime)
        {



            bool isAvailable = !db.AppTbl.Any(a => a.AppoinmentDate == appointmentDate && a.AppointmentTime == appointmentTime);
            return isAvailable;
        }

        /*public List<DateTime> GetAvailableTimesForDate(DateTime appointmentDate)
        {
            if (appointmentDate == null)
            {
                return new List<DateTime>();
            }


            List<DateTime> availableTimes = db.AppTbl
                .Where(a => a.AppoinmentDate == appointmentDate)
                .Select(a => a.AppointmentTime)
                .ToList();

            return availableTimes;
        }*/


        public ActionResult AppointmentSuccess()
        {
            return View();
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.AppTbl.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentId,FullName,Email,PhoneNumber,AnimalName,AnimalType,AppointmentType,PetSpecies,ConsultationType,Corona,DPV,Rabies,Clostridial,Leptospirosis,Brucellosis,AppoinmentDate,AppointmentTime,AppType,VaccineCost,ConsultationCost,TotalFee")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appointment);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.AppTbl.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.AppTbl.Find(id);
            db.AppTbl.Remove(appointment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AppointmentInfo()
        {
            return View();
        }

        public ActionResult DownloadInvoice(int Id)
        {
            Appointment appointment = db.AppTbl.Find(Id);
            if (appointment == null)
            {

                return RedirectToAction("Index");
            }

            byte[] pdfBytes = GenerateInvoicePdf(appointment);

            return File(pdfBytes, "application/pdf", "Invoice.pdf");
        }

        private byte[] GenerateInvoicePdf(Appointment appointment)
        {
            DateTime bookedDate = DateTime.Today;

            Document doc = new Document();
            MemoryStream memoryStream = new MemoryStream();
            //PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
            doc.Open();

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font font = new Font(bf, 12, Font.NORMAL);

            doc.Add(new Paragraph("\n\n"));

            PdfPTable headerTable = new PdfPTable(1);
            PdfPCell headerCell = new PdfPCell(new Phrase("Appointment Invoice", font));
            headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            headerTable.AddCell(headerCell);
            doc.Add(headerTable);

            Paragraph introParagraph = new Paragraph("Thank you for choosing SPCA Durban!", font);
            introParagraph.Alignment = Element.ALIGN_CENTER;
            doc.Add(introParagraph);

            doc.Add(new Paragraph("Your appointment is successfully processed. Here are the details:", font));
            doc.Add(Chunk.NEWLINE);

            PdfPTable detailsTable = new PdfPTable(2);
            detailsTable.WidthPercentage = 100;
            AddRowToPdfTable(detailsTable, "Booked Date:", bookedDate.ToString("yyyy-MM-dd"), font);
            AddRowToPdfTable(detailsTable, "Appointment ID:", appointment.AppointmentId.ToString(), font);
            AddRowToPdfTable(detailsTable, "Full Name:", appointment.FullName, font);
            AddRowToPdfTable(detailsTable, "Email:", appointment.Email, font);
            AddRowToPdfTable(detailsTable, "Phone Number:", appointment.PhoneNumber, font);
            AddRowToPdfTable(detailsTable, "Animal Name:", appointment.AnimalName, font);
            AddRowToPdfTable(detailsTable, "Animal Type:", appointment.AnimalType, font);
            AddRowToPdfTable(detailsTable, "Appointment Type:", appointment.AppointmentType, font);
            AddRowToPdfTable(detailsTable, "Pet Species:", appointment.PetSpecies, font);
            AddRowToPdfTable(detailsTable, "Consultation Type:", appointment.ConsultationType, font);
            AddRowToPdfTable(detailsTable, "Appointment Date:", appointment.AppoinmentDate.ToString("yyyy-MM-dd"), font);
            AddRowToPdfTable(detailsTable, "Appointment Time:", appointment.AppointmentTime.ToString("HH:mm"), font);

            List<string> selectedVaccinations = new List<string>();
            if (appointment.Corona) selectedVaccinations.Add("Corona");
            if (appointment.DPV) selectedVaccinations.Add("DPV");
            if (appointment.Rabies) selectedVaccinations.Add("Rabies");
            if (appointment.Clostridial) selectedVaccinations.Add("Clostridial");
            if (appointment.Leptospirosis) selectedVaccinations.Add("Leptospirosis");
            if (appointment.Brucellosis) selectedVaccinations.Add("Brucellosis");

            if (selectedVaccinations.Count > 0)
            {
                AddRowToPdfTable(detailsTable, "Vaccination Types:", string.Join(", ", selectedVaccinations), font);
            }

            AddRowToPdfTable(detailsTable, "Vaccine Cost:", " " + appointment.VaccineCost.ToString("C"), font);
            AddRowToPdfTable(detailsTable, "Appointment Type:", " " + appointment.AppType.ToString("C"), font);
            AddRowToPdfTable(detailsTable, "Consultation Cost:", " " + appointment.ConsultationCost.ToString("C"), font);
            AddRowToPdfTable(detailsTable, "Total Fee:", " " + appointment.TotalFee.ToString("C"), font);

            AddRowToPdfTable(detailsTable, "Paid:", "Yes", font);
            AddRowToPdfTable(detailsTable, "Payment Method:", "Card", font);

            doc.Add(detailsTable);

            doc.Close();

            return memoryStream.ToArray();
        }


        private void AddRowToPdfTable(PdfPTable table, string label, string value, Font font)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, font));
            PdfPCell valueCell = new PdfPCell(new Phrase(value, font));
            labelCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }





        public ActionResult SendInvoiceEmail(int Id)
        {
            Appointment appointment = db.AppTbl.Find(Id);

            if (appointment == null)
            {

                return RedirectToAction("Index");
            }

            byte[] pdfBytes = GenerateInvoicePdf(appointment);


            MemoryStream pdfStream = new MemoryStream(pdfBytes);


            SendEmailWithAttachment(appointment.Email, "Invoice.pdf", "Here is your invoice.", pdfStream);


            return View("EmailSentConfirmation");
        }

        private void SendEmailWithAttachment(string recipientEmail, string attachmentFileName, string emailBody, Stream attachmentStream)
        {

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda"),
                EnableSsl = true,
            };


            MailMessage mailMessage = new MailMessage("spcadurbanza@gmail.com", recipientEmail)
            {
                Subject = "Invoice for SPCA Durban Appointment",
                Body = emailBody,
                IsBodyHtml = true,

            };
            string emailBodyWithLogo = @"
        <html>
        <head>
            <style>
                .logo {
                    max-width: 200px;
                }
            </style>
        </head>
        <body>
             <img src=""https://i.postimg.cc/qRMDx9MV/Screenshot-12.png""alt='SPCA Durban Logo'>
            <h2>SPCA Durban Appointment Invoice</h2>
            <p>To Whom It May Concern</p>
            <p>We are pleased to provide you with the invoice for your recent appointment at SPCA Durban.<p>
            <p>Below, you will find the details of the invoice:</p>
            <p>If you have any questions or concerns about this invoice, please don't hesitate to contact our Clinic by responding to this Email.</p>

            <p>Thank you for choosing SPCA Durban for your pet's healthcare needs.</p>
        </body>
        </html>";
            mailMessage.Body = emailBodyWithLogo;

            attachmentStream.Seek(0, SeekOrigin.Begin);
            mailMessage.Attachments.Add(new Attachment(attachmentStream, attachmentFileName));


            smtpClient.Send(mailMessage);
        }

        public ActionResult EmailSentConfirmation()
        {
            return View();
        }
         public ActionResult VerifyOTP()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View();
        }


        public ActionResult Reschedule()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reschedule(Reschedule model)
        {
            if (ModelState.IsValid)
            {
                var appointment = db.AppTbl.FirstOrDefault(a => a.AppointmentId == model.AppointmentId && a.IdNumber == model.IdNumber);
                if (appointment == null)
                {
                    ModelState.AddModelError("", "Appointment not found for the provided ID number and Appointment ID.");
                    return View(model);
                }

                
                string recipientEmail = appointment.Email;

                
                string otp = GenerateOTP();

                
                SendOTPByEmail(recipientEmail, otp);

               
                TempData["OTP"] = otp;
                TempData["AppointmentId"] = model.AppointmentId;

               
                return RedirectToAction("VerifyOTP");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyOTP(string otp)
        {
            string expectedOTP = TempData["OTP"] as string;

            if (otp == expectedOTP)
            {
                
                int appointmentId = (int)TempData["AppointmentId"];
                return RedirectToAction("RescheduleAppointment", new { id = appointmentId });
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid OTP. Please try again.";
                return RedirectToAction("VerifyOTP");
            }
        }

        private void SendOTPByEmail(string recipientEmail, string otp)
        {

            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "spcadurbanza@gmail.com";
            string smtpPassword = "urpc bsvq bdmd wpda";

            string senderEmail = smtpUsername;


            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail);
            mailMessage.To.Add(recipientEmail);
            mailMessage.Subject = "OTP for Rescheduling";
            mailMessage.Body = $"Your OTP for rescheduling your Appointment is: {otp}";


            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true;


            smtpClient.Send(mailMessage);
        }

        private string GenerateOTP()
        {

            Random rnd = new Random();
            int otp = rnd.Next(100000, 999999);
            return otp.ToString();
        }



        public ActionResult RescheduleAppointment(int id, string idnumber)
        {
            var appointment = db.AppTbl.Find(id);
            if (appointment == null)
            {
                return RedirectToAction("Reschedule");
            }

            
            Reschedule model = new Reschedule
            {
                AppointmentId = appointment.AppointmentId,
                AppoinmentDate = appointment.AppoinmentDate,
                AppointmentTime = appointment.AppointmentTime
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RescheduleAppointment(Reschedule model)
        {
            if (ModelState.IsValid)
            {

                var currentAppointment = db.AppTbl.Find(model.AppointmentId);
                if (currentAppointment != null && currentAppointment.AppoinmentDate == model.AppoinmentDate && currentAppointment.AppointmentTime == model.AppointmentTime)
                {
                    ModelState.AddModelError("", "You cannot select the same date and time as the current appointment.");
                    return View(model);
                }


                if (!IsDateTimeAvailable(model.AppoinmentDate, model.AppointmentTime, model.AppointmentId))
                {
                    ModelState.AddModelError("", "An appointment already exists at the specified date and time.");
                    return View(model);
                }

                var existingAppointment = db.AppTbl.Find(model.AppointmentId);
                if (existingAppointment != null)
                {

                    existingAppointment.AppoinmentDate = model.AppoinmentDate;
                    existingAppointment.AppointmentTime = model.AppointmentTime;


                    db.Entry(existingAppointment).State = EntityState.Modified;
                    db.SaveChanges();


                    Debug.WriteLine("Appointment updated successfully.");

                    return RedirectToAction("Confirmation", new { appointmentId = existingAppointment.AppointmentId });
                }
                else
                {

                    Debug.WriteLine("Appointment not found for ID: " + model.AppointmentId);
                }
            }


            return View(model);
        }

        private bool IsDateTimeAvailable(DateTime date, DateTime time, int appointmentId)
        {

            var currentAppointment = db.AppTbl.Find(appointmentId);
            bool isCurrentAppointment = currentAppointment != null && currentAppointment.AppoinmentDate == date && currentAppointment.AppointmentTime == time;


           


            bool conflictingAppointmentExists = db.AppTbl.Any(a => a.AppoinmentDate == date
                                                               && a.AppointmentTime == time
                                                               && (a.AppointmentId != appointmentId));


            return !conflictingAppointmentExists;
        }












    }
}
















    



    

