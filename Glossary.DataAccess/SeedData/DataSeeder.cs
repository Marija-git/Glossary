using Glossary.DataAccess.AppData;
using Glossary.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Glossary.DataAccess.SeedData
{
    public class DataSeeder : IDataSeeder
    {
        private readonly GlossaryDbContext _context;
        private readonly UserManager<User> _userManager;

        public DataSeeder(GlossaryDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
   
        public async  Task SeedAsync()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }

            if (await _context.Database.CanConnectAsync())
            {

               /* _context.GlossaryTerms.RemoveRange(_context.GlossaryTerms);
                var users0 = _userManager.Users.ToList();
                foreach (var user in users0)
                {
                    await _userManager.DeleteAsync(user);
                }
                await _context.SaveChangesAsync();
               */

                var users = new List<(string UserName, string Email, string Password)>
                {
                    ("Author1", "author1@example.com", "Author1123!"),
                    ("Author2", "author2@example.com", "Author2123!")
                };

                var createdUsers = new List<User>();

                foreach (var (userName, email, password) in users)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new User
                        {
                            UserName = userName,
                            Email = email,
                            EmailConfirmed = true
                        };
                        await _userManager.CreateAsync(user, password);
                    }
                    createdUsers.Add(user);
                }

                if (!_context.GlossaryTerms.Any())
                {
                    _context.GlossaryTerms.AddRange(
                    new GlossaryTerm
                    {
                        Term = "algorithm",
                        Definition = "Step-by-step procedure for calculations.",
                        Status = Status.Published,
                        AuthorId = createdUsers[0].Id
                    },
                    new GlossaryTerm
                    {
                        Term = "binary tree",
                        Definition = "Tree data structure with at most two children per node.",
                        Status = Status.Draft,
                        AuthorId = createdUsers[0].Id
                    },
                    new GlossaryTerm
                    {
                        Term = "refactoring",
                        Definition = "Improving code without changing behavior.",
                        Status = Status.Draft,
                        AuthorId = createdUsers[1].Id
                    }
                    );
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
