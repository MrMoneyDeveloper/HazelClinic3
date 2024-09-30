using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HazelClinic3.Models;

namespace Groomingpage.Controllers
{
    public class GroomingsController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Groomings
        public ActionResult Index()
        {
            return View(db.Groomings.ToList());
        }

        // GET: Groomings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grooming grooming = db.Groomings.Find(id);
            if (grooming == null)
            {
                return HttpNotFound();
            }
            return View(grooming);
        }

        // GET: Groomings/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Groomings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroomingId,Name,Address,City,PostalCode,PhoneNumber,Email,PetName,PetType,Breed,HairLength,MedicalIssues,GroomingAppoinmentDate,GroomingAppointmentTime")] Grooming grooming)
        {
            if (ModelState.IsValid)
            {
                db.Groomings.Add(grooming);
                db.SaveChanges();
                return RedirectToAction("GroomingSuccess");

            }


            return View(grooming);
        }

        // GET: Groomings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grooming grooming = db.Groomings.Find(id);
            if (grooming == null)
            {
                return HttpNotFound();
            }
            return View(grooming);
        }

        // POST: Groomings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroomingId,Name,Address,City,PostalCode,PhoneNumber,Email,PetName,PetType,Breed,HairLength,MedicalIssues,GroomingAppoinmentDate")] Grooming grooming)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grooming).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(grooming);
        }

        // GET: Groomings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grooming grooming = db.Groomings.Find(id);
            if (grooming == null)
            {
                return HttpNotFound();
            }
            return View(grooming);
        }

        // POST: Groomings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grooming grooming = db.Groomings.Find(id);
            db.Groomings.Remove(grooming);
            db.SaveChanges();
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

        public ActionResult GroomingSuccess()
        {
            // Retrieve the appointment details from TempData and pass them to the view


            // Pass the appointment details to the view
            return View();
        }

        public ActionResult Landing()
        {
            // Retrieve the appointment details from TempData and pass them to the view


            // Pass the appointment details to the view
            return View();
        }




    }
}
