using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CoinTracker.Model;
using CoinTracker.Service;
using CoinTracker.View;


public partial class MainWindowViewModel : ObservableObject
{
    private readonly CoinGeckoApiService _coinGeckoApi;

    private List<Coin> _allCoinsCache;

    [ObservableProperty]
    private string searchQuery;

    public ObservableCollection<Coin> SearchSuggestions { get; } = new ObservableCollection<Coin>();

    [ObservableProperty]
    private Coin selectedSuggestion;


    [ObservableProperty]
    private bool areSuggestionsVisible;

    private INavigationService _navigationService;
    public IRelayCommand SwitchToEnglishCommand { get; }
    public IRelayCommand SwitchToUkrainianCommand { get; }
    public IRelayCommand<string> ChangeThemeCommand { get; }
    public IRelayCommand NavigateToHomePageCommand { get; }

    public IRelayCommand NavigateToConverterPageCommand { get; }
    public IRelayCommand GoBackCommand { get; }
    public MainWindowViewModel()
    {
        _coinGeckoApi = new CoinGeckoApiService();



        SwitchToEnglishCommand = new RelayCommand(() => SettingsService.ChangeLanguage("en-US"));
        SwitchToUkrainianCommand = new RelayCommand(() => SettingsService.ChangeLanguage("uk-UA"));
        ChangeThemeCommand = new RelayCommand<string>(theme => SettingsService.ChangeTheme(theme));

        NavigateToHomePageCommand = new RelayCommand(() =>
        {
            _navigationService?.NavigateTo(new CryptoListPage(_navigationService));
        });
        NavigateToConverterPageCommand = new RelayCommand(() =>
        {
            _navigationService?.NavigateTo(new CurrencyConverterPage(_navigationService));
        });

        GoBackCommand = new RelayCommand(() =>
        {
            _navigationService?.GoBack();
        });

        this.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(SearchQuery))
            {
                await UpdateSearchSuggestionsAsync();
            }
        };

        this.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SelectedSuggestion) && SelectedSuggestion != null)
            {
                _ = NavigateToSelectedCoinAsync(SelectedSuggestion);
            }
        };
    }
    private async Task UpdateSearchSuggestionsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            SearchSuggestions.Clear();
            AreSuggestionsVisible = false;
            return;
        }

        if (_allCoinsCache == null)
            _allCoinsCache = await _coinGeckoApi.GetAllCoinsAsync();

        var filtered = _allCoinsCache
            .Where(c => c.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                     || c.Symbol.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                     || c.Id.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
            .Take(10)
            .ToList();

        SearchSuggestions.Clear();
        foreach (var coin in filtered)
            SearchSuggestions.Add(coin);

        AreSuggestionsVisible = SearchSuggestions.Any();
    }

    private async Task NavigateToSelectedCoinAsync(Coin coin)
    {
        AreSuggestionsVisible = false;
        SearchQuery = string.Empty;
        SelectedSuggestion = null;

        if (_navigationService == null)
            return;

        var coinMarketData = await _coinGeckoApi.GetCoinMarketDataAsync(coin.Id);
        if (coinMarketData != null)
        {
            _navigationService.NavigateTo(new CoinDetailPage(coinMarketData));
        }
        else
        {
            System.Windows.MessageBox.Show("Дані по монеті не знайдено.");
        }
    }
    public void SetNavigationService(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

}
