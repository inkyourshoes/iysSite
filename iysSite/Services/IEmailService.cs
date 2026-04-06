namespace iysSite.Services;
using iysSite.Models;

public interface IEmailService
{
    Task SendCommissionRequestEmailAsync(
        CommissionRequest request,
        IReadOnlyCollection<string> savedFiles);

    Task SendClientConfirmationEmailAsync(string toEmail, string firstName, CommissionType commissionType);
}