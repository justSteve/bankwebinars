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
            HasKey(t => t.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            // Properties
            Property(t => t.Text).HasMaxLength(800).IsRequired();

            // Table & Column Mappings
            ToTable("Option");

            // Relationships

        }
    }
}
