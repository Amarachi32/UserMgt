using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using UserMgt.BLL.Configurations.Extension;
using UserMgt.BLL.Interface;
using UserMgt.DAL.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterService(builder.Configuration);
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Description = "Standard Authorization Header Using the Bearer Scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.EnableAnnotations();
    options.UseInlineDefinitionsForEnums();
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddControllers().AddNewtonsoftJson();



builder.Services.SeedData(builder.Configuration);
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

