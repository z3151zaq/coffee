namespace coffee.Data;

public interface ICoffeeService
{
    Task<CoffeeDTO> GetCoffee();
}