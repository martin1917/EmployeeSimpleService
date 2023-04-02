using Npgsql;

namespace Data.Postgres
{
    public class ConnectionFactory
    {
        private readonly DbConfig dbConfig;

        public ConnectionFactory(DbConfig  dbConfig)
        {
            this.dbConfig = dbConfig;
        }

        public NpgsqlConnection Create()
        {
            return new NpgsqlConnection(
                $"User ID={dbConfig.User};" +
                $"Password={dbConfig.Password};" +
                $"Host={dbConfig.Host};" +
                $"Port={dbConfig.Port};" +
                $"Database={dbConfig.DataBase};");
        }
    }
}
