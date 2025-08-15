using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HRMS.Utilities
{
    public static class ExceptionHandler
    {
        public static IActionResult HandleException(Exception ex, string tableName, string message = "")
        {
            switch (ex)
            {
                case DbUpdateConcurrencyException:
                    return new ConflictObjectResult(new { Message = $"The {tableName} record was modified by another user. Please refresh and try again." });

                case DbUpdateException dbEx when dbEx.InnerException is PostgresException pgEx:
                    switch (pgEx.SqlState)
                    {
                        case "23505": // Unique violation
                            return new ConflictObjectResult(new { Message = $"A {tableName} with this email already exists." });
                        case "23502": // Not-null violation
                            return new BadRequestObjectResult(new { Message = "A required field is missing." });
                        case "23503": // Foreign key violation
                            return new BadRequestObjectResult(new { Message = $"Invalid {tableName} ID." });
                        default:
                            Console.WriteLine($"Database error: {pgEx.Message}");
                            return new ObjectResult(new { Message = message }) { StatusCode = 500 };
                    }

                case PostgresException pgEx:
                    switch (pgEx.SqlState)
                    {
                        case "42703": // Invalid column
                            return new BadRequestObjectResult(new { Message = $"Invalid sort column." });
                        default:
                            Console.WriteLine($"Database error: {pgEx.Message}");
                            return new ObjectResult(new { Message = message }) { StatusCode = 500 };
                    }

                default:
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    return new ObjectResult(new { Message = message }) { StatusCode = 500 };
            }
        }
    }
}