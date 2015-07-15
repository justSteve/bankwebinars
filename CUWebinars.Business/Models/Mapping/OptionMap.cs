using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Business.Models.Mapping
{
    public class OptionMap : EntityTypeConfiguration<Option>
    {
        public OptionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.Property(t => t.Id);


            // Properties
            this.Property(t => t.Text)
                .IsRequired()
                .HasMaxLength(800);

            // Table & Column Mappings
            this.ToTable("Option");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Text).HasColumnName("Text");

            // Relationships
            HasMany(t => t.QuestionWithOptions)
                .WithRequired(t => t.Option)
                .HasForeignKey(t => t.idOption);
        }
    }
}
