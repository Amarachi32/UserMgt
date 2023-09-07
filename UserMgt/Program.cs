using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using UserMgt.BLL.Configurations.Extension;
using UserMgt.BLL.Configurations.mapping;
using UserMgt.BLL.Interface;
using UserMgt.DAL.Context;
using UserMgt.DAL.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterService(builder.Configuration);
builder.Services.AddSwaggerGenn();
builder.Services.AddControllers().AddNewtonsoftJson();



//builder.Services.SeedData(builder.Configuration);
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddAuthorization();
var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserMgt API v1");
        c.DocExpansion(DocExpansion.List);
    });
}

app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Seed data here
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RegisterServices.SeedIdentityData(userManager, roleManager);

    }
    catch (Exception ex)
    {
        var logg = services.GetRequiredService<ILogger<Program>>();
        logg.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

