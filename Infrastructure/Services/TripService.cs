using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TripService : ITripService
    {

        private readonly IUnitOfWork<Trip> _tripUow;
        private readonly IUnitOfWork<Booking> _bookingUow;
        private readonly IUnitOfWork<Route> _routeUow;
        private readonly IUnitOfWork<DriverProfile> _driverUow;


        // Constructor to inject repositories
        public TripService(
            IUnitOfWork<Trip> tripUow,
            IUnitOfWork<Booking> bookingUow,
            IUnitOfWork<Route> routeUow,
            IUnitOfWork<DriverProfile> driverUow)
        {
            _tripUow = tripUow;
            _bookingUow = bookingUow;
            _routeUow = routeUow;
            _driverUow = driverUow;
        }

        #region Driver Operations
        public async Task<Trip> CreateTripAsync(string driverId, string routeId, DateTime departureTime, int availableSeats)
        {
            var driver = _driverUow.Entity.Get(driverId);
            if (driver == null)
                throw new ArgumentException("Driver not found");


            var route = _routeUow.Entity.Get(routeId);
            if (route == null)
                throw new ArgumentException("Route not found");


            // Validate departure time is in future
            if (departureTime <= DateTime.UtcNow)
                throw new ArgumentException("Departure time must be in the future");


            var trip = new Trip
            {
                Id = Guid.NewGuid().ToString(),
                DriverId = driverId,
                RouteId = routeId,
                DepartureTime = departureTime,
                AvailableSeats = availableSeats
            };

            _tripUow.Entity.Insert(trip);
            _tripUow.Save();

            return trip;
        }

        public async Task<IEnumerable<Trip>> GetDriverTripsAsync(string driverId)
        {
           

            var trips = await _tripUow.Entity.GetAllAsyncAsQuery()
               .Include(t => t.Route)
                   .ThenInclude(r => r.StartStation)
               .Include(t => t.Route)
                   .ThenInclude(r => r.EndStation)
               .Include(t => t.Bookings.Where(b => b.Status != BookingStatus.Cancelled))
               .Where(t => t.DriverId == driverId)
               .OrderByDescending(t => t.DepartureTime)
               .ToListAsync();

            return trips;

        }


        public async Task<bool> UpdateTripAsync(string tripId, DateTime newDepartureTime, int newAvailableSeats)
        {
           

            var trip = await _tripUow.Entity.GetAllAsyncAsQuery()
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                return false;

            // Check if departure time is in future
            if (newDepartureTime <= DateTime.UtcNow)
                return false;

            // Check if new available seats is less than booked seats
            var confirmedBookings = trip.Bookings?.Count(b => b.Status == BookingStatus.Confirmed) ?? 0;
            if (newAvailableSeats < confirmedBookings)
                return false;


            trip.DepartureTime = newDepartureTime;
            trip.AvailableSeats = newAvailableSeats;

            _tripUow.Entity.Update(trip);
            _tripUow.Save();

            return true;
        }


        public async Task<bool> CancelTripAsync(string tripId)

        {
            
            var trip = await _tripUow.Entity.GetAllAsyncAsQuery()
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null) return false;

            // cancel all bookings associated with this trip
            if (trip.Bookings != null)
            {
                foreach (var booking in trip.Bookings.Where(b => b.Status != BookingStatus.Cancelled))
                {
                    booking.Status = BookingStatus.Cancelled;
                    _bookingUow.Entity.Update(booking);
                }
            }

            // Remove the trip
            _tripUow.Entity.Delete(trip);
            _tripUow.Save();

            return true;



        }


        #endregion

        #region Passenger Operations
        public async Task<IEnumerable<Trip>> SearchTripsAsync(string originCityId, string destinationCityId, DateTime date)
        {
     
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1);


            var trips = await _tripUow.Entity.GetAllAsyncAsQuery()
                .Include(t => t.Route)
                    .ThenInclude(r => r.StartStation)
                .Include(t => t.Route)
                    .ThenInclude(r => r.EndStation)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.appUsers)
                .Include(t => t.Bookings.Where(b => b.Status != BookingStatus.Cancelled))
                .Where(t => t.Route.StartStationId == originCityId &&
                           t.Route.EndStationId == destinationCityId &&
                           t.DepartureTime >= startDate &&
                           t.DepartureTime < endDate &&
                           t.DepartureTime > DateTime.UtcNow)
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();


            // Filter trips with available seats
            return trips.Where(t =>
            {
                var bookedSeats = t.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0;
                return bookedSeats < t.AvailableSeats;
            }).ToList();

        }

        public async Task<Trip> GetTripDetailsAsync(string tripId)
        {
         

            var trip = await _tripUow.Entity.GetAllAsyncAsQuery()
                .Include(t => t.Route)
                    .ThenInclude(r => r.StartStation)
                .Include(t => t.Route)
                    .ThenInclude(r => r.EndStation)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.appUsers)
                .Include(t => t.Bookings.Where(b => b.Status != BookingStatus.Cancelled))
                    .ThenInclude(b => b.Passenger)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(t => t.Id == tripId);
                

            return trip;
        }


        public async Task<IEnumerable<Trip>> GetAllAvailableTripsAsync()
        {
            var trips = await _tripUow.Entity.GetAllAsyncAsQuery()
                .Include(t => t.Route)
                    .ThenInclude(r => r.StartStation)
                .Include(t => t.Route)
                    .ThenInclude(r => r.EndStation)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.appUsers)
                .Include(t => t.Bookings.Where(b => b.Status != BookingStatus.Cancelled))
                .Where(t => t.DepartureTime > DateTime.UtcNow)
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();

            // Filter trips with available seats
            return trips.Where(t =>
            {
                var bookedSeats = t.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0;
                return bookedSeats < t.AvailableSeats;
            }).ToList();
        }

        #endregion

        #region Booking Operations
        public async Task<bool> HasAvailableSeatsAsync(string tripId, int seatsNeeded = 1)
        {
            var trip = await _tripUow.Entity.GetAllAsyncAsQuery()
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                return false;

            var bookedSeats = trip.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0;
            return (trip.AvailableSeats - bookedSeats) >= seatsNeeded;
        }

        public async Task<bool> BookSeatAsync(string tripId, string passengerId)
        {
           

            // Check if trip has available seats
            if (!await HasAvailableSeatsAsync(tripId))
                return false;

            var existingBooking = await _bookingUow.Entity.GetAllAsyncAsQuery()
              .FirstOrDefaultAsync(b => b.PassengerId == passengerId &&
                                      b.TripId == tripId &&
                                      b.Status != BookingStatus.Cancelled);

            if (existingBooking != null)
                return false;

            var booking = new Booking
            {
                Id = Guid.NewGuid().ToString(),
                PassengerId = passengerId,
                TripId = tripId,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Pending
            };

            _bookingUow.Entity.Insert(booking);
            _bookingUow.Save();

            return true;

        }

        public async Task<bool> CancelBookingAsync(string bookingId, string passengerId)
        {
           

            var booking = await _bookingUow.Entity.GetAllAsyncAsQuery()
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.PassengerId == passengerId);

            if (booking == null || booking.Status == BookingStatus.Cancelled)
                return false;

            booking.Status = BookingStatus.Cancelled;
            _bookingUow.Entity.Update(booking);
            _bookingUow.Save();

            return true;
        }

        public async Task<IEnumerable<Trip>> GetTripsByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1);

            var trips = await _tripUow.Entity.GetAllAsyncAsQuery()
                .Include(t => t.Route)
                    .ThenInclude(r => r.StartStation)
                .Include(t => t.Route)
                    .ThenInclude(r => r.EndStation)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.appUsers)
                .Include(t => t.Bookings.Where(b => b.Status != BookingStatus.Cancelled))
                .Where(t => t.DepartureTime >= startDate && t.DepartureTime < endDate)
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();

            return trips;
        }

        public async Task<IEnumerable<Trip>> GetTripsByStationAsync(string stationId)
        {
           

            var trips = await _tripUow.Entity.GetAllAsyncAsQuery()
                .Include(t => t.Route)
                    .ThenInclude(r => r.StartStation)
                .Include(t => t.Route)
                    .ThenInclude(r => r.EndStation)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.appUsers)
                .Include(t => t.Bookings.Where(b => b.Status != BookingStatus.Cancelled))
                .Where(t => t.Route.StartStationId == stationId || t.Route.EndStationId == stationId)
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();

            return trips;

        }

     
          #endregion
    

       


    }
}
