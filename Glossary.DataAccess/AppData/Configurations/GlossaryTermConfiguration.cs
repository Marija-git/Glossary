using Glossary.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Glossary.DataAccess.AppData.Configurations
{
    public class GlossaryTermConfiguration : IEntityTypeConfiguration<GlossaryTerm>
    {
        public void Configure(EntityTypeBuilder<GlossaryTerm> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Term).IsUnique().HasFilter("\"Term\" <> ''"); ;
            builder.Property(x => x.Term).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Definition).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.Status).HasDefaultValue(Status.Draft).IsRequired();

            builder.HasOne(x => x.Author)
                 .WithMany(a => a.OwnedGlossaryTerms)
                 .HasForeignKey(x => x.AuthorId);
        }
    }
}
