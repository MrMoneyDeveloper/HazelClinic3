using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using HazelClinic3.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Stripe;
using Stripe.Checkout;

namespace HazelClinic3.Controllers
{
    public class HomeController : Controller
    {
        private DataContext db = new DataContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Bookingform()
        {
            return View();
        }

        public ActionResult BookingInfo()
        {
            return View();
        }

        public ActionResult Appointmentform()
        {
            return View();
        }

        public ActionResult Adoptions()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }





        [HttpPost]
        public ActionResult Bookingform(Booking model)
        {

            if (ModelState.IsValid)
            {

                string userEmail = (string)Session["GlobalEmail"];
                string userName = (string)Session["GlobalPetName"];


                var user = db.Users2.FirstOrDefault(u => u.Email == userEmail);

                if (user != null)
                {

                    model.Email = user.Email;
                    model.Fname = user.Firstname;
                    model.Lname = user.Lastname;
                    model.Pname = user.Username;
                    model.Phone = user.Mobile;
                    model.Address = user.Address;
                    model.CityPostalCode = user.PostalCode;
                }
                if (!string.IsNullOrEmpty(model.PromoCode))
                {
                    // Check if the promo code exists in the ratings table
                    var rating = db.Ratings.FirstOrDefault(r => r.promocode == model.PromoCode);
                    if (rating != null)
                    {
                        // Reduce the total cost by 10%
                        model.CalculateTotalCostPromo();


                        rating.promocode = null;
                        db.SaveChanges();
                        db.Bookings.Add(model);
                        db.SaveChanges();
                    }
                }
                else
                {
                    model.CalculateTotalCost();
                    db.Bookings.Add(model);
                    db.SaveChanges();
                }



                TempData["BookingConfirmation"] = model;

                SendEmailWithInvoice(model);

                return RedirectToAction("Payment");
            }

            return View(model);
        }

