using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ROSTERRA_HR.Data;
using ROSTERRA_HR.Models;

namespace ROSTERRA_HR.Repositories
{
    /// <summary>
    /// ADO.NET repository for Leave Management.
    /// Handles leave requests, Sunday exclusions, overlap checks, and HR approval/rejection.
    /// </summary>
    public class LeaveRepository
    {
        /// <summary>
        /// Calculates working days between two dates, strictly excluding Sundays.
        /// </summary>
        public static int CalculateWorkingDaysExcludingSundays(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate) return 0;

            int count = 0;
            for (DateTime cur = startDate.Date; cur <= endDate.Date; cur = cur.AddDays(1))
            {
                if (cur.DayOfWeek != DayOfWeek.Sunday)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Checks if an employee has any approved or pending leave that overlaps with the given date range.
        /// </summary>
        public bool HasOverlappingLeave(int employeeId, DateTime startDate, DateTime endDate, int? excludeLeaveId = null)
        {
            string sql = @"
                SELECT COUNT(1)
                FROM dbo.Leaves
                WHERE EmployeeId = @EmployeeId
                  AND Status IN ('Pending', 'Approved')
                  AND StartDate <= @EndDate
                  AND EndDate >= @StartDate";

            if (excludeLeaveId.HasValue)
            {
                sql += " AND LeaveId <> @ExcludeLeaveId";
            }

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                command.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate.Date;
                command.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate.Date;

                if (excludeLeaveId.HasValue)
                {
                    command.Parameters.Add("@ExcludeLeaveId", SqlDbType.Int).Value = excludeLeaveId.Value;
                }

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        /// <summary>
        /// Retrieves list of all approved dates (in 'yyyy-MM-dd' format) for an employee to block them in calendar.
        /// </summary>
        public List<string> GetBlockedDatesForEmployee(int employeeId)
        {
            var dates = new List<string>();
            const string sql = @"
                SELECT StartDate, EndDate
                FROM dbo.Leaves
                WHERE EmployeeId = @EmployeeId
                  AND Status IN ('Approved', 'Pending')
                  AND EndDate >= CAST(GETDATE() AS DATE);";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime start = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                        DateTime end = reader.GetDateTime(reader.GetOrdinal("EndDate"));

                        for (DateTime d = start; d <= end; d = d.AddDays(1))
                        {
                            dates.Add(d.ToString("yyyy-MM-dd"));
                        }
                    }
                }
            }

            return dates;
        }

