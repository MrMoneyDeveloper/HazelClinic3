using HazelClinic3.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;

namespace HazelClinic3.Controllers
{
    public class LocationListController : Controller
    {
        private readonly DataContext _context = new DataContext();

        // Hardcoded locations with latitude and longitude for Google Maps
        private static List<Location> GetLocations()
        {
            return new List<Location>
            {
                new Location { Id = 1, Name = "Durban SPCA", Latitude = -29.8587, Longitude = 31.0218 },
                new Location { Id = 2, Name = "Kloof and Highway SPCA", Latitude = -29.8000, Longitude = 30.8500 },
                new Location { Id = 3, Name = "Amanzimtoti SPCA", Latitude = -30.0533, Longitude = 30.8917 },
                new Location { Id = 4, Name = "Phoenix Animal Care and Treatment (PACT)", Latitude = -29.7078, Longitude = 31.0374 },
                new Location { Id = 5, Name = "Umhlanga SPCA", Latitude = -29.7275, Longitude = 31.0650 },
                new Location { Id = 6, Name = "Queensburgh SPCA", Latitude = -29.8786, Longitude = 30.9208 }
            };
        }

        // GET: LocationList/SelectLocation
        public ActionResult SelectLocation()
        {
            var locations = GetLocations();
            return View(locations);  // Pass the list of locations to the view
        }

        // POST: LocationList/ConfirmLocation
        [HttpPost]
        public ActionResult ConfirmLocation(int selectedLocationId)
        {
            var selectedLocation = GetLocations().FirstOrDefault(l => l.Id == selectedLocationId);
            if (selectedLocation == null)
            {
                return RedirectToAction("SelectLocation");
            }

            var currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                // Initialize the newLocationEntry properly
                var newLocationEntry = new Location
                {
                    Name = selectedLocation.Name,
                    Latitude = selectedLocation.Latitude,
                    Longitude = selectedLocation.Longitude,
                    UserId = currentUser.UserId,
                    Email = currentUser.Email,
                    QRCode = GenerateQRCodeData(currentUser.Email, selectedLocation.Id)  // Generate QR data with email and location ID
                };

                _context.Locations.Add(newLocationEntry);
                _context.SaveChanges();  // Save the entry to the database

                var qrCodeBytes = GenerateQRCodeZXing(newLocationEntry.QRCode);  // Use ZXing to generate QR Code with the full QR data

                // Set session values
                Session["QRCodeBytes"] = qrCodeBytes;
                Session["SelectedLocation"] = selectedLocation.Name;
                Session["UserEmail"] = currentUser.Email;

                // Send email notifications
                SendDriverEmail(currentUser.Email, qrCodeBytes, selectedLocation);
                SendCustomerNotification(currentUser.Email, selectedLocation.Name);

                TempData["SuccessMessage"] = "Your location has been successfully selected. You will receive an email with details.";
                return RedirectToAction("OrderSummary");
            }

            return RedirectToAction("SelectLocation");
        }

        // New method to generate QR code data with email and location ID
        private string GenerateQRCodeData(string email, int locationId)
        {
            // We can combine the email and locationId as a simple string
            return $"{email}|{locationId}";
        }

        private User1 GetCurrentUser()
        {
            using (var context = new DataContext())
            {
                string email = Session["GlobalEmail"] as string;
                string idNum = Session["ID"] as string;
                return context.Users2.FirstOrDefault(u => u.Email == email && u.IDnum == idNum);
            }
        }

