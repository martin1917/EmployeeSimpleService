using Core.Model;

namespace Core.Repository
{
    public interface IEmployeeRepository
    {
        Employee? GetEmployeeById(int employeeId);

        Passport? GetEmployeePassport(string type, string number);

        int CreateEmployee(Employee employee);

        bool DeleteEmployeeById(int employeeId);

        void UpdateEmployee(Employee employee);

        IEnumerable<Employee> GetEmployeesFromCompany(int companyId);

        IEnumerable<Employee> GetEmployeesFromDepartment(int companyId, string departmentName);
    }
}
