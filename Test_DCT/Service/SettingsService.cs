using System.Globalization;
using System.Windows;

namespace CoinTracker.Service
{
    public static class SettingsService
    {
        private const string LocalizationDictName = "StringResources";
        private const string ThemeDictName = "Theme";

        public static event Action LanguageChanged;

        public static event Action ThemeChanged;
        public static void ChangeLanguage(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            var dict = new ResourceDictionary();
            switch (culture)
            {
                case "uk-UA":
                    dict.Source = new Uri("Resources/Localization/StringResources.uk-UA.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Resources/Localization/StringResources.xaml", UriKind.Relative);
                    break;
            }

            ReplaceResourceDictionary(LocalizationDictName, dict);

            LanguageChanged?.Invoke();
        }


        public static void ChangeTheme(string theme)
        {
            var dict = new ResourceDictionary();
            switch (theme)
            {
                case "Dark":
                    dict.Source = new Uri("Resources/Themes/DarkTheme.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Resources/Themes/LightTheme.xaml", UriKind.Relative);
                    break;
            }
            ReplaceResourceDictionary(ThemeDictName, dict);

            ThemeChanged?.Invoke();
        }


        private static void ReplaceResourceDictionary(string dictionaryKey, ResourceDictionary newDict)
        {
            var mergedDicts = Application.Current.Resources.MergedDictionaries;

            var oldDict = mergedDicts.FirstOrDefault(
                d => d.Source != null && d.Source.OriginalString.Contains(dictionaryKey));

            if (oldDict != null)
            {
                mergedDicts.Remove(oldDict);
            }

            mergedDicts.Add(newDict);
        }

     
        public static string GetString(string key)
        {
            var resource = Application.Current.TryFindResource(key);
            return resource as string ?? $"[{key}]";
        }

    }
}