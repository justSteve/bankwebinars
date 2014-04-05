
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            ToTable("Addresses");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AddressType).HasColumnName("AddressType");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Phone).HasColumnName("Phone");
            Property(t => t.StreetAddress).HasColumnName("StreetAddress");
            Property(t => t.StreetAddress2).HasColumnName("StreetAddress2");
            Property(t => t.City).HasColumnName("City");
            Property(t => t.Zip).HasColumnName("Zip");
            Property(t => t.State).HasColumnName("State");
            Property(t => t.Country).HasColumnName("Country");
            //this.Property(t => t.WebUser_Id).HasColumnName("WebUser_Id");
            Property(t => t.idUser).HasColumnName("idUser");

            // Relationships
            HasRequired(t => t.WebUser)
                            .WithMany(t => t.Addresses)
                            .HasForeignKey(d => d.idUser);

        }
    }
}
