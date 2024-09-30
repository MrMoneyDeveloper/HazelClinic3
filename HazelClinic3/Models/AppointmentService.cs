using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HazelClinic3.Models;
using System.Web.Mvc;
using System.Globalization;
using System.Data.Entity;



namespace HazelClinic3.Models
{
    public class AppointmentService
    {


        private readonly DataContext db; // Make sure YourDbContext is your actual EF context class.

        public AppointmentService(DataContext context)
        {
            db = context;
        }

        public List<SelectListItem> GetAvailableTimes(DateTime date)
        {
            // Fetch all appointments on the given date
            var appointmentsOnDate = db.AppTbl
                .Where(a => a.AppointmentTime.Year == date.Year &&
                            a.AppointmentTime.Month == date.Month &&
                            a.AppointmentTime.Day == date.Day)
                .ToList(); // Pull relevant data into memory

            // Now filter by time in memory
            var bookedTimes = appointmentsOnDate
                .Select(a => new TimeSpan(a.AppointmentTime.Hour, a.AppointmentTime.Minute, a.AppointmentTime.Second))
                .ToList();

            var allTimes = new List<SelectListItem> {
        new SelectListItem { Text = "08:00 AM", Value = "08:00" },
         new SelectListItem { Text = "09:00 AM", Value = "09:00" },
        new SelectListItem { Text = "10:00 AM", Value = "10:00" },
        new SelectListItem { Text = "11:00 AM", Value = "11:00" },
        new SelectListItem { Text = "12:00 PM", Value = "12:00" },
        new SelectListItem { Text = "01:00 PM", Value = "13:00" },
        new SelectListItem { Text = "02:00 PM", Value = "14:00" },
        new SelectListItem { Text = "03:00 PM", Value = "15:00" },
        new SelectListItem { Text = "04:00 PM", Value = "16:00" },
        new SelectListItem { Text = "05:00 PM", Value = "17:00" }
        };

            // Convert string Value to TimeSpan and check availability
            return allTimes.Where(t => !bookedTimes.Contains(TimeSpan.ParseExact(t.Value, "hh\\:mm", CultureInfo.InvariantCulture))).ToList();
        }

        public DateTime? GetNextAvailableDate(DateTime start)
        {
            var date = start.AddDays(1); // Start checking from the next day

            // Limit the number of days to check to avoid infinite loops
            int maxDaysToCheck = 365; // For example, check up to one year in the future
            for (int i = 0; i < maxDaysToCheck; i++)
            {
                var timesAvailable = GetAvailableTimes(date);
                if (timesAvailable.Any())
                    return date;

                date = date.AddDays(1);
            }

            // Optionally handle the case where no available date is found within the year
            return null; // Or log this issue, or handle it according to your application's needs
        }













    }
}