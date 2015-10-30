namespace HackathonPMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HackhatonPMAModelsEntities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "SpendingDetails", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "SpendingDetails");
        }
    }
}
