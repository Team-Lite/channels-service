using ChannelsService.Application;
using ChannelsService.Bootstrap;
using ChannelsService.External;
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
        options.Authority = "https://test.com";
        options.Audience = "Test";
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefinedEndpoints();

app.Run();