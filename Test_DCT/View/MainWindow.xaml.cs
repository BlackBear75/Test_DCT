
using System.Windows;
using CoinTracker.Service;

namespace CoinTracker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public INavigationService NavigationService { get; }

        public MainWindow()
        {
            InitializeComponent();

            NavigationService = new NavigationService(MainFrame);

            var viewModel = new MainWindowViewModel();
            viewModel.SetNavigationService(NavigationService);
            DataContext = viewModel;

            MainFrame.Navigate(new CryptoListPage(NavigationService));
        }
    }
}