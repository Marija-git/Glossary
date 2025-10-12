using System.ComponentModel.DataAnnotations;

namespace Glossary.API.DTOs.Request
{
    public class RegisterDtoRequest
    {
        [Required(ErrorMessage = "This is a required field")]
        [MaxLength(256, ErrorMessage = "Username cannot exceed 256 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "This is a required field")]
        [MaxLength(256, ErrorMessage = "Passwrod cannot exceed 256 characters")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{8,}$",
       ErrorMessage = "Password must be at least 8 characters long, contain at least one number, one lowercase letter, one uppercase letter, and one special character.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "This is a required field")]
        [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

    }
}
