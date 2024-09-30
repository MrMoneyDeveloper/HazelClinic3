using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HazelClinic3.Controllers;
using HazelClinic3.Models;

namespace HazelClinic3.Models
{
    public class DataContext : DbContext
    {

        public DataContext() : base("con") { }
        public DbSet<User1> Users2 { get; set; }
        public DbSet<EventReg> EventRegs { get; set; }
        public DbSet<AdoptionRequest> Adoptions { get; set; }
        public DbSet<Appointment> AppTbl { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Check> Checks { get; set; }
        public DbSet<Checkout> Checkout { get; set; }
        public DbSet<RsvpModel> RsvpModels { get; set; }
        public DbSet<Grooming> Groomings { get; set; }
        public virtual DbSet<PetSitting> PetSitting { get; set; }
        public DbSet<History> Histories { get; set; }

        public DbSet<ApprovedAdoptions> ApprovedAdoptions { get; set; }
        public DbSet<DeclinedAdoptions> DeclinedAdoptions { get; set; }

        public DbSet<OngoingDriver> OngoingDelivery { get; set; }
        public DbSet<CompDriver> CompletedDelivery { get; set; }
        public DbSet<Failed> FailedDelivery { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ReturnPolicy> ReturnPolicies { get; set; }
        public System.Data.Entity.DbSet<HazelClinic3.Models.User1> GetUsers() { return Users2; }

        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<DeclinedVolunteer> DeclinedVolunteers { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventDocument> EventDocuments { get; set; }

        public DbSet<AuctionItem> AuctionItems { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<SurveyModel> SurveyModels { get; set; }

        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuctionItem>()
                .HasRequired(ai => ai.Event)
                .WithMany(e => e.AuctionItems)
                .HasForeignKey(ai => ai.Event_Id)  // Match the property name in AuctionItem
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EventDocument>()
                .HasRequired(ed => ed.Event)
                .WithMany(e => e.EventDocuments)
                .HasForeignKey(ed => ed.Event_Id)  // Match the property name in EventDocument
                .WillCascadeOnDelete(false);

            // Defining relationships between Location and User1
            modelBuilder.Entity<Location>()
                .HasRequired(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }


    }
}
