using System.Net;
using System.Text.Json;
using coffee.Data;
using coffee.Services;
using Microsoft.Extensions.Configuration;
using Moq;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _fakeResponse;

    public FakeHttpMessageHandler(HttpResponseMessage fakeResponse)
    {
        _fakeResponse = fakeResponse;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_fakeResponse);
    }
}

public class CoffeeServiceTests
{
    [Theory]
    [InlineData(35, "iced")]
    [InlineData(31, "iced")]
    [InlineData(30, "hot")]
    [InlineData(10, "hot")]
    public async Task GetCoffee_ReturnsCorrectMessageBasedOnTemperature(double temp, string expectedKeyword)
    {
        var service = CreateServiceWithTemperature(temp);
        var result = await service.GetCoffee();
        Assert.Contains(expectedKeyword, result.message);
        Assert.False(string.IsNullOrWhiteSpace(result.prepared));
    }

    private CoffeeService CreateServiceWithTemperature(double temperature)
    {
        var fakeWeather = new WeatherResponse
        {
            Main = new Main { Temp = temperature }
        };

        var fakeJson = JsonSerializer.Serialize(fakeWeather);
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(fakeJson)
        };

        var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
        var httpClient = new HttpClient(fakeHandler);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["WeatherApiKey"]).Returns("fake-api-key");

        return new CoffeeService(httpClient, configMock.Object);
    }
}