        // Generate QR code based on the QR data string
        private byte[] GenerateQRCodeZXing(string qrData)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 200,
                    Width = 200,
                    Margin = 0
                }
            };

            var pixelData = writer.Write(qrData);
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                     System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                                     System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();  // Return the QR code as byte array
                }
            }
        }

        public ActionResult DriverRoute(int locationId)
        {
            // Hardcoded list of locations
            var locations = new List<Location>
    {
        new Location { Id = 1, Name = "Durban SPCA", Latitude = -29.8587, Longitude = 31.0218 },
        new Location { Id = 2, Name = "Kloof and Highway SPCA", Latitude = -29.8000, Longitude = 30.8500 },
        new Location { Id = 3, Name = "Amanzimtoti SPCA", Latitude = -30.0533, Longitude = 30.8917 },
        new Location { Id = 4, Name = "Phoenix Animal Care and Treatment (PACT)", Latitude = -29.7078, Longitude = 31.0374 },
        new Location { Id = 5, Name = "Umhlanga SPCA", Latitude = -29.7275, Longitude = 31.0650 },
        new Location { Id = 6, Name = "Queensburgh SPCA", Latitude = -29.8786, Longitude = 30.9208 }
    };

            // Find the location from the hardcoded list
            var location = locations.FirstOrDefault(l => l.Id == locationId);

            if (location == null)
            {
                return HttpNotFound("Location not found.");
            }

            return View(location);
        }



        // Allow the user to download the QR code
        public ActionResult DownloadQRCode()
        {
            var qrCodeBytes = Session["QRCodeBytes"] as byte[];
            if (qrCodeBytes != null)
            {
                Session["IsQRCodeDownloaded"] = true;
                return File(qrCodeBytes, "image/png", "UserQRCode.png");
            }

            return RedirectToAction("OrderSummary");
        }

        public ActionResult OrderSummary()
        {
            // Check if QR code was generated, and selected location exists
            if (Session["QRCodeBytes"] != null && Session["SelectedLocation"] != null)
            {
                ViewBag.SelectedLocation = Session["SelectedLocation"];
                ViewBag.UserEmail = Session["UserEmail"];
                return View();
            }

            // Redirect back to location selection if QR code is not ready
            return RedirectToAction("SelectLocation");
        }

        // Send email to the driver with the QR code and route link
        private void SendDriverEmail(string customerEmail, byte[] qrCodeBytes, Location selectedLocation)
        {
            var subject = "New Delivery Assigned";
            var body = $"You have been assigned a new delivery. Please deliver the item to {selectedLocation.Name}. See attached QR code for validation. " +
                       $"You can also view the route by clicking on this link: " +
                       $"https://localhost:44353/LocationList/DriverRoute?locationId={selectedLocation.Id}";

            using (var message = new MailMessage(customerEmail, "farhaanhotd1@gmail.com"))
            {
                message.Subject = subject;
                message.Body = body;

                var qrAttachment = new Attachment(new MemoryStream(qrCodeBytes), "QRCode.png", "image/png");
                message.Attachments.Add(qrAttachment);

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(message);
                }
            }
        }



        // Email notifications to the customer
        private void SendCustomerNotification(string customerEmail, string locationName)
        {
            var subject = "Your pickup location has been confirmed!";
            var body = $"You have successfully selected {locationName} as your pickup location.";

            using (var message = new MailMessage("spcadurbanza@gmail.com", customerEmail))
            {
                message.Subject = subject;
                message.Body = body;

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
        public ActionResult ScanPackage()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ScanPackage(string qrCode)
        {
            var qrDataParts = qrCode.Split('|'); // Splitting the QR data into parts (email and locationId)
            if (qrDataParts.Length != 2 || !int.TryParse(qrDataParts[1], out int locationId))
            {
                return Json(new { success = false, message = "Invalid QR Code data." });
            }

            string email = qrDataParts[0]; // Extract the email outside the query

            // Now use the extracted email in the query
            var location = _context.Locations.FirstOrDefault(l => l.Id == locationId && l.Email == email);
            if (location == null)
            {
                return Json(new { success = false, message = "Invalid QR Code or location not found." });
            }

            var user = _context.Users2.FirstOrDefault(u => u.Email == email); // Fetch user details
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Handle driver scan (to confirm that the package is ready for pickup)
            if (!location.IsReadyForPickup)
            {
                // Mark the package as ready for pickup
                location.IsReadyForPickup = true;
                _context.SaveChanges();

                // Generate QR code for the customer
                var customerQRCodeData = GenerateQRCodeData(location.Email, location.Id);
                var customerQRCodeBytes = GenerateQRCodeZXing(customerQRCodeData); // Generate the QR code as a byte array

                // Send notification email to the customer with QR code for pickup
                SendCustomerPickupNotification(location.Email, location.Name, customerQRCodeBytes);

                return Json(new
                {
                    success = true,
                    message = "Driver confirmed. Package is now ready for pickup by the customer.",
                    userDetails = new
                    {
                        email = user.Email,
                        name = user.Firstname + " " + user.Lastname, // assuming there’s a FullName property in your user model
                        contactNumber = user.Mobile // assuming there’s a ContactNumber property in your user model
                    }
                });
            }

            // Handle customer scan (to confirm they are picking up the package)
            if (location.IsReadyForPickup && !location.IsPickedUp)
            {
                // Mark the package as picked up
                location.IsPickedUp = true;
                _context.SaveChanges();

                SendPickupConfirmation(location.Email);

                return Json(new
                {
                    success = true,
                    message = "Customer confirmed. Package has been picked up.",
                    userDetails = new
                    {
                        email = user.Email,
                        name = user.Firstname + " " + user.Lastname,
                        contactNumber = user.Mobile,
                    }
                });
            }

            // In case the package has already been picked up
            return Json(new { success = false, message = "This package has already been picked up." });
        }


        private void SendCustomerPickupNotification(string customerEmail, string locationName, byte[] qrCodeBytes)
        {
            var subject = "Your package is ready for pickup!";
            var body = $"Dear customer, your package is now ready for pickup at {locationName}. " +
                       "Please find attached your QR code to use when picking up the package.";

            using (var message = new MailMessage("spcadurbanza@gmail.com", customerEmail))
            {
                message.Subject = subject;
                message.Body = body;

                var qrAttachment = new Attachment(new MemoryStream(qrCodeBytes), "PickupQRCode.png", "image/png");
                message.Attachments.Add(qrAttachment);

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(message);
                }
            }
        }




        // Confirm pickup of the package
        [HttpPost]
        public ActionResult ConfirmPickup(string qrCode)
        {
            var qrDataParts = qrCode.Split('|'); // Split QR data to get email and locationId
            if (qrDataParts.Length != 2 || !int.TryParse(qrDataParts[1], out int locationId))
            {
                return Json(new { success = false, message = "Invalid QR Code format." });
            }

            var location = _context.Locations.FirstOrDefault(l => l.Id == locationId && l.Email == qrDataParts[0]);
            if (location == null || !location.IsReadyForPickup)
            {
                return Json(new { success = false, message = "Invalid or not ready for pickup." });
            }

            location.IsPickedUp = true;
            _context.SaveChanges();

            SendPickupConfirmation(location.Email);
            return Json(new { success = true, message = "Pickup confirmed!" });
        }

        // Send pickup confirmation to the customer
        private void SendPickupConfirmation(string customerEmail)
        {
            var subject = "Your item has been picked up!";
            var body = "Thank you for collecting your item. If you have any questions, feel free to contact us.";

            using (var message = new MailMessage("spcadurbanza@gmail.com", customerEmail))
            {
                message.Subject = subject;
                message.Body = body;

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(message);
                }
            }
        }
    }
}
