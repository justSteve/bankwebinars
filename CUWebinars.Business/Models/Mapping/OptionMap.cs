using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OptionMap : EntityTypeConfiguration<Option>
    {
        public OptionMap()
        {
            // Primary Key
            this.HasKey(t => t.idOption);

            // Properties
            this.Property(t => t.OptionLabel)
                .HasMaxLength(200);

            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.MsgConfirm)
                .HasMaxLength(10);

            this.Property(t => t.SKU)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Options");
            this.Property(t => t.idOption).HasColumnName("idOption");
            this.Property(t => t.OptionExplain).HasColumnName("OptionExplain");
            this.Property(t => t.OptionLabel).HasColumnName("OptionLabel");
            this.Property(t => t.PriceToAdd).HasColumnName("PriceToAdd");
            this.Property(t => t.TaxExempt).HasColumnName("TaxExempt");
            this.Property(t => t.PercToAdd).HasColumnName("PercToAdd");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.MsgConfirm).HasColumnName("MsgConfirm");
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
