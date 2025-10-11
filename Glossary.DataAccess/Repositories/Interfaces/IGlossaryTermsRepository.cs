using Glossary.DataAccess.Entities;

namespace Glossary.DataAccess.Repositories.Interfaces
{
    public interface IGlossaryTermsRepository
    {
        Task Create(GlossaryTerm term);
        Task<GlossaryTerm?> GetById(int id);
        Task<GlossaryTerm?> GetByTerm(string term);
        Task Delete(GlossaryTerm term);
        Task Update(GlossaryTerm term);
        Task<PaginatedData<GlossaryTerm>> GetGlossaryTermsPaged(string? userId, int pageSize, int pageIndex);
    }
}
