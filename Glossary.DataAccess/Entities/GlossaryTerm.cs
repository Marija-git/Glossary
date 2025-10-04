using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glossary.DataAccess.Entities
{
    public class GlossaryTerm
    {
        public int Id { get; set; }
        public string Term { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public Status Status { get; set; } = Status.Draft;

        public string AuthorId { get; set; }
        public User Author { get; set; }
    }
}
