
using HRMS.Extensions;
using HRMS.Jobs;
using HRMS.Middleware; 
using HRMS.Repositories;
using HRMS.Repositories.Interfaces;
using HRMS.Services;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("AutoLeaveJob");
    q.AddJob<AutoLeaveJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
    .ForJob(jobKey)
    .WithIdentity("AutoLeaveTrigger")
    .WithCronSchedule(
        "*/5 * * * * ?",
        // 0 10 * * *
        cron => cron.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi"))
    )
);

});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


builder.Services.AddControllers();
builder.Services.ConfigPostgres(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();

builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();

builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();

builder.Services.ConfigAutomapper();

builder.Services.AddEmployeeIdentity();

builder.Services.ConfigCookieAuthentication();

// Configure JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // This indicates the authentication scheme that will be used by default when the app encounters an authentication challenge. 
    // Which authentication handler to use for responding to failed authentication or authorization attempts.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(options =>
            {
                // Define token validation parameters to ensure tokens are valid and trustworthy
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // Ensure the token was issued by a trusted issuer
                    ValidIssuer = builder.Configuration["Jwt:Issuer"], // The expected issuer value from configuration
                    ValidateAudience = false, // Disable audience validation (can be enabled as needed)
                    ValidateLifetime = true, // Ensure the token has not expired
                    ValidateIssuerSigningKey = true, // Ensure the token's signing key is valid
                    // Define a custom IssuerSigningKeyResolver to dynamically retrieve signing keys from the JWKS endpoint
                    //IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    //{
                    //    //Console.WriteLine($"Received Token: {token}");
                    //    //Console.WriteLine($"Token Issuer: {securityToken.Issuer}");
                    //    //Console.WriteLine($"Key ID: {kid}");
                    //    //Console.WriteLine($"Validate Lifetime: {parameters.ValidateLifetime}");
                    //    // Initialize an HttpClient instance for fetching the JWKS
                    //    var httpClient = new HttpClient();
                    //    // Synchronously fetch the JWKS (JSON Web Key Set) from the specified URL
                    //    var jwks = httpClient.GetStringAsync($"{builder.Configuration["Jwt:Issuer"]}/.well-known/jwks.json").Result;
                    //    // Parse the fetched JWKS into a JsonWebKeySet object
                    //    var keys = new JsonWebKeySet(jwks);
                    //    // Return the collection of JsonWebKey objects for token validation
                    //    return keys.Keys;
                    //}
                };
            });

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.SeedRoles();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseMiddleware();
//app.UseExceptionHandlerMiddleware(); 

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
app.MapControllers();

app.Run();