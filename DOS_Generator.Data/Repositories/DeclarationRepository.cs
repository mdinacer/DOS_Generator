using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOS_Generator.Core.Models;
using DOS_Generator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DOS_Generator.Data.Repositories
{
    public class DeclarationRepository : Repository<Declaration>, IDeclarationRepository
    {
        public DeclarationRepository(DbContext context) : base(context)
        {
        }

        private SecurityDbContext DbContext => Context as SecurityDbContext;

        public async Task<IEnumerable<Declaration>> GetByOfficerId(int id)
        {
            return await DbContext.Declarations
                .Where(declaration => declaration.OfficerId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Declaration>> GetByShipId(int id)
        {
            return await DbContext.Declarations
                .Where(declaration => declaration.ShipId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Declaration>> GetByFacilityId(int id)
        {
            return await DbContext.Declarations
                .Where(declaration => declaration.FacilityId == id)
                .ToListAsync();
        }

        public Task<Declaration> GetFullDeclarationById(int id)
        {
            return DbContext.Declarations
                .Include(declaration => declaration.Facility)
                .Include(declaration => declaration.Officer)
                .Include(declaration => declaration.Facility.Port)
                .SingleOrDefaultAsync(declaration => declaration.Id == id);
        }
    }
}