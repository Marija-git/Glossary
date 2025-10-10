using Glossary.BusinessLogic.Configurations;
using Glossary.BusinessLogic.Exceptions;
using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.DataAccess.Entities;
using Glossary.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace Glossary.BusinessLogic.Services
{
    public class GlossaryTermsService : IGlossaryTermsService
    {
        private readonly IGlossaryTermsRepository _glossaryTermRepository;
        private readonly GlossarySettings _glossarySettings;
        public GlossaryTermsService(IGlossaryTermsRepository glossaryTermRepository,
            IOptions<GlossarySettings> options)
        {
            _glossaryTermRepository = glossaryTermRepository;
            _glossarySettings = options.Value;

        }

        public async Task Archive(int id, string userId)
        {
            var glossaryTerm = await GetById(id);

            if (glossaryTerm.AuthorId != userId)
                throw new ForbidException();

            if (glossaryTerm.Status != Status.Published)
                throw new ConflictException("Only terms in Publish state can be archived.");

            glossaryTerm.Status = Status.Archived;
            await _glossaryTermRepository.Update(glossaryTerm);
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

        public async Task Delete(int id, string userId)
        {
            var glossaryTerm = await GetById(id);

            if (glossaryTerm.AuthorId != userId)
                throw new ForbidException();

            if(glossaryTerm.Status != Status.Draft)
                throw new ConflictException("Only terms in Draft state can be deleted.");
            
            await _glossaryTermRepository.Delete(glossaryTerm);
           
        }

        public async Task<GlossaryTerm?> GetById(int id)
        {
            var glossaryTerm =  await _glossaryTermRepository.GetById(id);

            if(glossaryTerm == null)
                throw new NotFoundException(nameof(GlossaryTerm),$"{id}");

            return glossaryTerm;

        }

        public async Task Publish(int id, GlossaryTerm termReq, string userId)
        {
            var glossaryTerm = await GetById(id);

            if (glossaryTerm.AuthorId != userId)
                throw new ForbidException();

            if (termReq.Term != null)
                glossaryTerm.Term = termReq.Term.Trim();

            if (termReq.Definition != null)
                glossaryTerm.Definition = termReq.Definition.Trim();

            if (string.IsNullOrWhiteSpace(glossaryTerm.Term))
                throw new BadRequestException("Term cannot be empty.");

            if (glossaryTerm.Definition.Length < _glossarySettings.MinDefinitionLength)
                throw new BadRequestException(
                    $"Definition must be at least {_glossarySettings.MinDefinitionLength} characters long.");

            // + forbidden words logic later.

            glossaryTerm.Status = Status.Published;
            await _glossaryTermRepository.Update(glossaryTerm);

        }
    }
}
