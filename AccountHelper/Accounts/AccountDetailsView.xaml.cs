using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AccountHelper.Accounts
{
    /// <summary>
    /// Interaction logic for AccountDetailsView.xaml
    /// </summary>
    public partial class AccountDetailsView : UserControl
    {
        public AccountDetailsView()
        {
            InitializeComponent();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
