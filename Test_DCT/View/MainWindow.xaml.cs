
using System.Windows;
using Test_DCT.Service;

namespace Test_DCT.View
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