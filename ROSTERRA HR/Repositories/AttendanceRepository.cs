using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ROSTERRA_HR.Data;
using ROSTERRA_HR.Models;

namespace ROSTERRA_HR.Repositories
{
    /// <summary>
    /// Repository managing employee attendance, punch-in / punch-out timestamps, and shift compliance.
    /// Uses manual ADO.NET with parameterized queries.
    /// </summary>
    public class AttendanceRepository
    {
        private readonly ShiftRepository _shiftRepository;

        public AttendanceRepository()
        {
            _shiftRepository = new ShiftRepository();
        }

        /// <summary>
        /// Retrieves day-wise monitoring information for all active employees on the given date.
        /// </summary>
        public AttendanceDayMonitorViewModel GetDayMonitor(DateTime date)
        {
            var model = new AttendanceDayMonitorViewModel
            {
                SelectedDate = date.Date
            };

            const string sql = @"
                SELECT e.EmployeeId, e.FirstName, e.LastName, e.Department, e.Position,
                       e.ShiftId, s.ShiftName, s.StartTime AS ShiftStartTime, s.EndTime AS ShiftEndTime,
                       a.AttendanceId, a.PunchInTime, a.PunchOutTime, a.Status, a.TotalHours, a.Remarks
                FROM dbo.Employees e
                LEFT JOIN dbo.Shifts s ON e.ShiftId = s.ShiftId
                LEFT JOIN dbo.Attendances a ON e.EmployeeId = a.EmployeeId AND a.AttendanceDate = @AttendanceDate
                WHERE e.IsActive = 1
                ORDER BY e.Department ASC, e.FirstName ASC;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@AttendanceDate", SqlDbType.Date).Value = date.Date;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new AttendanceMonitorItem
                        {
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            EmployeeName = reader.GetString(reader.GetOrdinal("FirstName")) + " " + reader.GetString(reader.GetOrdinal("LastName")),
                            Department = reader.GetString(reader.GetOrdinal("Department")),
                            Position = reader.GetString(reader.GetOrdinal("Position"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("ShiftId")))
                        {
                            item.ShiftId = reader.GetInt32(reader.GetOrdinal("ShiftId"));
                            item.ShiftName = reader.GetString(reader.GetOrdinal("ShiftName"));
                            item.ShiftStartTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("ShiftStartTime"));
                            item.ShiftEndTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("ShiftEndTime"));
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("AttendanceId")))
                        {
                            item.AttendanceId = reader.GetInt32(reader.GetOrdinal("AttendanceId"));
                            item.PunchInTime = reader.IsDBNull(reader.GetOrdinal("PunchInTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PunchInTime"));
                            item.PunchOutTime = reader.IsDBNull(reader.GetOrdinal("PunchOutTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PunchOutTime"));
                            item.Status = reader.GetString(reader.GetOrdinal("Status"));
                            item.TotalHours = reader.IsDBNull(reader.GetOrdinal("TotalHours")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("TotalHours"));
                            item.Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks"));
                        }
                        else
                        {
                            item.Status = "Not Marked";
                        }

                        model.Items.Add(item);

