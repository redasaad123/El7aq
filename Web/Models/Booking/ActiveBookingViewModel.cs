namespace Web.Models.Booking
{
    public class ActiveBookingViewModel : BookingViewModel
    {

        public TimeSpan? TimeUntilDeparture { get; set; }

        public string TimeUntilDepartureText
        {
            get
            {
                if (!TimeUntilDeparture.HasValue || TimeUntilDeparture.Value.TotalMinutes < 0)
                    return " Trip Ended";

                if (TimeUntilDeparture.Value.TotalDays >= 1)
                    return $"After {TimeUntilDeparture.Value.Days} Day";

                if (TimeUntilDeparture.Value.TotalHours >= 1)
                    return $"After {TimeUntilDeparture.Value.Hours} Hour";

                return $"After {TimeUntilDeparture.Value.Minutes} minute";
            }
        }

        public string UrgencyClass
        {
            get
            {
                if (!TimeUntilDeparture.HasValue || TimeUntilDeparture.Value.TotalMinutes < 0)
                    return "text-muted";

                if (TimeUntilDeparture.Value.TotalHours < 2)
                    return "text-danger";

                if (TimeUntilDeparture.Value.TotalHours < 6)
                    return "text-warning";

                return "text-success";
            }
        }
    }



}
