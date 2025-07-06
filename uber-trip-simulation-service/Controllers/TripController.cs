using Microsoft.AspNetCore.Mvc;
using uber_trip_simulation_service.Models;

namespace uber_trip_simulation_service.Controllers
{
    public class TripController : Controller
    {
        private readonly AppRepository _repo;

        public TripController(AppRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Welcome(int customerId = 1)
        {
            var customer = _repo.Customers.FirstOrDefault(c => c.Id == customerId);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> StartTrip(int customerId)
        {
            var customer = _repo.Customers.First(c => c.Id == customerId);
            var trip = new Trip
            {
                Id = _repo.Trips.Count + 1,
                Customer = customer,
                Origin = "Av. Siempre Viva 123", // Hardcodeado
                Status = TripStatus.Requested
            };
            _repo.Trips.Add(trip);

            // Alert: viaje solicitado
            TempData["Alert"] = $"Viaje solicitado por {customer.FirstName} desde {trip.Origin}";

            // Espera input destino (redirige a vista para ingresar destino)
            return RedirectToAction("SetDestination", new { tripId = trip.Id });
        }

        [HttpGet]
        public IActionResult SetDestination(int tripId)
        {
            return View(tripId);
        }

        [HttpPost]
        public async Task<IActionResult> SetDestination(int tripId, string destination)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            trip.Destination = destination;

            // Buscar chofer disponible
            var driver = _repo.Drivers.FirstOrDefault(d => d.IsAvailable);
            if (driver == null)
            {
                TempData["Alert"] = "No hay choferes disponibles.";
                return RedirectToAction("Welcome", new { customerId = trip.Customer.Id });
            }
            trip.Driver = driver;
            trip.Status = TripStatus.Accepted;
            driver.IsAvailable = false;

            TempData["Alert"] = $"Viaje confirmado con {driver.FirstName} {driver.LastName}";

            // Simular llegada del chofer (do-while con for)
            return RedirectToAction("DriverArriving", new { tripId = trip.Id });
        }

        public async Task<IActionResult> DriverArriving(int tripId)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            int distance = 10;
            do
            {
                for (int i = distance; i >= 0; i--)
                {
                    TempData["Alert"] = $"El chofer está a {i} cuadras";
                    await Task.Delay(1000);
                }
                distance = 0;
            } while (distance != 0);

            trip.Status = TripStatus.InProgress;
            return RedirectToAction("TripInProgress", new { tripId = trip.Id });
        }

        public async Task<IActionResult> TripInProgress(int tripId)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            int distance = 10;
            do
            {
                for (int i = distance; i >= 0; i--)
                {
                    TempData["Alert"] = $"Faltan {i} cuadras para llegar a destino";
                    await Task.Delay(1000);
                }
                distance = 0;
            } while (distance != 0);

            trip.Status = TripStatus.Finished;
            return RedirectToAction("FinishTrip", new { tripId = trip.Id });
        }

        [HttpGet]
        public IActionResult FinishTrip(int tripId)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            TempData["Alert"] = "Viaje finalizado";
            return View(trip);
        }

        [HttpPost]
        public IActionResult RateTrip(int tripId, float rating)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            trip.Driver.Rating = rating;
            trip.Driver.IsAvailable = true;
            TempData["Alert"] = $"Calificación del chofer: {rating}";
            return RedirectToAction("Welcome", new { customerId = trip.Customer.Id });
        }
    }
}
