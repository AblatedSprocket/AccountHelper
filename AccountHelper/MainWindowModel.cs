using AccountHelper.Utilities;
using CustomPresentationControls;
using CustomPresentationControls.Authentication;
using CustomPresentationControls.Utilities;

namespace AccountHelper
{
    class MainWindowModel : ObservableObject
    {
        private ObservableObject _currentViewModel;
        public ObservableObject CurrentViewModel
        {
            get { return _currentViewModel; }
            set { OnPropertyChanged(ref _currentViewModel, value); }
        }
        public MainWindowModel()
        {
            Logger.Log("Initializing main window.");
            AuthenticationViewModel authenticationViewModel = new AuthenticationViewModel();
            authenticationViewModel.UserAuthenticated += OnUserAuthenticated;
            CurrentViewModel = authenticationViewModel;
        }

        private void OnUserAuthenticated(object sender, UserAuthenticatedEventArgs e)
        {
            MainViewModel mainViewModel = new MainViewModel(e.Username);
            CurrentViewModel = mainViewModel;
        }
    }
}
