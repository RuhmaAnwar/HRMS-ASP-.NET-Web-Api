using HRMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IActionResult> GetAllAsync(string filter, int page, int pageSize);
        Task<IActionResult> GetByIdAsync(int id);
        Task<IActionResult> CreateAsync(Department department);
        Task<IActionResult> UpdateAsync(int id, Department department);
        Task<IActionResult> DeleteAsync(int id);
    }
}