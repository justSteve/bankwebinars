using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Addresses");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AddressType).HasColumnName("AddressType");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.StreetAddress).HasColumnName("StreetAddress");
            this.Property(t => t.StreetAddress2).HasColumnName("StreetAddress2");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.Zip).HasColumnName("Zip");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.Country).HasColumnName("Country");
            this.Property(t => t.WebUser_Id).HasColumnName("WebUser_Id");
            this.Property(t => t.WebUser_idUser).HasColumnName("WebUser_idUser");

            // Relationships
            this.HasOptional(t => t.WebUser)
                .WithMany(t => t.Addresses)
                .HasForeignKey(d => d.WebUser_idUser);

        }
    }
}
