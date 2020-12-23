using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOS_Generator.Core.Models;
using DOS_Generator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DOS_Generator.Data.Repositories
{
    public class ShipRepository : Repository<Ship>, IShipRepository
    {
        public ShipRepository(DbContext context) : base(context)
        {
        }

        private SecurityDbContext DbContext => Context as SecurityDbContext;


        public async Task<IEnumerable<Ship>> GetByAgencyId(int id)
        {
            return await DbContext.Ships
                .Where(ship => ship.AgencyId == id)
                .ToListAsync();
        }
    }
}