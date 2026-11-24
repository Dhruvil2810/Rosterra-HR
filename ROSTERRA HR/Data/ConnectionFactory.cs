using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ROSTERRA_HR.Data
{
    /// <summary>
    /// Factory for managing ADO.NET SQL Server connections to RosterraHRDB.
    /// Reads connection configuration from Web.config.
    /// </summary>
    public static class ConnectionFactory
    {
        private static readonly string ConnectionString;

        static ConnectionFactory()
        {
            var setting = ConfigurationManager.ConnectionStrings["RosterraHRConnection"];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
            {
                throw new InvalidOperationException("Connection string 'RosterraHRConnection' was not found in Web.config.");
            }
            ConnectionString = setting.ConnectionString;
        }

        /// <summary>
        /// Creates a new, unopened SqlConnection instance.
        /// </summary>
        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Creates and opens a new SqlConnection instance.
        /// Caller must dispose via using statement.
        /// </summary>
        public static SqlConnection CreateOpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
