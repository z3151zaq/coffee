namespace coffee.Services;
using coffee.Data;

public class SystemDateProvider : IDateProvider
{
    public DateTime Today => DateTime.Today;
}
