using Core.Enums;
using Web.Models.Trip;

namespace Web.Models.Booking
{
    public class BookingViewModel
    {
        public string BookingId { get; set; }
        public string TripId { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }
        public string StatusText { get; set; }
        public TripSummaryViewModel Trip { get; set; }
        public string PaymentStatus { get; set; }
        public bool CanCancel { get; set; }

        public string FormattedBookingDate => BookingDate.ToString("dd/MM/yyyy HH:mm");
        public string StatusBadgeClass => Status switch
        {
            BookingStatus.Pending => "badge-warning",
            BookingStatus.Confirmed => "badge-success",
            BookingStatus.Cancelled => "badge-danger",
            _ => "badge-secondary"
        };
        public bool IsFutureTrip => Trip.DepartureTime > DateTime.Now;
        public bool IsPastTrip => Trip.DepartureTime < DateTime.Now;
    }
}
