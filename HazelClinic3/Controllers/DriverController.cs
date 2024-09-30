using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using HazelClinic3.Models;

namespace HazelClinic3.Controllers
{
    public class DriverController : Controller
    {

        private DataContext db = new DataContext();
        public ActionResult DriverDashboard()
        {
            return View();
        }

        public ActionResult Ongoing()
        {
            var ongoingDeliveries = db.OngoingDelivery.ToList();
            return View(ongoingDeliveries);
        }

        public ActionResult Completed()
        {
            var CompletedDelivery = db.CompletedDelivery.ToList();
            return View(CompletedDelivery);
        }

        [HttpPost]
        public ActionResult OutForDelivery(int driverId)
        {
            
            var driver = db.OngoingDelivery.Find(driverId);
            
            if (driver == null)
            {
                return HttpNotFound();
            }


            var adoption = db.Adoptions.FirstOrDefault(a => a.AdopterEmail == driver.AdopterEmail);
            if (adoption == null)
            {
                return HttpNotFound("The corresponding adoption record was not found.");
            }

            // Update the TrackingStatus of the adoption to OnDelivery
            adoption.TrackingStatus = AdoptionStatus.OnDelivery;

            // Mark the adoption object as modified and save changes to the database
            db.Entry(adoption).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle the error appropriately
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                return View("Error"); // Ensure you have an Error view
            }



            string otp = GenerateOTP();


            SendOutForDeliveryEmail(driver.AdopterEmail, otp);


            TempData["OTP"] = otp;
            TempData["DriverId"] = driverId;

            return RedirectToAction("Ongoing");
        }

        [HttpPost]
        public ActionResult CompleteDelivery(string otp)
        {
            string storedOTP = TempData["OTP"] as string;
            int driverId = Convert.ToInt32(TempData["DriverId"]);

            if (otp == storedOTP)
            {

                var driver = db.OngoingDelivery.Find(driverId);
                var adoption = db.Adoptions.FirstOrDefault(a => a.AdopterEmail == driver.AdopterEmail);
                if (adoption == null)
                {
                    return HttpNotFound("The corresponding adoption record was not found.");
                }

                // Update the TrackingStatus of the adoption to OnDelivery
                adoption.TrackingStatus = AdoptionStatus.Delivered;

                // Mark the adoption object as modified and save changes to the database
                db.Entry(adoption).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Handle the error appropriately
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    return View("Error"); // Ensure you have an Error view
                }
                var completedDelivery = new CompDriver
                {
                    AdopterFName = driver.AdopterFName,
                    AdopterNo = driver.AdopterNo,
                    AdopterEmail = driver.AdopterEmail,
                        Address = driver.Address,
                };
                

                db.CompletedDelivery.Add(completedDelivery);
                db.OngoingDelivery.Remove(driver);
                db.SaveChanges();


                SendDeliveryCompletionEmail(driver.AdopterEmail);

                return RedirectToAction("DriverDashboard");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP");
                return RedirectToAction("Ongoing");
            }
        }
        private void SendDeliveryCompletionEmail(string userEmail)
        {
            string rateDeliveryUrl = Url.Action("RateDelivery", "Home", null, Request.Url.Scheme);

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("spcadurbanza@gmail.com");
            mail.To.Add(userEmail);
            mail.Subject = "Delivery Confirmation";

            mail.Body = "Dear Valued Customer,\n\n" +
                        "We are pleased to inform you that your beloved pet has been successfully delivered to your doorstep.\n\n" +
                        "At SPCA Durban, we understand the importance of your pet's safety and comfort. " +
                        "Rest assured, our delivery team handled your pet with the utmost care throughout the journey.\n\n" +
                        "Your satisfaction is our top priority. If you have any questions or concerns regarding your delivery, " +
                        "please do not hesitate to reach out to us immediately. We are here to assist you in any way possible.\n\n" +
                        "Thank you for choosing SPCA Durban for your pet delivery needs. We truly appreciate your trust and confidence in us.\n\n" +
                        "Warm regards,\nYour Dedicated Delivery Team\nSPCA Durban\n\n" +
                        $"Please take a moment to rate our delivery service: {rateDeliveryUrl}";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }


        private void SendOutForDeliveryEmail(string userEmail, string otp)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("spcadurbanza@gmail.com");
            mail.To.Add(userEmail);
            mail.Subject = "Your Pet is Out for Delivery";


            mail.Body = $"Hello,\n\nWe are excited to inform you that your pet is now out for delivery!\n\n" +
                        $"Our delivery team is on their way to bring your pet to you. " +
                        $"Your unique One-Time Password (OTP) for accepting the delivery is: {otp}.\n\n" +
                        $"Please ensure someone is available to receive the delivery. " +
                        $"If there are any special instructions or concerns, please don't hesitate to contact us.\n\n" +
                        $"Thank you for choosing us for your pet delivery needs. We hope to serve you again in the future!\n\n" +
                        $"Best regards,\nYour Delivery Team";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }


        private string GenerateOTP()
        {
            Random rnd = new Random();
            int otpValue = rnd.Next(100000, 999999);
            return otpValue.ToString();
        }


        public ActionResult RescheduleAdoption(string email)
        {
            var driver = db.OngoingDelivery.FirstOrDefault(d => d.AdopterEmail == email);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        [HttpPost]
        public ActionResult RescheduleAdoption(OngoingDriver model)
        {
            if (ModelState.IsValid)
            {
                var driver = db.OngoingDelivery.FirstOrDefault(d => d.AdopterEmail == model.AdopterEmail);
                if (driver == null)
                {
                    return HttpNotFound();
                }

                // Check if the user has already rescheduled twice
                if (driver.RescheduleCount >= 2)
                {
                    // Set a ViewBag flag to indicate the reschedule limit has been reached
                    ViewBag.RescheduleLimitReached = true;
                    ModelState.AddModelError("", "You have reached the maximum number of reschedule attempts.");
                    return View(model);
                }

                driver.DeliveryDate = model.DeliveryDate;
                driver.RescheduleCount++; // Increment the reschedule count
                db.Entry(driver).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                    SendRescheduleEmail(driver.AdopterEmail, model.DeliveryDate);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    return View(model);
                }

                return RedirectToAction("MyProfile", "Users2");
            }
            return View(model);
        }


        private void SendRescheduleEmail(string userEmail, DateTime deliveryDate)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("spcadurbanza@gmail.com");
            mail.To.Add(userEmail);
            mail.Subject = "Rescheduled Adoption Delivery Date";

            mail.Body = "Dear Valued Customer,\n\n" +
                        $"Dear Adopter,\n\nYour delivery date has successfully been rescheduled to: {deliveryDate.ToString("MMMM dd, yyyy")}\n\n" +
                        "Thank you for choosing SPCA Durban for your pet delivery needs. We truly appreciate your trust and confidence in us.\n\n" +
                        "Warm regards,\nYour Dedicated Delivery Team\nSPCA Durban";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }

    }






}
