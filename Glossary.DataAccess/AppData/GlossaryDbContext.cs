using Glossary.DataAccess.AppData.Configurations;
using Glossary.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Glossary.DataAccess.AppData
{
    public class GlossaryDbContext : DbContext
    {
        public GlossaryDbContext(DbContextOptions<GlossaryDbContext> options) : base(options) { }
        public DbSet<GlossaryTerm> GlossaryTerms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new GlossaryTermConfiguration());
        }
    }
}
