using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class RegTypeMap : EntityTypeConfiguration<RegType>
    {
        public RegTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.idRegType);

            // Properties
            //this.Property(t => t.RegTypeLabel)
            //    .HasMaxLength(200)
            //    .IsRequired();

            //this.Property(t => t.ShowLiveNotifications)
            //    .IsRequired();

            //this.Property(t => t.SKU)
            //    .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("RegType");
            this.Property(t => t.idRegType).HasColumnName("idRegType");
            this.Property(t => t.OptionExplain).HasColumnName("RegTypeExplain");
            this.Property(t => t.OptionLabel).HasColumnName("RegTypeLabel");
            this.Property(t => t.PriceToAdd).HasColumnName("PriceToAdd");
            this.Property(t => t.TaxExempt).HasColumnName("TaxExempt");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
            this.Property(t => t.SKU).HasColumnName("SKU");
            this.Property(t => t.ShowLiveNotifications).HasColumnName("ShowLiveNotifications");
            this.Property(t => t.ShowRecordingNotifications).HasColumnName("ShowRecordingNotifications");
            this.Property(t => t.ShowShippedNotifications).HasColumnName("ShowShippedNotifications");
            this.Property(t => t.Stage1CheckoutConfirmationMsg).HasColumnName("Stage1CheckoutConfirmationMsg");
            this.Property(t => t.Stage2CheckoutConfirmationMsg).HasColumnName("Stage2CheckoutConfirmationMsg");
            this.Property(t => t.Stage1EmailConfirmationMsg).HasColumnName("Stage1EmailConfirmationMsg");
            this.Property(t => t.Stage2EmailConfirmationMsg).HasColumnName("Stage2EmailConfirmationMsg");
        }
    }
}
