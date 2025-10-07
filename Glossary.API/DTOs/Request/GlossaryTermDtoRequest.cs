namespace Glossary.API.DTOs.Request
{
    public class GlossaryTermDtoRequest
    {
        public string Term { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
    }
}
