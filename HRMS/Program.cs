using HRMS.Data;
using HRMS.Mappings;
using HRMS.Repositories;
using HRMS.Repositories.Interfaces;
using HRMS.Services;
using HRMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg => {
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzg2NzUyMDAwIiwiaWF0IjoiMTc1NTIyODM0MyIsImFjY291bnRfaWQiOiIwMTk4YWJjMGU5ZDE3NTU4YWFhYzI4YmNkNzA0MzFhNyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazJudzYydjZ2MjExajNuYXh6MWt2OGU1Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.tKwsu-SYkV4EbdJx7WSUD-8SpE2l3PhkhXJpSiUCL6KpqxGis1sQN2OGWW6bswlS3H0Y9sIob1laMcv0CYMEASyoKeCKEt7NBb7lDOXtxXcO_Se4HvwnPZqpcE541yc_Jino9w0wwyBQSjLt_LEmq0jaV4ZRg-qElHDLXavDjgomcj9jcXi3E39osj9Inuzptt_ANnp7NSSaMBO8diG9xvWUXrEbhFDtIqBjJXgnnvajTP67ROMSRgpY93VaKT7yl3B7QTEq8zV18GYOLLsD6X_VEFZUFnMDIpwJOO-7cJHg_oM0N9ZXAtsR92dKlQdTA66eycnixfAkHsp_cNdkZw";
});

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
//builder.Services.AddScoped<IDepartmentService, DepartmentService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.OpenConnection();
        Console.WriteLine(" Database connected");
        dbContext.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();