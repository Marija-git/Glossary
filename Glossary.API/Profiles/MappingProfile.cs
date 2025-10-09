using AutoMapper;
using Glossary.API.DTOs.Request;
using Glossary.API.DTOs.Response;
using Glossary.DataAccess.Entities;

namespace Glossary.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateGlossaryTermDtoRequest, GlossaryTerm>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Status.Draft))
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore());

            CreateMap<GlossaryTerm, GlossaryTermDtoResponse>();

        }

    }
}
