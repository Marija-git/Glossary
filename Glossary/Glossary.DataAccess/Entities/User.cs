using Microsoft.AspNetCore.Identity;


namespace Glossary.DataAccess.Entities
{
    public class User : IdentityUser
    {
        public List<GlossaryTerm> OwnedGlossaryTerms { get; set; } = [];
    }
}
