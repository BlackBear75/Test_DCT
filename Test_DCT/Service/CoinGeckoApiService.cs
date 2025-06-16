using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using CoinTracker.Model;

namespace CoinTracker.Service;

public class CoinGeckoApiService
{
    private readonly HttpClient _httpClient;

    public CoinGeckoApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.coingecko.com/api/v3/")
        };

        _httpClient.DefaultRequestHeaders.Add("x-cg-demo-api-key", "CG-FbvfN8QwKFtAHdjsDNWNhgam");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MyApp/1.0)");
    }

   

    public async Task<List<CoinMarketData>> GetTopCoinsAsync(int count = 10)
    {
        var url = $"coins/markets?vs_currency=usd&order=market_cap_desc&per_page={count}&page=1&sparkline=false";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<CoinMarketData>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
    public async Task<List<Market>> GetMarketsAsync(string coinId)
    {
        if (string.IsNullOrWhiteSpace(coinId))
            throw new ArgumentException("coinId must not be null or empty");

        var response = await _httpClient.GetAsync($"coins/{coinId}/tickers");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        var result = new List<Market>();
        using var doc = JsonDocument.Parse(json);
        var tickers = doc.RootElement.GetProperty("tickers");

        foreach (var ticker in tickers.EnumerateArray())
        {
            try
            {
                var market = ticker.GetProperty("market").GetProperty("name").GetString();
                var baseCurrency = ticker.GetProperty("base").GetString();
                var targetCurrency = ticker.GetProperty("target").GetString();
                var price = ticker.GetProperty("last").GetDecimal();
                var tradeUrl = ticker.TryGetProperty("trade_url", out var tradeProp) && tradeProp.ValueKind == JsonValueKind.String
                    ? tradeProp.GetString()
                    : null;

                result.Add(new Market
                {
                    ExchangeName = market,
                    Pair = $"{baseCurrency}/{targetCurrency}",
                    Price = price,
                    Url = tradeUrl
                });
            }
            catch
            {
                continue;
            }
        }

        return result;
    }
    public async Task<List<OhlcPoint>> GetCoinOhlcAsync(string coinId, int days)
    {
        var response = await _httpClient.GetAsync($"coins/{coinId}/ohlc?vs_currency=usd&days={days}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var rawData = JsonSerializer.Deserialize<List<List<decimal>>>(json);

        return rawData?.Select(candle => new OhlcPoint
        {
            Date = DateTimeOffset.FromUnixTimeMilliseconds((long)candle[0]).DateTime,
            Open = candle[1],
            High = candle[2],
            Low = candle[3],
            Close = candle[4]
        }).ToList() ?? new List<OhlcPoint>();
    }

    public async Task<List<Coin>> GetCoinListAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("coins/markets?vs_currency=usd&order=market_cap_desc&per_page=100&page=1");
            response.EnsureSuccessStatusCode();
            var coins = await response.Content.ReadFromJsonAsync<List<Coin>>();
            return coins ?? new List<Coin>();
        }
        catch (HttpRequestException e)
        {
            MessageBox.Show($"API request error: {e.Message}");
            return new List<Coin>();
        }
    }

    public async Task<List<Coin>> GetAllCoinsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("coins/list");
            response.EnsureSuccessStatusCode();
            var coins = await response.Content.ReadFromJsonAsync<List<Coin>>();
            return coins ?? new List<Coin>();
        }
        catch (HttpRequestException e)
        {
            MessageBox.Show($"Помилка запиту до API: {e.Message}");
            return new List<Coin>();
        }
    }

    public async Task<CoinMarketData?> GetCoinMarketDataAsync(string coinId)
    {
        try
        {
            var url = $"coins/markets?vs_currency=usd&ids={coinId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<List<CoinMarketData>>();
            return list?.FirstOrDefault();
        }
        catch (HttpRequestException e)
        {
            MessageBox.Show($"Помилка запиту до API: {e.Message}");
            return null;
        }
    }

}
