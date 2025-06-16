using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CoinTracker.Model;
using CoinTracker.Service;
using CoinTracker.View;


public partial class CryptoListPageViewModel : ObservableObject
{
    private readonly CoinGeckoApiService _geckoApiService;

    public ObservableCollection<CoinMarketData> Cryptos { get; } = new();

    [ObservableProperty]
    private bool isLoading;

    private readonly INavigationService _navigationService;
    public IAsyncRelayCommand LoadDataCommand { get; }
    public IRelayCommand<CoinMarketData> OpenDetailCommand { get; }


    public CryptoListPageViewModel(INavigationService navigationService)
    {


        _navigationService = navigationService;

        _geckoApiService = new CoinGeckoApiService();
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        OpenDetailCommand = new RelayCommand<CoinMarketData>(OpenDetail);

    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;

        var coins = await _geckoApiService.GetTopCoinsAsync(10); 
        Cryptos.Clear();

        if (coins != null)
        {
            foreach (var c in coins)
                Cryptos.Add(c);
        }

        IsLoading = false;
    }


    public void OpenDetail(CoinMarketData selectedCoin)
    {
        var detailPage = new CoinDetailPage(selectedCoin);
        _navigationService.NavigateTo(detailPage);
    }



}
