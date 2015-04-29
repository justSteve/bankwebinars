using System;
using System.Collections.Generic;
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

            // Properties
            Property(t => t.Text).HasMaxLength(800).IsRequired();

            // Table & Column Mappings
            ToTable("OptionMap");

            // Relationships

        }
    }
}
