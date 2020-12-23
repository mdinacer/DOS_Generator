using DOS_Generator.Core.Models;
using DOS_Generator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DOS_Generator.Data.Repositories
{
    public class AgencyRepository : Repository<Agency>, IAgencyRepository
    {
        public AgencyRepository(DbContext context) : base(context)
        {
        }

        private SecurityDbContext DbContext => Context as SecurityDbContext;
    }
}