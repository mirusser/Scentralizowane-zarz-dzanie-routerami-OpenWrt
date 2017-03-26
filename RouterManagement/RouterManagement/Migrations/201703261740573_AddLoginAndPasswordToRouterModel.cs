namespace RouterManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoginAndPasswordToRouterModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RouterAccesDatas", "Login", c => c.String());
            AddColumn("dbo.RouterAccesDatas", "Password", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RouterAccesDatas", "Password");
            DropColumn("dbo.RouterAccesDatas", "Login");
        }
    }
}
