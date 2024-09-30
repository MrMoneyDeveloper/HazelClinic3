using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AnimalName { get; set; }

        [Required(ErrorMessage = "Animal Type is required")]
        [Display(Name = "Animal Type:")]
        public string AnimalType { get; set; }

        [Required(ErrorMessage = "Please select an appointment type")]
        [Display(Name = "Appointment Type:")]
        public string AppointmentType { get; set; }

        [Required(ErrorMessage = "Pet species is required")]
        [Display(Name = "Pet Species:")]
        [StringLength(10, ErrorMessage = "Not an accepted species")]
        public string PetSpecies { get; set; }


        [Required(ErrorMessage = "A selection is required for service")]
        [Display(Name = "Services:")]
        public string ConsultationType { get; set; }

        public bool Corona { get; set; }
        public bool DPV { get; set; }
        public bool Rabies { get; set; }

        public bool Clostridial { get; set; }
        public bool Leptospirosis { get; set; }
        public bool Brucellosis { get; set; }

        [Required(ErrorMessage = "Select appointment date")]
        [Display(Name = "Select Appointment date:")]
        [DataType(DataType.Date)]
        public DateTime AppoinmentDate { get; set; }

        [Required(ErrorMessage = "Select appointment time")]
        [Display(Name = "Select Appointment Time:")]
        [DataType(DataType.Time)]
        public DateTime AppointmentTime { get; set; }

        [Display(Name = "Vaccine Cost")]
        public double VaccineCost { get; set; }


        [Display(Name = "Consultation Cost")]
        public double ConsultationCost { get; set; }

        [Display(Name = "Appointment Type Cost")]
        public double AppType { get; set; }

        [Display(Name = "Total Fee")]
        public double TotalFee { get; set; }

        [Display(Name = "ID Number:")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "ID number must be exactly 13 digits.")]
        public string IdNumber { get; set; }


        [Display(Name = "Promo Code")]
        public string PromoCode { get; set; }




        public double CalcBasicAppType()
        {
            double AppType = 0.0;

            if (AppointmentType == "House Call")
            {
                AppType += 300;
            }

            else if (AppointmentType == "Clinic Visit")
            {
                AppType = 0.0;
            }

            return (AppType);
        }


        public double CalcVaccineCost()
        {
            double vaccineCost = 0.0;

            // Add logic here to calculate vaccine costs based on the selected "Animal Type"
            if (AnimalType == "Domestic Animal" || AnimalType == "Farm Animal")
            {
                if (Rabies == true)
                {
                    vaccineCost += 75;
                }

                if (DPV == true)
                {
                    vaccineCost += 50;
                }

                if (Corona == true)
                {
                    vaccineCost += 100;
                }


                if (Clostridial == true)
                {
                    vaccineCost += 50;
                }

                if (Leptospirosis == true)
                {
                    vaccineCost += 70;
                }

                if (Brucellosis == true)
                {
                    vaccineCost += 80;
                }

            }

            return vaccineCost;
        }



        public double CalcConsultationCost()
        {
            double consultationCost = 0.0;

            if (ConsultationType == "Consult Only")
            {
                consultationCost = 350;
            }

            else if (ConsultationType == "Vaccination and Consult")
            {
                consultationCost = 500;
            }

            return (consultationCost);
        }

        public double CalcTotalFee()
        {

            TotalFee = (CalcBasicAppType() + CalcVaccineCost() + CalcConsultationCost());
            return TotalFee;
        }

        public double CalcTotalFeePromo()
        {

            TotalFee = (CalcBasicAppType() + CalcVaccineCost() + CalcConsultationCost());
            TotalFee = TotalFee * 0.9;
            return TotalFee;
        }
    }
}