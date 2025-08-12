using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HRMS.Utilities
{
    public static class ExceptionHandler 
    { 
        public static IActionResult HandleException(Exception ex, string tableName, string message = "")
        {
            switch(ex){

                case DbUpdateConcurrencyException exception:
                    return new ConflictObjectResult(new { Message = $"The {tableName} record was modified by another user. Please refresh and try again." });
                   
                case Npgsql.PostgresException pgEx:
                        switch (pgEx.SqlState)
                        {
                            case "42703":
                                return new BadRequestObjectResult($"Invalid column name");
                            case "23505": // Unique violation 
                                return new ConflictObjectResult(new { Message = $"{tableName} with this email already exists." });
                            case "23502": // Not-null violation
                                return new BadRequestObjectResult(new { Message = "A required field is missing." });
                            default:
                                Console.WriteLine($"Database error: {pgEx.Message}");
                                return new ObjectResult(new { Message = message })
                                {
                                    StatusCode = 500
                                };
                        }

                default:
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    return new ObjectResult(message)
                    {
                        StatusCode = 500
                    };
            }
            return new ObjectResult(new { Message = "Unhandled Database Error" })
            {
                StatusCode = 500
            };

        }
    }
}
