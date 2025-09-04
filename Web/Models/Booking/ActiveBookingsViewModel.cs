namespace Web.Models.Booking
{
    public class ActiveBookingsViewModel
    {
        public List<ActiveBookingViewModel> ActiveBookings { get; set; } = new List<ActiveBookingViewModel>();
        public bool HasActiveBookings => ActiveBookings.Any();
        public int TotalActiveBookings => ActiveBookings.Count;
        public ActiveBookingViewModel NextTrip => ActiveBookings.OrderBy(b => b.Trip.DepartureTime).FirstOrDefault();
    }
}
