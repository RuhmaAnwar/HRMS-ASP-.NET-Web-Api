using HRMS.Data;
using HRMS.Mappings;
using HRMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRMS.NewFolder
{
    public static class ServiceExtension
    {
        public static void ConfigPostgres(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["ConnectionStrings"];
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
            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/api/v1/auth/login";
                    options.AccessDeniedPath = "/api/v1/auth/accessdenied";
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
            services.AddIdentity<Employee, IdentityRole<int>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}
