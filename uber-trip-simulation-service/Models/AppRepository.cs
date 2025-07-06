namespace uber_trip_simulation_service.Models
{
    public class AppRepository
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Driver> Drivers { get; set; } = new List<Driver>();
        public List<Trip> Trips { get; set; } = new List<Trip>();
    }
}
