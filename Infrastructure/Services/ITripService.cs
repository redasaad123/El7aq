using El7aq.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ITripService
    {

        // Driver Operations
        Task<Trip> CreateTripAsync(string driverId, string routeId, DateTime departureTime, int availableSeats);
        Task<IEnumerable<Trip>> GetDriverTripsAsync(string driverId);
        Task<bool> UpdateTripAsync(string tripId, DateTime newDepartureTime, int newAvailableSeats);
        Task<bool> CancelTripAsync(string tripId);

        // Passenger Operations
        Task<IEnumerable<Trip>> SearchTripsAsync(string originCityId, string destinationCityId, DateTime date);

        Task<Trip> GetTripDetailsAsync(string tripId);
        Task<IEnumerable<Trip>> GetAllAvailableTripsAsync();

        // Booking Operations
        Task<bool> HasAvailableSeatsAsync(string tripId, int seatsNeeded = 1);
        Task<bool> BookSeatAsync(string tripId, string passengerId);
        Task<bool> CancelBookingAsync(string bookingId, string passengerId);


        // staff Operations => mentor trips in specific date or station
        Task<IEnumerable<Trip>> GetTripsByStationAsync(string stationId);
        Task<IEnumerable<Trip>> GetTripsByDateAsync(DateTime date);


    }
}
