using Application.Exceptions;
using Application.Service.Base;
using Core.Model;
using Core.Repository;

namespace Application.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public int CreateEmployee(Employee employee)
        {
            var passport = employeeRepository.GetEmployeePassport(employee.Passport.Type, employee.Passport.Number);
            if (passport != null)
            {
                throw new PassportAlreadyExistException();
            }

            int employeeId = employeeRepository.CreateEmployee(employee);
            return employeeId;
        }

        public bool DeleteEmployeeById(int employeeId)
        {
            return employeeRepository.DeleteEmployeeById(employeeId);
        }

        public void UpdateEmployee(int employeeId, Employee updatedEmployee)
        {
            updatedEmployee.Id = employeeId;
            employeeRepository.UpdateEmployee(updatedEmployee);
        }

        public IEnumerable<Employee> GetEmployeesFromCompany(int companyId)
        {
            var employees = employeeRepository.GetEmployeesFromCompany(companyId);
            return employees;
        }

        public IEnumerable<Employee> GetEmployeesFromDepartment(int companyId, string departmentName)
        {
            var employees = employeeRepository.GetEmployeesFromDepartment(companyId, departmentName);
            return employees;
        }
    }
}
