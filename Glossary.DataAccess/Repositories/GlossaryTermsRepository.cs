using Glossary.DataAccess.AppData;
using Glossary.DataAccess.Entities;
using Glossary.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Glossary.DataAccess.Repositories
{
    public class GlossaryTermsRepository : IGlossaryTermsRepository
    {
        private readonly GlossaryDbContext _context;
        public GlossaryTermsRepository(GlossaryDbContext context)
        {
            _context = context;
        }
        public async Task Create(GlossaryTerm term)
        {
            _context.GlossaryTerms.Add(term);
            await _context.SaveChangesAsync();
        }

        public async Task<GlossaryTerm?> GetById(int id)
        {
            return await _context.GlossaryTerms.FirstOrDefaultAsync(gt => gt.Id == id);
        }

        public async Task<GlossaryTerm?> GetByTerm(string term)
        {
            return await _context.GlossaryTerms.FirstOrDefaultAsync(t => t.Term == term);
        }
    }
}
