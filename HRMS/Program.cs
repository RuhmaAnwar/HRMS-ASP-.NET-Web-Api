using HRMS.Data;
using HRMS.Mappings;
using HRMS.Models;
using HRMS.Repositories;
using HRMS.NewFolder;
using HRMS.Repositories.Interfaces;
using HRMS.Services;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.ConfigPostgres(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
//builder.Services.AddScoped<IDepartmentService, DepartmentService>();

// Scan the assembly where MappingProfile lives
builder.Services.ConfigAutomapper();

// Add Identity with Employee as user
builder.Services.AddEmployeeIdentity();

// Add Authentication with Cookies as default scheme
builder.Services.ConfigCookieAuthentication();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); //before authorization 
app.UseAuthorization();
app.MapControllers();

app.Run();