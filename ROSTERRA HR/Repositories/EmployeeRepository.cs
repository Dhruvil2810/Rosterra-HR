using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ROSTERRA_HR.Data;
using ROSTERRA_HR.Models;

namespace ROSTERRA_HR.Repositories
{
    /// <summary>
    /// Repository managing Employee data and related User account operations using manual ADO.NET.
    /// All queries are fully parameterized to protect against SQL Injection.
    /// </summary>
    public class EmployeeRepository
    {
        /// <summary>
        /// Retrieves all employees, optionally filtering by search text, department, and active status.
        /// </summary>
        public List<Employee> GetAllEmployees(string searchTerm = null, string department = null, bool? activeOnly = null)
        {
            var employees = new List<Employee>();

            string sql = @"
                SELECT e.EmployeeId, e.UserId, e.FirstName, e.LastName, e.Email, e.PhoneNumber,
                       e.Department, e.Position, e.Salary, e.HireDate, e.IsActive, e.CreatedAtUtc, e.UpdatedAtUtc,
                       u.Username
                FROM dbo.Employees e
                INNER JOIN dbo.Users u ON e.UserId = u.UserId
                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                sql += @" AND (e.FirstName LIKE @SearchTerm 
                             OR e.LastName LIKE @SearchTerm 
                             OR e.Email LIKE @SearchTerm 
                             OR e.Position LIKE @SearchTerm)";
            }

            if (!string.IsNullOrWhiteSpace(department) && department != "All")
            {
                sql += " AND e.Department = @Department";
            }

            if (activeOnly.HasValue)
            {
                sql += " AND e.IsActive = @ActiveOnly";
            }

