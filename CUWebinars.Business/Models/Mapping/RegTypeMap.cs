
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class RegTypeMap : EntityTypeConfiguration<RegType>
    {
        public RegTypeMap()
        {
            // Primary Key
            HasKey(t => t.idRegType);

            // Properties
            //this.Property(t => t.RegTypeLabel)
            //    .HasMaxLength(200)
            //    .IsRequired();

            //this.Property(t => t.ShowLiveNotifications)
            //    .IsRequired();

            //this.Property(t => t.SKU)
            //    .HasMaxLength(150);

            // Table & Column Mappings
            ToTable("RegType");
            Property(t => t.idRegType).HasColumnName("idRegType");
            Property(t => t.OptionExplain).HasColumnName("RegTypeExplain");
            Property(t => t.OptionLabel).HasColumnName("RegTypeLabel");
            Property(t => t.PriceToAdd).HasColumnName("PriceToAdd");
            Property(t => t.TaxExempt).HasColumnName("TaxExempt");
            Property(t => t.SortOrder).HasColumnName("SortOrder");
            Property(t => t.SKU).HasColumnName("SKU");
            Property(t => t.ShowLiveNotifications).HasColumnName("ShowLiveNotifications");
            Property(t => t.ShowRecordingNotifications).HasColumnName("ShowRecordingNotifications");
            Property(t => t.ShowShippedNotifications).HasColumnName("ShowShippedNotifications");
            Property(t => t.Stage1CheckoutConfirmationMsg).HasColumnName("Stage1CheckoutConfirmationMsg");
            Property(t => t.Stage2CheckoutConfirmationMsg).HasColumnName("Stage2CheckoutConfirmationMsg");
            Property(t => t.Stage1EmailConfirmationMsg).HasColumnName("Stage1EmailConfirmationMsg");
            Property(t => t.Stage2EmailConfirmationMsg).HasColumnName("Stage2EmailConfirmationMsg");
        }
    }
}
