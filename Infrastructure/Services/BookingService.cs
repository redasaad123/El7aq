using Core.Interfaces;
using El7aq.Domain.Entities;
using El7aq.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BookingService
    {
        private readonly IUnitOfWork<Booking> _bookingUow;
        private readonly IUnitOfWork<Trip> _tripUow;
        private readonly IUnitOfWork<PassengerProfile> _passengerUow;
        private readonly ApplicationDbContext _context;

        public BookingService(IUnitOfWork<Booking> bookingUow, IUnitOfWork<Trip> tripUow, IUnitOfWork<PassengerProfile> passengerUow, ApplicationDbContext context)
        {
            _bookingUow = bookingUow;
            _tripUow = tripUow;
            _passengerUow = passengerUow;
            _context = context;
        }

        #region Booking Operations

        public async Task<Booking> CreateBookingAsync(string passengerId, string tripId)
        {

           

            var passenger = _passengerUow.Entity.Get(passengerId);
            if (passenger == null)
                throw new ArgumentException("Passenger not found");


            // Check if trip exists and has available seats
            var trip = await _context.Trips
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                throw new ArgumentException("Trip not found");

            var currentBookings = trip.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0;
            if (currentBookings >= trip.AvailableSeats)
                throw new InvalidOperationException("No available seats");

            // Check if passenger already has a booking for this trip
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.PassengerId == passengerId &&
                                        b.TripId == tripId &&
                                        b.Status != BookingStatus.Cancelled);

            if (existingBooking != null)
                throw new InvalidOperationException("Passenger already has a booking for this trip");

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

            return booking;

        }
        public async Task<bool> ConfirmBookingAsync(string bookingId)
        {
            
            var booking = _bookingUow.Entity.Get(bookingId);
            if (booking == null)
                throw new ArgumentException("Booking not found");

            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Only pending bookings can be confirmed");

            // Check if trip has available seats
            var trip = await _context.Trips
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == booking.TripId);

            if (trip == null)
                throw new ArgumentException("Trip not found");

            var currentBookings = trip.Bookings?.Count(b => b.Status != BookingStatus.Cancelled) ?? 0;
            if (currentBookings >= trip.AvailableSeats)
                throw new InvalidOperationException("No available seats");

            booking.Status = BookingStatus.Confirmed;
            _bookingUow.Entity.Update(booking);
            _bookingUow.Save();

            return true;
        }

        public async Task<bool> CancelBookingAsync(string bookingId, string passengerId)
        {
            

            var booking = _bookingUow.Entity.Get(bookingId);
            if (booking == null)
                throw new ArgumentException("Booking not found");

            if (booking.PassengerId != passengerId)
                throw new UnauthorizedAccessException("Passenger not authorized to cancel this booking");

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Booking is already cancelled");

            booking.Status = BookingStatus.Cancelled;
            _bookingUow.Entity.Update(booking);
            _bookingUow.Save();

            return true;
        
        }

        public async Task<Booking> GetBookingDetailsAsync(string bookingId)
        {
     
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.StartStation)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.EndStation)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Driver)
                        .ThenInclude(d => d.appUsers)
                .Include(b => b.Passenger)
                    .ThenInclude(p => p.User)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                throw new ArgumentException("Booking not found");

            return booking;
        }

        #endregion



        #region Passenger  Operations

        public async Task<IEnumerable<Booking>> GetPassengerBookingsAsync(string passengerId)
        {

            var bookings = await _context.Bookings
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.StartStation)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.EndStation)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Driver)
                        .ThenInclude(d => d.appUsers)
                .Include(b => b.Passenger)
                    .ThenInclude(p => p.User)
                .Where(b => b.PassengerId == passengerId)
                .ToListAsync();

            return bookings;
        }

        public async Task<IEnumerable<Booking>> GetPassengerActiveBookingsAsync(string passengerId)
        {
           

            var bookings = await _context.Bookings
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.StartStation)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.EndStation)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Driver)
                        .ThenInclude(d => d.appUsers)
                .Where(b => b.PassengerId == passengerId &&
                           (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                           b.Trip.DepartureTime > DateTime.UtcNow)
                .OrderBy(b => b.Trip.DepartureTime)
                .ToListAsync();

            return bookings;
        }

        #endregion


        #region Driver Operations

        public async Task<IEnumerable<Booking>> GetDriverBookingsAsync(string driverId)
        {
        
            var bookings = await _context.Bookings
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.StartStation)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.EndStation)
                .Include(b => b.Passenger)
                    .ThenInclude(p => p.User)
                .Include(b => b.Payments)
                .Where(b => b.Trip.DriverId == driverId)
                .OrderByDescending(b => b.Trip.DepartureTime)
                .ToListAsync();

            return bookings;
        }
        #endregion
    }


}

