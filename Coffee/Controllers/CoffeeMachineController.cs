using coffee.Data;
using coffee.Services;
using Microsoft.AspNetCore.Mvc;

namespace coffee.Controllers;

[ApiController]
[Route("brew-coffee")]
public class CoffeeMachineController : ControllerBase
{
    private readonly ILogger<CoffeeMachineController> _logger;
    private readonly IDateProvider _dateProvider;
    private readonly CoffeeCounterProvider _counter;
    
    public CoffeeMachineController(ILogger<CoffeeMachineController> logger,  IDateProvider dateProvider, CoffeeCounterProvider coffeeCounterProvider)
    {
        _logger = logger;
        _dateProvider = dateProvider;
        _counter = coffeeCounterProvider;
    }

    [HttpGet]
    public IActionResult Get()
    {   
        DateTime today = _dateProvider.Today;
        if (today.Month == 4 && today.Day == 1)
        {
            return StatusCode(418, "I'm a teapot");
        }

        _counter.value++;
        if (_counter.value == 5)
        {
            _counter.value = 0;
            return StatusCode(503, null);
        } 
        else
        {
            var response = new CoffeeDTO()
            {
                prepared = DateTimeOffset.Now.ToString("O"),
                message = "Your piping hot coffee is ready",
            };
            return Ok(response);
        }
    }
}