using System.Threading.Tasks;
using DOS_Generator.Core;
using DOS_Generator.Core.Repositories;
using DOS_Generator.Data.Repositories;

namespace DOS_Generator.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(SecurityDbContext context) => _context = context;
        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();

        #region Private properties

        private readonly SecurityDbContext _context;
        private DeclarationRepository _declarationRepository;
        private AgencyRepository _agencyRepository;
        private FacilityRepository _facilityRepository;
        private PortRepository _portRepository;
        private OfficerRepository _officerRepository;
        private ShipRepository _shipRepository;
        private UserRepository _userRepository;

        #endregion

        #region Public properties

        public IPortRepository Ports => _portRepository ??= new PortRepository(_context);
        public IAgencyRepository Agencies => _agencyRepository ??= new AgencyRepository(_context);
        public IDeclarationRepository Declarations => _declarationRepository ??= new DeclarationRepository(_context);
        public IFacilityRepository Facilities => _facilityRepository ??= new FacilityRepository(_context);
        public IOfficerRepository Officers => _officerRepository ??= new OfficerRepository(_context);
        public IShipRepository Ships => _shipRepository ??= new ShipRepository(_context);
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        #endregion
    }
}