        public ActionResult Payment()
        {
            StripeConfiguration.ApiKey = "sk_test_51P1F4HKgNOMzGBDh6junKYp3kCPW3zvipfkiuPzCd2TAYjBgx72OR2OoyalAJ9lmLOSPMuVYQhCbOyWTQZO0M4tG000sJOeAP2";

            var bookingConfirmation = TempData["BookingConfirmation"] as Booking;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(bookingConfirmation.TotalCost * 100),
                            Currency = "zar",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = bookingConfirmation.Pname,
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Home", null, Request.Url.Scheme),
                CancelUrl = Url.Action("Bookingform", "Home", null, Request.Url.Scheme),
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        public ActionResult DownloadInvoice(int bookingId)
        {
            var booking = db.Bookings.Find(bookingId);

            if (booking == null)
            {
                return HttpNotFound();
            }

            byte[] invoicePdf = GenerateInvoicePdfContent(booking);
            return File(invoicePdf, "application/pdf", "Invoice.pdf");
        }

        public ActionResult Success()
        {
            var bookingConfirmation = TempData["BookingConfirmation"] as Booking;

            if (bookingConfirmation != null)
            {

                int bookingId = db.Bookings
                    .Where(b => b.Fname == bookingConfirmation.Fname && b.Email == bookingConfirmation.Email)
                    .Select(b => b.BookingId)
                    .FirstOrDefault();

                ViewBag.BookingId = bookingId;

                SendEmailWithInvoice(bookingConfirmation);
            }
            else
            {
                ViewBag.BookingId = null;
            }

            return View(bookingConfirmation);
        }

        public ActionResult CheckIn()
        {
            return View();
        }

        public ActionResult CheckOut()
        {
            return View();
        }

        public ActionResult RateDelivery()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RateDelivery(Rating model)
        {
            if (ModelState.IsValid)
            {
                
                model.promocode = GenerateUniquePromoCode();

                
                db.Ratings.Add(model);
                db.SaveChanges();

                SendPromoCodeEmail(model.Email, model.promocode);

                return RedirectToAction("Index", "Home"); 
            }

            
            return View("RateDriver", model);
        }

        private string GenerateUniquePromoCode()
        {
            string promoCode;
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            do
            {
                promoCode = new string(Enumerable.Repeat(chars, 6)
                                .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (db.Ratings.Any(r => r.promocode == promoCode));

            return promoCode;
        }

        private void SendPromoCodeEmail(string userEmail, string promoCode)
        {

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("spcadurbanza@gmail.com");
            mail.To.Add(userEmail);
            mail.Subject = "Your Promo Code";
            mail.Body = "Your promo code is: " + promoCode;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

        }

        private byte[] GenerateInvoicePdfContent(Booking booking)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DateTime bookedDate = DateTime.Today;

                Document doc = new Document();
                //PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);

                doc.Open();

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font font = new Font(bf, 12, Font.NORMAL);

                doc.Add(new Paragraph("\n\n"));
                PdfPTable headerTable = new PdfPTable(1);
                PdfPCell headerCell = new PdfPCell(new Phrase("Pet Boarding Invoice", font));
                headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerTable.AddCell(headerCell);
                doc.Add(headerTable);

                Paragraph introParagraph = new Paragraph("Thank you for choosing SPCA Durban for your pet's boarding needs!", font);
                introParagraph.Alignment = Element.ALIGN_CENTER;
                doc.Add(introParagraph);

                doc.Add(new Paragraph("Here is the invoice for your pet's boarding:", font));
                doc.Add(Chunk.NEWLINE);

                PdfPTable detailsTable = new PdfPTable(2);
                detailsTable.WidthPercentage = 100;

                AddRowToPdfTable(detailsTable, "Booked Date:", bookedDate.ToString("yyyy-MM-dd"), font);
                AddRowToPdfTable(detailsTable, "Booking ID:", booking.BookingId.ToString(), font);
                AddRowToPdfTable(detailsTable, "First Name:", booking.Fname, font);
                AddRowToPdfTable(detailsTable, "Last Name:", booking.Lname, font);
                AddRowToPdfTable(detailsTable, "Phone:", booking.Phone, font);
                AddRowToPdfTable(detailsTable, "Email:", booking.Email, font);
                AddRowToPdfTable(detailsTable, "Address:", booking.Address, font);
                AddRowToPdfTable(detailsTable, "City/Postal Code:", booking.CityPostalCode, font);
                AddRowToPdfTable(detailsTable, "Pet Name:", booking.Pname, font);
                AddRowToPdfTable(detailsTable, "Pet Species:", booking.SelectedSpecies, font);
                AddRowToPdfTable(detailsTable, "Pet Gender:", booking.Gender, font);
                AddRowToPdfTable(detailsTable, "Pet Breed/Color:", booking.BreedColor, font);
                AddRowToPdfTable(detailsTable, "Pet Age:", booking.Age.ToString(), font);
                AddRowToPdfTable(detailsTable, "Pet Weight:", booking.Weight.ToString(), font);
                AddRowToPdfTable(detailsTable, "Start Date:", booking.StartDate.ToShortDateString(), font);
                AddRowToPdfTable(detailsTable, "End Date:", booking.EndDate.ToShortDateString(), font);
                AddRowToPdfTable(detailsTable, "Total Cost:", $"R {booking.TotalCost}", font);




                doc.Add(detailsTable);

                doc.Close();

                return memoryStream.ToArray();
            }
        }




        private void AddRowToPdfTable(PdfPTable table, string label, string value, Font font)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, font));
            PdfPCell valueCell = new PdfPCell(new Phrase(value, font));
            labelCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        private void SendEmailWithInvoice(Booking booking)
        {
            byte[] invoicePdf = GenerateInvoicePdfContent(booking);

            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "spcadurbanza@gmail.com";
            string smtpPassword = "urpc bsvq bdmd wpda";

            string senderEmail = smtpUsername;
            string recipientEmail = booking.Email;

            MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail);
            mailMessage.Subject = "Booking Confirmation and Invoice";

            string body = $@"
    <img src=""https://i.postimg.cc/qRMDx9MV/Screenshot-12.png""alt='SPCA Durban Logo'>
    <h2>Confirmation of Pet Boarding Booking at SPCA Durban</h2>
    <p>Dear {booking.Fname},</p>
    <p>Below, you'll find the details of your booking:</p>
    <ul>
        <li>Pet's Name: {booking.Pname}</li>
        <li>Boarding Start Date: {booking.StartDate.ToShortDateString()}</li>
        <li>Boarding End Date: {booking.EndDate.ToShortDateString()}</li>
    </ul>
    <p>Thank you for choosing SPCA Durban for your pet's boarding needs. We are thrilled to confirm your booking and look forward to providing a safe and comfortable stay for your furry companion.</p>
    <p>Please ensure that you arrive at our facility on the designated start date of your pet's stay. Our team will be ready to welcome your pet and ensure they settle in comfortably. If you have any questions or need to make changes to your booking, feel free to contact us by replying to this email.</p>
    <p>Thank you again for choosing SPCA Durban. We're honored to be entrusted with the care of your beloved pet.</p>
    <p>Warm regards,</p>";

            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            using (MemoryStream ms = new MemoryStream(invoicePdf))
            {
                mailMessage.Attachments.Add(new Attachment(ms, "Invoice.pdf", "application/pdf"));

                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);
                }
            }
        }


        public ActionResult Reschedule()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Reschedule(RescheduleViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existingBooking = db.Bookings.FirstOrDefault(b =>
    b.BookingId == model.BookingId &&
    b.Fname == model.Fname &&
    b.StartDate == model.StartDate && 
    b.EndDate == model.EndDate);     





            if (existingBooking == null)
            {
                ModelState.AddModelError("", "Invalid booking details.");
                return View(model);
            }

            
            string otp = GenerateOTP();
            TempData["OTP"] = otp;
            TempData["BookingId"] = existingBooking.BookingId;
            TempData["RescheduleViewModel"] = model;

            
            SendOTPByEmail(existingBooking.Email, otp);

            return RedirectToAction("VerifyOTP");
        }

        public ActionResult VerifyOTP()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View();
        }

        [HttpPost]
        public ActionResult VerifyOTP(string otp)
        {
            string expectedOTP = TempData["OTP"] as string;

            if (otp == expectedOTP)
            {
                
                var bookingId = (int)TempData["BookingId"];

                
                return RedirectToAction("NewDate", new { bookingId = bookingId });
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid OTP. Please try again.";
                return RedirectToAction("VerifyOTP");
            }
        }

        public ActionResult NewDate(int bookingId)
        {
            
            var existingBooking = db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (existingBooking == null)
            {
                
                Debug.WriteLine($"Booking with ID {bookingId} not found.");
                return HttpNotFound();
            }

           
            ViewBag.ExistingBooking = existingBooking;
            return View();
        }
        [HttpPost]
        public ActionResult NewDate(RescheduleViewModel model)
        {
            
            var existingBooking = db.Bookings.FirstOrDefault(b => b.BookingId == model.BookingId);

            if (existingBooking == null)
            {
                return HttpNotFound();
            }

           
            DateTime StartDate = model.StartDate;
            DateTime EndDate = model.EndDate;

            TimeSpan originalDuration = existingBooking.EndDate - existingBooking.StartDate;
            TimeSpan newDuration = EndDate - StartDate;

            if (newDuration != originalDuration)
            {
                ModelState.AddModelError("", "New dates must have the same duration as the original booking.");
                return View(model);
            }

            
            existingBooking.StartDate = StartDate;
            existingBooking.EndDate = EndDate;
            db.Entry(existingBooking).State = EntityState.Modified;
            db.SaveChanges();

            
            TempData["BookingConfirmation"] = existingBooking;

            
            return RedirectToAction("Success");
        }



        private string GenerateOTP()
        {
            
            Random rnd = new Random();
            int otp = rnd.Next(100000, 999999);
            return otp.ToString();
        }
        public ActionResult ResendOTP()
        {
            var model = TempData["RescheduleViewModel"] as RescheduleViewModel;

            if (model == null)
            {
                
                return RedirectToAction("VerifyOTP");
            }

            
            var existingBooking = db.Bookings.FirstOrDefault(b => b.BookingId == model.BookingId);

            if (existingBooking == null)
            {
                
                ModelState.AddModelError("", "Invalid booking details.");
                return RedirectToAction("VerifyOTP");
            }

            
            string otp = GenerateOTP();
            TempData["OTP"] = otp;

            
            SendOTPByEmail(existingBooking.Email, otp);

            return RedirectToAction("VerifyOTP");
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
            mailMessage.Body = $"Your OTP for rescheduling your boarding is: {otp}";

            
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true;

            
            smtpClient.Send(mailMessage);
        }

         public ActionResult Survey()
        {
            return View(new SurveyModel());
        }

        
        [HttpPost]
        public ActionResult Survey(SurveyModel survey)
        {
            if (ModelState.IsValid)
            {
                db.SurveyModels.Add(survey);
                db.SaveChanges();
                return View("Index");
            }
            else { 
            // If the model state is not valid, redisplay the form with validation errors
            return View(survey);
            }
        }

        private void SendPostEventSurveyEmail(string userEmail)
        {
            // Generate the URL for the Survey page
            string surveyUrl = Url.Action("Survey", "Home", null, Request.Url.Scheme);

            // Create the email message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("spcadurbanza@gmail.com");
            mail.To.Add(userEmail);
            mail.Subject = "We Value Your Feedback - Post Event Survey";

            mail.Body = "Dear Valued Participant,\n\n" +
                        "We hope you had a wonderful experience at our recent event. " +
                        "Your feedback is very important to us, and we would love to hear your thoughts.\n\n" +
                        "Please take a moment to complete our post-event survey. Your insights will help us improve " +
                        "and ensure that future events are even better.\n\n" +
                        "Click the link below to start the survey:\n" +
                        $"{surveyUrl}\n\n" +
                        "Thank you for your time and participation. We look forward to hearing from you!\n\n" +
                        "Warm regards,\nYour Event Team\nSPCA Durban";

            // Configure the SMTP client
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
            smtp.EnableSsl = true;

            // Send the email
            smtp.Send(mail);
        }



        [HttpPost]
        public ActionResult SendSurveyEmail()
        {
            // Fetch all users who have purchased a ticket
            var usersWithTickets = db.EventRegs.Where(e => e.Quantity > 0).ToList();

            foreach (var user in usersWithTickets)
            {
                // Send the survey email to each user
                SendPostEventSurveyEmail(user.Email);
            }

            // Optionally, add a success message or log
            TempData["SuccessMessage"] = "Survey emails have been sent successfully.";

            // Redirect or return a view after sending the emails
            return RedirectToAction("Index");
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




