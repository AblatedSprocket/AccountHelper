using System.Windows;
using System.Windows.Controls;

namespace AccountHelper
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }
        private void OnSelectedGroupingChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is IMainViewModel viewModel && viewModel.SelectedGroupingChangedCommand.CanExecute(null))
            {
                viewModel.SelectedGroupingChangedCommand.Execute(null);
            }
        }
        private void OnSelectedAccountChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is IMainViewModel viewModel && viewModel.SelectedAccountChangedCommand.CanExecute(null))
            {
                viewModel.SelectedAccountChangedCommand.Execute(null);
            }
        }
    }
}
