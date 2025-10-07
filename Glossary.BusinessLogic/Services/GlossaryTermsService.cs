using Glossary.BusinessLogic.Exceptions;
using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.DataAccess.Entities;
using Glossary.DataAccess.Repositories.Interfaces;

namespace Glossary.BusinessLogic.Services
{
    public class GlossaryTermsService : IGlossaryTermsService
    {
        private readonly IGlossaryTermsRepository _glossaryTermRepository;
        public GlossaryTermsService(IGlossaryTermsRepository glossaryTermRepository)
        {
            _glossaryTermRepository = glossaryTermRepository;
        }

        public async Task Create(GlossaryTerm term, string userId)
        {
            if (!string.IsNullOrWhiteSpace(term.Term))
            {
                var glossaryTerm = await _glossaryTermRepository.GetByTerm(term.Term); 
                if (glossaryTerm != null)
                    throw new ConflictException($"A term '{term.Term}' already exists."); 
            }
            term.AuthorId = userId;

            await _glossaryTermRepository.Create(term);
        }

        public async Task<GlossaryTerm?> GetById(int id)
        {
            var glossaryTerm =  await _glossaryTermRepository.GetById(id);

            if(glossaryTerm == null)
                throw new NotFoundException(nameof(GlossaryTerm),$"{id}");

            return glossaryTerm;

        }
    }
}
