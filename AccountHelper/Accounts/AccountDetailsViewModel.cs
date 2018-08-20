using AccountHelper.Models;
using CustomPresentationControls.Utilities;

namespace AccountHelper.Accounts
{
    public class AccountDetailsViewModel : ViewModel
    {
        private Account _account;
        public Account Account
        {
            get { return _account; }
            set { OnPropertyChanged(ref _account, value); }
        }
    }
}
