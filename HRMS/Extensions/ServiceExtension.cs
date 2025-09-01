using HRMS.Data;
using HRMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HRMS.Mappings;

namespace HRMS.Extensions
{
    public static class ServiceExtension
    {

        //public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        //    {
        //        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        //    }


        // extension methods ... using this, lik this IServiceCollection is used to make ConfigPostgres an extension method to IServiceCollection
        public static void ConfigPostgres(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["ConnectionStrings:Postgres"];
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        public static void ConfigAutomapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
        }

        public static void ConfigCookieAuthentication(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(
                options =>
                {
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden; // return 403 instead of redirect
                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized; // return 401 instead of redirect
                        return Task.CompletedTask;
                    };
                });
            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(options =>
                {

                    options.AccessDeniedPath = "";
                    options.LoginPath = "/api/v1/auth/login";
                    options.Cookie.Name = ".HRMS.AuthCookie";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.SlidingExpiration = true;  // Renew on activity
                });
        }

        public static void AddEmployeeIdentity(this IServiceCollection services)
        {
            services.AddIdentity<Employee, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static async Task SeedRoles(this IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            string[] roles = { "Admin", "HR", "Manager", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                }
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<Employee>>();
            var user = await userManager.FindByEmailAsync("ruhma@email.com");

            if (user != null && !(await userManager.IsInRoleAsync(user, "Admin")))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }


    }
}
