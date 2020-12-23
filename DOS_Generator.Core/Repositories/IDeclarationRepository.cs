using System.Collections.Generic;
using System.Threading.Tasks;
using DOS_Generator.Core.Models;

namespace DOS_Generator.Core.Repositories
{
    public interface IDeclarationRepository : IRepository<Declaration>
    {
        Task<IEnumerable<Declaration>> GetByOfficerId(int id);
        Task<IEnumerable<Declaration>> GetByShipId(int id);
        Task<IEnumerable<Declaration>> GetByFacilityId(int id);
        Task<Declaration> GetFullDeclarationById(int id);
    }
}