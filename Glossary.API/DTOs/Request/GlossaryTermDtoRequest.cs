namespace Glossary.API.DTOs.Request
{
    public class CreateGlossaryTermDtoRequest
    {
        public string Term { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
    }
}
