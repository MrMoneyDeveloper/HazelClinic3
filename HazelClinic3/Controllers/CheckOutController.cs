using System.Data.Entity;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web.Mvc;
using HazelClinic3.Models;

namespace HazelClinic3.Controllers
{
    public class CheckOutController : Controller
    {
        private DataContext db = new DataContext();

        public ActionResult CheckOut(string kennelId)
        {
            
            ViewBag.KennelId = kennelId;
            return View();
        }

        [HttpPost]
        public ActionResult CheckOut(Checkout checkout)
        {
            if (ModelState.IsValid)
            {
                var booking = db.Bookings.FirstOrDefault(b => b.BookingId == checkout.BookingId);
                if (booking == null)
                {
                    ModelState.AddModelError("BookingId", "Booking ID does not exist.");
                    return View(checkout);
                }

                var checkIn = db.Checks.FirstOrDefault(c => c.BookId == checkout.BookingId);
                if (checkIn == null)
                {
                    ModelState.AddModelError("BookingId", "Booking ID is not checked in.");
                    return View(checkout);
                }

                // Additional check: Ensure the kennel ID exists in the CheckIn table
                var kennelCheckIn = db.Checks.FirstOrDefault(c => c.KennelId == checkIn.KennelId);
                if (kennelCheckIn == null)
                {
                    ModelState.AddModelError("KennelId", "Kennel ID is not checked in.");
                    return View(checkout);
                }

                // If all checks pass, proceed with the checkout process
                db.Checks.Remove(checkIn);


                db.Checkout.Add(checkout);
                db.SaveChanges();

                return RedirectToAction("CheckOutSuccess");
            }

            // If the model state is not valid, return the view with validation errors
            return View(checkout);
        }


        public ActionResult ViewCheckOut()
        {
           
            var checkouts = db.Checkout.ToList();

            return View("ViewCheckOut", checkouts);
        }
        public ActionResult CheckOutSuccess()
        {
            

            return View();
        }
        public ActionResult Edit(int id)
        {
            var checkout = db.Checkout.Find(id);
            if (checkout == null)
            {
                return HttpNotFound();
            }
            return View(checkout);
        }

        [HttpPost]
        public ActionResult Edit(Checkout checkout)
        {
            if (ModelState.IsValid)
            {
                db.Entry(checkout).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewCheckOut");
            }
            return View(checkout);
        }
        public ActionResult Delete(int id)
        {
            Checkout checkout = db.Checkout.Find(id);

            if (checkout != null)
            {
                return View(checkout);
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            Checkout checkout = db.Checkout.Find(id);

            if (checkout != null)
            {
                db.Checkout.Remove(checkout);
                db.SaveChanges();
            }

            return RedirectToAction("ViewCheckOut");
        }
    }
}



