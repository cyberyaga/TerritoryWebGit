namespace TerritoryWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CongregationID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CongregationID", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Language", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Language");
            DropColumn("dbo.AspNetUsers", "CongregationID");
        }
    }
}
