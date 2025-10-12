using System.ComponentModel.DataAnnotations;

namespace Glossary.API.DTOs.Request
{
    public class LoginDtoRequest
    {
        [Required(ErrorMessage = "This is a required field")]
        public string Username { get; set; }
        [Required(ErrorMessage = "This is a required field")]
        public string Password { get; set; }
    }
}
