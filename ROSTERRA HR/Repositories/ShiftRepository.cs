using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ROSTERRA_HR.Data;
using ROSTERRA_HR.Models;

namespace ROSTERRA_HR.Repositories
{
    /// <summary>
    /// Repository managing work shifts using manual ADO.NET and parameterized queries.
    /// </summary>
    public class ShiftRepository
    {
        public List<Shift> GetAllShifts(bool activeOnly = false)
        {
            var shifts = new List<Shift>();
            string sql = @"
                SELECT ShiftId, ShiftName, StartTime, EndTime, Description, IsActive, CreatedAtUtc
                FROM dbo.Shifts";

            if (activeOnly)
            {
                sql += " WHERE IsActive = 1";
            }

            sql += " ORDER BY StartTime ASC;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    shifts.Add(new Shift
                    {
                        ShiftId = reader.GetInt32(reader.GetOrdinal("ShiftId")),
                        ShiftName = reader.GetString(reader.GetOrdinal("ShiftName")),
                        StartTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("StartTime")),
                        EndTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("EndTime")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
                    });
                }
            }

            return shifts;
        }

        public Shift GetShiftById(int shiftId)
        {
            const string sql = @"
                SELECT ShiftId, ShiftName, StartTime, EndTime, Description, IsActive, CreatedAtUtc
                FROM dbo.Shifts
                WHERE ShiftId = @ShiftId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ShiftId", SqlDbType.Int).Value = shiftId;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Shift
                        {
                            ShiftId = reader.GetInt32(reader.GetOrdinal("ShiftId")),
                            ShiftName = reader.GetString(reader.GetOrdinal("ShiftName")),
                            StartTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("StartTime")),
                            EndTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("EndTime")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
                        };
                    }
                }
            }

            return null;
        }

        public bool IsShiftNameTaken(string shiftName, int? excludeShiftId = null)
        {
            string sql = "SELECT COUNT(1) FROM dbo.Shifts WHERE ShiftName = @ShiftName";
            if (excludeShiftId.HasValue)
            {
                sql += " AND ShiftId <> @ExcludeShiftId";
            }

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ShiftName", SqlDbType.NVarChar, 50).Value = shiftName.Trim();
                if (excludeShiftId.HasValue)
                {
                    command.Parameters.Add("@ExcludeShiftId", SqlDbType.Int).Value = excludeShiftId.Value;
                }

                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public int CreateShift(Shift shift)
        {
            const string sql = @"
                INSERT INTO dbo.Shifts (ShiftName, StartTime, EndTime, Description, IsActive, CreatedAtUtc)
                VALUES (@ShiftName, @StartTime, @EndTime, @Description, @IsActive, SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ShiftName", SqlDbType.NVarChar, 50).Value = shift.ShiftName.Trim();
                command.Parameters.Add("@StartTime", SqlDbType.Time).Value = shift.StartTime;
                command.Parameters.Add("@EndTime", SqlDbType.Time).Value = shift.EndTime;
                command.Parameters.Add("@Description", SqlDbType.NVarChar, 250).Value = (object)shift.Description?.Trim() ?? DBNull.Value;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = shift.IsActive;

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void UpdateShift(Shift shift)
        {
            const string sql = @"
                UPDATE dbo.Shifts
                SET ShiftName = @ShiftName,
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    Description = @Description,
                    IsActive = @IsActive
                WHERE ShiftId = @ShiftId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ShiftId", SqlDbType.Int).Value = shift.ShiftId;
                command.Parameters.Add("@ShiftName", SqlDbType.NVarChar, 50).Value = shift.ShiftName.Trim();
                command.Parameters.Add("@StartTime", SqlDbType.Time).Value = shift.StartTime;
                command.Parameters.Add("@EndTime", SqlDbType.Time).Value = shift.EndTime;
                command.Parameters.Add("@Description", SqlDbType.NVarChar, 250).Value = (object)shift.Description?.Trim() ?? DBNull.Value;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = shift.IsActive;

                command.ExecuteNonQuery();
            }
        }

        public void AssignShiftToEmployee(int employeeId, int shiftId)
        {
            const string sql = @"
                UPDATE dbo.Employees
                SET ShiftId = @ShiftId,
                    UpdatedAtUtc = SYSUTCDATETIME()
                WHERE EmployeeId = @EmployeeId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                command.Parameters.Add("@ShiftId", SqlDbType.Int).Value = shiftId;
                command.ExecuteNonQuery();
            }
        }
    }
}
