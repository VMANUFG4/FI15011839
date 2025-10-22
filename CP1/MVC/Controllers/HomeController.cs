using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Controllers;
//****Uso de ChatGPT
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(TheModel model)
    {
        ViewBag.Valid = ModelState.IsValid;
        if (ViewBag.Valid)
        {
            var filteredChars = model.Phrase!.Where(c => c != ' ').ToList();
            filteredChars.ForEach(c =>
            {
                if (!model.Counts!.ContainsKey(c))
                {
                    model.Counts[c] = 0;
                }
                model.Counts[c]++;
            });
            model.Lower = string.Concat(filteredChars.Select(c => char.ToLower(c)));
            model.Upper = string.Concat(filteredChars.Select(c => char.ToUpper(c)));
        }
        return View(model);
    }
}
