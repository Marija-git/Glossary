using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.DataAccess.SeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glossary.BusinessLogic.Services
{
    public class DataSeederService : IDataSeederService
    {
        private readonly IDataSeeder _dataSeeder;
        public DataSeederService(IDataSeeder dataSeeder)
        {
            _dataSeeder = dataSeeder;
        }
        public async Task SeedAsync()
        {
            await _dataSeeder.SeedAsync();
        }
    }
}
