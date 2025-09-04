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
        
        public async Task<int?> GetPassengerIdFromUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            // Find the passenger profile by UserId
            var passengerProfile = await _context.Passengers
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (passengerProfile == null)
                return null;

            // Convert string ID to int
            if (int.TryParse(passengerProfile.Id, out int passengerId))
                return passengerId;

            return null;
        }

        public async Task<string> GetUserIdFromPassengerIdAsync(int passengerId)
        {
           
            var passengerIdStr = passengerId.ToString();
            var passengerProfile = await _context.Passengers
                .FirstOrDefaultAsync(p => p.Id == passengerIdStr);

            return passengerProfile?.UserId!;
        }

        public async Task<bool> IsValidPassengerAsync(int passengerId)
        {
            var passengerIdStr = passengerId.ToString();
            return await _context.Passengers
                .AnyAsync(p => p.Id == passengerIdStr);
        }
    }
}

    

