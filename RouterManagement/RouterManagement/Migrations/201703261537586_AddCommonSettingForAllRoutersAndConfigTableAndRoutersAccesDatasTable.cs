namespace RouterManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommonSettingForAllRoutersAndConfigTableAndRoutersAccesDatasTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommonSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Setting = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConfigSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RouterAccesDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RouterIp_Address = c.Long(nullable: false),
                        RouterIp_ScopeId = c.Long(nullable: false),
                        Port = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RouterAccesDatas");
            DropTable("dbo.ConfigSettings");
            DropTable("dbo.CommonSettings");
        }
    }
}
