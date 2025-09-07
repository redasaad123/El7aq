using Core.Enums;
using Web.Models.Trip;

namespace Web.Models.History
{
    public class HistoryViewModel
    {
        public List<BookingHistoryViewModel> Bookings { get; set; } = new List<BookingHistoryViewModel>();
        public List<TripHistoryViewModel> Trips { get; set; } = new List<TripHistoryViewModel>();
    }

    public class BookingHistoryViewModel
    {
        public string BookingId { get; set; } = string.Empty;
        public string TripId { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public TripSummaryViewModel Trip { get; set; } = new TripSummaryViewModel();
    }

    public class TripHistoryViewModel
    {
        public string TripId { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public string OriginStation { get; set; } = string.Empty;
        public string DestinationStation { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string CarNumber { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int PassengerBookingCount { get; set; }
    }
}
