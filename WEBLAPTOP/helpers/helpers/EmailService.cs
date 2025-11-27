using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public static class EmailService
{
    // Gửi email async A
    public static async Task Send(string toEmail, string subject, string body)
    {
        var fromAddress = new MailAddress("dongtranquoc951@gmail.com", "Web laptop");
        var toAddress = new MailAddress(toEmail);

        const string fromPassword = "iufu luih uydz yqqb";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        })
        {
            await smtp.SendMailAsync(message);
        }
    }
}
