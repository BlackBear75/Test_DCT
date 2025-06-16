using System.Windows;
using System.Windows.Controls;
using CoinTracker.Service;

namespace CoinTracker.View;

    /// <summary>
    /// Interaction logic for CryptoListPage.xaml
    /// </summary>
    public partial class CryptoListPage : Page
    {
        public CryptoListPage(INavigationService navigationService)
        {
            InitializeComponent();
            DataContext = new CryptoListPageViewModel(navigationService);
        }

        private void CryptoListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CryptoListView.View is GridView gridView)
            {
                double totalWidth = CryptoListView.ActualWidth - SystemParameters.VerticalScrollBarWidth;

                if (gridView.Columns.Count == 0) return;

                double columnWidth = totalWidth / gridView.Columns.Count;

                foreach (var column in gridView.Columns)
                {
                    column.Width = columnWidth;
                }
            }
        }

    }


