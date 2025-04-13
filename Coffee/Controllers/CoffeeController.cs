using coffee.Data;
using coffee.Services;
using Microsoft.AspNetCore.Mvc;

namespace coffee.Controllers;

[ApiController]
[Route("brew-coffee")]
public class CoffeeController : ControllerBase
{
    private readonly ILogger<CoffeeController> _logger;
    private readonly IDateProvider _dateProvider;
    private readonly CoffeeCounterProvider _counter;
    private readonly ICoffeeService _coffeeService;
    
    public CoffeeController(ILogger<CoffeeController> logger,  IDateProvider dateProvider, CoffeeCounterProvider coffeeCounterProvider, ICoffeeService coffeeService)
    {
        _logger = logger;
        _dateProvider = dateProvider;
        _counter = coffeeCounterProvider;
        _coffeeService = coffeeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
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
            var response = await _coffeeService.GetCoffee();
            return Ok(response);
        }
    }
}