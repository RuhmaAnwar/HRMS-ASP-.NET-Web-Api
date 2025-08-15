using HRMS.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRMS.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IActionResult> GetAllAsync(string filter, string sort, bool sortDescending, int page, int pageSize);
        Task<IActionResult> GetByIdAsync(int id);
        Task<IActionResult> CreateAsync(EmployeeDTO employeeDTO);
        Task<IActionResult> UpdateAsync(int id, EmployeeDTO employeeDTO);
        Task<IActionResult> PatchAsync(int id, EmployeeDTOPatch employeeDTO);
        Task<IActionResult> DeleteAsync(int id);
    }
}