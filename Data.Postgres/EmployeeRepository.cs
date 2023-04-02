using Core.Model;
using Core.Repository;
using Dapper;

namespace Data.Postgres
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ConnectionFactory connectionFactory;

        public EmployeeRepository(ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public int CreateEmployee(Employee employee)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();

                int employeeId = -1;
                using (var transaction = connection.BeginTransaction())
                {
                    var insertPassportSql =
                        $"INSERT INTO passports (type, number) " +
                        $"VALUES ('{employee.Passport.Type}', '{employee.Passport.Number}') " +
                        $"RETURNING id";

                    int passportId = connection.Query<int>(insertPassportSql, transaction: transaction).FirstOrDefault();

                    var insertEmployeeSql = 
                        $"INSERT INTO employees (name, surname, phone, company_id, passport_id, department_id) " +
                        $"VALUES ('{employee.Name}', '{employee.Surname}', '{employee.Phone}', '{employee.CompanyId}', '{passportId}', '{employee.DepartmentId}') " +
                        $"RETURNING id";

                    employeeId = connection.Query<int>(insertEmployeeSql, transaction: transaction).FirstOrDefault();

                    transaction.Commit();
                }

                return employeeId;
            }
        }

        public bool DeleteEmployeeById(int employeeId)
        {
            var deleteSql = $"DELETE FROM employees WHERE id = '{employeeId}'";
            using (var connection = connectionFactory.Create())
            {
                int res = connection.Execute(deleteSql);
                return res != 0;
            }
        }

        public Employee? GetEmployeeById(int employeeId)
        {
            var selectSql = 
                $"SELECT * FROM employees e " +
                $"JOIN passports p " +
                $"ON e.passport_id = p.id " +
                $"JOIN departments d " +
                $"ON e.department_id = d.id " +
                $"WHERE id = '{employeeId}'";

            using (var connection = connectionFactory.Create())
            {
                var employee = connection.Query<Employee, Passport, Department, Employee>(
                    selectSql, 
                    (employee, passport, department) =>
                    {
                        employee.Passport = passport;
                        employee.Department = department;
                        return employee;
                    }).FirstOrDefault();

                return employee;
            }
        }

        public Passport? GetEmployeePassport(string type, string number)
        {
            var selectSql = $"SELECT * FROM passports WHERE type = '{type}' AND number = '{number}'";
            using (var connection = connectionFactory.Create())
            {
                var passport = connection.Query<Passport>(selectSql).FirstOrDefault();
                return passport;
            }
        }

        public IEnumerable<Employee> GetEmployeesFromCompany(int companyId)
        {
            var selectSql =
                $"SELECT * FROM employees e " +
                $"JOIN passports p " +
                $"ON e.passport_id = p.id " +
                $"JOIN departments d " +
                $"ON e.department_id = d.id " +
                $"WHERE e.company_id = '{companyId}'";

            var dictDepartment = new Dictionary<int, Department>();

            using (var connection = connectionFactory.Create())
            {
                var employees = connection.Query<Employee, Passport, Department, Employee>(
                    selectSql,
                    (employee, passport, department) =>
                    {
                        employee.Passport = passport;

                        if (!dictDepartment.ContainsKey(department.Id))
                        {
                            dictDepartment.Add(department.Id, department);
                        }

                        employee.Department = dictDepartment[department.Id];
                        return employee;
                    });

                return employees;
            }
        }

        public IEnumerable<Employee> GetEmployeesFromDepartment(int companyId, string departmentName)
        {
            var selectSql =
                $"SELECT * FROM employees e " +
                $"JOIN passports p " +
                $"ON e.passport_id = p.id " +
                $"JOIN departments d " +
                $"ON e.department_id = d.id " +
                $"WHERE e.company_id = '{companyId}' AND d.name = '{departmentName}'";

            var dictDepartment = new Dictionary<int, Department>();

            using (var connection = connectionFactory.Create())
            {
                var employees = connection.Query<Employee, Passport, Department, Employee>(
                    selectSql,
                    (employee, passport, department) =>
                    {
                        employee.Passport = passport;

                        if (!dictDepartment.ContainsKey(department.Id))
                        {
                            dictDepartment.Add(department.Id, department);
                        }

                        employee.Department = dictDepartment[department.Id];
                        return employee;
                    });

                return employees;
            }
        }

        public void UpdateEmployee(Employee updatedEmployee)
        {
            var existEmployee = GetEmployeeById(updatedEmployee.Id);

            if (existEmployee == null)
            {
                return;
            }

            if (updatedEmployee.Name != default)
            {
                existEmployee.Name = updatedEmployee.Name;
            }

            if (updatedEmployee.Surname != default)
            {
                existEmployee.Surname = updatedEmployee.Surname;
            }

            if (updatedEmployee.Phone != default)
            {
                existEmployee.Phone = updatedEmployee.Phone;
            }

            if (updatedEmployee.CompanyId != default)
            {
                existEmployee.CompanyId = updatedEmployee.CompanyId;
            }

            if (updatedEmployee.DepartmentId != default)
            {
                existEmployee.DepartmentId = updatedEmployee.DepartmentId;
            }

            if (updatedEmployee.Passport != default)
            {
                if (updatedEmployee.Passport.Type != default)
                {
                    existEmployee.Passport.Type = updatedEmployee.Passport.Type;
                }

                if (updatedEmployee.Passport.Number != default)
                {
                    existEmployee.Passport.Number = updatedEmployee.Passport.Number;
                }
            }

            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var updateEmployeeSql = 
                        $"UPDATE employees SET " +
                        $"name = '{existEmployee.Name}', " +
                        $"surname = '{existEmployee.Surname}', " +
                        $"phone = '{existEmployee.Phone}', " +
                        $"company_id = '{existEmployee.CompanyId}', " +
                        $"department_id = '{existEmployee.DepartmentId}' " +
                        $"WHERE id = '{existEmployee.Id}'";

                    var updatePassportSql =
                        $"UPDATE passports SET " +
                        $"type = '{existEmployee.Passport.Type}', " +
                        $"number = '{existEmployee.Passport.Number}' " +
                        $"WHERE id = '{existEmployee.Passport.Id}'";

                    connection.Execute(updateEmployeeSql, transaction: transaction);
                    connection.Execute(updatePassportSql, transaction: transaction);
                    transaction.Commit();
                }
            }
        }
    }
}
