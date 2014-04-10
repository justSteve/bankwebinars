using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectContextObjectMaterialized;
        }

        public TTSWebinarsContext(string name)
            : base(string.Format("Name={0}", name))
        {
            //this.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.LazyLoadingEnabled = false;
        }

        private void ObjectContextObjectMaterialized(object sender, ObjectMaterializedEventArgs objectMaterializedEventArgs)
        {
            var entity = objectMaterializedEventArgs.Entity as IObjectWithState;

            if (ReferenceEquals(entity, null)) return;
            entity.EntityState = State.Unchanged;
            entity.OriginalValues = BuildOriginalValues(Entry(entity).OriginalValues);
        }

        private Dictionary<string, object> BuildOriginalValues(DbPropertyValues originalValues)
        {
            var result = new Dictionary<string, object>();

            foreach (var propertyName in originalValues.PropertyNames)
            {
                var value = originalValues[propertyName];

                if (value is DbPropertyValues)
                {
                    result[propertyName] = BuildOriginalValues((DbPropertyValues)value);
                }
                else
                {
                    result[propertyName] = value;
                }
            }
            return result;
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<AdditionalLocation> AdditionalLocation { get; set; }
        public DbSet<Affiliate> Affiliates { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<RegType> RegTypes { get; set; }
        public DbSet<RegTypesGroup> RegTypesGroups { get; set; }
        public DbSet<RegTypesGroupsXref> RegTypesGroupsXrefs { get; set; }
        public DbSet<RegTypesXref> RegTypesXrefs { get; set; }
        public DbSet<OrderRow> OrderRows { get; set; }
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
            modelBuilder.Configurations.Add(new InstitutionMap());
            modelBuilder.Configurations.Add(new RegTypeMap());
            modelBuilder.Configurations.Add(new RegTypesGroupMap());
            modelBuilder.Configurations.Add(new RegTypesGroupsXrefMap());
            modelBuilder.Configurations.Add(new RegTypesXrefMap());
            modelBuilder.Configurations.Add(new OrderRowMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new PresenterMap());
            modelBuilder.Configurations.Add(new TopicMap());
            modelBuilder.Configurations.Add(new WebinarMap());
            modelBuilder.Configurations.Add(new WebinarFileMap());
            modelBuilder.Configurations.Add(new WebinarTopicXrefMap());
            modelBuilder.Configurations.Add(new WebUserMap());

        }
    }
}
