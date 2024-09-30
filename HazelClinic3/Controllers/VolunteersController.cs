using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using HazelClinic3.Models;
using Newtonsoft.Json;



namespace HazelClinic3.Controllers
{
    public class VolunteersController : Controller
    {
        private const int MaxVolunteersPerType = 2; // Maximum number of volunteers allowed per type
        private readonly DataContext _context; // DataContext instance for database access
        private static readonly Random random = new Random();

        // Constructor to initialize the DataContext
        public VolunteersController()
        {
            _context = new DataContext(); // Initialize the DataContext
        }

        // GET: Volunteers
        public ActionResult VolunteerInfo()
        {
            return View("VolunteerInfo");
        }

        public ActionResult VolunteerSuccess()
        {
            var email = Session["VolunteerEmail"] as string;

            if (!string.IsNullOrEmpty(email))
            {
                SendConfirmationEmail(email);
                Session["VolunteerEmail"] = null;
            }

            return View();
        }

        public ActionResult ViewVolunteerRequests()
        {
            var volunteerRequests = _context.Volunteers.Where(v => v.Status == null).ToList();
            return View(volunteerRequests);
        }

        // GET: Volunteers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Volunteer volunteer = _context.Volunteers.Find(id);
            if (volunteer == null)
            {
                return HttpNotFound();
            }
            return View(volunteer);
        }

        
        public ActionResult Create()
        {
            InitializeSelectLists();
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VolunteerId,EmergencyContactName,EmergencyContactCellNo,Experience,Availability,VolunteerType,Question1Answer,Question2Answer,Question3Answer,Question4Answer,Question5Answer,Question6Answer,Question7Answer,Question8Answer,Question9Answer,Question10Answer,PuppyTrainingQuestion1Answer,PuppyTrainingQuestion2Answer,PuppyTrainingQuestion3Answer,PuppyTrainingQuestion4Answer,PuppyTrainingQuestion5Answer,CatCuddlingQuestion1Answer,CatCuddlingQuestion2Answer,CatCuddlingQuestion3Answer,CatCuddlingQuestion4Answer,CatCuddlingQuestion5Answer")] Volunteer volunteer)
        {
            string globalEmail = Session["GlobalEmail"] as string;
            if (string.IsNullOrEmpty(globalEmail))
            {
                ModelState.AddModelError("", "User is not logged in.");
                InitializeSelectLists();
                return View("Login");
            }

            var user = _context.Users2.FirstOrDefault(u => u.Email == globalEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                InitializeSelectLists();
                return View("Login");
            }

            var existingVolunteer = _context.Volunteers.FirstOrDefault(v => v.Email == volunteer.Email);
            if (existingVolunteer != null)
            {
                ModelState.AddModelError("Email", "This email is already registered as a volunteer.");
            }

            if (ModelState.IsValid)
            {
               
                volunteer.CalculateAndSetSuitability();
                volunteer.FullName = user.Firstname;
                volunteer.Surname = user.Lastname;
                volunteer.Email = user.Email;
                volunteer.CellNo = user.Mobile;

                
                _context.Volunteers.Add(volunteer);
                _context.SaveChanges();

                
                Session["VolunteerEmail"] = volunteer.Email;

                
                if (volunteer.Suitability == "Suitable" || volunteer.Suitability == "Very Suitable")
                {
                    Approve(volunteer.VolunteerId); 
                }

                return RedirectToAction("VolunteerSuccess");
            }

            InitializeSelectLists();
            return View(volunteer);
        }


        // Method to get the count of volunteers per type
        private Dictionary<string, int> GetVolunteerCounts()
        {
            return _context.Volunteers
                .GroupBy(v => v.VolunteerType)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Method to initialize SelectLists
        private void InitializeSelectLists()
        {
            var volunteerCounts = GetVolunteerCounts();
            var volunteerLimits = new Dictionary<string, int>
            {
                { "Dog Walking", MaxVolunteersPerType },
                { "Cat Cuddling", MaxVolunteersPerType },
                { "Puppy Training", MaxVolunteersPerType },
                { "Pet Grooming", MaxVolunteersPerType },
                { "No Preference", int.MaxValue } // No limit for "No Preference"
            };

            var availableVolunteerTypes = volunteerLimits
                .Where(kvp => !volunteerCounts.ContainsKey(kvp.Key) || volunteerCounts[kvp.Key] < kvp.Value)
                .Select(kvp => new SelectListItem { Text = kvp.Key, Value = kvp.Key })
                .ToList();

            ViewBag.Experience = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Experienced", Value = "Experienced" },
                new SelectListItem { Text = "Somewhat", Value = "Somewhat" },
                new SelectListItem { Text = "Little to none", Value = "Little to none" }
            }, "Value", "Text");

            ViewBag.Availability = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Weekdays", Value = "Weekdays" },
                new SelectListItem { Text = "Weekends", Value = "Weekends" },
                new SelectListItem { Text = "Both", Value = "Both" }
            }, "Value", "Text");

            ViewBag.VolunteerType = new SelectList(availableVolunteerTypes, "Value", "Text");
        }

        // Method to send confirmation email
        private void SendConfirmationEmail(string userEmail)
        {
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("spcadurbanza@gmail.com"),
                Subject = "Volunteer Registration Confirmation",
                Body = "Dear Volunteer,\n\nThank you for registering to volunteer with SPCA Durban.\n\nWe appreciate your support and will notify you pending the outcome of your request.\n\nWarm regards,\nSPCA Durban Team"
            };
            mail.To.Add(userEmail);

            SmtpClient smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda"),
                EnableSsl = true
            };

            smtp.Send(mail);
        }

        // GET: Volunteers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Volunteer volunteer = _context.Volunteers.Find(id);
            if (volunteer == null)
            {
                return HttpNotFound();
            }
            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VolunteerId,FullName,Surname,Email,CellNo,EmergencyContactName,EmergencyContactCellNo,Experience,Availability,VolunteerType")] Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(volunteer).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(volunteer);
        }

        // GET: Volunteers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Volunteer volunteer = _context.Volunteers.Find(id);
            if (volunteer == null)
            {
                return HttpNotFound();
            }
            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Volunteer volunteer = _context.Volunteers.Find(id);
            _context.Volunteers.Remove(volunteer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Approve(int id)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer != null)
            {
                volunteer.Status = "Approved";

                // Determine and save the orientation date
                DateTime orientationDate = DetermineOrientationDate(volunteer);

                // Send the approval email with the orientation date
                string subject = "Volunteer Application Approved";
                string body = $"Dear {volunteer.FullName},\n\nWe are pleased to inform you that your volunteer application has been approved.\n\nYour orientation is scheduled for {orientationDate:dddd, MMMM d, yyyy}.\n\n";

                if (!string.IsNullOrEmpty(volunteer.VolunteerType) && volunteer.VolunteerType != "No Preference")
                {
                    body += $"You have been allocated to {volunteer.VolunteerType}.\n\n";
                }

                body += "Thank you for your willingness to support the Durban SPCA.\n\nWarm regards,\nSPCA Durban Team";

                SendConfirmationEmail2(volunteer.Email, subject, body);
            }
            return RedirectToAction("ViewVolunteerRequests");
        }
        // Method to determine orientation date (placeholder)
        private DateTime DetermineOrientationDate(Volunteer volunteer)
        {
            DateTime today = DateTime.Today;
            DateTime orientationDate;

            if (volunteer.Availability == "Weekdays")
            {
                orientationDate = today.AddDays(2);
                while (orientationDate.DayOfWeek == DayOfWeek.Saturday || orientationDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    orientationDate = orientationDate.AddDays(1);
                }
            }
            else if (volunteer.Availability == "Weekends")
            {
                orientationDate = today.AddDays(2);
                while (orientationDate.DayOfWeek != DayOfWeek.Saturday && orientationDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    orientationDate = orientationDate.AddDays(1);
                }
            }
            else // Both
            {
                orientationDate = DateTime.Today;
            }

            // Save the orientation date to the database
            volunteer.OrientationDate = orientationDate.ToString("yyyy-MM-dd");
            _context.Entry(volunteer).State = EntityState.Modified;
            _context.SaveChanges();

            return orientationDate;
        }

        // Method to send confirmation email (overloaded version for approval)
        private void SendConfirmationEmail2(string userEmail, string subject, string body)
        {
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("spcadurbanza@gmail.com"),
                Subject = subject,
                Body = body
            };
            mail.To.Add(userEmail);

            SmtpClient smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda"),
                EnableSsl = true
            };

            smtp.Send(mail);
        }

        // GET: Volunteers/ApprovedRequests

        [HttpPost]
        public ActionResult AllocateVolunteer(int id, string newVolunteerType)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer != null && volunteer.VolunteerType == "No Preference")
            {
                var currentCount = GetVolunteerCountForType(newVolunteerType);
                if (currentCount >= MaxVolunteersPerType)
                {
                    TempData["ErrorMessage"] = $"The limit for {newVolunteerType} has been reached.";
                    return RedirectToAction("VolunteerTypeChart");
                }

                volunteer.VolunteerType = newVolunteerType;
                _context.SaveChanges();
                UpdateVolunteerTypeChart();
            }
            return RedirectToAction("VolunteerTypeChart");
        }

        private int GetVolunteerCountForType(string volunteerType)
        {
            var counts = GetVolunteerCounts();
            return counts.ContainsKey(volunteerType) ? counts[volunteerType] : 0;
        }

        private void UpdateVolunteerTypeChart()
        {
            var volunteerCounts = GetVolunteerCounts();
            var volunteerLimits = new Dictionary<string, int>
            {
                { "Cat Cuddling", 2 },
                { "Dog Walking", 2 },
                { "Puppy Training", 2 },
                { "Pet Grooming", 2 }
            };

            // Debugging information
            Console.WriteLine("Volunteer Limits: " + JsonConvert.SerializeObject(volunteerLimits));
            Console.WriteLine("Volunteer Counts: " + JsonConvert.SerializeObject(volunteerCounts));

            ViewBag.ChartLabels = JsonConvert.SerializeObject(volunteerCounts.Keys.ToArray());
            ViewBag.ChartCounts = JsonConvert.SerializeObject(volunteerCounts.Values.ToArray());
            ViewBag.VolunteerCounts = volunteerCounts; // Ensure this is correctly set
            ViewBag.VolunteerLimits = volunteerLimits;  // Ensure this is correctly set
            ViewBag.MaxVolunteersPerType = 2; // Example value
        }

        public ActionResult VolunteerTypeChart()
        {
            // Ensure that ViewBag.Volunteers is initialized
            var volunteers = _context.Volunteers.ToList();
            ViewBag.Volunteers = volunteers; // Set ViewBag.Volunteers

            UpdateVolunteerTypeChart(); // Update chart-related data

            return View();
        }


        public ActionResult ApprovedRequests()
        {
            var approvedVolunteers = _context.Volunteers.Where(v => v.Status == "Approved").ToList();

            foreach (var volunteer in approvedVolunteers)
            {
                if (!volunteer.AssignedDay.HasValue)
                {
                    volunteer.AssignedDay = DetermineAssignedDay(volunteer.Availability);
                }

                volunteer.SecondAssignedDay = DetermineSecondAssignedDay(volunteer);
            }

            _context.SaveChanges(); // Save changes to the database

            // Send orientation reminder
            SendOrientationReminder(approvedVolunteers);
            return View(approvedVolunteers);
        }



        private DateTime DetermineAssignedDay(string availability)
        {
            DateTime today = DateTime.Today;
            DateTime assignedDay;

            // Initialize a loop to find a valid date
            do
            {
                assignedDay = GetNextAvailableDate(availability, today);

                // If the selected date already has 3 occurrences, move to the next date for checking
                if (CountOccurrencesOfDate(assignedDay) >= 3)
                {
                    today = assignedDay.AddDays(1); // Move to the next date
                }

            } while (CountOccurrencesOfDate(assignedDay) >= 3); // Ensure no more than 3 occurrences

            return assignedDay;
        }

        private DateTime GetNextAvailableDate(string availability, DateTime today)
        {
            DateTime nextAvailableDate;

            if (availability == "Weekdays")
            {
                nextAvailableDate = today.AddDays(2); // Start checking from 2 days from now
                while (nextAvailableDate.DayOfWeek == DayOfWeek.Saturday || nextAvailableDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    nextAvailableDate = nextAvailableDate.AddDays(1);
                }
                return nextAvailableDate.Date.AddHours(10); // Set time to 10 AM
            }
            else if (availability == "Weekends")
            {
                nextAvailableDate = today.AddDays(2); // Start checking from 2 days from now
                while (nextAvailableDate.DayOfWeek != DayOfWeek.Saturday && nextAvailableDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    nextAvailableDate = nextAvailableDate.AddDays(1);
                }
                return nextAvailableDate.Date.AddHours(12); // Set time to 12 PM
            }
            else if (availability == "Both")
            {
                // Randomly choose between weekday and weekend
                bool isWeekday = random.Next(2) == 0;

                if (isWeekday)
                {
                    return GetNextAvailableDate("Weekdays", today);
                }
                else
                {
                    return GetNextAvailableDate("Weekends", today);
                }
            }

            throw new ArgumentException("Invalid availability value");
        }

        private int CountOccurrencesOfDate(DateTime date)
        {
            DateTime startOfDay = date.Date;
            DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1); // End of the day

            return _context.Volunteers.Count(v => v.AssignedDay.HasValue && v.AssignedDay.Value >= startOfDay && v.AssignedDay.Value <= endOfDay);
        }

        private DateTime? DetermineSecondAssignedDay(Volunteer volunteer)
        {
            if (!volunteer.AssignedDay.HasValue)
            {
                return null; // No assigned day means no second assigned day
            }

            DateTime assignedDay = volunteer.AssignedDay.Value;
            DateTime secondAssignedDay;

            // Handle each case based on availability
            switch (volunteer.Availability)
            {
                case "Weekdays":
                    secondAssignedDay = assignedDay.AddDays(7); // At least one week later
                    while (secondAssignedDay.DayOfWeek == DayOfWeek.Saturday || secondAssignedDay.DayOfWeek == DayOfWeek.Sunday ||
                           CountOccurrencesOfDate(secondAssignedDay) >= 3)
                    {
                        secondAssignedDay = secondAssignedDay.AddDays(1); // Find the next valid weekday
                    }
                    return secondAssignedDay.Date.AddHours(10); // Set time to 10 AM

                case "Weekends":
                    return null; // No second assigned day needed for weekends

                case "Both":
                    if (assignedDay.DayOfWeek == DayOfWeek.Saturday || assignedDay.DayOfWeek == DayOfWeek.Sunday)
                    {
                        return null; // No second assigned day if the first day is a weekend
                    }
                    else
                    {
                        // If assigned day is a weekday, follow the same logic as for "Weekdays"
                        return DetermineSecondAssignedDay(new Volunteer
                        {
                            AssignedDay = assignedDay,
                            Availability = "Weekdays"
                        });
                    }

                default:
                    throw new ArgumentException("Invalid availability value");
            }
        }

        private void SendOrientationReminder(List<Volunteer> volunteers)
        {
            DateTime today = DateTime.Today;

            foreach (var volunteer in volunteers)
            {
                if (!string.IsNullOrEmpty(volunteer.OrientationDate))
                {
                    // Parse OrientationDate to DateTime
                    if (DateTime.TryParse(volunteer.OrientationDate, out DateTime orientationDate))
                    {
                        // Check if orientationDate is today
                        if (orientationDate.Date == today)
                        {
                            // Call the email method without attachment
                            SendOrientationEmail(
                                volunteer.Email,
                                volunteer.FullName,
                                orientationDate
                            );
                        }
                    }
                }
            }
        }


        private void SendOrientationEmail(string userEmail, string fullName, DateTime orientationDate)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda"),
                EnableSsl = true,
            };

            MailMessage mailMessage = new MailMessage("spcadurbanza@gmail.com", userEmail)
            {
                Subject = "Orientation Video",
                IsBodyHtml = true,
            };

            string emailBodyWithLogo = $@"
    <html>
    <head>
    </head>
    <body>
        <h2>Durban & Coast SPCA Orientation Video</h2>
        <p>Dear {fullName},</p>
        <p>Your orientation is scheduled for {orientationDate.ToString("dddd, MMMM d, yyyy")}.</p>
        <p>We are pleased to welcome you to our team, please watch the virtual orientation for your duties at the Durban SPCA.</p>
        <p>Please scan the QR code below to access the video:</p>
        <img src=""https://i.ibb.co/51WVpPv/900ac618-9482-4f46-b588-bb2d1736f83d.png"" alt='SPCA Orientation' style='max-width:60%; height:60%;' />
        <p>Thank you for choosing the Durban SPCA.</p>
    </body>
    </html>";

            mailMessage.Body = emailBodyWithLogo;

            smtpClient.Send(mailMessage);
        }


        public ActionResult DeclinedRequests()
        {
            List<DeclinedVolunteer> declinedVolunteers = _context.DeclinedVolunteers.ToList(); // Assuming you have a list of DeclinedVolunteers in your database context
            return View(declinedVolunteers);
        }





        public ActionResult Decline(int id)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer != null)
            {
                // Create a new DeclinedVolunteer object
                var declinedVolunteer = new DeclinedVolunteer
                {
                    VolunteerId = volunteer.VolunteerId,
                    FullName = volunteer.FullName,
                    Surname = volunteer.Surname,
                    Email = volunteer.Email,
                    CellNo = volunteer.CellNo,
                    EmergencyContactName = volunteer.EmergencyContactName,
                    EmergencyContactCellNo = volunteer.EmergencyContactCellNo,
                    Experience = volunteer.Experience,
                    Availability = volunteer.Availability,
                    VolunteerType = volunteer.VolunteerType,
                    Status = "Declined"
                };

                // Add to DeclinedVolunteers table
                _context.DeclinedVolunteers.Add(declinedVolunteer);

                // Remove from Volunteers table
                _context.Volunteers.Remove(volunteer);

                // Save changes
                _context.SaveChanges();

                // Send decline email
                string subject = "Volunteer Application Declined";
                string body = $"Dear {volunteer.FullName},\n\n" +
                              "We regret to inform you that your volunteer application has been declined as the type of volunteer work you are interested is already full.\n\n" +
                              "Thank you for your interest in supporting SPCA Durban.\n\n" +
                              "Warm regards,\nSPCA Durban Team";

                SendConfirmationEmail2(volunteer.Email, subject, body);
            }
            return RedirectToAction("ViewVolunteerRequests");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}