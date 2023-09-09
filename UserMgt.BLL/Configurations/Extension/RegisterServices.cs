using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.Interface;
using UserMgt.BLL.Services;
using UserMgt.DAL.Context;
using UserMgt.DAL.Entities;

namespace UserMgt.BLL.Configurations.Extension
{
    public static class RegisterServices
    {
        public static void RegisterService(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(config.GetSection("ConnectionStrings")["DefaultConnection"]!);

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
    }
}
