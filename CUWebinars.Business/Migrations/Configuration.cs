using System.Data.Entity;

namespace CUWebinars.Business.Migrations
{
    using CUWebinars.Business.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<TTSWebinarsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //ProxyCreationEnabled = false;
            

        }

        protected override void Seed(CUWebinars.Business.Models.TTSWebinarsContext context)
        {
            // 'FirstName=Mark&LastName=Bennett&Email=affiliate@ttstrain.com&Institution=TTS&AddressType=Billing&City=city&Country=country&Name=Mark Bennett&Phone=608-849-5563&State=state&StreetAddress=street&StreetAddress2=street2&Zip=zip&Password=bennett....&ConfirmPassword=bennett....&userType=2&title=na&idWebUser=19'
            //var regModel = context.Institutions.Add()

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
