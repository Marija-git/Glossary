using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glossary.DataAccess.SeedData
{
    public interface IDataSeeder
    {
        Task SeedAsync();
    }
}
