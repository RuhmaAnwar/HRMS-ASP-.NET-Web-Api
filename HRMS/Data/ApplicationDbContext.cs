using HRMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasIndex(e => e.Email)
                    .IsUnique();
                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }

        // Method to execute raw SQL query for employees
        public async Task<List<Employee>> GetEmployeesWithRawSqlAsync(string filter, string sortColumn, bool sortDescending, int page, int pageSize)
        {
            // Sanitize inputs to prevent SQL injection
            Console.WriteLine(filter);
            filter = string.IsNullOrWhiteSpace(filter) ? "%%" : $"%{filter.Replace("'", "''")}%";
            Console.WriteLine("filter: ",filter);
            sortColumn = sortColumn switch
            {
                "FirstName" => "first_name",
                "LastName" => "last_name",
                "Email" => "email",
                "Department" => "department",
                "Role" => "role",
                _ => "id" // Default to id
            };
            var sortDirection = sortDescending ? "DESC" : "ASC";
            var offset = (page - 1) * pageSize;

            // Build raw SQL query
            var query = @"
                SELECT id, first_name, last_name, email, department, role
                FROM employees
                WHERE first_name ILIKE @filter
                   OR last_name ILIKE @filter
                   OR email ILIKE @filter
                ORDER BY {0} {1}
                LIMIT @pageSize OFFSET @offset";

            // Format query with sort column and direction
            query = string.Format(query, sortColumn, sortDirection);
            //Console.WriteLine(query);
            // Execute query using FromSql
            var e = Employees
                .FromSqlRaw(query, new Npgsql.NpgsqlParameter("@filter", filter),
                                  new Npgsql.NpgsqlParameter("@pageSize", pageSize),
                                  new Npgsql.NpgsqlParameter("@offset", offset))
                .ToListAsync();
            //Console.WriteLine(e);
            return await e;
        }
    }
}