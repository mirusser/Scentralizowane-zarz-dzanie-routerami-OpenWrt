namespace RouterManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeIpRouterFromIPAdressToString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RouterAccesDatas", "RouterIp", c => c.String());
            DropColumn("dbo.RouterAccesDatas", "RouterIp_Address");
            DropColumn("dbo.RouterAccesDatas", "RouterIp_ScopeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RouterAccesDatas", "RouterIp_ScopeId", c => c.Long(nullable: false));
            AddColumn("dbo.RouterAccesDatas", "RouterIp_Address", c => c.Long(nullable: false));
            DropColumn("dbo.RouterAccesDatas", "RouterIp");
        }
    }
}
