namespace HackathonPMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HackathonPMAModelsEntitiesProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "State", c => c.Int());
            AddColumn("dbo.Projects", "IsParent", c => c.Boolean());
            AddColumn("dbo.Projects", "CreatedOn", c => c.DateTime());
            AddColumn("dbo.Projects", "ModifiedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "State");
            DropColumn("dbo.Projects", "IsParent");
            DropColumn("dbo.Projects", "CreatedOn");
            DropColumn("dbo.Projects", "ModifiedOn");
        }
    }
}
