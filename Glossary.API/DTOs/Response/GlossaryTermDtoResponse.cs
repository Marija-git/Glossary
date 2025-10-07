using Glossary.DataAccess.Entities;

namespace Glossary.API.DTOs.Response
{
    public class GlossaryTermDtoResponse
    {
        public int Id { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; } 
        public Status Status { get; set; }
    }
}
