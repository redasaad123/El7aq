using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    // This service provides helper methods to interact with passenger data.
    public interface IPassengerHelperService
    {

        Task<int?> GetPassengerIdFromUserIdAsync(string userId);
        Task<string> GetUserIdFromPassengerIdAsync(int passengerId);
        Task<bool> IsValidPassengerAsync(int passengerId);
    }
}
