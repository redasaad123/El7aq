using Core.Enums;
using Web.Models.Trip;

namespace Web.Models.Booking
{
    public class BookingDetailsViewModel
    {
        public string BookingId { get; set; }
        public string TripId { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }
        public string StatusText { get; set; }
        public TripDetailsViewModel Trip { get; set; }

        //   public List<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();
        public bool CanCancel { get; set; }
        public bool CanConfirm { get; set; }

        public string FormattedBookingDate => BookingDate.ToString("dd/MM/yyyy HH:mm");
        public string StatusBadgeClass => Status switch
        {
            BookingStatus.Pending => "badge-warning",
            BookingStatus.Confirmed => "badge-success",
            BookingStatus.Cancelled => "badge-danger",
            _ => "badge-secondary"
        };

        //public bool HasPayments => Payments.Any();
        //public PaymentViewModel LatestPayment => Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
        //public decimal TotalPaid => Payments.Where(p => p.Status == PaymentStatus.Success).Sum(p => p.Amount);
    }
}
