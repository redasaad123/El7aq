using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmailSend
    {
        Task SendEmail(string? email, string v1, string v2);
    }
}
