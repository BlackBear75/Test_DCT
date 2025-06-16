using System.Windows.Controls;
using CoinTracker.ViewModel;
using CoinTracker.Model;

namespace CoinTracker.View
{
    /// <summary>
    /// Interaction logic for CoinDetailPage.xaml
    /// </summary>
    public partial class CoinDetailPage : Page
    {
        public CoinDetailPage(CoinMarketData coin)
        {
            InitializeComponent();
            DataContext = new CoinDetailViewModel(coin);
        }


    }
}
