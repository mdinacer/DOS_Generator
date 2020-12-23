using System.Collections.Generic;
using System.Threading.Tasks;
using DOS_Generator.Core.Models;

namespace DOS_Generator.Core.Repositories
{
    public interface IShipRepository : IRepository<Ship>
    {
        Task<IEnumerable<Ship>> GetByAgencyId(int id);
    }
}