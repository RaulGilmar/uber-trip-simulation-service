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
            TempData["Alert"] = "Bienvenido, solicita tu viaje.";
            _logger.LogInformation("[Welcome] Bienvenido, solicita tu viaje. Cliente: {0}", customer?.FirstName);
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
                Origin = "Av. Siempre Viva 123",
                Status = TripStatus.Requested
            };
            _repo.Trips.Add(trip);

            TempData["Alert"] = $"Viaje solicitado por {customer.FirstName} desde {trip.Origin}";
            _logger.LogInformation("[StartTrip] Viaje solicitado por {0} desde {1}", customer.FirstName, trip.Origin);

            return RedirectToAction("SetDestination", new { tripId = trip.Id });
        }

        [HttpGet]
        public IActionResult SetDestination(int tripId)
        {
            TempData["Alert"] = "Por favor, ingresa tu destino.";
            _logger.LogInformation("[SetDestination] Esperando destino para el viaje {0}", tripId);
            return View(tripId);
        }

        [HttpPost]
        public IActionResult SetDestination(int tripId, string destination)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            trip.Destination = destination;

            trip.Amount = CalcularPrecio(trip.Origin, trip.Destination);

            TempData["Alert"] = $"Destino ingresado: {trip.Destination}. Precio estimado: ${trip.Amount}";
            _logger.LogInformation("[SetDestination] Destino ingresado: {0}. Precio estimado: ${1}", trip.Destination, trip.Amount);

            return RedirectToAction("ConfirmPrice", new { tripId = trip.Id });
        }

        [HttpGet]
        public IActionResult ConfirmPrice(int tripId)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            TempData["Alert"] = $"Confirma el viaje por ${trip.Amount} de {trip.Origin} a {trip.Destination}";
            _logger.LogInformation("[ConfirmPrice] Mostrando precio para confirmación. Viaje {0}", tripId);
            return View(trip);
        }

        [HttpPost]
        public IActionResult ConfirmPrice(int tripId, bool confirmar)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);

            if (!confirmar)
            {
                trip.Status = TripStatus.Cancelled;
                TempData["Alert"] = "El viaje fue cancelado por el usuario.";
                _logger.LogInformation("[ConfirmPrice] El viaje {0} fue cancelado por el usuario.", tripId);
                return RedirectToAction("Welcome", new { customerId = trip.Customer.Id });
            }

            var driver = _repo.Drivers.FirstOrDefault(d => d.IsAvailable);
            if (driver == null)
            {
                TempData["Alert"] = "No hay choferes disponibles.";
                _logger.LogInformation("[ConfirmPrice] No hay choferes disponibles para el viaje {0}.", tripId);
                return RedirectToAction("Welcome", new { customerId = trip.Customer.Id });
            }
            trip.Driver = driver;
            trip.Status = TripStatus.Accepted;
            driver.IsAvailable = false;

            TempData["Alert"] = $"Viaje confirmado con {driver.FirstName} {driver.LastName}";
            _logger.LogInformation("[ConfirmPrice] Viaje confirmado con {0} {1} para el viaje {2}", driver.FirstName, driver.LastName, tripId);

            return RedirectToAction("DriverArriving", new { tripId = trip.Id });
        }

        private decimal CalcularPrecio(string origen, string destino)
        {
            return 100m;
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
                    _logger.LogInformation("[DriverArriving] El chofer está a {0} cuadras. Viaje {1}", i, tripId);
                    await Task.Delay(1000);
                }
                distance = 0;
            } while (distance != 0);

            TempData["Alert"] = "Cliente a bordo, viaje iniciado.";
            _logger.LogInformation("[DriverArriving] Cliente a bordo, viaje iniciado. Viaje {0}", tripId);

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
                    _logger.LogInformation("[TripInProgress] Faltan {0} cuadras para llegar a destino. Viaje {1}", i, tripId);
                    await Task.Delay(1000);
                }
                distance = 0;
            } while (distance != 0);

            trip.Status = TripStatus.Finished;
            TempData["Alert"] = "Has llegado a destino.";
            _logger.LogInformation("[TripInProgress] Viaje finalizado. Viaje {0}", tripId);

            return RedirectToAction("FinishTrip", new { tripId = trip.Id });
        }

        [HttpGet]
        public IActionResult FinishTrip(int tripId)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            TempData["Alert"] = "Viaje finalizado";
            _logger.LogInformation("[FinishTrip] Viaje finalizado. Viaje {0}", tripId);
            return View(trip);
        }

        [HttpPost]
        public IActionResult RateTrip(int tripId, float rating)
        {
            var trip = _repo.Trips.First(t => t.Id == tripId);
            trip.Driver.Rating = rating;
            trip.Driver.IsAvailable = true;
            TempData["Alert"] = $"Calificación del chofer: {rating}";
            _logger.LogInformation("[RateTrip] Calificación del chofer: {0}. Viaje {1}", rating, tripId);
            return RedirectToAction("Welcome", new { customerId = trip.Customer.Id });
        }
    }
}
