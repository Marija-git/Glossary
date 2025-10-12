using Glossary.DataAccess.AppData;
using Glossary.DataAccess.Entities;
using Glossary.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task Delete(GlossaryTerm term)
        {
            _context.Remove(term);
            await _context.SaveChangesAsync();
        }

        public async Task Update(GlossaryTerm term)
        {
            _context.GlossaryTerms.Update(term);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedData<GlossaryTerm>> GetGlossaryTermsPaged(string? userId, int pageSize, int pageIndex)
        {
            var baseQuery = _context.GlossaryTerms.AsQueryable();
            if (!string.IsNullOrEmpty(userId))
            {
                baseQuery = baseQuery.Where(t => t.Status == Status.Published || t.AuthorId == userId);
            }
            else
            {
                baseQuery = baseQuery.Where(t => t.Status == Status.Published);
            }

            var totalCount = await baseQuery.CountAsync();

            var glossartyTerms = await baseQuery
            .OrderBy(t => t.Term)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
 
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return new PaginatedData<GlossaryTerm>(glossartyTerms, pageIndex, totalPages, totalCount);
        }
    }
}
