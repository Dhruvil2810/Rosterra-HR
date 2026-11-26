# Phase 4 — Leave Management, Sunday Holiday Policy & Overnight Shift Logic

## 1. Overview
This guide documents the implementation of:
1. **Leave Management System**: Employee time-off application, date-range selection, conflict/overlap prevention, and HR approve/reject review workflow.
2. **Automatic Sunday Holiday Exclusion**: Sunday is an official weekly off for all employees. Applying for leave across dates (e.g. Friday to Tuesday) automatically excludes Sundays (counts 4 working days instead of 5).
3. **Calendar Blocking**: Approved and pending leave dates are passed to the employee portal calendar to prevent duplicate submissions on the same dates.
4. **Overnight Shift Punch Logic**: For shifts crossing midnight (e.g. 10:00 PM to 06:00 AM), punch-in is recorded on the shift start date, and punch-out on the following morning seamlessly calculates total duration and shift compliance.
5. **Employee Attendance Log Enhancement**: Complete date-wise historical audit table of daily punch-in/out records.

---

## 2. Database Design (`dbo.Leaves`)

```sql
CREATE TABLE dbo.Leaves (
    LeaveId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    EmployeeId INT NOT NULL FOREIGN KEY REFERENCES dbo.Employees(EmployeeId) ON DELETE CASCADE,
    LeaveType NVARCHAR(50) NOT NULL DEFAULT 'Casual Leave',
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    TotalDays INT NOT NULL, -- Excludes Sundays
    Reason NVARCHAR(500) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- 'Pending', 'Approved', 'Rejected'
    AdminRemarks NVARCHAR(250) NULL,
    DecidedByUserId INT NULL FOREIGN KEY REFERENCES dbo.Users(UserId),
    DecidedAtUtc DATETIME2(7) NULL,
    CreatedAtUtc DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2(7) NULL
);
```

---

## 3. Team File Breakdown

### Files for Radhesh (Employee Module):
1. `ROSTERRA HR/Models/LeaveApplicationViewModel.cs`
2. `ROSTERRA HR/Models/EmployeeLeaveListViewModel.cs`
3. `ROSTERRA HR/Controllers/EmployeeLeaveController.cs`
4. `ROSTERRA HR/Views/EmployeeLeave/Index.cshtml`
5. `ROSTERRA HR/Views/EmployeeAttendance/Index.cshtml` *(Updated with date-wise log table)*

### Files for Dhruvil (HR Module & Core):
1. `ROSTERRA HR/Scripts/Database/007_CreateLeavesTable.sql`
2. `ROSTERRA HR/Models/Leave.cs`
3. `ROSTERRA HR/Models/HRLeaveManageViewModel.cs`
4. `ROSTERRA HR/Repositories/LeaveRepository.cs`
5. `ROSTERRA HR/Repositories/AttendanceRepository.cs` *(Updated with overnight shift handling)*
6. `ROSTERRA HR/Controllers/LeaveController.cs`
7. `ROSTERRA HR/Views/Leave/Index.cshtml`
8. `ROSTERRA HR/Views/Dashboard/Index.cshtml`, `Views/EmployeeDashboard/Index.cshtml`, `Views/Shared/_Layout.cshtml`, `ROSTERRA HR.csproj`
