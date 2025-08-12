using Microsoft.AspNetCore.Mvc;

namespace HRMS.Utilities
{
    public static class RequestValidator
    {
        public static IActionResult? ValidatePaginationParameters( int page, int pageSize, int maxPageSize = 100)
        {
            if(page < 1 || pageSize < 1)
            {   
                return new BadRequestObjectResult("Page and pageSize must be greater than 0.");
                
            }
            if (pageSize > 100) // Prevent excessive data
            {
                return new BadRequestObjectResult("PageSize cannot exceed 100.");
            }
            return null;
        }
    }
}