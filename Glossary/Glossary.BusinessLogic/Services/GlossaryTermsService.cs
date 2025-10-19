using Glossary.BusinessLogic.Configurations;
using Glossary.BusinessLogic.Exceptions;
using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.DataAccess.Entities;
using Glossary.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Glossary.BusinessLogic.Services
{
    public class GlossaryTermsService : IGlossaryTermsService
    {
        private readonly IGlossaryTermsRepository _glossaryTermRepository;
        private readonly GlossarySettings _glossarySettings;
        private readonly IForbiddenWordsRepository _forbiddenWordRepository;
        private readonly ILogger<GlossaryTermsService> _logger;
        public GlossaryTermsService(IGlossaryTermsRepository glossaryTermRepository,
            IOptions<GlossarySettings> options,
            IForbiddenWordsRepository forbiddenWordRepository,
            ILogger<GlossaryTermsService> logger)
        {
            _glossaryTermRepository = glossaryTermRepository;
            _glossarySettings = options.Value;
            _forbiddenWordRepository = forbiddenWordRepository;
            _logger = logger;

        }

        public async Task Archive(int id, string userId)
        {
            _logger.LogInformation($"Archiving glossary term {id} requested by user {userId}.");

            var glossaryTerm = await GetById(id);

            if (glossaryTerm.AuthorId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to archive term {id} without permission.");
                throw new ForbidException();
            }
                
            if (glossaryTerm.Status != Status.Published)
            {
                _logger.LogWarning($"Cannot archive term {id}: status is {glossaryTerm.Status}." );
                throw new ConflictException("Only terms in Publish state can be archived.");
            }
                

            glossaryTerm.Status = Status.Archived;
            await _glossaryTermRepository.Update(glossaryTerm);

            _logger.LogInformation($"Glossary term {id} successfully archived by user {userId}.");
        }

        public async Task Create(GlossaryTerm term, string userId)
        {
            _logger.LogInformation($"User {userId} is attempting to create a new glossary term '{term.Term}'");

            if (!string.IsNullOrWhiteSpace(term.Term))
            {
                var glossaryTerm = await _glossaryTermRepository.GetByTerm(term.Term); 
                if (glossaryTerm != null)
                {
                    _logger.LogWarning($"Creation failed: term '{term.Term}' already exists (attempt by user {userId})");
                    throw new ConflictException($"A term '{term.Term}' already exists.");
                }
                     
            }
            term.AuthorId = userId;
            await _glossaryTermRepository.Create(term);

            _logger.LogInformation($"Glossary term '{term.Term}' successfully created by user {userId}");
        }

        public async Task Delete(int id, string userId)
        {
            _logger.LogInformation($"User {userId} is attempting to delete glossary term with ID {id}");

            var glossaryTerm = await GetById(id);

            if (glossaryTerm.AuthorId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to delete term {id} without permission");
                throw new ForbidException();
            }
               

            if(glossaryTerm.Status != Status.Draft)
            {
                _logger.LogWarning($"Cannot delete term {id}: status is {glossaryTerm.Status} (attempt by user {userId})");
                throw new ConflictException("Only terms in Draft state can be deleted.");
            }
                           
            await _glossaryTermRepository.Delete(glossaryTerm);

            _logger.LogInformation($"Glossary term {id} successfully deleted by user {userId}");
        }

        public async Task<GlossaryTerm?> GetById(int id)
        {
            _logger.LogInformation($"Getting glossary term with ID {id}");

            var glossaryTerm =  await _glossaryTermRepository.GetById(id);

            if(glossaryTerm == null)
            {
                _logger.LogWarning($"Glossary term with ID {id} not found");
                throw new NotFoundException(nameof(GlossaryTerm), $"{id}");
            }

            _logger.LogInformation($"Glossary term with ID {id} successfully retrieved");
            return glossaryTerm;

        }
        public async Task Publish(int id, GlossaryTerm termReq, string userId)
        {
            _logger.LogInformation($"User {userId} is attempting to publish glossary term with ID {id}");

            var glossaryTerm = await GetById(id);

            if (glossaryTerm.AuthorId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to publish term {id} without permission");
                throw new ForbidException();
            }                

            if (termReq.Term != null)
                glossaryTerm.Term = termReq.Term.Trim();

            if (termReq.Definition != null)
                glossaryTerm.Definition = termReq.Definition.Trim();

            if (string.IsNullOrWhiteSpace(glossaryTerm.Term))
            {
                _logger.LogWarning($"Cannot publish term {id}: term is empty (attempt by user {userId})");
                throw new BadRequestException("Term cannot be empty.");
            }
                
            if (glossaryTerm.Definition.Length < _glossarySettings.MinDefinitionLength)
            {
                _logger.LogWarning($"Cannot publish term {id}: definition too short (length {glossaryTerm.Definition.Length}, required {_glossarySettings.MinDefinitionLength}) by user {userId}");
                throw new BadRequestException(
                   $"Definition must be at least {_glossarySettings.MinDefinitionLength} characters long.");
            }
               

            await CheckForbiddenWords(glossaryTerm.Definition);

            glossaryTerm.Status = Status.Published;
            await _glossaryTermRepository.Update(glossaryTerm);
            _logger.LogInformation($"Glossary term {id} successfully published by user {userId}");
        }

        public async Task<PaginatedData<GlossaryTerm>> GetGlossaryTermsPaged(string? userId, int pageSize, int pageIndex)
        {
            _logger.LogInformation($"Getting paginated glossary terms.");
            return await _glossaryTermRepository.GetGlossaryTermsPaged(userId, pageSize, pageIndex);
        }


        private async Task CheckForbiddenWords(string definition)
        {
            var forbiddenWords = await _forbiddenWordRepository.GetAll();

            foreach (var word in forbiddenWords)
            {
                var regex = new Regex($@"(?i)(?<!\w){Regex.Escape(word.Word)}(?!\w)");
                if (regex.IsMatch(definition))
                {
                    throw new BadRequestException($"The definition contains a forbidden word: '{word.Word}'.");
                }
            }
        }
    }
}
