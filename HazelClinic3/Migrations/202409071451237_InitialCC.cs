namespace HazelClinic3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCC : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdoptionRequests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Adopterinterested = c.String(),
                        AdopterFName = c.String(),
                        AdopterNo = c.String(),
                        AdopterEmail = c.String(),
                        AppointDate = c.DateTime(nullable: false),
                        AppointTime = c.Time(nullable: false, precision: 7),
                        Address = c.String(),
                        AdopterMessage = c.String(),
                        InspectorId = c.Int(nullable: false),
                        Status = c.String(),
                        SafetyChecked = c.Boolean(nullable: false),
                        CleanlinessChecked = c.Boolean(nullable: false),
                        SpaceChecked = c.Boolean(nullable: false),
                        ProvisionsChecked = c.Boolean(nullable: false),
                        InteractiveFamilyChecked = c.Boolean(nullable: false),
                        FencedChecked = c.Boolean(nullable: false),
                        TrackingStatus = c.Int(nullable: false),
                        IsInspectionComplete = c.Boolean(nullable: false),
                        InspectionDate = c.DateTime(),
                        SubmittedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ApprovedAdoptions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Adopterinterested = c.String(),
                        AdopterFName = c.String(nullable: false),
                        AdopterNo = c.String(nullable: false),
                        AdopterEmail = c.String(nullable: false),
                        AdopterMessage = c.String(),
                        Address = c.String(nullable: false),
                        InspectorId = c.Int(nullable: false),
                        Status = c.String(),
                        SafetyChecked = c.Boolean(nullable: false),
                        CleanlinessChecked = c.Boolean(nullable: false),
                        SpaceChecked = c.Boolean(nullable: false),
                        ProvisionsChecked = c.Boolean(nullable: false),
                        InteractiveFamilyChecked = c.Boolean(nullable: false),
                        FencedChecked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        AppointmentId = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        AnimalName = c.String(),
                        AnimalType = c.String(nullable: false),
                        AppointmentType = c.String(nullable: false),
                        PetSpecies = c.String(nullable: false, maxLength: 10),
                        ConsultationType = c.String(nullable: false),
                        Corona = c.Boolean(nullable: false),
                        DPV = c.Boolean(nullable: false),
                        Rabies = c.Boolean(nullable: false),
                        Clostridial = c.Boolean(nullable: false),
                        Leptospirosis = c.Boolean(nullable: false),
                        Brucellosis = c.Boolean(nullable: false),
                        AppoinmentDate = c.DateTime(nullable: false),
                        AppointmentTime = c.DateTime(nullable: false),
                        VaccineCost = c.Double(nullable: false),
                        ConsultationCost = c.Double(nullable: false),
                        AppType = c.Double(nullable: false),
                        TotalFee = c.Double(nullable: false),
                        IdNumber = c.String(maxLength: 13),
                        PromoCode = c.String(),
                    })
                .PrimaryKey(t => t.AppointmentId);
            
            CreateTable(
                "dbo.AuctionItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Image = c.Binary(),
                        Event_Id = c.Int(nullable: false),
                        AuctionEndTime = c.DateTime(nullable: false),
                        IsEmailSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .Index(t => t.Event_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventName = c.String(nullable: false, maxLength: 100),
                        EventDate = c.DateTime(nullable: false),
                        EventTime = c.Time(nullable: false, precision: 7),
                        LimitOfAttendees = c.Int(nullable: false),
                        EventPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ArePetsAllowed = c.Boolean(nullable: false),
                        EventLocation = c.String(nullable: false, maxLength: 200),
                        Image = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        FileContent = c.Binary(),
                        Event_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .Index(t => t.Event_Id);
            
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuctionItemId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BidTime = c.DateTime(nullable: false),
                        UserEmail = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuctionItems", t => t.AuctionItemId, cascadeDelete: true)
                .Index(t => t.AuctionItemId);
            
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        Fname = c.String(),
                        Lname = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Address = c.String(),
                        CityPostalCode = c.String(),
                        Pname = c.String(),
                        SelectedSpecies = c.String(nullable: false),
                        Gender = c.String(nullable: false),
                        BreedColor = c.String(nullable: false),
                        Age = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        PromoCode = c.String(),
                        TotalCost = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId);
            
            CreateTable(
                "dbo.Checkouts",
                c => new
                    {
                        PetId = c.Int(nullable: false, identity: true),
                        BookingId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        ContactNumber = c.String(nullable: false),
                        EmergencyContactNumber = c.String(nullable: false),
                        KennelId = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PetId);
            
            CreateTable(
                "dbo.Checks",
                c => new
                    {
                        PetId = c.Int(nullable: false, identity: true),
                        BookId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        ContactNumber = c.String(nullable: false),
                        EmergencyContactNumber = c.String(nullable: false),
                        KennelId = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PetId);
            
            CreateTable(
                "dbo.CompDrivers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AdopterFName = c.String(nullable: false),
                        AdopterNo = c.String(nullable: false),
                        AdopterEmail = c.String(nullable: false),
                        Address = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DeclinedAdoptions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Adopterinterested = c.String(),
                        AdopterFName = c.String(nullable: false),
                        AdopterNo = c.String(nullable: false),
                        AdopterEmail = c.String(nullable: false),
                        AdopterMessage = c.String(),
                        InspectorId = c.Int(nullable: false),
                        Status = c.String(),
                        SafetyChecked = c.Boolean(nullable: false),
                        CleanlinessChecked = c.Boolean(nullable: false),
                        SpaceChecked = c.Boolean(nullable: false),
                        ProvisionsChecked = c.Boolean(nullable: false),
                        InteractiveFamilyChecked = c.Boolean(nullable: false),
                        FencedChecked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DeclinedVolunteers",
                c => new
                    {
                        VolunteerId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        CellNo = c.String(nullable: false, maxLength: 10),
                        EmergencyContactName = c.String(nullable: false),
                        EmergencyContactCellNo = c.String(nullable: false, maxLength: 10),
                        Experience = c.String(),
                        Availability = c.String(nullable: false),
                        VolunteerType = c.String(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.VolunteerId);
            
            CreateTable(
                "dbo.EventRegs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        FullName = c.String(nullable: false, maxLength: 100),
                        ContactNumber = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        IsNotRefundable = c.Boolean(nullable: false),
                        Quantity = c.Int(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TicketNumber = c.String(),
                        CheckedIn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Faileds",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AdopterFName = c.String(nullable: false),
                        AdopterNo = c.String(nullable: false),
                        AdopterEmail = c.String(nullable: false),
                        CustomerUnavailable = c.Boolean(nullable: false),
                        NotAbleToProvideOTP = c.Boolean(nullable: false),
                        NotAtAddress = c.Boolean(nullable: false),
                        Breakdown = c.Boolean(nullable: false),
                        OtherReason = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Groomings",
                c => new
                    {
                        GroomingId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        City = c.String(nullable: false),
                        PostalCode = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false, maxLength: 10),
                        Email = c.String(nullable: false),
                        PetName = c.String(nullable: false),
                        PetType = c.String(nullable: false),
                        Breed = c.String(nullable: false),
                        HairLength = c.String(nullable: false),
                        MedicalIssues = c.String(),
                        GroomingAppoinmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GroomingId);
            
            CreateTable(
                "dbo.Histories",
                c => new
                    {
                        HistoryId = c.Int(nullable: false, identity: true),
                        AppointmentId = c.Int(nullable: false),
                        Details = c.String(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        IdNumber = c.String(nullable: false, maxLength: 13),
                    })
                .PrimaryKey(t => t.HistoryId);
            
            CreateTable(
                "dbo.OngoingDrivers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AdopterFName = c.String(),
                        AdopterNo = c.String(),
                        AdopterEmail = c.String(),
                        DeliveryDate = c.DateTime(nullable: false),
                        Address = c.String(),
                        RescheduleCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PetSittings",
                c => new
                    {
                        SittingId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        CellNo = c.String(nullable: false, maxLength: 10),
                        EmergencyContactName = c.String(nullable: false),
                        EmergencyContactCellNo = c.String(nullable: false, maxLength: 10),
                        ResAddress = c.String(nullable: false, maxLength: 30),
                        PetName = c.String(nullable: false, maxLength: 15),
                        PetType = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        SpecialRequests = c.String(),
                    })
                .PrimaryKey(t => t.SittingId);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        RatingID = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Rate = c.Int(nullable: false),
                        Note = c.String(),
                        promocode = c.String(),
                    })
                .PrimaryKey(t => t.RatingID);
            
            CreateTable(
                "dbo.ReturnPolicies",
                c => new
                    {
                        ReturnRequestID = c.Int(nullable: false, identity: true),
                        AdoptionID = c.Int(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        ScheduledReturnDate = c.DateTime(nullable: false),
                        ReturnStatus = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.ReturnRequestID);
            
            CreateTable(
                "dbo.RsvpModels",
                c => new
                    {
                        Rsvpno = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        PetTalentShow = c.Boolean(nullable: false),
                        PetParty = c.Boolean(nullable: false),
                        CharityAuction = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Rsvpno);
            
            CreateTable(
                "dbo.SurveyModels",
                c => new
                    {
                        Email = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        ERating = c.Int(nullable: false),
                        VRating = c.Int(nullable: false),
                        RRating = c.Int(nullable: false),
                        ORating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Email);
            
            CreateTable(
                "dbo.User1",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        IDnum = c.String(),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Firstname = c.String(nullable: false),
                        Lastname = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Mobile = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        PostalCode = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Volunteers",
                c => new
                    {
                        VolunteerId = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Surname = c.String(),
                        Email = c.String(),
                        CellNo = c.String(maxLength: 10),
                        EmergencyContactName = c.String(nullable: false),
                        EmergencyContactCellNo = c.String(nullable: false, maxLength: 10),
                        Experience = c.String(),
                        Availability = c.String(nullable: false),
                        VolunteerType = c.String(nullable: false),
                        Status = c.String(),
                        AssignedDay = c.DateTime(),
                        SecondAssignedDay = c.DateTime(),
                        OrientationDate = c.String(),
                        Suitability = c.String(),
                        Question1Answer = c.String(),
                        Question2Answer = c.String(),
                        Question3Answer = c.String(),
                        Question4Answer = c.String(),
                        Question5Answer = c.String(),
                        Question6Answer = c.String(),
                        Question7Answer = c.String(),
                        Question8Answer = c.String(),
                        Question9Answer = c.String(),
                        Question10Answer = c.String(),
                        PuppyTrainingQuestion1Answer = c.String(),
                        PuppyTrainingQuestion2Answer = c.String(),
                        PuppyTrainingQuestion3Answer = c.String(),
                        PuppyTrainingQuestion4Answer = c.String(),
                        PuppyTrainingQuestion5Answer = c.String(),
                        CatCuddlingQuestion1Answer = c.String(),
                        CatCuddlingQuestion2Answer = c.String(),
                        CatCuddlingQuestion3Answer = c.String(),
                        CatCuddlingQuestion4Answer = c.String(),
                        CatCuddlingQuestion5Answer = c.String(),
                    })
                .PrimaryKey(t => t.VolunteerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventRegs", "EventId", "dbo.Events");
            DropForeignKey("dbo.Bids", "AuctionItemId", "dbo.AuctionItems");
            DropForeignKey("dbo.AuctionItems", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.EventDocuments", "Event_Id", "dbo.Events");
            DropIndex("dbo.EventRegs", new[] { "EventId" });
            DropIndex("dbo.Bids", new[] { "AuctionItemId" });
            DropIndex("dbo.EventDocuments", new[] { "Event_Id" });
            DropIndex("dbo.AuctionItems", new[] { "Event_Id" });
            DropTable("dbo.Volunteers");
            DropTable("dbo.User1");
            DropTable("dbo.SurveyModels");
            DropTable("dbo.RsvpModels");
            DropTable("dbo.ReturnPolicies");
            DropTable("dbo.Ratings");
            DropTable("dbo.PetSittings");
            DropTable("dbo.OngoingDrivers");
            DropTable("dbo.Histories");
            DropTable("dbo.Groomings");
            DropTable("dbo.Faileds");
            DropTable("dbo.EventRegs");
            DropTable("dbo.DeclinedVolunteers");
            DropTable("dbo.DeclinedAdoptions");
            DropTable("dbo.CompDrivers");
            DropTable("dbo.Checks");
            DropTable("dbo.Checkouts");
            DropTable("dbo.Bookings");
            DropTable("dbo.Bids");
            DropTable("dbo.EventDocuments");
            DropTable("dbo.Events");
            DropTable("dbo.AuctionItems");
            DropTable("dbo.Appointments");
            DropTable("dbo.ApprovedAdoptions");
            DropTable("dbo.AdoptionRequests");
        }
    }
}
