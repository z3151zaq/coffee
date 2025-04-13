using coffee.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using coffee.Data;
using coffee.Services;

public class CoffeeControllerTests
{
    private CoffeeController CreateController(DateTime? mockToday = null)
    {
        var mockLogger = new Mock<ILogger<CoffeeController>>();
        var mockCounterProvider = new Mock<CoffeeCounterProvider>();
        var mockDateProvider = new Mock<IDateProvider>();
        var mockCoffeeService = new Mock<ICoffeeService>( );
        var expectedDto = new CoffeeDTO
        {
            prepared = "2025-04-13T12:00:00Z",
            message = "Your refreshing iced coffee is ready"
        };
        mockCoffeeService.Setup(s => s.GetCoffee()).ReturnsAsync(expectedDto);
        
        if (mockToday.HasValue)
        {
            mockDateProvider.Setup(d => d.Today).Returns(mockToday.Value);
        }
        else
        {
            mockDateProvider.Setup(d => d.Today).Returns(DateTime.Today);
        }

        var controller = new CoffeeController(mockLogger.Object, mockDateProvider.Object,mockCounterProvider.Object, mockCoffeeService.Object);

        return controller;
    }

    [Fact]
    public async Task Returns418()
    {
        var controller = CreateController(new DateTime(2025, 4, 1));
        var result = await controller.Get();
        
        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(418, objectResult.StatusCode);
        Assert.Equal("I'm a teapot", objectResult.Value);
    }

    [Fact]
    public async Task Returns200()
    {
        var controller = CreateController();
        var result = await controller.Get();
        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic body = okResult.Value;
        
        Assert.Contains("coffee is ready", (string)body.message);
        Assert.NotNull(body.prepared);
    }

    [Fact]
    public async Task Returns503()
    {
        var controller = CreateController();

        for (int i = 0; i < 4; i++)
        {
            var result = await controller.Get();
            Assert.IsType<OkObjectResult>(result);
        }

        var result503 = await controller.Get();
        var status = Assert.IsType<ObjectResult>(result503);
        Assert.Equal(503, status.StatusCode);
    }
}