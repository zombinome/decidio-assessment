using System.Net;
using DecidioTestExcersice.Errors;
using DecidioTestExcersice.Initialization;
using DecidioTestExcersice.Models;
using DecidioTestExcersice.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSendGridMailSender();
builder.Services.AddMailAddressRepository();
builder.Services.AddScoped<MailService, MailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// Register handler
app.MapPost("/sendmails", async (MailListModel model, MailService mailService) =>
{
    try
    {
        await mailService.SendMailsAsync(model.Addresses, MailTemplate.Default);
    }
    catch (InvalidMailAddressException)
    {
        return Results.StatusCode((int)HttpStatusCode.BadRequest);
    }
    catch (Exception)
    {
        return Results.BadRequest();
    }

    return Results.NoContent();
})
.WithName("SendMails");

// Running app
app.Run();