            sql += " ORDER BY e.EmployeeId DESC;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    command.Parameters.Add("@SearchTerm", SqlDbType.NVarChar, 100).Value = "%" + searchTerm.Trim() + "%";
                }

                if (!string.IsNullOrWhiteSpace(department) && department != "All")
                {
                    command.Parameters.Add("@Department", SqlDbType.NVarChar, 50).Value = department.Trim();
                }

                if (activeOnly.HasValue)
                {
                    command.Parameters.Add("@ActiveOnly", SqlDbType.Bit).Value = activeOnly.Value;
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(MapEmployee(reader));
                    }
                }
            }

            return employees;
        }

        /// <summary>
        /// Retrieves a single employee by EmployeeId.
        /// </summary>
        public Employee GetEmployeeById(int employeeId)
        {
            const string sql = @"
                SELECT e.EmployeeId, e.UserId, e.FirstName, e.LastName, e.Email, e.PhoneNumber,
                       e.Department, e.Position, e.Salary, e.HireDate, e.IsActive, e.CreatedAtUtc, e.UpdatedAtUtc,
                       u.Username
                FROM dbo.Employees e
                INNER JOIN dbo.Users u ON e.UserId = u.UserId
                WHERE e.EmployeeId = @EmployeeId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapEmployee(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves an employee by their linked User account ID.
        /// Used for employee self-service profile dashboard.
        /// </summary>
        public Employee GetEmployeeByUserId(int userId)
        {
            const string sql = @"
                SELECT e.EmployeeId, e.UserId, e.FirstName, e.LastName, e.Email, e.PhoneNumber,
                       e.Department, e.Position, e.Salary, e.HireDate, e.IsActive, e.CreatedAtUtc, e.UpdatedAtUtc,
                       u.Username
                FROM dbo.Employees e
                INNER JOIN dbo.Users u ON e.UserId = u.UserId
                WHERE e.UserId = @UserId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapEmployee(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if an email address is already registered to another employee.
        /// </summary>
        public bool IsEmailTaken(string email, int? excludeEmployeeId = null)
        {
            string sql = "SELECT COUNT(1) FROM dbo.Employees WHERE Email = @Email";
            if (excludeEmployeeId.HasValue)
            {
                sql += " AND EmployeeId <> @ExcludeEmployeeId";
            }

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email.Trim();
                if (excludeEmployeeId.HasValue)
                {
                    command.Parameters.Add("@ExcludeEmployeeId", SqlDbType.Int).Value = excludeEmployeeId.Value;
                }

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        /// <summary>
        /// Checks if a username is already taken in the Users table.
        /// </summary>
        public bool IsUsernameTaken(string username, int? excludeUserId = null)
        {
            string sql = "SELECT COUNT(1) FROM dbo.Users WHERE Username = @Username";
            if (excludeUserId.HasValue)
            {
                sql += " AND UserId <> @ExcludeUserId";
            }

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username.Trim();
                if (excludeUserId.HasValue)
                {
                    command.Parameters.Add("@ExcludeUserId", SqlDbType.Int).Value = excludeUserId.Value;
                }

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        /// <summary>
        /// Atomically creates a user account with role 'Employee' and the corresponding employee record.
        /// Uses SqlTransaction to maintain database consistency.
        /// </summary>
        public int CreateEmployeeWithUser(Employee employee, string username, string passwordHash, string passwordSalt)
        {
            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // 1. Insert User Account
                    const string insertUserSql = @"
                        INSERT INTO dbo.Users (Username, PasswordHash, PasswordSalt, Role, IsActive, CreatedAtUtc)
                        VALUES (@Username, @PasswordHash, @PasswordSalt, 'Employee', 1, SYSUTCDATETIME());
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int newUserId;
                    using (var userCommand = new SqlCommand(insertUserSql, connection, transaction))
                    {
                        userCommand.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username.Trim();
                        userCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = passwordHash;
                        userCommand.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar, 256).Value = passwordSalt;

                        newUserId = Convert.ToInt32(userCommand.ExecuteScalar());
                    }

                    // 2. Insert Employee Record
                    const string insertEmployeeSql = @"
                        INSERT INTO dbo.Employees (
                            UserId, FirstName, LastName, Email, PhoneNumber,
                            Department, Position, Salary, HireDate, IsActive, CreatedAtUtc
                        )
                        VALUES (
                            @UserId, @FirstName, @LastName, @Email, @PhoneNumber,
                            @Department, @Position, @Salary, @HireDate, 1, SYSUTCDATETIME()
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int newEmployeeId;
                    using (var empCommand = new SqlCommand(insertEmployeeSql, connection, transaction))
                    {
                        empCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = newUserId;
                        empCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = employee.FirstName.Trim();
                        empCommand.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = employee.LastName.Trim();
                        empCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = employee.Email.Trim();
                        empCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = (object)employee.PhoneNumber?.Trim() ?? DBNull.Value;
                        empCommand.Parameters.Add("@Department", SqlDbType.NVarChar, 50).Value = employee.Department.Trim();
                        empCommand.Parameters.Add("@Position", SqlDbType.NVarChar, 50).Value = employee.Position.Trim();
                        empCommand.Parameters.Add("@Salary", SqlDbType.Decimal).Value = employee.Salary;
                        empCommand.Parameters.Add("@HireDate", SqlDbType.Date).Value = employee.HireDate;

                        newEmployeeId = Convert.ToInt32(empCommand.ExecuteScalar());
                    }

                    transaction.Commit();
                    return newEmployeeId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates basic employee details.
        /// </summary>
        public void UpdateEmployee(Employee employee)
        {
            const string sql = @"
                UPDATE dbo.Employees
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    PhoneNumber = @PhoneNumber,
                    Department = @Department,
                    Position = @Position,
                    Salary = @Salary,
                    HireDate = @HireDate,
                    UpdatedAtUtc = SYSUTCDATETIME()
                WHERE EmployeeId = @EmployeeId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employee.EmployeeId;
                command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = employee.FirstName.Trim();
                command.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = employee.LastName.Trim();
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = employee.Email.Trim();
                command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = (object)employee.PhoneNumber?.Trim() ?? DBNull.Value;
                command.Parameters.Add("@Department", SqlDbType.NVarChar, 50).Value = employee.Department.Trim();
                command.Parameters.Add("@Position", SqlDbType.NVarChar, 50).Value = employee.Position.Trim();
                command.Parameters.Add("@Salary", SqlDbType.Decimal).Value = employee.Salary;
                command.Parameters.Add("@HireDate", SqlDbType.Date).Value = employee.HireDate;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Soft activates or deactivates an employee and synchronizes their login account.
        /// </summary>
        public void SetActiveStatus(int employeeId, bool isActive)
        {
            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    const string updateEmpSql = @"
                        UPDATE dbo.Employees
                        SET IsActive = @IsActive,
                            UpdatedAtUtc = SYSUTCDATETIME()
                        WHERE EmployeeId = @EmployeeId;";

                    using (var cmd = new SqlCommand(updateEmpSql, connection, transaction))
                    {
                        cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;
                        cmd.ExecuteNonQuery();
                    }

                    const string updateUserSql = @"
                        UPDATE dbo.Users
                        SET IsActive = @IsActive
                        WHERE UserId = (SELECT UserId FROM dbo.Employees WHERE EmployeeId = @EmployeeId);";

                    using (var cmd = new SqlCommand(updateUserSql, connection, transaction))
                    {
                        cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private static Employee MapEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                Department = reader.GetString(reader.GetOrdinal("Department")),
                Position = reader.GetString(reader.GetOrdinal("Position")),
                Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                UpdatedAtUtc = reader.IsDBNull(reader.GetOrdinal("UpdatedAtUtc")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc")),
                Username = reader.GetString(reader.GetOrdinal("Username"))
            };
        }
    }
}
