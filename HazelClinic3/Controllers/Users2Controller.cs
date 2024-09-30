using HazelClinic3.Models;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace HazelClinic3.Controllers
{
    public class Users2Controller : Controller
    {
        private DataContext db = new DataContext();
        private readonly DataContext db1 = new DataContext();
        public ActionResult Index()
        {
            return View(db.Users2.ToList());
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["Login"] = false;
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult Register(User1 usr)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users2.FirstOrDefault(u => u.Email == usr.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email already exists, Please proceed to Login");
                    return View(usr);
                }

                Session["ID"] = usr.IDnum;
                Session["GlobalEmail"] = usr.Email;
                Session["PetName"] = usr.Username;
                db.Users2.Add(usr);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "Some Error Occurred!");
            }
            return View(usr);
        }



        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User1 log)
        {

            const string adminUsername = "admin";
            const string adminPassword = "adminpassword";

            const string inspector1Username = "inspector1";
            const string inspector1Password = "password1";
            const string inspector2Username = "inspector2";
            const string inspector2Password = "password2";
            const string inspector3Username = "inspector3";
            const string inspector3Password = "password3";


            var user = db.Users2.FirstOrDefault(x => x.Username == log.Username &&
                                                      x.Password == log.Password &&
                                                      x.Email == log.Email
                                                     );

            const string driverUsername = "driver";
            const string driverPassword = "driverpassword";

            if (log.Username == driverUsername && log.Password == driverPassword)
            {
                return RedirectToAction("DriverDashboard", "Driver");
            }


            if (user != null)
            {
                Session["ID"] = user.IDnum;
                Session["PetName"] = user.Username;
                Session["GlobalEmail"] = user.Email;
                Session["Login"] = true;
                return RedirectToAction("LoggedIn");
            }

            if (log.Username == inspector1Username && log.Password == inspector1Password)
            {
                return RedirectToAction("InspectorAdoptions", "PetAdoption", new { inspectorId = 1 });
            }
            else if (log.Username == inspector2Username && log.Password == inspector2Password)
            {
                return RedirectToAction("InspectorAdoptions", "PetAdoption", new { inspectorId = 2 });
            }
            else if (log.Username == inspector3Username && log.Password == inspector3Password)
            {
                return RedirectToAction("InspectorAdoptions", "PetAdoption", new { inspectorId = 3 });
            }


            if (log.Username == adminUsername && log.Password == adminPassword)
            {

                Session["Login"] = true;
                return RedirectToAction("AdminLog");
            }


            ModelState.AddModelError("", "Invalid username, password, email, or ID number.");
            return View();
        }



        public ActionResult AdminLog()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            var user = db.Users2.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Username,Password,Firstname,Lastname,Email,Mobile")] User1 user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(int id)
        {
            var user = db.Users2.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users2.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            db.Users2.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(User1 user)
        {
            var existingUser = db.Users2.FirstOrDefault(u =>
                u.Email == user.Email &&
                u.Mobile == user.Mobile &&
                u.Firstname == user.Firstname &&
                u.Lastname == user.Lastname &&
                u.Username == user.Username
            );

            if (existingUser != null)
            {
                return RedirectToAction("ResetPassword", new { userId = existingUser.UserId });
            }
            else
            {
                ModelState.AddModelError("", "User not found. Please check your details.");
            }

            return View(user);
        }

        public ActionResult ResetPassword(int userId)
        {
            var user = db.Users2.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(User1 user)
        {
            var existingUser = db.Users2.Find(user.UserId);
            if (existingUser == null)
            {
                return HttpNotFound();
            }


            existingUser.Password = user.Password;

            db.Entry(existingUser).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("Login");
        }


        public ActionResult MyProfile()
        {
            if (Session["Login"] != null && (bool)Session["Login"] == true)
            {
                string email = Session["GlobalEmail"].ToString();

                // Fetch the user's adoptions, appointments, and bookings
                var userAdoptions = db.Adoptions.Where(a => a.AdopterEmail == email).ToList();
                var userAppointments = db.AppTbl.Where(a => a.Email == email).ToList();
                var userBookings = db.Bookings.Where(b => b.Email == email).ToList();

                // Fetch the user's volunteers
                var userVolunteers = db.Volunteers.Where(v => v.Email == email).ToList(); // Assuming Volunteer has an Email property

                // Populate the view model with the fetched data
                var model = new UserProfileViewModel
                {
                    Adoptions = userAdoptions,
                    Appointments = userAppointments,
                    Bookings = userBookings,
                    Volunteers = userVolunteers // Assign the fetched volunteer data to the model
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        [HttpGet]
        public ActionResult AddPet()
        {
            if (Session["Login"] != null && (bool)Session["Login"] == true)
            {
                var pet = new HazelClinic3.Models.User1();
                pet.Email = Session["GlobalEmail"] as string;  // Assign email from session to the pet model
                pet.IDnum = Session["ID"] as string;          // Assuming ID is also stored in the session

                return View(pet);  // Pass the populated pet model to the view
            }
            else
            {
                return RedirectToAction("Login");  // Redirect to login if session is not set
            }
        }
        
        [HttpPost]
        public ActionResult AddPet(HazelClinic3.Models.User1 pet)
        {
            if (ModelState.IsValid)
            {
                // Assuming you have a way to differentiate pets in your model or database
                string email = Session["GlobalEmail"].ToString();

                // Adjust according to your actual DB context and model
                // Make sure to add to the correct DbSet if different from Users2
                db.Users2.Add(pet);
                db.SaveChanges();

                // Redirect to the profile page or another appropriate page after successful addition
                return RedirectToAction("MyProfile");
            }
            else
            {
                // Add model error if needed, might customize the error message based on the context
                ModelState.AddModelError("", "Some Error Occurred!");
            }

            // Return the AddPet view when there are validation errors
            return View("AddPet", pet);  // Pointing back to AddPet view instead of Register
        }

       public ActionResult LoggedIn()
{
    System.Diagnostics.Debug.WriteLine($"Login: {Session["Login"]}, GlobalEmail: {Session["GlobalEmail"]}, PetName: {Session["PetName"]}");

    if (Session["Login"] != null && (bool)Session["Login"] == true)
    {
        string email = Session["GlobalEmail"]?.ToString();
        string username = Session["PetName"] as string;

        // Fetch all records with the same email
        var pets = db.Users2.Where(u => u.Email == email).ToList();

        if (pets != null && pets.Count > 0)
        {
            System.Diagnostics.Debug.WriteLine($"Records found: {pets.Count}, Email={email}");

            var model = new DashboardViewModel
            {
                Pets = pets, // All matching users are considered pets
                PetName = username,
                Email = email
            };

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(model);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("No records match the session details.");
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
    else
    {
        return RedirectToAction("Login");
    }
}




        [HttpPost]
        public ActionResult SwitchUser(string selectedUsername, string selectedEmail)
        {
            var sessionEmail = Session["GlobalEmail"] as string;
            System.Diagnostics.Debug.WriteLine($"Current session email: {Session["GlobalEmail"]}");
            var user = db.Users2.FirstOrDefault(u => u.Username == selectedUsername );
            //var pets = db.Users2.Where(u => u.Email == sessionEmail).ToList();
            if (user != null)
            {
                System.Diagnostics.Debug.WriteLine($"Switching to User ID: {user.IDnum}, Username: {user.Username}, Email: {user.Email}");
               
                
                    Session["ID"] = user.IDnum;
                    Session["GlobalEmail"] = user.Email;
                    Session["PetName"] = user.Username;
                    Session["Login"] = true;

                
                // Update session information
               
                System.Diagnostics.Debug.WriteLine($"New session state: ID={Session["ID"]}, Email={Session["GlobalEmail"]}, PetName={Session["PetName"]}");

                return Json(new { success = true, message = "User context switched successfully." });
            }

            return Json(new { success = false, message = "No user found matching the username and email." });
        }







    }
















}





