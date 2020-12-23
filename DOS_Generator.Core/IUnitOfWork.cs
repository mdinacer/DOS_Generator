using System;
using System.Threading.Tasks;
using DOS_Generator.Core.Repositories;

namespace DOS_Generator.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IAgencyRepository Agencies { get; }
        IDeclarationRepository Declarations { get; }
        IFacilityRepository Facilities { get; }
        IOfficerRepository Officers { get; }
        IPortRepository Ports { get; }
        IShipRepository Ships { get; }
        IUserRepository Users { get; }
        Task<int> CommitAsync();
    }
}