using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
     public interface IBookingService
    {


        // Booking Operations
        Task<Booking> CreateBookingAsync(string passengerId, string tripId);
        Task<bool> ConfirmBookingAsync(string bookingId);
        Task<bool> CancelBookingAsync(string bookingId, string passengerId);
        Task<Booking> GetBookingDetailsAsync(string bookingId);


        // Passenger  Operations
        Task<IEnumerable<Booking>> GetPassengerBookingsAsync(string passengerId);
        Task<IEnumerable<Booking>> GetPassengerActiveBookingsAsync(string passengerId);

        // Driver Operations
        Task<IEnumerable<Booking>> GetDriverBookingsAsync(string driverId);
    }
}
