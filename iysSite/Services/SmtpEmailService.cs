namespace iysSite.Services;

using System.Text;
using iysSite.Models;
using Microsoft.Extensions.Options;
using Resend;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly IResend _resend;

    public SmtpEmailService(IOptions<EmailSettings> emailSettings, IResend resend)
    {
        _emailSettings = emailSettings.Value;
        _resend = resend;
    }

    public async Task SendCommissionRequestEmailAsync(
        CommissionRequest request,
        IReadOnlyCollection<string> savedFiles)
    {
        var message = new EmailMessage();
        message.From = _emailSettings.FromEmail;
        message.To.Add(_emailSettings.ToEmail);
        message.Subject = $"New {request.CommissionType} commission request from {request.FirstName}".Trim();
        message.TextBody = BuildEmailBody(request, savedFiles);

        message.Attachments = new List<EmailAttachment>();
        foreach (var filePath in savedFiles)
        {
            if (!File.Exists(filePath)) continue;
            var bytes = await File.ReadAllBytesAsync(filePath);
            message.Attachments.Add(new EmailAttachment
            {
                Filename = Path.GetFileName(filePath),
                Content = bytes,
            });
        }

        await _resend.EmailSendAsync(message);
    }

    public async Task SendClientConfirmationEmailAsync(string toEmail, string firstName, CommissionType commissionType)
    {
        var commissionLabel = commissionType == CommissionType.Shoes ? "custom shoes" : "tattoo";

        var message = new EmailMessage();
        message.From = _emailSettings.FromEmail;
        message.To.Add(toEmail);
        message.Subject = "Your commission request has been received — Ink Your Shoes";
        message.TextBody = $"""
            Hi {firstName},

            Thank you for submitting your {commissionLabel} commission request!

            We've received your request and will review the details you provided. We'll be in touch with you soon to discuss next steps.

            If you have any questions in the meantime, feel free to reply to this email.

            Talk soon,
            Ink Your Shoes
            """;

        await _resend.EmailSendAsync(message);
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
                sb.AppendLine($"- {Path.GetFileName(file)}");
            }
        }

        return sb.ToString();
    }
}
