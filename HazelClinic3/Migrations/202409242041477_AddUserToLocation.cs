namespace HazelClinic3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserToLocation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        UserId = c.Int(nullable: false),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User1", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Locations", "UserId", "dbo.User1");
            DropIndex("dbo.Locations", new[] { "UserId" });
            DropTable("dbo.Locations");
        }
    }
}
