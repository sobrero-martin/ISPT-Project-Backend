using Microsoft.Extensions.Logging;
using Resend;

namespace Repositorio.Repository;

public class EmailService : IEmailService
{
    private readonly IResend resend;
    private readonly ILogger<EmailService> logger;

    public EmailService(IResend resend, ILogger<EmailService> logger)
    {
        this.resend = resend;
        this.logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string bodyHtml, string? bodyText = null)
    {
        try
        {
            var message = new EmailMessage
            {
                From = "onboarding@resend.dev",
                Subject = subject,
                HtmlBody = bodyHtml,
                TextBody = bodyText
            };

            message.To.Add(to);

            var response = await resend.EmailSendAsync(message);
            logger.LogInformation("Email enviado con éxito a {To}. Resend ID: {Id}", to, response.Content);
        }
        catch (Exception ex)
        {
            Console.WriteLine("EL ERROR ES DE: " + ex.Message);
            logger.LogError(ex, "Error al enviar email a {To} mediante Resend", to);
            throw; // O manejar según la estrategia de errores del dominio
        }
    }
}