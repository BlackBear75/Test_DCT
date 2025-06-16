using System.Windows.Controls;
using Test_DCT.ViewModel;
using Test_DCT.Model;

namespace Test_DCT.View
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
