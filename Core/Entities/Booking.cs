using El7aq.Domain.Enums;

namespace El7aq.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int PassengerId { get; set; }
        public int TripId { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }

        // Navigation
        public PassengerProfile Passenger { get; set; }
        public Trip Trip { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }


}
