using HazelClinic3.Models;
using Stripe.Checkout;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace HazelClinic3.Controllers
{
    public class PetAdoptionController : Controller
    {
        private DataContext db = new DataContext();

      
        public ActionResult AdoptionInfo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetExistingAppointments(DateTime selectedDate)
        {
            
            var existingAppointments = db.Adoptions
                .Where(a => DbFunctions.TruncateTime(a.AppointDate) == selectedDate.Date)
                .Select(a => a.AppointTime)
                .ToList()
                .Select(time => time.ToString(@"hh\:mm")) 
                .ToList();

            return Json(existingAppointments, JsonRequestBehavior.AllowGet);
        }




        public ActionResult AdoptionForm()
        {
            
           
            return View();
        }

        [HttpPost]
        public ActionResult SubmitAdoption(AdoptionRequest adoptionRequest)
        {
            if (ModelState.IsValid)
            {
                string userEmail = (string)Session["GlobalEmail"];
                string userName = (string)Session["GlobalPetName"];


                var user = db.Users2.FirstOrDefault(u => u.Email == userEmail);

                if (user != null)
                {

                    adoptionRequest.AdopterEmail = user.Email;
                    adoptionRequest.AdopterFName = user.Firstname + user.Lastname;
                    adoptionRequest.AdopterNo = user.Mobile;
                    adoptionRequest.Address = user.Address;
                }
                var existingRequest = db.Adoptions.FirstOrDefault(a => a.AdopterEmail == adoptionRequest.AdopterEmail);
                if (existingRequest != null)
                {
                    ModelState.AddModelError("", "An adoption request has already been made with this email. Please check your Adoption progress.");
                    return View("AdoptionForm", adoptionRequest);
                }


                db.Adoptions.Add(adoptionRequest);
                db.SaveChanges();

                return RedirectToAction("StripePayment", new { adoptionRequestId = adoptionRequest.ID });
            }

            return View("AdoptionForm", adoptionRequest);
        }


        public ActionResult StripePayment(int adoptionRequestId)
        {
            
            var adoptionRequest = db.Adoptions.Find(adoptionRequestId);
            if (adoptionRequest == null)
            {
                return HttpNotFound();
            }

            
            StripeConfiguration.ApiKey = "sk_test_51P1F4HKgNOMzGBDh6junKYp3kCPW3zvipfkiuPzCd2TAYjBgx72OR2OoyalAJ9lmLOSPMuVYQhCbOyWTQZO0M4tG000sJOeAP2";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "zar",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Pet Adoption Fee"
                            },
                            UnitAmount = 10000 
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = Url.Action("AdoptionSuccess", "PetAdoption", new { adoptionRequestId }, Request.Url.Scheme),
               
                CancelUrl = Url.Action("AdoptionForm", "PetAdoption", null, Request.Url.Scheme),
            };
            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }







        public ActionResult ViewAdoptions()
        {
            var adoptionData = db.Adoptions.ToList();
            return View(adoptionData);
        }

       
        public ActionResult Edit(int id)
        {
            var adoptionRequest = db.Adoptions.Find(id);
            if (adoptionRequest == null)
            {
                return HttpNotFound(); 
            }
            return View(adoptionRequest);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdoptionRequest adoptionRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adoptionRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewAdoptions");
            }
            return View(adoptionRequest);
        }

        
        public ActionResult Delete(int id)
        {
            var adoptionRequest = db.Adoptions.Find(id);
            if (adoptionRequest == null)
            {
                return HttpNotFound(); 
            }
            return View(adoptionRequest);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var adoptionRequest = db.Adoptions.Find(id);
            if (adoptionRequest == null)
            {
                return HttpNotFound(); 
            }

            db.Adoptions.Remove(adoptionRequest);
            db.SaveChanges();
            return RedirectToAction("ViewAdoptions");
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdoptionSuccess(int adoptionRequestId)
        {
            var adoptionRequest = db.Adoptions.Find(adoptionRequestId);
            if (adoptionRequest == null)
            {
                return HttpNotFound();
            }

           
            SendConfirmationEmail(adoptionRequest);

            return View();
        }

        
        private void SendConfirmationEmail(AdoptionRequest adoptionRequest)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("spcadurbanza@gmail.com");
                mail.To.Add(adoptionRequest.AdopterEmail);
                mail.Subject = "Pet Adoption Request Confirmation";
                mail.Body = $"Dear {adoptionRequest.AdopterFName},\n\n" +
                            $"We are delighted to inform you that we have received your deposit payment for the adoption of a pet. Your request has been successfully processed.\n\n" +
                            $"Appointment Details:\n" +
                            $"Date: {adoptionRequest.AppointDate.ToShortDateString()}\n" +
                            $"Time: {adoptionRequest.AppointTime}\n\n" +
                            $"Please be ready at least 15 minutes before your scheduled appointment time. Our team will be waiting to assist you.\n\n" +
                            $"Thank you for choosing SPCA DURBAN for your pet adoption needs. If you have any questions or require further assistance, feel free to contact us.\n\n" +
                            $"Best regards,\n" +
                            $"SPCA Durban Team";
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.ToString());
            }
        }

        public ActionResult InspectorAdoptions(int inspectorId)
        {
            // Filter the adoptions based on the inspectorId
            var adoptions = db.Adoptions.Where(a => a.InspectorId == inspectorId).ToList();

            if (adoptions == null)
            {
                adoptions = new List<AdoptionRequest>(); // Ensure the model is never null
            }

            return View(adoptions);
        }

        private void SendEmail(string fromEmail, string toEmail, string subject, string body)
        {
            MailMessage mail = new MailMessage(fromEmail, toEmail);
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda"),
                EnableSsl = true
            };

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            smtpClient.Send(mail);
        }

        // POST: Assign inspector
    
        [HttpPost]
        public ActionResult AssignInspector(int? ID, int inspector, DateTime inspectionDate)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var adoptionRequest = db.Adoptions.Find(ID);
            if (adoptionRequest == null)
            {
                return HttpNotFound();
            }

            // Hardcoded inspector emails
            Dictionary<int, string> inspectorEmails = new Dictionary<int, string>
            {
                    { 1, "ammaarm161@gmail.com" },
                    { 2, "ammaarm161@gmail.com" },
                    { 3, "ammaarm161@gmail.com" }
            };

            if (!inspectorEmails.ContainsKey(inspector))
            {
                return HttpNotFound("Inspector not found.");
            }

            // Update the inspector ID, inspection date, and potentially other details
            adoptionRequest.InspectorId = inspector;
            adoptionRequest.InspectionDate = inspectionDate; // Assuming this field is part of your model
            db.Entry(adoptionRequest).State = EntityState.Modified;

            try
            {
                db.SaveChanges();

                // Construct email details
                string userEmail = adoptionRequest.AdopterEmail; // Assuming 'AdopterEmail' is the property
                string userName = adoptionRequest.AdopterFName;  // Assuming 'AdopterFName' is the property
                string inspectorEmail = inspectorEmails[inspector];
                string subject = "Inspector Assignment for Adoption Request";
                string address = adoptionRequest.Address;  // Assuming 'Address' is the property
                string body = $"Dear {userName},<br/><br/>An inspector has been assigned to your adoption request. " +
                              $"Here are the details:<br/>" +
                              $"Inspector Email: {inspectorEmail}<br/>" +
                              $"Inspection Address: {address}<br/>" +
                              $"Inspection Date: {inspectionDate.ToString("dddd, dd MMMM yyyy")}<br/><br/>" +
                              $"Thank you.";

                // Send email to user
                SendEmail("spcadurbanza@gmail.com", userEmail, subject, body);

                // Send email to inspector
                string inspectorBody = $"You have been assigned to a new adoption request by {userName}. " +
                                       $"Please be prepared for an inspection on {inspectionDate.ToString("dddd, dd MMMM yyyy")} " +
                                       $"at the following address: {address}.";
                SendEmail("spcadurbanza@gmail.com", inspectorEmail, "Assignment Notification", inspectorBody);
            }
            catch (Exception ex)
            {
                // Log the error (add a logging framework like NLog or log4net here)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return RedirectToAction("ViewAdoptions"); // Redirect back to view page or show error
            }

            return RedirectToAction("ViewAdoptions");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]

        public ActionResult UpdateStatus(int adoptionId, string status, string safety, string cleanliness, string space, string provisions, string interactive, string fenced)
        {
            var adoption = db.Adoptions.FirstOrDefault(a => a.ID == adoptionId);
            if (adoption == null)
            {
                return HttpNotFound("Adoption request not found.");
            }

            // Update the status text and checkmarks based on form submission
            adoption.Status = status; // Update the main status with additional comments
            adoption.SafetyChecked = safety == "Safety Checked"; // Boolean field in the model (true if checked)
            adoption.CleanlinessChecked = cleanliness == "Cleanliness Checked"; // Boolean field
            adoption.SpaceChecked = space == "Adequate Living Space"; // Boolean field
            adoption.ProvisionsChecked = provisions == "Provisions Available"; // Boolean field
            adoption.InteractiveFamilyChecked = interactive == "Interactive Family"; // Boolean field
            adoption.FencedChecked = fenced == "Fenced Property"; // Boolean field

            adoption.TrackingStatus = AdoptionStatus.AwaitingInspectorFeedback;

            bool isComplete = adoption.SafetyChecked || adoption.CleanlinessChecked || adoption.SpaceChecked ||
                       adoption.ProvisionsChecked || adoption.InteractiveFamilyChecked || adoption.FencedChecked &&
                       status == "Completed"; // Example condition where all checks must be true and status must be "Completed"

            // Set the completion flag
            adoption.IsInspectionComplete = isComplete;

            db.Entry(adoption).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the error (add a logging framework like NLog or log4net here)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                return View("Error"); // Redirect to a generic error page or the same page with error message
            }

            return RedirectToAction("InspectorAdoptions", new { inspectorId = adoption.InspectorId }); // Redirect to the list of adoptions after update
        }


        public ActionResult InspecFb(int? inspectorId)
        {
            ViewBag.SelectedInspectorId = inspectorId;

            // Fetch only the adoptions where the inspection is completed
            var model = db.Adoptions
                           .Where(x => x.InspectorId == inspectorId && x.IsInspectionComplete)
                           .ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Approve(int adoptionId)
        {
            var adoption = db.Adoptions.Find(adoptionId);
            if (adoption == null)
            {
                return HttpNotFound();
            }
            adoption.TrackingStatus = AdoptionStatus.Approved;

            var approvedAdoption = new ApprovedAdoptions
            {
                Adopterinterested = adoption.Adopterinterested,
                AdopterFName = adoption.AdopterFName,
                AdopterNo = adoption.AdopterNo,
                AdopterEmail = adoption.AdopterEmail,
                AdopterMessage = adoption.AdopterMessage,
                Address = adoption.Address,
                InspectorId = adoption.InspectorId,
                Status = adoption.Status,
                SafetyChecked = adoption.SafetyChecked,
                CleanlinessChecked = adoption.CleanlinessChecked,
                SpaceChecked = adoption.SpaceChecked,
                ProvisionsChecked = adoption.ProvisionsChecked,
                InteractiveFamilyChecked = adoption.InteractiveFamilyChecked,
                FencedChecked = adoption.FencedChecked

            };

           

            db.ApprovedAdoptions.Add(approvedAdoption);

            db.SaveChanges();

            SendApprovalEmail(adoption.AdopterEmail);

            return RedirectToAction("ViewAdoptions");
        }



        private void SendApprovalEmail(string recipientEmail)

        {

            try

            {

                string subject = "Congratulations! Your Pet Adoption request has been approved.";

                string body = @"

            <html>

            <body>

            <p>Dear Adopter,</p>

            <p>We are thrilled to inform you that your adoption request has been approved! Your commitment to providing a loving home to a pet is truly commendable, and we are excited to facilitate this process for you.</p>

            <p>To finalize the adoption, please proceed with the payment by clicking on the link below:</p>

            <p><a href='https://buy.stripe.com/test_cN228x93Gd44568bII'>Complete Payment</a></p>

            <p>Payment Details:</p>

            <ul>

                <li>Adoption Fee: R850</li>

                <li>Payment Method: Secure online payment</li>

            </ul>

            <p>As part of our commitment to transparency, please find below a QR code for your reference:</p>

            <p><img src='https://i.ibb.co/bgpvxFW/qr-test-c-N228x93-Gd44568b-II.png' alt='QR Code Placeholder' /></p>

            <p>Once the payment is successfully processed, our team will contact you to arrange the final steps of the adoption process.</p>

            <p>If you have any questions or need further assistance, feel free to reach out to us at spcadurbanza@gmail.com</p>

            <p>Thank you once again for choosing to adopt from us. We appreciate your support for our cause of finding loving homes for pets in need.</p>

            <p>Best regards,</p>

            <p>SPCA DURBAN</p>

            </body>

            </html>";

                using (MailMessage mail = new MailMessage())

                {

                    mail.From = new MailAddress("spcadurbanza@gmail.com");

                    mail.To.Add(recipientEmail);

                    mail.Subject = subject;

                    mail.Body = body;

                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))

                    {

                        smtp.Port = 587;

                        smtp.Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");

                        smtp.EnableSsl = true;

                        smtp.Send(mail);

                    }

                }

            }

            catch (Exception ex)

            {

            }

        }

        [HttpPost]

        public ActionResult Decline(int adoptionId)

        {

            var adoption = db.Adoptions.Find(adoptionId);

            if (adoption == null)

            {

                return HttpNotFound();

            }

            adoption.TrackingStatus = AdoptionStatus.Declined;

            var declinedAdoption = new DeclinedAdoptions

            {

                Adopterinterested = adoption.Adopterinterested,

                AdopterFName = adoption.AdopterFName,

                AdopterNo = adoption.AdopterNo,

                AdopterEmail = adoption.AdopterEmail,

                AdopterMessage = adoption.AdopterMessage,

                InspectorId = adoption.InspectorId,

                Status = adoption.Status,

                SafetyChecked = adoption.SafetyChecked,

                CleanlinessChecked = adoption.CleanlinessChecked,

                SpaceChecked = adoption.SpaceChecked,

                ProvisionsChecked = adoption.ProvisionsChecked,

                InteractiveFamilyChecked = adoption.InteractiveFamilyChecked,

                FencedChecked = adoption.FencedChecked

            };

         

            db.DeclinedAdoptions.Add(declinedAdoption);

            db.SaveChanges();



            try

            {

                MailMessage mail = new MailMessage();

                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("spcadurbanza@gmail.com");

                mail.To.Add(adoption.AdopterEmail);

                mail.Subject = "Adoption Declined";

                mail.Body = "Dear " + adoption.AdopterFName + ",\n\n";

                mail.Body += "Unfortunately, your adoption request has been declined.\n\n";

                mail.Body += "We appreciate your interest in adopting, but after careful consideration, we have decided not to proceed with the adoption at this time.\n\n";

                mail.Body += "Thank you for understanding.\n\n";

                mail.Body += "Sincerely,\n";

                mail.Body += "SPCA DURBAN";

                SmtpServer.Port = 587;

                SmtpServer.Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");

                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                Console.WriteLine("Email sent successfully.");

            }

            catch (Exception ex)

            {

                Console.WriteLine("Failed to send email. Error: " + ex.Message);

            }

            return RedirectToAction("ViewAdoptions");

        }

        public ActionResult ApprovedAdoptions()
        {
            var approvedAdoptions = db.ApprovedAdoptions.ToList();
            var adoptionDeliveryDates = db.OngoingDelivery
                .ToDictionary(od => od.AdopterEmail, od => od.DeliveryDate);

            ViewBag.AdoptionDeliveryDates = adoptionDeliveryDates;

            return View(approvedAdoptions);
        }

        public ActionResult DeclinedAdoptions()

        {

            var declinedAdoptions = db.DeclinedAdoptions.ToList();

            return View(declinedAdoptions);

        }

        [HttpPost]
        public ActionResult AssignDriver(int adoptionId, DateTime deliveryDate)
        {
            var adoption = db.ApprovedAdoptions.Find(adoptionId);
            if (adoption == null)
            {
                return HttpNotFound();
            }

         
            var ongoingDelivery = new OngoingDriver
            {
                AdopterFName = adoption.AdopterFName,
                AdopterNo = adoption.AdopterNo,
                AdopterEmail = adoption.AdopterEmail,
                Address = adoption.Address,

                DeliveryDate = deliveryDate
            };
            db.OngoingDelivery.Add(ongoingDelivery);
            db.SaveChanges();

           
            SendEmailNotification(adoption.AdopterEmail, adoption.AdopterFName, deliveryDate);

            return RedirectToAction("AdminLog", "Users2");

        }

        private void SendEmailNotification(string toEmail, string adopterName, DateTime deliveryDate)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("spcadurbanza@gmail.com");
            mail.To.Add(toEmail);
            mail.Subject = "Driver Assigned for Pet Delivery";

            string body = $"Dear {adopterName},\n\n";
            body += "We are pleased to inform you that a dedicated driver has been assigned to deliver your beloved pet.\n\n";
            body += $"Delivery Date: {deliveryDate.ToShortDateString()}\n\n";
            body += "You will receive an OTP (One-Time Password) when the driver is OUT FOR DELIVERY on your scheduled day. Please keep the OTP safe as you are required to provide it to the driver upon delivery. Failure to provide the OTP will result in an unsuccessful delivery.\n\n";
            body += "Our driver will arrive punctually and ensure the safe and comfortable transport of your pet to its new home.\n\n";
            body += "Should you have any questions or require further assistance, please do not hesitate to contact us.\n\n";
            body += "Thank you for choosing our adoption services.\n\n";
            body += "Best regards,\nYour Pet Adoption Team";

            mail.Body = body;

            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
            smtpServer.EnableSsl = true;

            try
            {
                smtpServer.Send(mail);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }








        }


        public ActionResult Returns()
        {
            var returnRequests = db.ReturnPolicies.ToList();
            return View(returnRequests);
        }

        public ActionResult CreateReturn()
        {
            var model = new ReturnPolicy();
            model.ReturnDate = DateTime.Now; // Set the return date to the current date and time
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReturn(ReturnPolicy model)
        {
            model.ReturnStatus = "In Progress";
            db.ReturnPolicies.Add(model);
            db.SaveChanges();

            return RedirectToAction("Returns");
        }


        public ActionResult CompleteReturn(int id)
        {
            var returnPolicy = db.ReturnPolicies.Find(id); // Assuming db is your DbContext
            if (returnPolicy != null)
            {
                returnPolicy.ReturnStatus = "Complete";
                db.SaveChanges();
            }
            return RedirectToAction("Returns"); // Adjust as needed
        }


    }











}

