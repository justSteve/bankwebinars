using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CUWebinars.Business.Models.Mapping;

namespace CUWebinars.Business.Models
{
    public partial class TTSWebinarsContext : DbContext
    {
        static TTSWebinarsContext()
        {
            Database.SetInitializer<TTSWebinarsContext>(null);
        }

        public TTSWebinarsContext()
            : this("DefaultConnection")
        {

        }

        public TTSWebinarsContext(string name)
            : base(string.Format("Name={0}", name))
        {
            //this.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Affiliate> Affiliates { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        //public DbSet<HostProperty> HostProperties { get; set; }
        //public DbSet<HostPropertyValue> HostPropertyValues { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<RegType> Options { get; set; }
        //public DbSet<OptionsGroup> OptionsGroups { get; set; }
        //public DbSet<OptionsGroupsXref> OptionsGroupsXrefs { get; set; }
        //public DbSet<OptionsXref> OptionsXrefs { get; set; }
        public DbSet<OrderRow> OrderRows { get; set; }
        public DbSet<AdditionalLocations> AdditionalLocations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Presenter> Presenters { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Webinar> Webinars { get; set; }
        public DbSet<WebinarFile> WebinarFiles { get; set; }
        public DbSet<WebinarTopicXref> WebinarTopicXrefs { get; set; }
        public DbSet<WebUser> WebUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new AdditionalLocationMap());
            modelBuilder.Configurations.Add(new AffiliateMap());
            modelBuilder.Configurations.Add(new DiscountMap());
            //modelBuilder.Configurations.Add(new HostPropertyMap());
            //modelBuilder.Configurations.Add(new HostPropertyValueMap());
            modelBuilder.Configurations.Add(new InstitutionMap());
            modelBuilder.Configurations.Add(new RegTypeMap());
            //modelBuilder.Configurations.Add(new OptionsGroupMap());
            //modelBuilder.Configurations.Add(new OptionsGroupsXrefMap());
            //modelBuilder.Configurations.Add(new OptionsXrefMap());
            modelBuilder.Configurations.Add(new OrderRowMap());
            //modelBuilder.Configurations.Add(new OrderRowRegTypeMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new PresenterMap());
            modelBuilder.Configurations.Add(new TopicMap());
            modelBuilder.Configurations.Add(new WebinarMap());
            modelBuilder.Configurations.Add(new WebinarFileMap());
            modelBuilder.Configurations.Add(new WebinarTopicXrefMap());
            modelBuilder.Configurations.Add(new WebUserMap());
            //modelBuilder.Configurations.Add(new AdditionalLocationsRegTypeMap());
        }
    }
}
