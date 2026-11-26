using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ROSTERRA_HR.Data;
using ROSTERRA_HR.Models;

namespace ROSTERRA_HR.Repositories
{
    /// <summary>
    /// Repository managing salary structures, monthly payroll generations, LOP deductions, and payslips.
    /// Uses manual ADO.NET with parameterized queries.
    /// </summary>
    public class PayrollRepository
    {
        /// <summary>
        /// Calculates total working days in a month (excluding Sundays).
        /// </summary>
        public static int GetWorkingDaysInMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            int count = 0;
            for (int d = 1; d <= days; d++)
            {
                var dt = new DateTime(year, month, d);
                if (dt.DayOfWeek != DayOfWeek.Sunday)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Retrieves the salary structure for an employee or returns default values if not configured yet.
        /// </summary>
        public SalaryStructure GetSalaryStructure(int employeeId)
        {
            const string sql = @"
                SELECT s.SalaryStructureId, s.EmployeeId, s.BasicSalary, s.Allowances, s.Deductions,
                       s.CreatedAtUtc, s.UpdatedAtUtc,
                       e.FirstName + ' ' + e.LastName AS EmployeeName, e.Department, e.Position
                FROM dbo.Employees e
                LEFT JOIN dbo.SalaryStructures s ON e.EmployeeId = s.EmployeeId
                WHERE e.EmployeeId = @EmployeeId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SalaryStructure
                        {
                            SalaryStructureId = reader.IsDBNull(reader.GetOrdinal("SalaryStructureId")) ? 0 : reader.GetInt32(reader.GetOrdinal("SalaryStructureId")),
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                            Department = reader.GetString(reader.GetOrdinal("Department")),
                            Position = reader.GetString(reader.GetOrdinal("Position")),
                            BasicSalary = reader.IsDBNull(reader.GetOrdinal("BasicSalary")) ? 30000.00m : reader.GetDecimal(reader.GetOrdinal("BasicSalary")),
                            Allowances = reader.IsDBNull(reader.GetOrdinal("Allowances")) ? 5000.00m : reader.GetDecimal(reader.GetOrdinal("Allowances")),
                            Deductions = reader.IsDBNull(reader.GetOrdinal("Deductions")) ? 1000.00m : reader.GetDecimal(reader.GetOrdinal("Deductions")),
                            CreatedAtUtc = reader.IsDBNull(reader.GetOrdinal("CreatedAtUtc")) ? DateTime.UtcNow : reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                            UpdatedAtUtc = reader.IsDBNull(reader.GetOrdinal("UpdatedAtUtc")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc"))
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves all employee salary structures for HR management.
        /// </summary>
        public List<SalaryStructure> GetAllSalaryStructures()
        {
            var list = new List<SalaryStructure>();
            const string sql = @"
                SELECT e.EmployeeId, e.FirstName + ' ' + e.LastName AS EmployeeName, e.Department, e.Position,
                       ISNULL(s.SalaryStructureId, 0) AS SalaryStructureId,
                       ISNULL(s.BasicSalary, 30000.00) AS BasicSalary,
                       ISNULL(s.Allowances, 5000.00) AS Allowances,
                       ISNULL(s.Deductions, 1000.00) AS Deductions,
                       s.CreatedAtUtc, s.UpdatedAtUtc
                FROM dbo.Employees e
                LEFT JOIN dbo.SalaryStructures s ON e.EmployeeId = s.EmployeeId
                WHERE e.IsActive = 1
                ORDER BY e.Department ASC, e.FirstName ASC;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new SalaryStructure
                    {
                        SalaryStructureId = reader.GetInt32(reader.GetOrdinal("SalaryStructureId")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                        Department = reader.GetString(reader.GetOrdinal("Department")),
                        Position = reader.GetString(reader.GetOrdinal("Position")),
                        BasicSalary = reader.GetDecimal(reader.GetOrdinal("BasicSalary")),
                        Allowances = reader.GetDecimal(reader.GetOrdinal("Allowances")),
                        Deductions = reader.GetDecimal(reader.GetOrdinal("Deductions")),
                        CreatedAtUtc = reader.IsDBNull(reader.GetOrdinal("CreatedAtUtc")) ? DateTime.UtcNow : reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                        UpdatedAtUtc = reader.IsDBNull(reader.GetOrdinal("UpdatedAtUtc")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc"))
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// Upserts salary structure for an employee.
        /// </summary>
        public void SaveSalaryStructure(int employeeId, decimal basicSalary, decimal allowances, decimal deductions)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM dbo.SalaryStructures WHERE EmployeeId = @EmployeeId)
                BEGIN
                    UPDATE dbo.SalaryStructures
                    SET BasicSalary = @BasicSalary,
                        Allowances = @Allowances,
                        Deductions = @Deductions,
                        UpdatedAtUtc = SYSUTCDATETIME()
                    WHERE EmployeeId = @EmployeeId;
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.SalaryStructures (EmployeeId, BasicSalary, Allowances, Deductions, CreatedAtUtc)
                    VALUES (@EmployeeId, @BasicSalary, @Allowances, @Deductions, SYSUTCDATETIME());
                END;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                command.Parameters.Add("@BasicSalary", SqlDbType.Decimal).Value = basicSalary;
                command.Parameters.Add("@Allowances", SqlDbType.Decimal).Value = allowances;
                command.Parameters.Add("@Deductions", SqlDbType.Decimal).Value = deductions;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Generates or recalculates monthly payroll for all active employees for the given month and year.
        /// Automatically checks Attendance records and Approved Leaves to determine Absent/Loss of Pay days.
        /// </summary>
        public void GenerateMonthlyPayroll(int month, int year, int hrUserId)
        {
            int totalWorkingDays = GetWorkingDaysInMonth(year, month);
            DateTime startOfMonth = new DateTime(year, month, 1);
            DateTime endOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            // Fetch active employees with salary structure
            var structures = GetAllSalaryStructures();

            using (var connection = ConnectionFactory.CreateOpenConnection())
            {
                foreach (var emp in structures)
                {
                    // 1. Count days marked Absent in dbo.Attendances
                    int absentAttendanceCount = 0;
                    const string attSql = @"
                        SELECT COUNT(1)
                        FROM dbo.Attendances
                        WHERE EmployeeId = @EmployeeId
                          AND AttendanceDate BETWEEN @StartOfMonth AND @EndOfMonth
                          AND Status = 'Absent';";

                    using (var attCmd = new SqlCommand(attSql, connection))
                    {
                        attCmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = emp.EmployeeId;
                        attCmd.Parameters.Add("@StartOfMonth", SqlDbType.Date).Value = startOfMonth;
                        attCmd.Parameters.Add("@EndOfMonth", SqlDbType.Date).Value = endOfMonth;
                        absentAttendanceCount = Convert.ToInt32(attCmd.ExecuteScalar());
                    }

                    // 2. Count days marked as Unpaid Leave in dbo.Leaves
                    int unpaidLeaveDays = 0;
                    const string leaveSql = @"
                        SELECT ISNULL(SUM(TotalDays), 0)
                        FROM dbo.Leaves
                        WHERE EmployeeId = @EmployeeId
                          AND LeaveType = 'Unpaid Leave'
                          AND Status = 'Approved'
                          AND StartDate <= @EndOfMonth
                          AND EndDate >= @StartOfMonth;";

                    using (var leaveCmd = new SqlCommand(leaveSql, connection))
                    {
                        leaveCmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = emp.EmployeeId;
                        leaveCmd.Parameters.Add("@StartOfMonth", SqlDbType.Date).Value = startOfMonth;
                        leaveCmd.Parameters.Add("@EndOfMonth", SqlDbType.Date).Value = endOfMonth;
                        unpaidLeaveDays = Convert.ToInt32(leaveCmd.ExecuteScalar());
                    }

                    int totalAbsentDays = Math.Min(totalWorkingDays, absentAttendanceCount + unpaidLeaveDays);
                    int presentDays = Math.Max(0, totalWorkingDays - totalAbsentDays);

                    decimal grossSalary = emp.BasicSalary + emp.Allowances;
                    decimal perDaySalary = totalWorkingDays > 0 ? (grossSalary / totalWorkingDays) : 0m;
                    decimal lossOfPay = Math.Round(totalAbsentDays * perDaySalary, 2);
                    decimal netSalary = Math.Max(0m, Math.Round(grossSalary - (emp.Deductions + lossOfPay), 2));

                    // 3. Upsert into dbo.Payrolls
                    const string upsertSql = @"
                        IF EXISTS (SELECT 1 FROM dbo.Payrolls WHERE EmployeeId = @EmployeeId AND Month = @Month AND Year = @Year)
                        BEGIN
                            UPDATE dbo.Payrolls
                            SET TotalWorkingDays = @TotalWorkingDays,
                                PresentDays = @PresentDays,
                                AbsentDays = @AbsentDays,
                                BasicSalary = @BasicSalary,
                                Allowances = @Allowances,
                                GrossSalary = @GrossSalary,
                                StandardDeductions = @StandardDeductions,
                                LossOfPay = @LossOfPay,
                                NetSalary = @NetSalary,
                                GeneratedByUserId = @GeneratedByUserId,
                                UpdatedAtUtc = SYSUTCDATETIME()
                            WHERE EmployeeId = @EmployeeId AND Month = @Month AND Year = @Year;
                        END
                        ELSE
                        BEGIN
                            INSERT INTO dbo.Payrolls (
                                EmployeeId, Month, Year, TotalWorkingDays, PresentDays, AbsentDays,
                                BasicSalary, Allowances, GrossSalary, StandardDeductions, LossOfPay,
                                NetSalary, Status, GeneratedByUserId, CreatedAtUtc
                            )
                            VALUES (
                                @EmployeeId, @Month, @Year, @TotalWorkingDays, @PresentDays, @AbsentDays,
                                @BasicSalary, @Allowances, @GrossSalary, @StandardDeductions, @LossOfPay,
                                @NetSalary, 'Generated', @GeneratedByUserId, SYSUTCDATETIME()
                            );
                        END;";

                    using (var cmd = new SqlCommand(upsertSql, connection))
                    {
                        cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = emp.EmployeeId;
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                        cmd.Parameters.Add("@TotalWorkingDays", SqlDbType.Int).Value = totalWorkingDays;
                        cmd.Parameters.Add("@PresentDays", SqlDbType.Int).Value = presentDays;
                        cmd.Parameters.Add("@AbsentDays", SqlDbType.Int).Value = totalAbsentDays;
                        cmd.Parameters.Add("@BasicSalary", SqlDbType.Decimal).Value = emp.BasicSalary;
                        cmd.Parameters.Add("@Allowances", SqlDbType.Decimal).Value = emp.Allowances;
                        cmd.Parameters.Add("@GrossSalary", SqlDbType.Decimal).Value = grossSalary;
                        cmd.Parameters.Add("@StandardDeductions", SqlDbType.Decimal).Value = emp.Deductions;
                        cmd.Parameters.Add("@LossOfPay", SqlDbType.Decimal).Value = lossOfPay;
                        cmd.Parameters.Add("@NetSalary", SqlDbType.Decimal).Value = netSalary;
                        cmd.Parameters.Add("@GeneratedByUserId", SqlDbType.Int).Value = hrUserId;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves payroll records for a specific month and year.
        /// </summary>
        public List<Payroll> GetMonthlyPayrolls(int month, int year)
        {
            var list = new List<Payroll>();
            const string sql = @"
                SELECT p.PayrollId, p.EmployeeId, p.Month, p.Year, p.TotalWorkingDays, p.PresentDays, p.AbsentDays,
                       p.BasicSalary, p.Allowances, p.GrossSalary, p.StandardDeductions, p.LossOfPay, p.NetSalary,
                       p.Status, p.PaymentDate, p.GeneratedByUserId, p.CreatedAtUtc,
                       e.FirstName + ' ' + e.LastName AS EmployeeName, e.Department, e.Position, e.Email,
                       u.Username AS GeneratedByUsername
                FROM dbo.Payrolls p
                INNER JOIN dbo.Employees e ON p.EmployeeId = e.EmployeeId
                LEFT JOIN dbo.Users u ON p.GeneratedByUserId = u.UserId
                WHERE p.Month = @Month AND p.Year = @Year
                ORDER BY e.Department ASC, e.FirstName ASC;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapPayroll(reader));
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves an individual payroll record by PayrollId (for payslip generation).
        /// </summary>
        public Payroll GetPayrollById(int payrollId)
        {
            const string sql = @"
                SELECT p.PayrollId, p.EmployeeId, p.Month, p.Year, p.TotalWorkingDays, p.PresentDays, p.AbsentDays,
                       p.BasicSalary, p.Allowances, p.GrossSalary, p.StandardDeductions, p.LossOfPay, p.NetSalary,
                       p.Status, p.PaymentDate, p.GeneratedByUserId, p.CreatedAtUtc,
                       e.FirstName + ' ' + e.LastName AS EmployeeName, e.Department, e.Position, e.Email,
                       u.Username AS GeneratedByUsername
                FROM dbo.Payrolls p
                INNER JOIN dbo.Employees e ON p.EmployeeId = e.EmployeeId
                LEFT JOIN dbo.Users u ON p.GeneratedByUserId = u.UserId
                WHERE p.PayrollId = @PayrollId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@PayrollId", SqlDbType.Int).Value = payrollId;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapPayroll(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves an employee's chronological payroll history.
        /// </summary>
        public List<Payroll> GetPayrollHistoryForEmployee(int employeeId)
        {
            var list = new List<Payroll>();
            const string sql = @"
                SELECT p.PayrollId, p.EmployeeId, p.Month, p.Year, p.TotalWorkingDays, p.PresentDays, p.AbsentDays,
                       p.BasicSalary, p.Allowances, p.GrossSalary, p.StandardDeductions, p.LossOfPay, p.NetSalary,
                       p.Status, p.PaymentDate, p.GeneratedByUserId, p.CreatedAtUtc,
                       e.FirstName + ' ' + e.LastName AS EmployeeName, e.Department, e.Position, e.Email,
                       u.Username AS GeneratedByUsername
                FROM dbo.Payrolls p
                INNER JOIN dbo.Employees e ON p.EmployeeId = e.EmployeeId
                LEFT JOIN dbo.Users u ON p.GeneratedByUserId = u.UserId
                WHERE p.EmployeeId = @EmployeeId
                ORDER BY p.Year DESC, p.Month DESC;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapPayroll(reader));
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Updates the status of a payroll (e.g. marking as 'Paid').
        /// </summary>
        public void MarkAsPaid(int payrollId)
        {
            const string sql = @"
                UPDATE dbo.Payrolls
                SET Status = 'Paid',
                    PaymentDate = CAST(GETDATE() AS DATE),
                    UpdatedAtUtc = SYSUTCDATETIME()
                WHERE PayrollId = @PayrollId;";

            using (var connection = ConnectionFactory.CreateOpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@PayrollId", SqlDbType.Int).Value = payrollId;
                command.ExecuteNonQuery();
            }
        }

        private Payroll MapPayroll(SqlDataReader reader)
        {
            return new Payroll
            {
                PayrollId = reader.GetInt32(reader.GetOrdinal("PayrollId")),
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                Department = reader.GetString(reader.GetOrdinal("Department")),
                Position = reader.GetString(reader.GetOrdinal("Position")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Month = reader.GetInt32(reader.GetOrdinal("Month")),
                Year = reader.GetInt32(reader.GetOrdinal("Year")),
                TotalWorkingDays = reader.GetInt32(reader.GetOrdinal("TotalWorkingDays")),
                PresentDays = reader.GetInt32(reader.GetOrdinal("PresentDays")),
                AbsentDays = reader.GetInt32(reader.GetOrdinal("AbsentDays")),
                BasicSalary = reader.GetDecimal(reader.GetOrdinal("BasicSalary")),
                Allowances = reader.GetDecimal(reader.GetOrdinal("Allowances")),
                GrossSalary = reader.GetDecimal(reader.GetOrdinal("GrossSalary")),
                StandardDeductions = reader.GetDecimal(reader.GetOrdinal("StandardDeductions")),
                LossOfPay = reader.GetDecimal(reader.GetOrdinal("LossOfPay")),
                NetSalary = reader.GetDecimal(reader.GetOrdinal("NetSalary")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                GeneratedByUserId = reader.IsDBNull(reader.GetOrdinal("GeneratedByUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("GeneratedByUserId")),
                GeneratedByUsername = reader.IsDBNull(reader.GetOrdinal("GeneratedByUsername")) ? null : reader.GetString(reader.GetOrdinal("GeneratedByUsername")),
                CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
            };
        }
    }
}
