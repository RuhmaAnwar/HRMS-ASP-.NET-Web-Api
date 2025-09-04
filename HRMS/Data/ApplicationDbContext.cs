using HRMS.Enums;
using HRMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Cryptography;

namespace HRMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;

        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<Leave> Leaves { get; set; } = null!;
        public DbSet<SigningKey> SigningKeys { get; set; } = null!;

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // ignores the dynamic values error in seeding
        // ////////////////////////////////////////////////////////////// remove in production
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        // //////////////////////////////////////////////////////////////////////////////////
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SigningKey>(entity =>
            {
                entity.ToTable("signing_keys");

                entity.HasKey(k => k.Id);
                using var rsa = RSA.Create(2048);
                // Export the private key as a Base64-encoded string.
                var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                // Export the public key as a Base64-encoded string.
                var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                var newKeyId = Guid.NewGuid().ToString();

                entity.HasData(new SigningKey
                {
                    Id = Guid.NewGuid(),
                    KeyId = newKeyId,
                    PrivateKey = privateKey,
                    PublicKey = publicKey,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddHours(5),
                    ExpiresAt = DateTime.UtcNow.AddYears(1) // Set the new key to expire in one year.
                });
            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens");
                entity.HasKey(rf => rf.Id);

                // When a User is deleted, their associated refresh tokens are also deleted to maintain data integrity
                entity.HasOne(rt => rt.Employee)
                .WithMany(e => e.RefreshTokens)
                .HasForeignKey(rt => rt.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(rt => rt.Token).IsUnique();
            });

            // Configure Employee entity
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employees");
                // Primary Key
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.FirstName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.LastName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.DateOfBirth)
                      .IsRequired()
                      .HasColumnType("date");

                entity.Property(e => e.Address)
                      .HasMaxLength(500);

                entity.Property(e => e.PhoneNumber)
                      .HasMaxLength(20);

                entity.Property(e => e.HireDate)
                      .IsRequired()
                      .HasColumnType("date");

                entity.Property(e => e.Salary)
                      .IsRequired()
                      .HasColumnType("numeric(18,2)");

                entity.Property(e => e.DepartmentId)
                      .IsRequired()
                      .HasColumnType("uuid");

                entity.Property(e => e.PositionId)
                      .IsRequired()
                      .HasColumnType("uuid");

                entity.Property(e => e.ManagerId)
                      .HasColumnType("uuid");

                entity.Property(e => e.TotalLeaves)
                      .IsRequired()
                      .HasDefaultValue(25);

                entity.Property(e => e.LeavesUsed)
                      .IsRequired()
                      .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.IsDeleted)
                      .IsRequired()
                      .HasDefaultValue(false);

                // Relationships
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Employees)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Position)
                      .WithMany(p => p.Employees)
                      .HasForeignKey(e => e.PositionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Manager)
                      .WithMany(e => e.Subordinates)
                      .HasForeignKey(e => e.ManagerId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.RefreshTokens)
                      .WithOne(rf => rf.Employee)
                      .HasForeignKey(rf => rf.EmployeeId);

                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.PositionId);
                entity.HasIndex(e => e.ManagerId);
                entity.HasIndex(e => e.IsDeleted);

                // employee seeding
                var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                var deptId = Guid.Parse("550e8400-e29b-41d4-a716-446655440001");
                var posId = Guid.Parse("550e8400-e29b-41d4-a716-446655440101");

                entity.HasData(new Employee
                {
                    Id = adminId,
                    FirstName = "Ruhma",
                    LastName = "Anwar",
                    UserName = "ruhma@email.com",   
                    NormalizedUserName = "RUHMA@EMAIL.COM",
                    Email = "ruhma@email.com",
                    NormalizedEmail = "RUHMA@EMAIL.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "03077147775",
                    PhoneNumberConfirmed = true,
                    DateOfBirth = new DateTime(2003, 1, 1),
                    Address = "HQ",
                    HireDate = new DateTime(2025, 1, 1),
                    Salary = 1000m,
                    DepartmentId = deptId,
                    PositionId = posId,
                    ManagerId = null,
                    TotalLeaves = 25,
                    LeavesUsed = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = new PasswordHasher<Employee>().HashPassword(null!, "Admin.123")
                });
            });


            // Configure Position entity
            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("positions");
                entity.HasKey(e => e.Id);

                // Properties
                entity.Property(e => e.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Description)
                      .HasMaxLength(1000);

                entity.Property(e => e.SalaryRangeMin)
                      .IsRequired()
                      .HasColumnType("numeric(18,2)");

                entity.Property(e => e.SalaryRangeMax)
                      .IsRequired()
                      .HasColumnType("numeric(18,2)");

                entity.Property(e => e.DepartmentId)
                      .HasColumnType("uuid"); // Nullable, as positions don't always need a department

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.IsDeleted)
                      .IsRequired()
                      .HasDefaultValue(false);

                // Relationships
                // Department (Many-to-One, optional)
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Positions)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.SetNull); 

                // Employees (One-to-Many)
                entity.HasMany(e => e.Employees)
                      .WithOne(e => e.Position)
                      .HasForeignKey(e => e.PositionId)
                      .OnDelete(DeleteBehavior.Restrict); 

                // Indexes 
                entity.HasIndex(e => e.Title)
                      .IsUnique();

                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.IsDeleted);

                // Seed data for Positions
                entity.HasData(
                    new Position
                    {
                        Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440100"),
                        Title = "HR Manager",
                        Description = "human resource manager",
                        SalaryRangeMin = 320000.00m,
                        SalaryRangeMax = 200000.00m,
                        DepartmentId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Position
                    {
                        Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440101"),
                        Title = "Software Engineer",
                        Description = "snr soft engr",
                        SalaryRangeMin = 150000.00m,
                        SalaryRangeMax = 200000.00m,
                        DepartmentId = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Position
                    {
                        Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440102"),
                        Title = "Financial Analyst",
                        Description = "snr soft engr",
                        SalaryRangeMin = 320000.00m,
                        SalaryRangeMax = 200000.00m,
                        DepartmentId = Guid.Parse("550e8400-e29b-41d4-a716-446655440002"),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                );
            });

            // Configure Department entity
            modelBuilder.Entity<Department>(entity =>
            {
                // Table name
                entity.ToTable("departments");

                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties
                entity.Property(e => e.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Description)
                      .HasMaxLength(1000);

                entity.Property(e => e.HeadEmployeeId)
                      .HasColumnName("head_employee_id")
                      .HasColumnType("uuid"); // Nullable, as not all departments require a head

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.IsDeleted)
                      .IsRequired()
                      .HasDefaultValue(false);

                // Relationships
                // HeadEmployee (One-to-One, optional)
                entity.HasOne(e => e.HeadEmployee)
                      .WithOne(e => e.DepartmentLed)
                      .HasForeignKey<Department>(e => e.HeadEmployeeId)
                      .OnDelete(DeleteBehavior.SetNull); 

                // Employees (One-to-Many)
                entity.HasMany(e => e.Employees)
                      .WithOne(d => d.Department)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict); 

                // Positions (One-to-Many)
                entity.HasMany(e => e.Positions)
                      .WithOne(p => p.Department)
                      .HasForeignKey(p => p.DepartmentId)
                      .OnDelete(DeleteBehavior.SetNull); 

                // Indexes
                entity.HasIndex(e => e.Name)
                      .IsUnique();

                entity.HasIndex(e => e.HeadEmployeeId)
                      .IsUnique() //employee can head only one department
                      .HasFilter("\"head_employee_id\" IS NOT NULL"); // Only apply uniqueness to non-null values

                entity.HasIndex(e => e.IsDeleted);

                // Seed data for Departments
                entity.HasData(
                    new Department
                    {
                        Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                        Name = "HR",
                        Description = "Human Resource dept",
                        HeadEmployeeId = null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Department
                    {
                        Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
                        Name = "Engineering",
                        Description = "engineering dept",
                        HeadEmployeeId = null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Department
                    {
                        Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440002"),
                        Name = "Finance",
                        Description = "Finance dept",
                        HeadEmployeeId = null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                );
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                // Table name
                entity.ToTable("attendances");

                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties
                entity.Property(e => e.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.EmployeeId)
                      .IsRequired()
                      .HasColumnType("uuid");

                entity.Property(e => e.Date)
                      .IsRequired()
                      .HasColumnType("date");

                entity.Property(e => e.CheckInTime)
                      .HasColumnType("time");

                entity.Property(e => e.CheckOutTime)
                      .HasColumnType("time"); 

                entity.Property(e => e.Status)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasConversion(
                          v => v.ToString(), // enum to string, to db 
                          v => Enum.Parse<AttendanceStatus>(v) // string to enum, from db
                      )
                      .HasDefaultValue(AttendanceStatus.Pending);

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.IsDeleted)
                      .IsRequired()
                      .HasDefaultValue(false);

                // Relationships
                // Employee (Many-to-One)
                entity.HasOne(e => e.Employee)
                      .WithMany(e => e.Attendances) 
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict); 

                // Indexes 
                entity.HasIndex(e => e.EmployeeId);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => new { e.EmployeeId, e.Date })
                      .IsUnique(); // one attendance record per employee per date
                entity.HasIndex(e => e.IsDeleted);
            });

            modelBuilder.Entity<Leave>(entity =>
            {
                entity.ToTable("leaves");
                entity.HasKey(l => l.Id);

                entity.Property(l => l.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(l => l.EmployeeId)
                      .IsRequired()
                      .HasColumnType("uuid");

                entity.Property(l => l.StartDate)
                      .IsRequired()
                      .HasColumnType("date");

                entity.Property(l => l.EndDate)
                      .IsRequired()
                      .HasColumnType("date");

                entity.Property(l => l.Type)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasConversion(
                          v => v.ToString(),
                          v => Enum.Parse<LeaveType>(v) 
                      )
                      .HasDefaultValue(LeaveType.Casual);

                entity.Property(l => l.Status)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasConversion(
                          v => v.ToString(), 
                          v => Enum.Parse<LeaveStatus>(v) 
                      )
                      .HasDefaultValue(LeaveStatus.Pending);

                entity.Property(l => l.ApproverId)
                      .HasColumnType("uuid"); 

                entity.Property(l => l.Reason)
                      .HasMaxLength(1000);

                entity.Property(l => l.CreatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(l => l.UpdatedAt)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAddOrUpdate();

                entity.Property(l => l.IsDeleted)
                      .IsRequired()
                      .HasDefaultValue(false);

                // Relationships
                // Employee (Many-to-One for leave requester)
                entity.HasOne(l => l.Employee)
                      .WithMany(e => e.LeaveRequests) 
                      .HasForeignKey(l => l.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict); 

                // Approver (Many-to-One, optional, for leave approver)
                entity.HasOne(l => l.Approver)
                      .WithMany(e => e.ApprovedLeaves) 
                      .HasForeignKey(l => l.ApproverId)
                      .OnDelete(DeleteBehavior.SetNull); 

                entity.HasIndex(l => l.EmployeeId);
                entity.HasIndex(l => l.ApproverId);
                entity.HasIndex(l => l.StartDate);
                entity.HasIndex(l => l.EndDate);
                entity.HasIndex(l => l.Status);
                entity.HasIndex(l => l.IsDeleted);
            });

            

            modelBuilder.Entity<IdentityRole>().HasData(
           new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
           new IdentityRole { Id = "2", Name = "Hr", NormalizedName = "HR" },
           new IdentityRole { Id = "3", Name = "Manager", NormalizedName = "MANAGER" },
           new IdentityRole { Id = "4", Name = "Employee", NormalizedName = "EMPLOYEE" }
           );

            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");

           

        }
    }
}