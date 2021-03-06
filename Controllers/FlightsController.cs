using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using alaska_airlines_test.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace alaska_airlines_test.Controllers
{
  public class FlightsController : Controller
  {
    private readonly FlightDbContext _context;
    public FlightsController(FlightDbContext context)
    {
      _context = context;
    }
    // GET: /flights

    public async Task<IActionResult> Index(string sort, string from, string to)
    {
      ViewData["SortByMainCabinPrice"] = sort == "main_cabin_price" ? "r_main_cabin_price" : "main_cabin_price";
      ViewData["SortByFirstClassPrice"] = sort == "first_class_price" ? "r_first_class_price" : "first_class_price";
      ViewData["SortByDeparture"] = sort == "departure_time" ? "r_departure_time" : "departure_time";
      var flights = from flight in _context.Flight select flight;

      ViewData["AllOrigins"] = await flights.Select(flight => flight.From).Distinct().ToListAsync();
      ViewData["AllDestinations"] = await flights.Select(flight => flight.To).Distinct().ToListAsync();
      ViewData["CurrentSearchFrom"] = "";
      ViewData["CurrentSearchTo"] = "";

      if(!string.IsNullOrEmpty(from))
      {
        flights = flights.Where(flight => flight.From == from);
        ViewData["CurrentSearchFrom"] = from;
      }
      if(!string.IsNullOrEmpty(to))
      {
        flights = flights.Where(flight => flight.To == to);
        ViewData["CurrentSearchTo"] = to;
      }

      switch (sort)
      {
        case "main_cabin_price":
          flights = flights.OrderBy(flight => flight.MainCabinPrice);
          break;
        case "r_main_cabin_price":
          flights = flights.OrderByDescending(flight => flight.MainCabinPrice);
          break;
        case "first_class_price":
          flights = flights.OrderBy(flight => flight.FirstClassPrice);
          break;
        case "r_first_class_price":
          flights = flights.OrderByDescending(flight => flight.FirstClassPrice);
          break;
        case "departure_time":
          flights = flights.OrderBy(flight => flight.Departs);
          break;
        case "r_departure_time":
          flights = flights.OrderByDescending(flight => flight.Departs);
          break;
        default:
          flights = flights.OrderBy(flight => flight.FlightNumber);
          break;
      }
      return View(await flights.AsNoTracking().ToListAsync());
    }
  }
}