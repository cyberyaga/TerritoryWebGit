namespace TerritoryWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TypeID : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "TypeID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "TypeID", c => c.Int(nullable: false));
        }
    }
}
