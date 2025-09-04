using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PassengerHelperService : IPassengerHelperService
    {

        private readonly ApplicationDbContext _context;

        public PassengerHelperService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<string?> GetPassengerIdFromUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            // Find the passenger profile by UserId
            var passengerProfile = await _context.Passengers
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (passengerProfile == null)
                return null;

            return passengerProfile.Id;
        }

        public async Task<string> GetUserIdFromPassengerIdAsync(string passengerId)
        {
            var passengerProfile = await _context.Passengers
                .FirstOrDefaultAsync(p => p.Id == passengerId);

            return passengerProfile?.UserId!;
        }

        public async Task<bool> IsValidPassengerAsync(string passengerId)
        {
            return await _context.Passengers
                .AnyAsync(p => p.Id == passengerId);
        }
    }
}

    

