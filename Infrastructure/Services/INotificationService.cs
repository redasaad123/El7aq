using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Infrastructure.Services
{
    using Core.Entities;
    public interface INotificationService
    {
        Task AddNotificationAsync(string userId, string message);
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(string notificationId);
    }
}
