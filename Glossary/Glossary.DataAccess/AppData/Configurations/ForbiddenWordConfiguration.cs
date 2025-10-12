using Glossary.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Glossary.DataAccess.AppData.Configurations
{
    public class ForbiddenWordConfiguration : IEntityTypeConfiguration<ForbiddenWord>
    {
        public void Configure(EntityTypeBuilder<ForbiddenWord> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Word).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Word).IsUnique();
        }
    }
}
