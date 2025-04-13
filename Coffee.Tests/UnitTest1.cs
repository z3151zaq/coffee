using Xunit;
using coffee.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using coffee.Data;
using coffee.Services;

public class CoffeeMachineControllerTests
{
    private CoffeeMachineController CreateController(DateTime? mockToday = null, string ip = "127.0.0.1", string timezone = "")
    {
        var mockLogger = new Mock<ILogger<CoffeeMachineController>>();
        var mockCounterProvider = new Mock<CoffeeCounterProvider>();
        var mockDateProvider = new Mock<IDateProvider>();
        if (mockToday.HasValue)
        {
            mockDateProvider.Setup(d => d.Today).Returns(mockToday.Value);
        }
        else
        {
            mockDateProvider.Setup(d => d.Today).Returns(DateTime.Today);
        }

        var controller = new CoffeeMachineController(mockLogger.Object, mockDateProvider.Object,mockCounterProvider.Object);

        return controller;
    }

    [Fact]
    public void Returns418()
    {
        var controller = CreateController(new DateTime(2025, 4, 1));
        var result = controller.Get();
        
        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(418, objectResult.StatusCode);
        Assert.Equal("I'm a teapot", objectResult.Value);
    }

    [Fact]
    public void Returns200()
    {
        var controller = CreateController();
        var result = controller.Get();
        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic body = okResult.Value;
        
        Assert.Contains("piping hot coffee", (string)body.message);
        Assert.NotNull(body.prepared);
    }

    [Fact]
    public void Returns503()
    {
        var controller = CreateController();

        for (int i = 0; i < 4; i++)
        {
            var result = controller.Get();
            Assert.IsType<OkObjectResult>(result);
        }

        var result503 = controller.Get();
        var status = Assert.IsType<ObjectResult>(result503);
        Assert.Equal(503, status.StatusCode);
    }
}