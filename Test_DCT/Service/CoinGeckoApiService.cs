using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using Test_DCT.Model;

namespace Test_DCT.Service;

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


    
}
