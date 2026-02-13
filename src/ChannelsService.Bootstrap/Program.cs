using ChannelsService.Application;
using ChannelsService.Bootstrap;
using ChannelsService.External;
using ChannelsService.Outbox;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCommandHandlers();
builder.Services.AddQueryHandlers();
builder.Services.AddDatabase();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "https://keycloak.monoclocker.ru/realms/lite-messenger";
        options.Audience = "account";
    });

builder.Services.AddAuthorization();

builder.Services.AddOutboxProcessor();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/api.json");
}

app.UseAuthentication();
app.UseAuthorization();

app.UseDefinedEndpoints();

app.Run();