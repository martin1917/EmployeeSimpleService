using Api.Model;
using Application.Exceptions;
using Application.Service.Base;
using Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateEmployeeRequest createEmployeeRequest)
        {
            var employee = new Employee
            {
                Name = createEmployeeRequest.Name,
                Surname = createEmployeeRequest.Surname,
                Phone = createEmployeeRequest.Phone,
                CompanyId = createEmployeeRequest.CompanyId,
                DepartmentId = createEmployeeRequest.DepartmentId,
                Passport = new Passport
                {
                    Type = createEmployeeRequest.Passport.Type,
                    Number = createEmployeeRequest.Passport.Number,
                }
            };

            int employeeId = -1;
            try
            {
                employeeId = employeeService.CreateEmployee(employee);
            }
            catch (PassportAlreadyExistException)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "Такой паспорт уже существует",
                });
            }

            return Ok(employeeId);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            bool deleteSuccess = employeeService.DeleteEmployeeById(id);
            if (!deleteSuccess)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = $"Ошибка удаленияю. Сотрудника с id = {id} не существует"
                });
            }

            return Ok(id);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateEmployeeRequest updateEmployeeRequest)
        {
            var employee = new Employee
            {
                Name = updateEmployeeRequest.Name,
                Surname = updateEmployeeRequest.Surname,
                Phone = updateEmployeeRequest.Phone,
                CompanyId = updateEmployeeRequest.CompanyId ?? default,
                DepartmentId = updateEmployeeRequest.DepartmentId ?? default
            };

            if (updateEmployeeRequest.Passport != null)
            {
                employee.Passport = new Passport
                {
                    Type = updateEmployeeRequest.Passport.Type,
                    Number = updateEmployeeRequest.Passport.Number
                };
            }

            employeeService.UpdateEmployee(id, employee);
            return Ok(id);
        }
    }
}
