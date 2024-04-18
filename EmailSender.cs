using Microsoft.AspNetCore.Identity.UI.Services;

namespace CCD_Attendance
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //throw new NotImplementedException();

            return Task.CompletedTask;
        }
    }
}
