using Glossary.DataAccess.Entities;

namespace Glossary.DataAccess.Repositories.Interfaces
{
    public interface IForbiddenWordsRepository
    {
        Task<List<ForbiddenWord>> GetAll();
    }
}
