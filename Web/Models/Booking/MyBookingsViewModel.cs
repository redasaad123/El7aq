using El7aq.Domain.Enums;

namespace Web.Models.Booking
{
    public class MyBookingsViewModel
    {
        public List<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();
        public bool HasBookings => Bookings.Any();
        public int TotalBookings => Bookings.Count;
        public int ActiveBookings => Bookings.Count(b => b.Status != BookingStatus.Cancelled);
        public int CompletedTrips => Bookings.Count(b => b.Trip.DepartureTime < DateTime.Now);

    }
}
