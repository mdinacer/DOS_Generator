using System.Threading.Tasks;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;

namespace DOS_Generator.WPF.ViewModels.Forms
{
    public class AgencyFormViewModel : ViewModelBase
    {
        #region Constructors

        public AgencyFormViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AgencyFormViewModel()
        {
            
        }

        #endregion

        #region Properties

        public Agency Agency { get; set; } = new Agency();
        public bool IsEdit { get; set; }

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Functions

        public async Task LoadAgency(int agencyId)
        {
            var agency = await _unitOfWork.Agencies.GetByIdAsync(agencyId);
            if (agency == null) return;
            IsEdit = true;
            Agency = agency;
        }

        public async Task<Agency> SaveAgency()
        {
            if (IsEdit)
                _unitOfWork.Agencies.Update(Agency);
            else await _unitOfWork.Agencies.AddAsync(Agency);
            await _unitOfWork.CommitAsync();
            return Agency;
        }

        #endregion

        #region Events

        #endregion

        #region Commands

        #endregion
    }
}