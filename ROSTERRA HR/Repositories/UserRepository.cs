using System;
using System.Data;
using System.Data.SqlClient;
using ROSTERRA_HR.Data;
using ROSTERRA_HR.Models;

namespace ROSTERRA_HR.Repositories
{
    /// <summary>
    /// Data repository for User operations using manual ADO.NET.
    /// Uses parameterized queries to prevent SQL injection.
    /// </summary>
    public class UserRepository
    {
        /// <summary>
        /// Retrieves a user record by their unique username.
        /// Returns null if no user is found.
        /// </summary>
        public User GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            const string sql = @"
                SELECT UserId, Username, PasswordHash, PasswordSalt, Role, IsActive, CreatedAtUtc, LastLoginAtUtc
                FROM dbo.Users
                WHERE Username = @Username;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username.Trim();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                            Role = reader.GetString(reader.GetOrdinal("Role")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                            LastLoginAtUtc = reader.IsDBNull(reader.GetOrdinal("LastLoginAtUtc"))
                                ? (DateTime?)null
                                : reader.GetDateTime(reader.GetOrdinal("LastLoginAtUtc"))
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the LastLoginAtUtc timestamp to current UTC time.
        /// </summary>
        public void UpdateLastLogin(int userId)
        {
            const string sql = @"
                UPDATE dbo.Users
                SET LastLoginAtUtc = SYSUTCDATETIME()
                WHERE UserId = @UserId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.ExecuteNonQuery();
            }
        }
    }
}
