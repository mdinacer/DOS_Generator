using DOS_Generator.Core.Models;
using DOS_Generator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DOS_Generator.Data.Repositories
{
    public class OfficerRepository : Repository<Officer>, IOfficerRepository
    {
        public OfficerRepository(DbContext context) : base(context)
        {
        }

        
    }
}