using System.Collections.Generic;
using System.Threading.Tasks;
using DOS_Generator.Core.Models;

namespace DOS_Generator.Core.Repositories
{
    public interface IFacilityRepository : IRepository<Facility>
    {
        Task<Facility> GetWithPortById(int id); 
        Task<IEnumerable<Facility>> GetByPortId(int id);
    }
}