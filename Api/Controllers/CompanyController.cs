using Application.Service.Base;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public CompanyController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpGet("{companyId:int}/employees")]
        public IActionResult GetEmployeesFromCompany(int companyId)
        {
            return Ok(employeeService.GetEmployeesFromCompany(companyId));
        }

        [HttpGet("{companyId:int}/employees/departments/{departmentName:alpha}")]
        public IActionResult GetEmployeesFromCompany(int companyId, string departmentName)
        {
            return Ok(employeeService.GetEmployeesFromDepartment(companyId, departmentName));
        }
    }
}
