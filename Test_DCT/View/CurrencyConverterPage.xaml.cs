using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Test_DCT.Service;
using Test_DCT.ViewModel;

namespace Test_DCT.View
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
