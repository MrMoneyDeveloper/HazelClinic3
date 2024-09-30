using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using HazelClinic3.Models;

public class AdminController : Controller
{
    private DataContext db = new DataContext();

    public ActionResult ViewBookings()
    {
        List<Booking> bookings = db.Bookings.ToList();
        return View(bookings);
    }

    public ActionResult Delete(int id)
    {
        Booking booking = db.Bookings.Find(id);

        if (booking != null)
        {
            return View(booking);
        }

        return HttpNotFound();
    }

    [HttpPost]
    public ActionResult DeleteConfirmed(int id)
    {
        Booking booking = db.Bookings.Find(id);

        if (booking != null)
        {
            db.Bookings.Remove(booking);
            db.SaveChanges();
        }

        return RedirectToAction("ViewBookings");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
    public ActionResult Edit(int id)
    {
        Booking booking = db.Bookings.Find(id);

        if (booking != null)
        {
            return View(booking);
        }

        return HttpNotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Booking updatedBooking)
    {
        if (ModelState.IsValid)
        {
            
            Booking originalBooking = db.Bookings.Find(updatedBooking.BookingId);

            if (originalBooking != null)
            {
               
                originalBooking.Fname = updatedBooking.Fname;
                originalBooking.Lname = updatedBooking.Lname;
                originalBooking.Phone = updatedBooking.Phone;
                originalBooking.Email = updatedBooking.Email;
                originalBooking.Address = updatedBooking.Address;
                originalBooking.CityPostalCode = updatedBooking.CityPostalCode;
                originalBooking.Pname = updatedBooking.Pname;
                originalBooking.SelectedSpecies = updatedBooking.SelectedSpecies;
                originalBooking.Gender = updatedBooking.Gender;
                originalBooking.BreedColor = updatedBooking.BreedColor;
                originalBooking.Age = updatedBooking.Age;
                originalBooking.Weight = updatedBooking.Weight;
               

                
                int originalDays = (originalBooking.EndDate - originalBooking.StartDate).Days;

                
                originalBooking.StartDate = updatedBooking.StartDate;

                
                originalBooking.EndDate = updatedBooking.StartDate.AddDays(originalDays);

               
                db.SaveChanges();

                return RedirectToAction("ViewBookings");
            }
            else
            {
                return HttpNotFound();
            }
        }

        return View(updatedBooking);
    }

    public ActionResult SurveyResults()
    {
        // Fetch all survey data from the database
        List<SurveyModel> surveyResults = db.SurveyModels.ToList();

        // Pass the survey data to the view
        return View(surveyResults);
    }
}