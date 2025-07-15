using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using uber_trip_simulation_service.Models;
using uber_trip_simulation_service.Models.Auxiliares;

namespace uber_trip_simulation_service.Controllers
{
    public class TripController : Controller
    {
        private readonly AppRepository _repo;
        private readonly ILogger<TripController> _logger;

        public TripController(AppRepository repo, ILogger<TripController> logger)
        {
            _repo = repo;
            _logger = logger;
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

            var customer = trip.Customer;

            // Alert: viaje solicitado
            TempData["Alert"] = $"Viaje solicitado por {customer.FirstName} desde {trip.Origin}";
            _logger.LogInformation($"[SetDestination]\n      Viaje solicitado por {customer.FirstName} desde {trip.Origin} hasta {trip.Destination}.\n      Trip {trip.Status}\n      ----------");

            // Buscar chofer disponible
            var driver = _repo.Drivers.FirstOrDefault(d => d.IsAvailable);
            if (driver == null)
            {
                TempData["Alert"] = "No hay choferes disponibles.";
                _logger.LogInformation($"[SetDestination]\n      No hay choferes disponibles.\n      ----------");

                return RedirectToAction("Welcome", new { customerId = trip.Customer.Id });
            }
            trip.Driver = driver;
            trip.Status = TripStatus.Accepted;
            driver.IsAvailable = false;

            TempData["Alert"] = $"Viaje confirmado con {driver.FirstName} {driver.LastName}";
            _logger.LogInformation($"[SetDestination]\n      Viaje confirmado con {driver.FirstName} {driver.LastName} {driver.CarBrand} {driver.CarModel} {driver.LicensePlate}\n      Trip {trip.Status}\n      ----------");

            // Simular llegada del chofer (do-while con for)
            return RedirectToAction("DriverArriving", new { tripId = trip.Id });
        }

        public async Task<IActionResult> DriverArriving(int tripId)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            int distance = 3;
            trip.Status = TripStatus.InProgress;

            do
            {
                for (int i = distance; i >= 0; i--)
                {
                    TempData["Alert"] = $"El chofer está a {i} cuadras";
                    _logger.LogInformation($"[DriverArriving]\n      El chofer está a {i} cuadras.\n      Trip {trip.Status}\n      ----------");
                    await Task.Delay(1000);
                }
                distance = 0;
            } while (distance != 0);

            _logger.LogInformation($"[DriverArriving]\n      Cliente a bordo del Uber, viaje iniciado.\n      Trip {trip.Status}\n      --------");

            return RedirectToAction("TripInProgress", new { tripId = trip.Id });
        }

        public async Task<IActionResult> TripInProgress(int tripId)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            int distance = 5;
            do
            {
                for (int i = distance; i >= 0; i--)
                {
                    TempData["Alert"] = $"Faltan {i} cuadras para llegar a destino";
                    _logger.LogInformation($"[TripInProgress]\n      Faltan {i} cuadras para llegar a destino\n      Trip {trip.Status}\n      ----------");
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
            _logger.LogInformation($"[FinishTrip]\n      Viaje finalizado.\n      Trip {trip.Status}\n      ----------");
            return View(trip);
        }

        [HttpPost]
        public IActionResult RateTrip(int tripId, float rating)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            trip.Driver.Rating = rating;
            trip.Driver.IsAvailable = true;
            TempData["Alert"] = $"Calificación del chofer: {rating}";
            _logger.LogInformation($"[RateTrip]\n      Calificación del chofer: {rating}\n      Trip {trip.Status}\n      ----------");
            return RedirectToAction("Welcome", new { customerId = trip.Customer.Id });
        }
    }
}
