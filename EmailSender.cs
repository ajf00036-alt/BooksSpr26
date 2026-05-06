using Microsoft.AspNetCore.Identity.UI.Services;

namespace BooksSpr2026
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
