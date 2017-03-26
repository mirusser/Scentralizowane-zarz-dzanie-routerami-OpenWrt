namespace RouterManagement.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<RouterManagement.Models.Context.RouterManagementEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RouterManagement.Models.Context.RouterManagementEntities context)
        {
        }
    }
}
