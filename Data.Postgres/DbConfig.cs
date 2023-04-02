namespace Data.Postgres
{
    public record DbConfig(string User, string Password, string Host, string Port, string DataBase);
}
