using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HazelClinic3.Models;


namespace HazelClinic3.Controllers
{
    public class PetSittingsController : Controller
    {
        private DataContext db = new DataContext();


        public ActionResult Index()
        {
            return View(db.PetSitting.ToList());
        }

      
        public ActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SittingId,FullName,CellNo,EmergencyContactName,EmergencyContactCellNo,ResAddress,PetName,PetType,StartDate,EndDate,SpecialRequests")] PetSitting petSitting)
        {
            // Check if the CellNo and EmergencyContactCellNo are the same
            if (string.Equals(petSitting.CellNo, petSitting.EmergencyContactCellNo, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("CellNo", "CellNo and EmergencyContactCellNo cannot be the same.");
                ModelState.AddModelError("EmergencyContactCellNo", "CellNo and EmergencyContactCellNo cannot be the same.");
            }

            // Check if the FullName and EmergencyContactName are the same
            if (string.Equals(petSitting.FullName, petSitting.EmergencyContactName, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("FullName", "FullName and EmergencyContactName cannot be the same.");
                ModelState.AddModelError("EmergencyContactName", "FullName and EmergencyContactName cannot be the same.");
            }

            if (ModelState.IsValid)
            {
                db.PetSitting.Add(petSitting);
                db.SaveChanges();


                return RedirectToAction("PetSittingSuccess", new { id = petSitting.SittingId });
            }

            return View(petSitting);
        }


        public ActionResult PetSittingInfo()
        {
            return View("PetSittingInfo");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PetSitting petSitting = db.PetSitting.Find(id);

            if (petSitting == null)
            {
                return HttpNotFound();
            }

            return View(petSitting);
        }

        // POST: PetSittings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SittingId,FullName,CellNo,EmergencyContactName,EmergencyContactCellNo,ResAddress,PetName,PetType,StartDate,EndDate,SpecialRequests")] PetSitting petSitting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(petSitting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(petSitting);
        }

     
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PetSitting petSitting = db.PetSitting.Find(id);

            if (petSitting == null)
            {
                return HttpNotFound();
            }

            return View(petSitting);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PetSitting petSitting = db.PetSitting.Find(id);

            if (petSitting == null)
            {
                return HttpNotFound();
            }

            db.PetSitting.Remove(petSitting);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private PetSitting GetPetSittingById(int id)
        {
            return db.PetSitting.Find(id);
        }

        public ActionResult PetSittingSuccess(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            PetSitting petSitting = GetPetSittingById(id);

            if (petSitting == null)
            {
                return HttpNotFound();
            }


            return View(petSitting);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

 