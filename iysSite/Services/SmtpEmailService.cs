namespace iysSite.Services;

using System.Text;
using iysSite.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public SmtpEmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendCommissionRequestEmailAsync(
        CommissionRequest request,
        IReadOnlyCollection<string> savedFiles)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        message.To.Add(MailboxAddress.Parse(_emailSettings.ToEmail));
        message.Subject = $"New {request.CommissionType} commission request from {request.FirstName}".Trim();

        var builder = new BodyBuilder
        {
            TextBody = BuildEmailBody(request, savedFiles)
        };

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _emailSettings.Host,
            _emailSettings.Port,
            SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(
            _emailSettings.UserName,
            _emailSettings.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendClientConfirmationEmailAsync(string toEmail, string firstName, CommissionType commissionType)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Your commission request has been received — Ink Your Shoes";

        var commissionLabel = commissionType == CommissionType.Shoes ? "custom shoes" : "tattoo";

        var builder = new BodyBuilder
        {
            TextBody = $"""
                Hi {firstName},

                Thank you for submitting your {commissionLabel} commission request!

                We've received your request and will review the details you provided. We'll be in touch with you soon to discuss next steps.

                If you have any questions in the meantime, feel free to reply to this email.

                Talk soon,
                Ink Your Shoes
                """
        };

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _emailSettings.Host,
            _emailSettings.Port,
            SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(
            _emailSettings.UserName,
            _emailSettings.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private static string BuildEmailBody(
        CommissionRequest request,
        IReadOnlyCollection<string> savedFiles)
    {
        var sb = new StringBuilder();

        sb.AppendLine("A new commission request was submitted.");
        sb.AppendLine();
        sb.AppendLine($"Commission Type: {request.CommissionType}");
        sb.AppendLine($"Email: {request.Email}");
        sb.AppendLine($"First Name: {request.FirstName}");
        sb.AppendLine();

        if (request.Shoes is not null)
        {
            sb.AppendLine("Shoe Details:");
            sb.AppendLine($"- Brand Type: {request.Shoes.BrandType}");
            sb.AppendLine($"- Size: {request.Shoes.Size}");
            sb.AppendLine($"- Ideas: {request.Shoes.CustomStyleColorsIdeas}");
            sb.AppendLine();
        }

        if (request.Tattoo is not null)
        {
            sb.AppendLine("Tattoo Details:");
            sb.AppendLine($"- Description: {request.Tattoo.Description}");
            sb.AppendLine($"- Size: {request.Tattoo.Size}");
            sb.AppendLine($"- Body Placement: {request.Tattoo.BodyPlacement}");
            sb.AppendLine();
        }

        sb.AppendLine("Uploaded Files:");
        if (savedFiles.Count == 0)
        {
            sb.AppendLine("- None");
        }
        else
        {
            foreach (var file in savedFiles)
            {
                sb.AppendLine($"- {file}");
            }
        }

        return sb.ToString();
    }
}


// namespace iysSite.Models;
//
// using System.Text;
// using MailKit.Net.Smtp;
// using MailKit.Security;
// using Microsoft.Extensions.Options;
// using MimeKit;
//
// public class SmtpEmailService : IEmailService
// {
//     private readonly EmailSettings _emailSettings;
//
//     public SmtpEmailService(IOptions<EmailSettings> emailSettings)
//     {
//         _emailSettings = emailSettings.Value;
//     }
//
//     public async Task SendCommissionRequestEmailAsync(
//         CommissionRequest request,
//         IReadOnlyCollection<string> savedFiles)
//     {
//         var message = new MimeMessage();
//
//         message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
//         message.To.Add(MailboxAddress.Parse(_emailSettings.ToEmail));
//         message.Subject = $"New {request.CommissionType} commission request from {request.FirstName}".Trim();
//
//         var builder = new BodyBuilder
//         {
//             TextBody = BuildEmailBody(request, savedFiles)
//         };
//
//         message.Body = builder.ToMessageBody();
//
//         using var client = new SmtpClient();
//
//         await client.ConnectAsync(
//             _emailSettings.Host,
//             _emailSettings.Port,
//             SecureSocketOptions.StartTls);
//
//         await client.AuthenticateAsync(
//             _emailSettings.UserName,
//             _emailSettings.Password);
//
//         await client.SendAsync(message);
//         await client.DisconnectAsync(true);
//     }
//
//     private static string BuildEmailBody(
//         CommissionRequest request,
//         IReadOnlyCollection<string> savedFiles)
//     {
//         var sb = new StringBuilder();
//
//         sb.AppendLine("A new commission request was submitted.");
//         sb.AppendLine();
//         sb.AppendLine($"Commission Type: {request.CommissionType}");
//         sb.AppendLine($"Email: {request.Email}");
//         sb.AppendLine($"First Name: {request.FirstName}");
//         sb.AppendLine();
//
//         if (request.Shoes is not null)
//         {
//             sb.AppendLine("Shoe Details:");
//             sb.AppendLine($"- Brand Type: {request.Shoes.BrandType}");
//             sb.AppendLine($"- Size: {request.Shoes.Size}");
//             sb.AppendLine($"- Ideas: {request.Shoes.CustomStyleColorsIdeas}");
//             sb.AppendLine();
//         }
//
//         if (request.Tattoo is not null)
//         {
//             sb.AppendLine("Tattoo Details:");
//             sb.AppendLine($"- Description: {request.Tattoo.Description}");
//             sb.AppendLine($"- Size: {request.Tattoo.Size}");
//             sb.AppendLine($"- Body Placement: {request.Tattoo.BodyPlacement}");
//             sb.AppendLine();
//         }
//
//         sb.AppendLine("Uploaded Files:");
//         if (savedFiles.Count == 0)
//         {
//             sb.AppendLine("- None");
//         }
//         else
//         {
//             foreach (var file in savedFiles)
//             {
//                 sb.AppendLine($"- {file}");
//             }
//         }
//
//         return sb.ToString();
//     }
// }