using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CoinTracker.Model;
using CoinTracker.Service;

namespace CoinTracker.ViewModel;


public partial class CurrencyConverterViewModel : ObservableObject
{
    private readonly CoinGeckoApiService _coinGeckoApi;

    [ObservableProperty]
    private ObservableCollection<Coin> _currencies;

    [ObservableProperty]
    private Coin _fromCurrency;

    [ObservableProperty]
    private Coin _toCurrency;

    [ObservableProperty]
    private decimal _amountToConvert = 1; 

    [ObservableProperty]
    private string _result;

    [ObservableProperty]
    private bool _isLoading = true;

    private string _resultKey;

    public CurrencyConverterViewModel()
    {
        _coinGeckoApi = new CoinGeckoApiService();
        Currencies = new ObservableCollection<Coin>();
        LoadDataAsync();
        SettingsService.LanguageChanged += OnLanguageChanged;
    }
    private void OnLanguageChanged()
    {
        if (!string.IsNullOrWhiteSpace(_resultKey))
        {
            Result = SettingsService.GetString(_resultKey);
        }
    }
    private async void LoadDataAsync()
    {
        IsLoading = true;
        var coinList = await _coinGeckoApi.GetCoinListAsync();
        foreach (var coin in coinList)
        {
            Currencies.Add(coin);
        }
        IsLoading = false;
    }
    [RelayCommand]
    private async Task ConvertAsync()
    {
        if (FromCurrency == null || ToCurrency == null || AmountToConvert <= 0)
        {
            _resultKey = "SelectCurrenciesMessage";
            Result = SettingsService.GetString(_resultKey);
            return;
        }

        if (FromCurrency.Current_Price == 0 || ToCurrency.Current_Price == 0)
        {
            _resultKey = "NoRateMessage";
            Result = SettingsService.GetString(_resultKey);
            return;
        }

        _resultKey = "ConvertingMessage";
        Result = SettingsService.GetString(_resultKey);

        decimal usdAmount = AmountToConvert * FromCurrency.Current_Price;
        decimal targetAmount = usdAmount / ToCurrency.Current_Price;

        string fromFormatted = AmountToConvert.ToString("N6").TrimEnd('0').TrimEnd(',');
        string toFormatted = targetAmount.ToString("N6").TrimEnd('0').TrimEnd(',');

        _resultKey = null;
        Result = $"{fromFormatted} {FromCurrency.Symbol.ToUpper()} = {toFormatted} {ToCurrency.Symbol.ToUpper()}";
    }




}





