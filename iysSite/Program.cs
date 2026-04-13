using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using iysSite.Models;
using iysSite.Services;
using Resend;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = builder.Configuration["RESEND_API_KEY"]!;
});
builder.Services.AddTransient<IResend, ResendClient>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

var app = builder.Build();

var emailSettings = app.Services.GetRequiredService<IOptions<iysSite.Models.EmailSettings>>().Value;
app.Logger.LogInformation("EmailSettings check — Host: {Host}, FromEmail: '{FromEmail}', ToEmail: '{ToEmail}'",
    emailSettings.Host, emailSettings.FromEmail, emailSettings.ToEmail);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();