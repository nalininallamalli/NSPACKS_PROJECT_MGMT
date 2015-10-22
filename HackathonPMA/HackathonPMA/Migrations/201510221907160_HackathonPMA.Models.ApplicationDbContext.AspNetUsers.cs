namespace HackathonPMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HackathonPMAModelsApplicationDbContextAspNetUsers : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Salary");
            DropColumn("dbo.AspNetUsers", "AddressLine");
        }
    }
}
