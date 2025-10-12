using Glossary.DataAccess.AppData;
using Glossary.DataAccess.Entities;
using Glossary.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Glossary.DataAccess.Repositories
{
    public class ForbiddenWordsRepository : IForbiddenWordsRepository
    {
        private readonly GlossaryDbContext _context;
        public ForbiddenWordsRepository(GlossaryDbContext context)
        {
            _context = context;
        }
        public async Task<List<ForbiddenWord>> GetAll()
        {
            return await _context.ForbiddenWords.ToListAsync();
        }
    }
}
