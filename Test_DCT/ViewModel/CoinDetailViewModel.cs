using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CoinTracker.Model;
using CoinTracker.Service;

namespace CoinTracker.ViewModel;

public partial class CoinDetailViewModel : ObservableObject
{
    private CoinMarketData _selectedCoin;
    private readonly CoinGeckoApiService _geckoApiService;

    public decimal Price => SelectedCoin?.CurrentPrice ?? 0;
    public decimal Volume24h => SelectedCoin?.TotalVolume ?? 0;
    public decimal PercentChange24h => SelectedCoin?.PriceChangePercentage24h ?? 0;
    public string LogoUrl => SelectedCoin?.Image;

    private int _selectedPeriod = 1;
    public int SelectedPeriod
    {
        get => _selectedPeriod;
        set
        {
            if (SetProperty(ref _selectedPeriod, value))
            {
                LoadOhlcChartDataAsync(_selectedPeriod);
            }
        }
    }

    [ObservableProperty]
    private PlotModel priceChartModel;
    public PlotController PlotController { get; } = new PlotController();
    public CoinMarketData SelectedCoin
    {
        get => _selectedCoin;
        set
        {
            SetProperty(ref _selectedCoin, value);
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(Volume24h));
            OnPropertyChanged(nameof(PercentChange24h));
            OnPropertyChanged(nameof(LogoUrl));
        }
    }

    public ObservableCollection<Market> Markets { get; } = new ObservableCollection<Market>();
    public ICommand NavigateToMarketCommand { get; }

    public CoinDetailViewModel(CoinMarketData coin)
    {
        PlotController.BindMouseDown(OxyMouseButton.Right, PlotCommands.PanAt);

        SelectedCoin = coin;
        _geckoApiService = new CoinGeckoApiService();

        InitializeAsync();

        NavigateToMarketCommand = new RelayCommand<string>(OpenMarketUrl);

        SettingsService.LanguageChanged += OnLanguageChanged;
        SettingsService.ThemeChanged += OnThemeChanged;
    }
    private void OnThemeChanged()
    {
        LoadOhlcChartDataAsync(SelectedPeriod);
    }
    private void OnLanguageChanged()
    {
        LoadOhlcChartDataAsync(SelectedPeriod);
    }
   

    ~CoinDetailViewModel()
    {
        SettingsService.LanguageChanged -= OnLanguageChanged;
        SettingsService.ThemeChanged -= OnThemeChanged;

    }

    private async void InitializeAsync()
    {
        await LoadMarketsAsync();
        await LoadOhlcChartDataAsync(1);
    }

    private async Task LoadMarketsAsync()
    {
        if (SelectedCoin == null)
            return;

        var markets = await _geckoApiService.GetMarketsAsync(SelectedCoin.Id);

        Markets.Clear();
        foreach (var market in markets)
        {
            Markets.Add(market);
        }
    }

    private void OpenMarketUrl(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch {  }
        }
    }

    private async Task LoadOhlcChartDataAsync(int days)
    {
        if (SelectedCoin == null) return;

        var ohlcData = await _geckoApiService.GetCoinOhlcAsync(SelectedCoin.Id, days);

        var titleFormat = SettingsService.GetString("ChartTitleFormat");
        var model = new PlotModel { Title = string.Format(titleFormat, days) };

        var backgroundBrush = Application.Current.TryFindResource("ChartBackgroundBrush") as System.Windows.Media.SolidColorBrush;
        var textBrush = Application.Current.TryFindResource("ChartTextBrush") as System.Windows.Media.SolidColorBrush;
        var increasingBrush = Application.Current.TryFindResource("CandleIncreasingBrush") as System.Windows.Media.SolidColorBrush;
        var decreasingBrush = Application.Current.TryFindResource("CandleDecreasingBrush") as System.Windows.Media.SolidColorBrush;

        var oxyBackground = backgroundBrush != null ? OxyColor.FromArgb(backgroundBrush.Color.A, backgroundBrush.Color.R, backgroundBrush.Color.G, backgroundBrush.Color.B) : OxyColors.White;
        var oxyText = textBrush != null ? OxyColor.FromArgb(textBrush.Color.A, textBrush.Color.R, textBrush.Color.G, textBrush.Color.B) : OxyColors.Black;
        var oxyIncreasing = increasingBrush != null ? OxyColor.FromArgb(increasingBrush.Color.A, increasingBrush.Color.R, increasingBrush.Color.G, increasingBrush.Color.B) : OxyColors.Green;
        var oxyDecreasing = decreasingBrush != null ? OxyColor.FromArgb(decreasingBrush.Color.A, decreasingBrush.Color.R, decreasingBrush.Color.G, decreasingBrush.Color.B) : OxyColors.Red;

        model.Background = oxyBackground;
        model.TextColor = oxyText;

        model.Axes.Add(new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            StringFormat = "dd.MM HH:mm",
            Title = SettingsService.GetString("DateAxisTitle"),
            IntervalType = DateTimeIntervalType.Hours,
            MinorIntervalType = DateTimeIntervalType.Minutes,
            IsZoomEnabled = true,
            IsPanEnabled = true,
            TextColor = oxyText,
            TitleColor = oxyText,
            AxislineColor = oxyText,
            MinorTicklineColor = oxyText
        });

        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = SettingsService.GetString("PriceAxisTitle"),
            MinimumPadding = 0.1,
            MaximumPadding = 0.1,
            IsZoomEnabled = true,
            IsPanEnabled = true,
            TextColor = oxyText,
            TitleColor = oxyText,
            AxislineColor = oxyText,
            MinorTicklineColor = oxyText
        });

        var candleSeries = new CandleStickSeries
        {
            IncreasingColor = oxyIncreasing,
            DecreasingColor = oxyDecreasing
        };

        foreach (var p in ohlcData)
        {
            candleSeries.Items.Add(new HighLowItem(
                DateTimeAxis.ToDouble(p.Date),
                (double)p.High, (double)p.Low,
                (double)p.Open, (double)p.Close
            ));
        }

        model.Series.Add(candleSeries);

        PriceChartModel = model;
    }



}