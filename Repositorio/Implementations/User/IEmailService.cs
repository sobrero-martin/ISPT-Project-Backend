namespace Repositorio.Repository;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string bodyHtml, string? bodyText = null);
}