using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Test_DCT.Service;
using Test_DCT.View;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly CoinGeckoApiService _coinGeckoApi;




    [ObservableProperty]
    private bool areSuggestionsVisible;

    private INavigationService _navigationService;
    public IRelayCommand SwitchToEnglishCommand { get; }
    public IRelayCommand SwitchToUkrainianCommand { get; }
    public IRelayCommand<string> ChangeThemeCommand { get; }
    public IRelayCommand NavigateToHomePageCommand { get; }
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

        GoBackCommand = new RelayCommand(() =>
        {
            _navigationService?.GoBack();
        });

    }

    public void SetNavigationService(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

}
