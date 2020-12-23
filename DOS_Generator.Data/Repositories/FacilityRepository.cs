using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOS_Generator.Core.Models;
using DOS_Generator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DOS_Generator.Data.Repositories
{
    public class FacilityRepository : Repository<Facility>, IFacilityRepository
    {
        public FacilityRepository(DbContext context) : base(context)
        {
        }

        private SecurityDbContext DbContext => Context as SecurityDbContext;

        public async Task<Facility> GetWithPortById(int id)
        {
            return await DbContext.Facilities
                .Include(facility => facility.Port)
                .SingleOrDefaultAsync(facility => facility.Id == id);
        }

        public async Task<IEnumerable<Facility>> GetByPortId(int id)
        {
            return await DbContext.Facilities
                .Where(facility => facility.PortId == id)
                .ToListAsync();
        }
    }
}