using Microsoft.AspNetCore.Mvc;
using Transportation.Interfaces;
using Transportation.Models;
using Microsoft.EntityFrameworkCore;

namespace Transportation.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    // ChatGPT, me daba error esta parte e hice la consulta
    public IActionResult Index([FromServices] IEnumerable<IAirplanes> airplanes)
    {
        using var db = new CarsContext();
        var customer = db.Customers.First((c) => c.LastName == "Mouse");
        var ownership = db.CustomerOwnerships.First((o) => o.CustomerId == customer.CustomerId);
        var vin = db.CarVins.First((v) => v.Vin == ownership.Vin);
        var model = db.Models.First((m) => m.ModelId == vin.Vin);
        var brand = db.Brands.First((b) => b.BrandId == model.BrandId);
        ViewData["BrandModel"] = $"{brand.BrandName} - {model.ModelName}";

        var dealer = db.Dealers
                       .Include(d => d.Brands) // ChatGPT
                       .First(d => d.Brands.Any(b => b.BrandId == brand.BrandId));

        ViewData["Dealer"] = $"{dealer.DealerName} - {dealer.DealerAddress}";

        var airbus = airplanes.First(a => a.GetBrand == "Airbus");
        var boeing = airplanes.First(a => a.GetBrand == "Boeing");

        ViewData["Airbus"] = $"{airbus.GetBrand}: {string.Join(" - ", airbus.GetModels)}";
        ViewData["Boeing"] = $"{boeing.GetBrand}: {string.Join(" - ", boeing.GetModels)}";

        return View();
    }
}