                        // Accumulate summary stats
                        model.TotalActiveEmployees++;
                        if (string.Equals(item.Status, "Present", StringComparison.OrdinalIgnoreCase))
                        {
                            model.PresentCount++;
                        }
                        else if (string.Equals(item.Status, "Late", StringComparison.OrdinalIgnoreCase))
                        {
                            model.LateCount++;
                        }
                        else if (string.Equals(item.Status, "Absent", StringComparison.OrdinalIgnoreCase))
                        {
                            model.AbsentCount++;
                        }
                        else
                        {
                            model.PendingCount++;
                        }
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Retrieves an attendance record for a specific employee and date.
        /// </summary>
        public Attendance GetAttendance(int employeeId, DateTime date)
        {
            const string sql = @"
                SELECT a.AttendanceId, a.EmployeeId, a.ShiftId, a.AttendanceDate,
                       a.PunchInTime, a.PunchOutTime, a.Status,
                       a.ShiftStartTimeSnapshot, a.ShiftEndTimeSnapshot, a.TotalHours,
                       a.Remarks, a.MarkedByUserId, a.CreatedAtUtc, a.UpdatedAtUtc,
                       (e.FirstName + ' ' + e.LastName) AS EmployeeName, e.Department, e.Position,
                       s.ShiftName
                FROM dbo.Attendances a
                INNER JOIN dbo.Employees e ON a.EmployeeId = e.EmployeeId
                INNER JOIN dbo.Shifts s ON a.ShiftId = s.ShiftId
                WHERE a.EmployeeId = @EmployeeId AND a.AttendanceDate = @AttendanceDate;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                command.Parameters.Add("@AttendanceDate", SqlDbType.Date).Value = date.Date;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapAttendance(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves the complete attendance history for an employee, ordered newest first.
        /// </summary>
        public List<Attendance> GetEmployeeAttendanceHistory(int employeeId)
        {
            var list = new List<Attendance>();

            const string sql = @"
                SELECT a.AttendanceId, a.EmployeeId, a.ShiftId, a.AttendanceDate,
                       a.PunchInTime, a.PunchOutTime, a.Status,
                       a.ShiftStartTimeSnapshot, a.ShiftEndTimeSnapshot, a.TotalHours,
                       a.Remarks, a.MarkedByUserId, a.CreatedAtUtc, a.UpdatedAtUtc,
                       (e.FirstName + ' ' + e.LastName) AS EmployeeName, e.Department, e.Position,
                       s.ShiftName
                FROM dbo.Attendances a
                INNER JOIN dbo.Employees e ON a.EmployeeId = e.EmployeeId
                INNER JOIN dbo.Shifts s ON a.ShiftId = s.ShiftId
                WHERE a.EmployeeId = @EmployeeId
                ORDER BY a.AttendanceDate DESC;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapAttendance(reader));
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Records employee punch-in. Calculates whether employee is on-time or late compared to shift start.
        /// </summary>
        public void PunchIn(int employeeId, DateTime punchInTime, int markedByUserId, string customRemarks = null)
        {
            DateTime date = punchInTime.Date;

            // 1. Get employee shift
            Shift shift = null;
            const string empShiftSql = @"
                SELECT s.ShiftId, s.ShiftName, s.StartTime, s.EndTime
                FROM dbo.Employees e
                INNER JOIN dbo.Shifts s ON e.ShiftId = s.ShiftId
                WHERE e.EmployeeId = @EmployeeId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(empShiftSql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        shift = new Shift
                        {
                            ShiftId = reader.GetInt32(reader.GetOrdinal("ShiftId")),
                            ShiftName = reader.GetString(reader.GetOrdinal("ShiftName")),
                            StartTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("StartTime")),
                            EndTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("EndTime"))
                        };
                    }
                }
            }

            if (shift == null)
            {
                // Fallback to default Morning Shift if not assigned
                shift = _shiftRepository.GetShiftById(1) ?? new Shift
                {
                    ShiftId = 1,
                    ShiftName = "Default Shift",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0)
                };
            }

            // 2. Evaluate arrival time vs scheduled shift start
            TimeSpan arrivalTime = punchInTime.TimeOfDay;
            string status;
            string remarkText;

            if (arrivalTime <= shift.StartTime)
            {
                status = "Present";
                int minutesEarly = (int)(shift.StartTime - arrivalTime).TotalMinutes;
                remarkText = minutesEarly > 0
                    ? $"Present (Arrived {minutesEarly} min(s) early at {punchInTime:hh:mm tt})"
                    : $"Present (Arrived on time at {punchInTime:hh:mm tt})";
            }
            else
            {
                status = "Late";
                int minutesLate = (int)(arrivalTime - shift.StartTime).TotalMinutes;
                remarkText = $"Late by {minutesLate} min(s) (Arrived at {punchInTime:hh:mm tt}, scheduled at {DateTime.Today.Add(shift.StartTime):hh:mm tt})";
            }

            if (!string.IsNullOrWhiteSpace(customRemarks))
            {
                remarkText += $" | {customRemarks.Trim()}";
            }

            // 3. Upsert attendance record for that date
            const string upsertSql = @"
                IF EXISTS (SELECT 1 FROM dbo.Attendances WHERE EmployeeId = @EmployeeId AND AttendanceDate = @AttendanceDate)
                BEGIN
                    UPDATE dbo.Attendances
                    SET PunchInTime = @PunchInTime,
                        Status = @Status,
                        ShiftId = @ShiftId,
                        ShiftStartTimeSnapshot = @ShiftStartTime,
                        ShiftEndTimeSnapshot = @ShiftEndTime,
                        Remarks = @Remarks,
                        MarkedByUserId = @MarkedByUserId,
                        UpdatedAtUtc = SYSUTCDATETIME()
                    WHERE EmployeeId = @EmployeeId AND AttendanceDate = @AttendanceDate;
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.Attendances (
                        EmployeeId, ShiftId, AttendanceDate, PunchInTime,
                        Status, ShiftStartTimeSnapshot, ShiftEndTimeSnapshot,
                        Remarks, MarkedByUserId, CreatedAtUtc
                    )
                    VALUES (
                        @EmployeeId, @ShiftId, @AttendanceDate, @PunchInTime,
                        @Status, @ShiftStartTime, @ShiftEndTime,
                        @Remarks, @MarkedByUserId, SYSUTCDATETIME()
                    );
                END;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(upsertSql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                command.Parameters.Add("@ShiftId", SqlDbType.Int).Value = shift.ShiftId;
                command.Parameters.Add("@AttendanceDate", SqlDbType.Date).Value = date;
                command.Parameters.Add("@PunchInTime", SqlDbType.DateTime2).Value = punchInTime;
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = status;
                command.Parameters.Add("@ShiftStartTime", SqlDbType.Time).Value = shift.StartTime;
                command.Parameters.Add("@ShiftEndTime", SqlDbType.Time).Value = shift.EndTime;
                command.Parameters.Add("@Remarks", SqlDbType.NVarChar, 250).Value = remarkText;
                command.Parameters.Add("@MarkedByUserId", SqlDbType.Int).Value = markedByUserId;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Records employee punch-out. Computes total hours worked and notes early departure or full shift completion.
        /// </summary>
        public void PunchOut(int employeeId, DateTime punchOutTime, int markedByUserId, string customRemarks = null)
        {
            DateTime date = punchOutTime.Date;
            var attendance = GetAttendance(employeeId, date);

            if (attendance == null || !attendance.PunchInTime.HasValue)
            {
                throw new InvalidOperationException("Cannot record Punch-Out without an existing Punch-In for this date.");
            }

            // Compute total worked hours
            TimeSpan duration = punchOutTime - attendance.PunchInTime.Value;
            decimal totalHours = Math.Max(0, Math.Round((decimal)duration.TotalHours, 2));

            // Check if employee met scheduled shift end
            string outNote;
            if (punchOutTime.TimeOfDay >= attendance.ShiftEndTimeSnapshot)
            {
                int overMinutes = (int)(punchOutTime.TimeOfDay - attendance.ShiftEndTimeSnapshot).TotalMinutes;
                outNote = overMinutes > 0
                    ? $"Punched out at {punchOutTime:hh:mm tt} ({overMinutes} mins post-shift). Total: {totalHours} hrs."
                    : $"Completed full shift. Punched out at {punchOutTime:hh:mm tt}. Total: {totalHours} hrs.";
            }
            else
            {
                int earlyMins = (int)(attendance.ShiftEndTimeSnapshot - punchOutTime.TimeOfDay).TotalMinutes;
                outNote = $"Early departure ({earlyMins} mins before shift end). Punched out at {punchOutTime:hh:mm tt}. Total: {totalHours} hrs.";
            }

            string combinedRemarks = string.IsNullOrWhiteSpace(attendance.Remarks)
                ? outNote
                : $"{attendance.Remarks} | {outNote}";

            if (!string.IsNullOrWhiteSpace(customRemarks))
            {
                combinedRemarks += $" | {customRemarks.Trim()}";
            }

            if (combinedRemarks.Length > 250)
            {
                combinedRemarks = combinedRemarks.Substring(0, 247) + "...";
            }

            const string sql = @"
                UPDATE dbo.Attendances
                SET PunchOutTime = @PunchOutTime,
                    TotalHours = @TotalHours,
                    Remarks = @Remarks,
                    MarkedByUserId = @MarkedByUserId,
                    UpdatedAtUtc = SYSUTCDATETIME()
                WHERE AttendanceId = @AttendanceId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@AttendanceId", SqlDbType.Int).Value = attendance.AttendanceId;
                command.Parameters.Add("@PunchOutTime", SqlDbType.DateTime2).Value = punchOutTime;
                command.Parameters.Add("@TotalHours", SqlDbType.Decimal).Value = totalHours;
                command.Parameters.Add("@Remarks", SqlDbType.NVarChar, 250).Value = combinedRemarks;
                command.Parameters.Add("@MarkedByUserId", SqlDbType.Int).Value = markedByUserId;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Marks an employee as absent for the given date.
        /// </summary>
        public void MarkAbsent(int employeeId, DateTime date, int markedByUserId, string remarks = null)
        {
            // 1. Get employee shift
            Shift shift = null;
            const string empShiftSql = @"
                SELECT s.ShiftId, s.ShiftName, s.StartTime, s.EndTime
                FROM dbo.Employees e
                INNER JOIN dbo.Shifts s ON e.ShiftId = s.ShiftId
                WHERE e.EmployeeId = @EmployeeId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(empShiftSql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        shift = new Shift
                        {
                            ShiftId = reader.GetInt32(reader.GetOrdinal("ShiftId")),
                            ShiftName = reader.GetString(reader.GetOrdinal("ShiftName")),
                            StartTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("StartTime")),
                            EndTime = (TimeSpan)reader.GetValue(reader.GetOrdinal("EndTime"))
                        };
                    }
                }
            }

            if (shift == null)
            {
                shift = _shiftRepository.GetShiftById(1) ?? new Shift
                {
                    ShiftId = 1,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0)
                };
            }

            string remarkText = string.IsNullOrWhiteSpace(remarks) ? "Marked absent by HR administrator." : remarks.Trim();

            const string upsertSql = @"
                IF EXISTS (SELECT 1 FROM dbo.Attendances WHERE EmployeeId = @EmployeeId AND AttendanceDate = @AttendanceDate)
                BEGIN
                    UPDATE dbo.Attendances
                    SET Status = 'Absent',
                        PunchInTime = NULL,
                        PunchOutTime = NULL,
                        TotalHours = 0.00,
                        Remarks = @Remarks,
                        MarkedByUserId = @MarkedByUserId,
                        UpdatedAtUtc = SYSUTCDATETIME()
                    WHERE EmployeeId = @EmployeeId AND AttendanceDate = @AttendanceDate;
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.Attendances (
                        EmployeeId, ShiftId, AttendanceDate, PunchInTime, PunchOutTime,
                        Status, ShiftStartTimeSnapshot, ShiftEndTimeSnapshot, TotalHours,
                        Remarks, MarkedByUserId, CreatedAtUtc
                    )
                    VALUES (
                        @EmployeeId, @ShiftId, @AttendanceDate, NULL, NULL,
                        'Absent', @ShiftStartTime, @ShiftEndTime, 0.00,
                        @Remarks, @MarkedByUserId, SYSUTCDATETIME()
                    );
                END;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(upsertSql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                command.Parameters.Add("@ShiftId", SqlDbType.Int).Value = shift.ShiftId;
                command.Parameters.Add("@AttendanceDate", SqlDbType.Date).Value = date.Date;
                command.Parameters.Add("@ShiftStartTime", SqlDbType.Time).Value = shift.StartTime;
                command.Parameters.Add("@ShiftEndTime", SqlDbType.Time).Value = shift.EndTime;
                command.Parameters.Add("@Remarks", SqlDbType.NVarChar, 250).Value = remarkText;
                command.Parameters.Add("@MarkedByUserId", SqlDbType.Int).Value = markedByUserId;

                command.ExecuteNonQuery();
            }
        }

        private static Attendance MapAttendance(SqlDataReader reader)
        {
            return new Attendance
            {
                AttendanceId = reader.GetInt32(reader.GetOrdinal("AttendanceId")),
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                ShiftId = reader.GetInt32(reader.GetOrdinal("ShiftId")),
                AttendanceDate = reader.GetDateTime(reader.GetOrdinal("AttendanceDate")),
                PunchInTime = reader.IsDBNull(reader.GetOrdinal("PunchInTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PunchInTime")),
                PunchOutTime = reader.IsDBNull(reader.GetOrdinal("PunchOutTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PunchOutTime")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                ShiftStartTimeSnapshot = (TimeSpan)reader.GetValue(reader.GetOrdinal("ShiftStartTimeSnapshot")),
                ShiftEndTimeSnapshot = (TimeSpan)reader.GetValue(reader.GetOrdinal("ShiftEndTimeSnapshot")),
                TotalHours = reader.IsDBNull(reader.GetOrdinal("TotalHours")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("TotalHours")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                MarkedByUserId = reader.GetInt32(reader.GetOrdinal("MarkedByUserId")),
                CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                UpdatedAtUtc = reader.IsDBNull(reader.GetOrdinal("UpdatedAtUtc")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc")),
                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                Department = reader.GetString(reader.GetOrdinal("Department")),
                Position = reader.GetString(reader.GetOrdinal("Position")),
                ShiftName = reader.GetString(reader.GetOrdinal("ShiftName"))
            };
        }
    }
}
