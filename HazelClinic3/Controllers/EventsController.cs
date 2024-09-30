using HazelClinic3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Image;
using QRCoder;
using ZXing;
using ZXing.Common;


namespace Events.Controllers
{
    public class EventsController : Controller
    {
        private DataContext _context;

        public EventsController()
        {
            _context = new DataContext();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,EventDate,EventTime,LimitOfAttendees,EventPrice,ArePetsAllowed,EventLocation")] HazelClinic3.Models.Event @event, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(image.InputStream))
                    {
                        @event.Image = reader.ReadBytes(image.ContentLength);
                    }
                }

                _context.Events.Add(@event);
                _context.SaveChanges();
                return RedirectToAction("AdminEvents");
            }

            return View(@event);
        }

        public ActionResult UserEvent()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        public ActionResult AdminEvents()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        public ActionResult GetEventImage(int id)
        {
            var @event = _context.Events.Find(id);
            if (@event != null && @event.Image != null)
            {
                return File(@event.Image, "image/jpeg");
            }
            return HttpNotFound();
        }

        public ActionResult AddAuctionItem(int eventId)
        {
            var auctionItem = new AuctionItem { Event_Id = eventId };
            return View(auctionItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAuctionItem([Bind(Include = "Id,Event_Id,Name")] AuctionItem auctionItem, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(image.InputStream))
                    {
                        auctionItem.Image = reader.ReadBytes(image.ContentLength);
                    }
                }
                auctionItem.AuctionEndTime = DateTime.Now.AddMinutes(7);
                _context.AuctionItems.Add(auctionItem);
                _context.SaveChanges();
                return RedirectToAction("AdminEvents");
            }

            return View(auctionItem);
        }

        public ActionResult AddDocument(int eventId)
        {
            var document = new EventDocument { Event_Id = eventId };
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDocument([Bind(Include = "Id,Event_Id,FileName")] EventDocument document, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {
                using (var reader = new BinaryReader(file.InputStream))
                {
                    document.FileContent = reader.ReadBytes(file.ContentLength);
                    document.FileName = Path.GetFileName(file.FileName);
                }
                _context.EventDocuments.Add(document);
                _context.SaveChanges();
                return RedirectToAction("AdminEvents");
            }

            return View(document);
        }

        public ActionResult Content(int id)
        {
            var auctionItems = _context.AuctionItems.Where(ai => ai.Event_Id == id).ToList();
            var documents = _context.EventDocuments.Where(ed => ed.Event_Id == id).ToList();

            var model = new EventContentViewModel
            {
                AuctionItems = auctionItems,
                Documents = documents
            };

            return View(model);
        }

        public ActionResult GetAuctionItemImage(int id)
        {
            var item = _context.AuctionItems.Find(id);
            if (item != null && item.Image != null)
            {
                return File(item.Image, "image/jpeg");
            }
            return HttpNotFound();
        }

        public ActionResult DownloadDocument(int id)
        {
            var document = _context.EventDocuments.Find(id);
            if (document != null && document.FileContent != null)
            {
                return File(document.FileContent, "application/pdf", document.FileName);
            }
            return HttpNotFound();
        }

        public ActionResult Edit(int id)
        {
            var @event = _context.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,EventDate,EventTime,LimitOfAttendees,EventPrice,ArePetsAllowed,EventLocation")] HazelClinic3.Models.Event @event, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var eventInDb = _context.Events.Find(@event.Id);
                if (eventInDb == null)
                {
                    return HttpNotFound();
                }

                // Update event fields
                eventInDb.EventName = @event.EventName;
                eventInDb.EventDate = @event.EventDate;
                eventInDb.EventTime = @event.EventTime;
                eventInDb.LimitOfAttendees = @event.LimitOfAttendees;
                eventInDb.EventPrice = @event.EventPrice;
                eventInDb.ArePetsAllowed = @event.ArePetsAllowed;
                eventInDb.EventLocation = @event.EventLocation;

                // Update image if a new one is uploaded
                if (image != null && image.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(image.InputStream))
                    {
                        eventInDb.Image = reader.ReadBytes(image.ContentLength);
                    }
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            HazelClinic3.Models.Event @event = _context.Events.Find(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
                _context.SaveChanges();
            }
            return RedirectToAction("AdminEvents", "Events");
        }

        public ActionResult BookEvent(int id)
        {
            var @event = _context.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            string userEmail = (string)Session["GlobalEmail"];
            var user = _context.Users2.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return RedirectToAction("Login", "Users"); // Redirect to login if the user is not found
            }

            var model = new EventReg
            {
                EventId = @event.Id,
                TotalCost = @event.EventPrice,
                Quantity = 1,
                FullName = $"{user.Firstname} {user.Lastname}",  // Automatically set the user's full name
                ContactNumber = user.Mobile,                     // Automatically set the user's contact number
                Email = user.Email

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookEvent(EventReg eventReg)
        {
            if (ModelState.IsValid)
            {
                var @event = _context.Events.Find(eventReg.EventId);
                if (@event == null)
                {
                    return HttpNotFound();
                }

                eventReg.TotalCost = eventReg.Quantity * @event.EventPrice;

                _context.EventRegs.Add(eventReg);
                _context.SaveChanges();

                // Update the TicketNumber with the actual Id and EventId after saving
                eventReg.TicketNumber = $"TIK_{eventReg.Id}_{eventReg.EventId}";

                // Update the event registration in the context to set the TicketNumber
                _context.Entry(eventReg).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();

                return RedirectToAction("CreateCheckoutSession", new { eventRegId = eventReg.Id, EventId = eventReg.EventId, TotalCost = eventReg.TotalCost, Quantity = eventReg.Quantity });
            }

            return View(eventReg);
        }


        [HttpGet]
        public async Task<ActionResult> CreateCheckoutSession(int eventRegId, int EventId, decimal TotalCost, int Quantity)
        {
            StripeConfiguration.ApiKey = "sk_test_51P1F4HKgNOMzGBDh6junKYp3kCPW3zvipfkiuPzCd2TAYjBgx72OR2OoyalAJ9lmLOSPMuVYQhCbOyWTQZO0M4tG000sJOeAP2";

            var @event = _context.Events.Find(EventId);
            if (@event == null)
            {
                return HttpNotFound();
            }

            var pricePerTicket = (long)(@event.EventPrice * 100);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = pricePerTicket,
                    Currency = "zar",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Event Ticket",
                    },
                },
                Quantity = Quantity,
            },
        },
                Mode = "payment",
                SuccessUrl = Url.Action("ThankYou", "Events", new { eventRegId = eventRegId }, Request.Url.Scheme),
                CancelUrl = Url.Action("BookEvent", "Events", new { id = EventId }, Request.Url.Scheme),
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }




        public ActionResult ThankYou(int? eventRegId)
        {
            if (eventRegId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventReg = _context.EventRegs.Find(eventRegId);
            var eventEntity = _context.Events.Find(eventReg.EventId);

            if (eventReg == null || eventEntity == null)
            {
                return HttpNotFound();
            }

            var pdfBytes = GenerateTicketPdf(eventReg, eventEntity);

            SendConfirmationEmail(eventReg, eventEntity, pdfBytes);

            Session["HasPurchasedTicket"] = true;

            return View();
        }


        private byte[] GenerateTicketPdf(EventReg eventReg, HazelClinic3.Models.Event eventEntity)
        {
            using (var stream = new MemoryStream())
            {
                var writerProperties = new WriterProperties();
                using (var writer = new PdfWriter(stream, writerProperties))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);


                        var headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                        var bodyFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);


                        var logoUrl = "https://i.ibb.co/S3wt3sM/SPCA-logo.png";
                        var logoImage = new Image(ImageDataFactory.Create(logoUrl))
                            .SetAutoScale(true)
                            .SetWidth(150)
                            .SetHeight(75)
                            .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                        document.Add(logoImage);


                        document.Add(new Paragraph("Event Ticket")
                            .SetFont(headerFont)
                            .SetFontSize(30)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMarginBottom(20));


                        document.Add(new Paragraph($"Event Name: {eventEntity.EventName}")
                            .SetFont(bodyFont)
                            .SetFontSize(16)
                            .SetMarginBottom(5)
                            .SetTextAlignment(TextAlignment.LEFT));

                        document.Add(new Paragraph($"Date: {eventEntity.EventDate.ToString("dd MMMM yyyy")}")
                            .SetFont(bodyFont)
                            .SetFontSize(16)
                            .SetMarginBottom(5)
                            .SetTextAlignment(TextAlignment.LEFT));

                        document.Add(new Paragraph($"Time: {eventEntity.EventTime.ToString(@"hh\:mm")}")
                            .SetFont(bodyFont)
                            .SetFontSize(16)
                            .SetMarginBottom(5)
                            .SetTextAlignment(TextAlignment.LEFT));

                        document.Add(new Paragraph($"Venue: {eventEntity.EventLocation}")
                            .SetFont(bodyFont)
                            .SetFontSize(16)
                            .SetMarginBottom(15)
                            .SetTextAlignment(TextAlignment.LEFT));


                        document.Add(new Paragraph($"Ticket Holder: {eventReg.FullName}")
                            .SetFont(bodyFont)
                            .SetFontSize(16)
                            .SetMarginBottom(10)
                            .SetTextAlignment(TextAlignment.LEFT));

                        // Generate QR Code containing only the TicketNumber
                        var qrCodeText = eventReg.TicketNumber;
                        var qrCodeImage = GenerateQrCodeImage(qrCodeText);
                        document.Add(qrCodeImage.SetHorizontalAlignment(HorizontalAlignment.CENTER));
                       
                        document.Add(new Paragraph("SPCA DURBAN")
                            .SetFont(headerFont)
                            .SetFontSize(16)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMarginTop(20));

                        document.Add(new Paragraph("Springfield Park, 2 Willowfield Cres, Sea Cow Lake, Durban, 4001")
                            .SetFont(bodyFont)
                            .SetFontSize(14)
                            .SetTextAlignment(TextAlignment.CENTER));

                        document.Add(new Paragraph("Tel: +27 31 579 6500 | Email: spcadurbanza@gmail.com")
                            .SetFont(bodyFont)
                            .SetFontSize(14)
                            .SetTextAlignment(TextAlignment.CENTER));

                        
                    }
                }

                return stream.ToArray();
            }
        }

        private Image GenerateQrCodeImage(string text, int pixelSize = 10)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    using (var qrCodeBitmap = qrCode.GetGraphic(pixelSize))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            qrCodeBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                            var qrImage = new Image(ImageDataFactory.Create(memoryStream.ToArray()))
                                .SetWidth(80)  // Adjust width
                                .SetHeight(80)  // Adjust height
                                .SetHorizontalAlignment(HorizontalAlignment.CENTER);  // Center the QR code

                            return qrImage;
                        }
                    }
                }
            }
        }

        // QR Code Scanning and Check-In Methods

        public ActionResult ScanQRCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ScanQRCode(string scannedCode)
        {
            if (string.IsNullOrEmpty(scannedCode))
            {
                ViewBag.Message = "Invalid QR Code. Please try again.";
                return View();
            }

            var eventReg = _context.EventRegs.SingleOrDefault(e => e.TicketNumber == scannedCode);

            if (eventReg == null)
            {
                ViewBag.Message = "Invalid Ticket Number.";
                return View();
            }

            if (eventReg.CheckedIn)
            {
                ViewBag.Message = "This ticket has already been checked in.";
                return View();
            }

            // Mark the ticket as checked in
            eventReg.CheckedIn = true;
            _context.SaveChanges();

            ViewBag.Message = "Ticket successfully checked in.";
            return View();
        }

        private void SendConfirmationEmail(EventReg eventReg, HazelClinic3.Models.Event eventEntity, byte[] pdfBytes)
        {
            string subject = "Your Ticket Purchase Confirmation";
            string body = $@"
              Dear {eventReg.FullName},

              Thank you for your purchase!
 
              We are pleased to confirm your ticket purchase for the upcoming event hosted by SPCA Durban. 
              Your support helps us continue our mission of protecting and caring for animals in need.

             Event Details:

             Event Name: {eventEntity.EventName}
             Date: {eventEntity.EventDate.ToString("dd MMMM yyyy")}
             Time: {eventEntity.EventTime.ToString(@"hh\:mm")}
             Venue: {eventEntity.EventLocation}
             Please find your ticket(s) attached to this email. Kindly print them out or have them available on your mobile device for entry to the event.

             If you have any questions or need further assistance. 
             We look forward to seeing you at the event and thank you once again for your generous support.

             Best regards,
             SPCA Durban";

            using (var message = new MailMessage("spcadurbanza@gmail.com", eventReg.Email))
            {
                message.Subject = subject;
                message.Body = body;


                var attachment = new Attachment(new MemoryStream(pdfBytes), "Ticket.pdf", "application/pdf");
                message.Attachments.Add(attachment);

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(message);
                }
            }
        }

        [HttpGet]
        public JsonResult GetHighestBid(int id)
        {
            var highestBid = _context.Bids.Where(b => b.AuctionItemId == id)
                                          .OrderByDescending(b => b.Amount)
                                          .FirstOrDefault();

            decimal highestBidAmount = highestBid?.Amount ?? 0;

            return Json(new { highestBid = highestBidAmount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PlaceBid(int auctionItemId, decimal bidAmount)
        {
            var auctionItem = _context.AuctionItems.Find(auctionItemId);


            if (auctionItem.AuctionEndTime <= DateTime.Now)
            {
                return Json(new { success = false, message = "This auction has ended. You cannot place a bid." });
            }

            var highestBid = _context.Bids.Where(b => b.AuctionItemId == auctionItemId)
                                          .OrderByDescending(b => b.Amount)
                                          .FirstOrDefault();

            if (highestBid != null && bidAmount <= highestBid.Amount)
            {
                return Json(new { success = false, message = "Your bid must be higher than the current highest bid." });
            }

            if (bidAmount < 20)
            {
                return Json(new { success = false, message = "Bid amount must be at least R20." });
            }

            var bid = new Bid
            {
                AuctionItemId = auctionItemId,
                Amount = bidAmount,
                BidTime = DateTime.Now,
                UserEmail = (string)Session["GlobalEmail"]
            };

            _context.Bids.Add(bid);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult SendPaymentLink(int auctionItemId)
        {
            // Fetch the highest bid for the specified auction item.
            var highestBid = _context.Bids
                                     .Where(b => b.AuctionItemId == auctionItemId)
                                     .OrderByDescending(b => b.Amount)
                                     .FirstOrDefault();

            // Ensure only the highest bidder receives the email.
            if (highestBid != null)
            {
                var auctionItem = _context.AuctionItems.Find(auctionItemId);

                // Check if the email has already been sent.
                if (auctionItem != null && auctionItem.IsEmailSent)
                {
                    return Json(new { success = false, message = "Email has already been sent to the highest bidder." });
                }

                string userEmail = highestBid.UserEmail; // Get the email of the highest bidder.
                decimal amount = highestBid.Amount; // Get the highest bid amount.

                // Generate a payment link using Stripe for the highest bid amount.
                string paymentLink = GenerateStripePaymentLink(amount);

                // Send an email to the highest bidder with the payment link.
                bool emailSent = SendEmail(userEmail, paymentLink);

                if (emailSent)
                {
                    // Update the auction item to indicate that the email has been sent.
                    auctionItem.IsEmailSent = true;
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to send the payment link." });
                }
            }

            return Json(new { success = false, message = "No bids found for this auction item." });
        }

        private string GenerateStripePaymentLink(decimal amount)
        {

            StripeConfiguration.ApiKey = "sk_test_51P1F4HKgNOMzGBDh6junKYp3kCPW3zvipfkiuPzCd2TAYjBgx72OR2OoyalAJ9lmLOSPMuVYQhCbOyWTQZO0M4tG000sJOeAP2"; // Replace with your actual Stripe Secret Key


            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
            {
                new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        Currency = "zar",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Auction Payment"
                        },
                        UnitAmount = (long)(amount * 100)
                    },
                    Quantity = 1
                }
            },
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Events", null, Request.Url.Scheme),
                CancelUrl = Url.Action("Cancel", "Events", null, Request.Url.Scheme),
            };

            var service = new Stripe.Checkout.SessionService();
            var session = service.Create(options);

            return session.Url;
        }

        private bool SendEmail(string userEmail, string paymentLink)
        {
            try
            {

                var message = new MailMessage("spcadurbanza@gmail.com", userEmail)
                {
                    Subject = "Action Required: Complete Your Payment for Auction Item",
                    Body = $@"
        <p>Dear Valued Bidder,</p>
        <p>Congratulations on winning the auction! To secure your auction item, please complete the payment by clicking the link below:</p>
        <p><a href='{paymentLink}' style='color: #007bff; text-decoration: none;'>Complete Your Payment</a></p>
        <p>If you have any questions or need assistance, please feel free to contact us.</p>
        <p>Thank you for your prompt attention to this matter.</p>
        <p>Best Regards,<br/>
        The Auction Team<br/>
        SPCA Durban</p>",
                    IsBodyHtml = true
                };



                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(message);
                }

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public ActionResult Success()
        {

            return View();
        }

        public ActionResult Cancel()
        {

            return View();
        }


        [HttpPost]
        public JsonResult ValidateQRCode(string qrCodeText)
        {
            if (string.IsNullOrEmpty(qrCodeText))
            {
                return Json(new { success = false, message = "Invalid QR Code.", reload = true });
            }

            var eventReg = _context.EventRegs.Include(er => er.Event).FirstOrDefault(er => er.TicketNumber == qrCodeText);
            if (eventReg == null)
            {
                return Json(new { success = false, message = "Invalid Ticket.", reload = true });
            }

            if (eventReg.CheckedIn)
            {
                return Json(new { success = false, message = "Ticket already checked in.", reload = true });
            }

            // Mark the ticket as checked in
            eventReg.CheckedIn = true;
            _context.Entry(eventReg).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();

            // Return user information to display in a table
            var userInfo = new
            {
                FullName = eventReg.FullName,
                ContactNumber = eventReg.ContactNumber,
                Email = eventReg.Email,
                EventName = eventReg.Event.EventName,
                EventDate = eventReg.Event.EventDate.ToString("dd MMMM yyyy"),
                EventTime = eventReg.Event.EventTime.ToString(@"hh\:mm"),
                EventLocation = eventReg.Event.EventLocation,
                TicketNumber = eventReg.TicketNumber
            };

            return Json(new { success = true, message = "Check-in successful!", userInfo });
        }

    }
}
