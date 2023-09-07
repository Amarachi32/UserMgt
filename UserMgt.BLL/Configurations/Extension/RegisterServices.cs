using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore.SqlServer;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.Interface;
using UserMgt.BLL.Services;
using UserMgt.DAL.Context;
using UserMgt.DAL.Entities;
using AutoMapper;

namespace UserMgt.BLL.Configurations.Extension
{
    public static class RegisterServices
    {
        public static void RegisterService(this IServiceCollection services)
        {
            IConfiguration config;

            using (var serviceProvider = services.BuildServiceProvider())
            {
                config = serviceProvider.GetService<IConfiguration>();
            }

            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            });

            object value = services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IValidator<UpdateRequestDto>, UpdateRequestValidator>();
            services.AddScoped<ValidationFilterAttribute>();
            services.AddTransient<IJWTAuthenticator, JwtAuthenticator>();
        }

        public static IServiceCollection SeedData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentityCore<AppUser>()
                .AddEntityFrameworkStores<UserDbContext>()
                .AddSignInManager<SignInManager<AppUser>>()
                  .AddDefaultTokenProviders();


            var serviceProvider = services.BuildServiceProvider();

            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed data
            SeedIdentityData(userManager, roleManager).Wait();

            return services;
        }


        public static async Task SeedIdentityData(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (userManager.FindByEmailAsync("useradmin@gmail.com").Result == null)
            {
                var user = new AppUser
                {
                    Email = "useradmin@gmail.com",
                    UserName = "admin",
                    Gender = Gender.Female,
                    PhoneNumber = "1234567890",
                    IsUserActive = true,
                };

                var result = await userManager.CreateAsync(user, "@Admin123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
        public static void AddSwaggerGenn(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "please insert a token",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.SwaggerDoc("v1", new OpenApiInfo { Title = "UserMgt API", Version = "v1" });
                options.EnableAnnotations();
                options.UseInlineDefinitionsForEnums();
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }

    }
}
