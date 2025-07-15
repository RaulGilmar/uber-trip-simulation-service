namespace uber_trip_simulation_service.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string LicensePlate { get; set; }
        public float Rating { get; set; }
        public bool IsAvailable { get; set; }
        public int PhoneNumber { get; set; }
    }
}
