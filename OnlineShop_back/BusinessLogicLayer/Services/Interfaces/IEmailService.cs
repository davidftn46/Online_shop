using Addition.Mutual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendMailAsync(EmailData emailData);
    }
}
