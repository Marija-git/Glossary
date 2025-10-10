using AutoMapper;
using Glossary.API.DTOs.Request;
using Glossary.API.DTOs.Response;
using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Glossary.API.Controllers
{
    [Route("api/GlossaryTerms")]
    [ApiController]
    [Authorize]
    public class GlossaryTermsController : ControllerBase
    {
        private readonly IGlossaryTermsService _glossaryTermsService;
        private readonly IMapper _mapper;


        public GlossaryTermsController(IGlossaryTermsService glossaryTermsService, IMapper mapper)
        {
            _glossaryTermsService = glossaryTermsService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GlossaryTermDtoRequest dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var glossaryTerm = _mapper.Map<GlossaryTerm>(dto);
            await _glossaryTermsService.Create(glossaryTerm, userId);
            return CreatedAtAction(nameof(GetById), new { id = glossaryTerm.Id }, null);
        }

        [HttpGet("{id}")]
        public async Task<GlossaryTermDtoResponse> GetById([FromRoute] int id)
        {
            return _mapper.Map<GlossaryTermDtoResponse>(await _glossaryTermsService.GetById(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _glossaryTermsService.Delete(id, userId);
            return NoContent();
        }

        [HttpPatch("{id}/archive")]
        public async Task<IActionResult> Archive([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _glossaryTermsService.Archive(id, userId);
            return NoContent();
        }

        [HttpPatch("{id}/publish")]
        public async Task<IActionResult> Publish([FromRoute]int id, [FromBody]GlossaryTermDtoRequest dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var glossaryTerm = _mapper.Map<GlossaryTerm>(dto);
            await _glossaryTermsService.Publish(id, glossaryTerm, userId);
            return NoContent();
        }

    }
}
