using Glossary.DataAccess.Entities;

namespace Glossary.BusinessLogic.Services.Interfaces
{
    public interface IGlossaryTermsService
    {
        Task Create(GlossaryTerm term, string userId);
        Task<GlossaryTerm?> GetById(int id);
        Task Delete(int id, string userId);
        Task Archive(int id, string userId);
    }
}
