using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HazelClinic3.Models
{
    public class Volunteer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VolunteerId { get; set; }

        
        [Display(Name = "Enter your full name:")]
        public string FullName { get; set; }

     
        [Display(Name = "Enter your surname:")]
        public string Surname { get; set; }


        [Display(Name = "Enter your email address:")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

 
        [Display(Name = "Enter your cellphone number:")]
        [MinLength(10)]
        [StringLength(10)]
        public string CellNo { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter emergency contact's full name:")]
        public string EmergencyContactName { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter emergency contact's cellphone number:")]
        [MinLength(10)]
        [StringLength(10)]
        [NotSameAsCellNo(ErrorMessage = "Emergency contact number cannot be the same as your cell number.")]
        public string EmergencyContactCellNo { get; set; }

        [Display(Name = "Enter any relevant experience or skills:")]
        public string Experience { get; set; }

        [Required(ErrorMessage = "Select availability")]
        [Display(Name = "Select your availability:")]
        public string Availability { get; set; }

        [Required(ErrorMessage = "Select volunteer type")]
        [Display(Name = "Select the type of volunteer work you are interested in:")]
        public string VolunteerType { get; set; }

        public string Status { get; set; }

        // Additional properties from Model 1
        public DateTime? AssignedDay { get; set; }
        public DateTime? SecondAssignedDay { get; set; }
        public string OrientationDate { get; set; }

        [Display(Name = "Suitability")]
        public string Suitability { get; set; }

        public string Question1Answer { get; set; }
        public string Question2Answer { get; set; }
        public string Question3Answer { get; set; }
        public string Question4Answer { get; set; }
        public string Question5Answer { get; set; }
        public string Question6Answer { get; set; }
        public string Question7Answer { get; set; }
        public string Question8Answer { get; set; }
        public string Question9Answer { get; set; }
        public string Question10Answer { get; set; }

        public string PuppyTrainingQuestion1Answer { get; set; }
        public string PuppyTrainingQuestion2Answer { get; set; }
        public string PuppyTrainingQuestion3Answer { get; set; }
        public string PuppyTrainingQuestion4Answer { get; set; }
        public string PuppyTrainingQuestion5Answer { get; set; }

      
        public string CatCuddlingQuestion1Answer { get; set; }
        public string CatCuddlingQuestion2Answer { get; set; }
        public string CatCuddlingQuestion3Answer { get; set; }
        public string CatCuddlingQuestion4Answer { get; set; }
        public string CatCuddlingQuestion5Answer { get; set; }

        public void CalculateAndSetSuitability()
        {
            var correctAnswers = new Dictionary<string, string>
        {
            
            { nameof(PuppyTrainingQuestion1Answer), "a" },
            { nameof(PuppyTrainingQuestion2Answer), "b" },
            { nameof(PuppyTrainingQuestion3Answer), "a" },
            { nameof(PuppyTrainingQuestion4Answer), "b" },
            { nameof(PuppyTrainingQuestion5Answer), "b" },

            
            { nameof(CatCuddlingQuestion1Answer), "a" },
            { nameof(CatCuddlingQuestion2Answer), "a" },
            { nameof(CatCuddlingQuestion3Answer), "a" },
            { nameof(CatCuddlingQuestion4Answer), "a" },
            { nameof(CatCuddlingQuestion5Answer), "a" },

            { nameof(Question1Answer), "a" },
            { nameof(Question2Answer), "a" },
            { nameof(Question3Answer), "a" },
            { nameof(Question4Answer), "b" },
            { nameof(Question5Answer), "a" },
            { nameof(Question6Answer), "a" },
            { nameof(Question7Answer), "a" },
            { nameof(Question8Answer), "a" },
            { nameof(Question9Answer), "a" },
            { nameof(Question10Answer), "a" }
        };

            int score = 0;

            
            int totalQuestions = 10; 
            if (VolunteerType == "Puppy Training" || VolunteerType == "Cat Cuddling")
            {
                totalQuestions = 5;
            }

            foreach (var answer in correctAnswers)
            {
                var userAnswer = GetType().GetProperty(answer.Key)?.GetValue(this)?.ToString();
                if (userAnswer == answer.Value)
                {
                    score++;
                }
            }

           
            if (score == totalQuestions)
            {
                Suitability = "Very Suitable";
            }
            else if (score >= (totalQuestions * 0.7)) 
            {
                Suitability = "Suitable";
            }
            else
            {
                Suitability = "Not Suitable";
            }

        }


        public class NotSameAsCellNoAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var volunteer = (Volunteer)validationContext.ObjectInstance;

                if (value != null && value.ToString() == volunteer.CellNo)
                {
                    return new ValidationResult(ErrorMessage);
                }

                return ValidationResult.Success;
            }
        }
    }
}
