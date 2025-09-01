
using HRMS.Extensions;
using HRMS.Middleware; 
using HRMS.Repositories;
using HRMS.Repositories.Interfaces;
using HRMS.Services;
using HRMS.Services.Interfaces;
using HRMS.Jobs;
using Quartz;
//using Microsoft.AspNetCore.Authentication.JwtBearer;

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
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//           .AddJwtBearer(jwtOptions =>
//           {
//               jwtOptions.Authority = "https://{--your-authority--}";
//               jwtOptions.Audience = "https://{--your-audience--}";
//           });

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