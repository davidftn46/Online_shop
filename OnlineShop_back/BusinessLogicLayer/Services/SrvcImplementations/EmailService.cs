using Addition.Mutual;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.SrvcImplementations
{
        public class EmailService : IEmailService
        {
            private readonly IOptions<EmailStructure> _emailConfiguration;

            public EmailService(IOptions<EmailStructure> emailConfiguration)
            {
                this._emailConfiguration = emailConfiguration;
            }

            public async Task<bool> SendMailAsync(EmailData emailData)
            {
                MailMessage msg = new MailMessage();
                try
                {
                    msg.To.Add(new MailAddress(emailData.Towho));
                    foreach (var ccItem in emailData.ClientList)
                    {
                        msg.CC.Add(new MailAddress(ccItem));
                    }
                    msg.From = new MailAddress(_emailConfiguration.Value.Fromwho);

                    msg.Subject = emailData.Subject;

                    if (emailData.HtmlContent)
                        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailData.Content, null, MediaTypeNames.Text.Html));
                    else
                        msg.Body = emailData.Content;

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(_emailConfiguration.Value.Email, _emailConfiguration.Value.Password)
                    };

                    await smtp.SendMailAsync(msg);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }

                return true;
            }
        }
}
