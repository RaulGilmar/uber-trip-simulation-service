namespace uber_trip_simulation_service.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Driver Driver { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public TripStatus Status { get; set; }
        public decimal Amount { get; set; }
    }
}
