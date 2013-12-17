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
            this.Property(t => t.AddressType)
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Phone)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.StreetAddress)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.StreetAddress2)
                .HasMaxLength(100);

            this.Property(t => t.City)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Zip)
                .IsRequired()
                .HasMaxLength(75);

            this.Property(t => t.State)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Country)
                .IsRequired()
                .HasMaxLength(75);

            // Table & Column Mappings
            this.ToTable("Address");
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

            // Relationships
            this.HasOptional(t => t.WebUser)
                .WithMany(t => t.Addresses)
                .HasForeignKey(d => d.WebUser_Id);

        }
    }
}
