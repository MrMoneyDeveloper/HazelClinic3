using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.ModelBinding;

namespace HazelClinic3.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public string Fname { get; set; }

        public string Lname { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }


        [Display(Name = "Address")]
        public string Address { get; set; }


        [Display(Name = "City/Postal Code")]
        public string CityPostalCode { get; set; }

        public string Pname { get; set; }

        [Required(ErrorMessage = "Please Enter Your Pet Species")]
        [Display(Name = "Pet Species")]
        public string SelectedSpecies { get; set; }

        [Required(ErrorMessage = "Please Enter Your Pet Gender")]
        [Display(Name = "Pet Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please Enter Your Pet Breed or Color")]
        [Display(Name = "Pet Breed/Color")]
        public string BreedColor { get; set; }

        [Required(ErrorMessage = "Please Enter Your Pet Age")]
        [Display(Name = "Pet Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Please Enter Your Pet Weight")]
        [Display(Name = "Pet Weight")]
        public int Weight { get; set; }



        [Required(ErrorMessage = "Please Select a Start Date")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please Select an End Date")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Promo Code")]
        public string PromoCode { get; set; }


        public double TotalCost { get; set; }

        public void CalculateTotalCost()
        {
            double totalCostPerDay = 0;

            if (Weight >= 2 && Weight <= 5)
                totalCostPerDay = 100;
            else if (Weight > 5 && Weight <= 10)
                totalCostPerDay = 100;
            else if (Weight > 10 && Weight <= 20)
                totalCostPerDay = 120;
            else if (Weight > 20 && Weight <= 40)
                totalCostPerDay = 150;
            else if (Weight > 40)
                totalCostPerDay = 170;

            int totalDays = (EndDate - StartDate).Days;

            TotalCost = totalDays * totalCostPerDay;
        }
        public void CalculateTotalCostPromo()
        {
            double totalCostPerDay = 0;

            if (Weight >= 2 && Weight <= 5)
                totalCostPerDay = 100;
            else if (Weight > 5 && Weight <= 10)
                totalCostPerDay = 100;
            else if (Weight > 10 && Weight <= 20)
                totalCostPerDay = 120;
            else if (Weight > 20 && Weight <= 40)
                totalCostPerDay = 150;
            else if (Weight > 40)
                totalCostPerDay = 170;

            int totalDays = (EndDate - StartDate).Days;

            TotalCost = totalDays * totalCostPerDay;
            TotalCost = TotalCost * 0.9;
        }

    }
}