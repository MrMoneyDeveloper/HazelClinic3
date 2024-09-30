using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HazelClinic3.Controllers
{
    public class KennelController : Controller
    {
        // GET: Kennel
        public ActionResult CheckAvail()
        {
            return View();
        }

        public ActionResult CheckIn()
        {
            return View();
        }
    }
}