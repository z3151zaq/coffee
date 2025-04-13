using System.Text.Json;
using coffee.Data;

namespace coffee.Services;

public class CoffeeService: ICoffeeService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public CoffeeService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<CoffeeDTO> GetCoffee()
    {
        try{
            var response = 
                await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?units=metric&lat={36.85}&lon={174.76}&appid={_configuration["WeatherApiKey"]}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var weather = JsonSerializer.Deserialize<WeatherResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return new CoffeeDTO()
            {
                prepared = DateTimeOffset.Now.ToString("O"),
                message = weather.Main.Temp > 30 ? "Your refreshing iced coffee is ready" : "Your piping hot coffee is ready",
            };
        }
        catch (Exception)
        {
            return new CoffeeDTO()
            {
                prepared = DateTimeOffset.Now.ToString("O"),
                message = "Your piping hot coffee is ready",
            };
        }
    }
    

}