        /// <summary>
        /// Submits a new leave request for an employee.
        /// </summary>
        public int SubmitLeaveRequest(int employeeId, string leaveType, DateTime startDate, DateTime endDate, string reason)
        {
            int totalDays = CalculateWorkingDaysExcludingSundays(startDate, endDate);
            if (totalDays <= 0)
            {
                throw new InvalidOperationException("Leave request contains no working days (e.g. only Sunday was selected).");
            }

            if (HasOverlappingLeave(employeeId, startDate, endDate))
            {
                throw new InvalidOperationException("You already have an active leave request covering one or more dates in this range.");
            }

            const string sql = @"
                INSERT INTO dbo.Leaves (
                    EmployeeId, LeaveType, StartDate, EndDate, TotalDays,
                    Reason, Status, CreatedAtUtc
                )
                VALUES (
                    @EmployeeId, @LeaveType, @StartDate, @EndDate, @TotalDays,
                    @Reason, 'Pending', SYSUTCDATETIME()
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                command.Parameters.Add("@LeaveType", SqlDbType.NVarChar, 50).Value = leaveType;
                command.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate.Date;
                command.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate.Date;
                command.Parameters.Add("@TotalDays", SqlDbType.Int).Value = totalDays;
                command.Parameters.Add("@Reason", SqlDbType.NVarChar, 500).Value = reason.Trim();

                return (int)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Retrieves all leave requests of a specific employee.
        /// </summary>
        public List<Leave> GetLeavesByEmployee(int employeeId)
        {
            var list = new List<Leave>();
            const string sql = @"
                SELECT l.LeaveId, l.EmployeeId, l.LeaveType, l.StartDate, l.EndDate, l.TotalDays,
                       l.Reason, l.Status, l.AdminRemarks, l.DecidedByUserId, l.DecidedAtUtc, l.CreatedAtUtc,
                       e.FirstName + ' ' + e.LastName AS EmployeeName, e.Department, e.Position,
                       u.Username AS DecidedByUsername
                FROM dbo.Leaves l
                INNER JOIN dbo.Employees e ON l.EmployeeId = e.EmployeeId
                LEFT JOIN dbo.Users u ON l.DecidedByUserId = u.UserId
                WHERE l.EmployeeId = @EmployeeId
                ORDER BY l.CreatedAtUtc DESC;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapLeave(reader));
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all leave requests across the company for HR Admin view with optional status filtering.
        /// </summary>
        public List<Leave> GetAllLeaves(string statusFilter = "All", string searchTerm = null)
        {
            var list = new List<Leave>();
            var sql = @"
                SELECT l.LeaveId, l.EmployeeId, l.LeaveType, l.StartDate, l.EndDate, l.TotalDays,
                       l.Reason, l.Status, l.AdminRemarks, l.DecidedByUserId, l.DecidedAtUtc, l.CreatedAtUtc,
                       e.FirstName + ' ' + e.LastName AS EmployeeName, e.Department, e.Position,
                       u.Username AS DecidedByUsername
                FROM dbo.Leaves l
                INNER JOIN dbo.Employees e ON l.EmployeeId = e.EmployeeId
                LEFT JOIN dbo.Users u ON l.DecidedByUserId = u.UserId
                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(statusFilter) && !string.Equals(statusFilter, "All", StringComparison.OrdinalIgnoreCase))
            {
                sql += " AND l.Status = @Status";
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                sql += @" AND (e.FirstName LIKE @Search 
                             OR e.LastName LIKE @Search 
                             OR e.Department LIKE @Search 
                             OR l.Reason LIKE @Search)";
            }

            sql += " ORDER BY CASE WHEN l.Status = 'Pending' THEN 0 ELSE 1 END, l.CreatedAtUtc DESC;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                if (!string.IsNullOrWhiteSpace(statusFilter) && !string.Equals(statusFilter, "All", StringComparison.OrdinalIgnoreCase))
                {
                    command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = statusFilter;
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    command.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value = "%" + searchTerm.Trim() + "%";
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapLeave(reader));
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Approves or rejects a leave request with HR Admin remarks.
        /// </summary>
        public void UpdateLeaveStatus(int leaveId, string newStatus, int hrUserId, string remarks)
        {
            if (newStatus != "Approved" && newStatus != "Rejected")
            {
                throw new ArgumentException("Status must be either 'Approved' or 'Rejected'.");
            }

            const string sql = @"
                UPDATE dbo.Leaves
                SET Status = @Status,
                    AdminRemarks = @AdminRemarks,
                    DecidedByUserId = @DecidedByUserId,
                    DecidedAtUtc = SYSUTCDATETIME(),
                    UpdatedAtUtc = SYSUTCDATETIME()
                WHERE LeaveId = @LeaveId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@LeaveId", SqlDbType.Int).Value = leaveId;
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = newStatus;
                command.Parameters.Add("@AdminRemarks", SqlDbType.NVarChar, 250).Value = (object)remarks ?? DBNull.Value;
                command.Parameters.Add("@DecidedByUserId", SqlDbType.Int).Value = hrUserId;

                command.ExecuteNonQuery();
            }
        }

        private Leave MapLeave(SqlDataReader reader)
        {
            return new Leave
            {
                LeaveId = reader.GetInt32(reader.GetOrdinal("LeaveId")),
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                Department = reader.GetString(reader.GetOrdinal("Department")),
                Position = reader.GetString(reader.GetOrdinal("Position")),
                LeaveType = reader.GetString(reader.GetOrdinal("LeaveType")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                TotalDays = reader.GetInt32(reader.GetOrdinal("TotalDays")),
                Reason = reader.GetString(reader.GetOrdinal("Reason")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AdminRemarks = reader.IsDBNull(reader.GetOrdinal("AdminRemarks")) ? null : reader.GetString(reader.GetOrdinal("AdminRemarks")),
                DecidedByUserId = reader.IsDBNull(reader.GetOrdinal("DecidedByUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("DecidedByUserId")),
                DecidedByUsername = reader.IsDBNull(reader.GetOrdinal("DecidedByUsername")) ? null : reader.GetString(reader.GetOrdinal("DecidedByUsername")),
                DecidedAtUtc = reader.IsDBNull(reader.GetOrdinal("DecidedAtUtc")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DecidedAtUtc")),
                CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
            };
        }
    }
}
