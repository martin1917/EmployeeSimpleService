namespace Api.Model
{
    public class UpdateEmployeeRequest
    {
        public string? Name { get; set; }

        public string? Surname { get; set; }

        public string? Phone { get; set; }

        public int? CompanyId { get; set; }

        public int? DepartmentId { get; set; }

        public PassportDto? Passport { get; set; }
    }
}
