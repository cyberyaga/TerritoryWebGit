namespace TerritoryWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CongUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "CongregationID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "CongregationID", c => c.Int(nullable: false));
        }
    }
}
