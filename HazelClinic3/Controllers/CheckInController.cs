using System.Web.Mvc;
using HazelClinic3.Models;
using System.Data.Entity;
using System.Linq;

namespace HazelClinic3.Controllers
{
    public class CheckInController : Controller
    {
        private DataContext db = new DataContext();

        public ActionResult CheckIn(string kennelId)
        {
            
            ViewBag.KennelId = kennelId;
            return View();
        }


        [HttpPost]
        public ActionResult CheckIn(Check check)
        {
            if (ModelState.IsValid)
            {
                
                var booking = db.Bookings.FirstOrDefault(b => b.BookingId == check.BookId);
                if (booking == null)
                {
                    ModelState.AddModelError("BookId", "Booking ID does not exist.");
                    return View(check);
                }

                
                var existingCheck = db.Checks.Any(c => c.BookId == check.BookId);
                if (existingCheck)
                {
                    ModelState.AddModelError("BookId", "Booking ID already checked in.");
                    return View(check);
                }

                
                db.Checks.Add(check);
                db.SaveChanges();

                return RedirectToAction("CheckInSuccess");
            }

            
            return View(check);
        }
        public ActionResult ViewCheckIn()
        {
            
            var checkins = db.Checks.ToList();

            return View("ViewCheckIn", checkins);
        }
        public ActionResult CheckInSuccess()
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
            var check = db.Checks.Find(id);
            if (check == null)
            {
                return HttpNotFound(); 
            }
            return View(check);
        }

        [HttpPost]
        public ActionResult Edit(Check check)
        {
            if (ModelState.IsValid)
            {
                db.Entry(check).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewCheckIn");
            }
            return View(check);
        }


        public ActionResult Delete(int id)
        {
            Check check = db.Checks.Find(id);

            if (check != null)
            {
                return View(check);
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            Check check = db.Checks.Find(id);

            if (check != null)
            {
                db.Checks.Remove(check);
                db.SaveChanges();
            }

            return RedirectToAction("ViewCheckIn");
        }

    }
}

 