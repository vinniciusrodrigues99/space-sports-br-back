using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FSP.Api.Infrastructure.Connector
{
    public class DbConnector(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
