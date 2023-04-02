using Core.Model;

namespace Application.Service.Base
{
    public interface IEmployeeService
    {
        int CreateEmployee(Employee employee);

        bool DeleteEmployeeById(int employeeId);

        IEnumerable<Employee> GetEmployeesFromCompany(int companyId);

        IEnumerable<Employee> GetEmployeesFromDepartment(int companyId, string departmentName);

        void UpdateEmployee(int employeeId, Employee updatedEmployee);
    }
}