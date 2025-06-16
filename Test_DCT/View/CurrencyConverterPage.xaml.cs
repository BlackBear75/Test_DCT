using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CoinTracker.Service;
using CoinTracker.ViewModel;

namespace CoinTracker.View
{
    /// <summary>
    /// Interaction logic for CurrencyConverterPage.xaml
    /// </summary>
    public partial class CurrencyConverterPage : Page
    {
        public CurrencyConverterPage(INavigationService navService)
        {
            InitializeComponent();
            DataContext = new CurrencyConverterViewModel();
        }
    }
}
