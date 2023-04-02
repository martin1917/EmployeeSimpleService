using Core.Model;
using Core.Repository;
using Dapper;
using Npgsql;

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
                        $"INSERT INTO passports (Type, Number) " +
                        $"VALUES ('{employee.Passport.Type}', '{employee.Passport.Number}') " +
                        $"RETURNING Id";

                    int passportId = connection.Query<int>(insertPassportSql, transaction: transaction).FirstOrDefault();

                    var insertEmployeeSql = 
                        $"INSERT INTO employees (Name, Surname, Phone, CompanyId, PassportId, DepartmentId) " +
                        $"VALUES ('{employee.Name}', '{employee.Surname}', '{employee.Phone}', {employee.CompanyId}, {passportId}, {employee.DepartmentId}) " +
                        $"RETURNING Id";

                    employeeId = connection.Query<int>(insertEmployeeSql, transaction: transaction).FirstOrDefault();

                    transaction.Commit();
                }

                return employeeId;
            }
        }

        public bool DeleteEmployeeById(int employeeId)
        {
            var deleteSql = $"DELETE FROM employees WHERE Id = {employeeId}";
            using (var connection = connectionFactory.Create())
            {
                int res = connection.Execute(deleteSql);
                return res != 0;
            }
        }

        public Passport? GetEmployeePassport(string type, string number)
        {
            var selectSql = $"SELECT * FROM passports WHERE Type = '{type}' AND Number = '{number}'";
            using (var connection = connectionFactory.Create())
            {
                var passport = connection.Query<Passport>(selectSql).FirstOrDefault();
                return passport;
            }
        }

        public Employee? GetEmployeeById(int employeeId)
        {
            var selectSql = 
                $"SELECT * FROM employees e " +
                $"JOIN passports p " +
                $"ON e.PassportId = p.Id " +
                $"JOIN departments d " +
                $"ON e.DepartmentId = d.Id " +
                $"WHERE e.Id = {employeeId}";

            using (var connection = connectionFactory.Create())
            {
                var employees = LoadEmployeesWithPassportAndDepartment(connection, selectSql);
                return employees.FirstOrDefault();
            }
        }

        public IEnumerable<Employee> GetEmployeesFromCompany(int companyId)
        {
            var selectSql =
                $"SELECT * FROM employees e " +
                $"JOIN passports p " +
                $"ON e.PassportId = p.Id " +
                $"JOIN departments d " +
                $"ON e.DepartmentId = d.Id " +
                $"WHERE e.CompanyId = {companyId}";

            using (var connection = connectionFactory.Create())
            {
                var employees = LoadEmployeesWithPassportAndDepartment(connection, selectSql);
                return employees;
            }
        }

        public IEnumerable<Employee> GetEmployeesFromDepartment(int companyId, string departmentName)
        {
            var selectSql =
                $"SELECT * FROM employees e " +
                $"JOIN passports p " +
                $"ON e.PassportId = p.Id " +
                $"JOIN departments d " +
                $"ON e.DepartmentId = d.Id " +
                $"WHERE e.CompanyId = {companyId} AND d.Name = '{departmentName}'";

            using (var connection = connectionFactory.Create())
            {
                var employees = LoadEmployeesWithPassportAndDepartment(connection, selectSql);
                return employees;
            }
        }

        private IEnumerable<Employee> LoadEmployeesWithPassportAndDepartment(NpgsqlConnection connection, string selectSql)
        {
            var dictDepartment = new Dictionary<int, Department>();
            return connection.Query<Employee, Passport, Department, Employee>(
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
        }

        public void UpdateEmployee(Employee updatedEmployee)
        {
            var existEmployee = GetEmployeeById(updatedEmployee.Id);

            if (existEmployee == null)
            {
                return;
            }

            if (updatedEmployee.Name != null)
            {
                existEmployee.Name = updatedEmployee.Name;
            }

            if (updatedEmployee.Surname != null)
            {
                existEmployee.Surname = updatedEmployee.Surname;
            }

            if (updatedEmployee.Phone != null)
            {
                existEmployee.Phone = updatedEmployee.Phone;
            }

            if (updatedEmployee.CompanyId > 0)
            {
                existEmployee.CompanyId = updatedEmployee.CompanyId;
            }

            if (updatedEmployee.DepartmentId > 0)
            {
                existEmployee.DepartmentId = updatedEmployee.DepartmentId;
            }

            if (updatedEmployee.Passport != null)
            {
                if (updatedEmployee.Passport.Type != null)
                {
                    existEmployee.Passport.Type = updatedEmployee.Passport.Type;
                }

                if (updatedEmployee.Passport.Number != null)
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
                        $"Name = '{existEmployee.Name}', " +
                        $"Surname = '{existEmployee.Surname}', " +
                        $"Phone = '{existEmployee.Phone}', " +
                        $"CompanyId = {existEmployee.CompanyId}, " +
                        $"DepartmentId = {existEmployee.DepartmentId} " +
                        $"WHERE Id = {existEmployee.Id}";

                    var updatePassportSql =
                        $"UPDATE passports SET " +
                        $"Type = '{existEmployee.Passport.Type}', " +
                        $"Number = '{existEmployee.Passport.Number}' " +
                        $"WHERE Id = {existEmployee.Passport.Id}";

                    connection.Execute(updateEmployeeSql, transaction: transaction);
                    connection.Execute(updatePassportSql, transaction: transaction);
                    transaction.Commit();
                }
            }
        }
    }
}
