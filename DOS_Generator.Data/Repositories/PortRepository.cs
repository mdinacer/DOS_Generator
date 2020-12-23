using DOS_Generator.Core.Models;
using DOS_Generator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DOS_Generator.Data.Repositories
{
    public class PortRepository : Repository<Port>, IPortRepository
    {
        public PortRepository(DbContext context) : base(context)
        {
        }

        private SecurityDbContext DbContext => Context as SecurityDbContext;
    }
}