# Database Design Specification — Rosterra HR

## 1. Database Overview
- **Database Engine**: Microsoft SQL Server LocalDB (`(localdb)\MSSQLLocalDB`)
- **Database Name**: `RosterraHRDB`
- **Design Philosophy**: Database-First with versioned SQL DDL scripts, manual ADO.NET, and strict relational integrity.
- **Auditing Baseline**: UTC timestamps (`CreatedAtUtc`, `LastLoginAtUtc`, `UpdatedAtUtc`) and soft status flags (`IsActive`).

---

## 2. Table: `dbo.Users`

The `Users` table manages authentication and role assignments across the system.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `UserId` | `INT IDENTITY(1,1)` | NO | Auto (1,1) | `PK_Users` (Clustered) | Primary Key; auto-incrementing surrogate identifier. |
| `Username` | `NVARCHAR(50)` | NO | None | `UQ_Users_Username` (Unique) | Unique login username. |
| `PasswordHash` | `NVARCHAR(256)` | NO | None | None | Base64-encoded PBKDF2 HMAC-SHA256 hash. |
| `PasswordSalt` | `NVARCHAR(256)` | NO | None | None | Base64-encoded 128-bit cryptographic salt. |
| `Role` | `NVARCHAR(20)` | NO | `'HR'` | `DF_Users_Role`, `CK_Users_Role` | Role indicator (`HR` or `Employee`). |
| `IsActive` | `BIT` | NO | `1` | `DF_Users_IsActive` | Active status flag (1 = Active, 0 = Suspended). |
| `CreatedAtUtc` | `DATETIME2` | NO | `SYSUTCDATETIME()` | `DF_Users_CreatedAtUtc` | High-precision record creation timestamp (UTC). |
| `LastLoginAtUtc` | `DATETIME2` | YES | None | None | High-precision timestamp of last successful authentication (UTC). |

---

## 3. Table: `dbo.Shifts`

The `Shifts` table defines workplace operational shifts.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `ShiftId` | `INT IDENTITY(1,1)` | NO | Auto (1,1) | `PK_Shifts` (Clustered) | Primary Key. |
| `ShiftName` | `NVARCHAR(50)` | NO | None | `UQ_Shifts_ShiftName` | Unique shift identifier name. |
| `StartTime` | `TIME(0)` | NO | None | None | Scheduled start of operational shift. |
| `EndTime` | `TIME(0)` | NO | None | None | Scheduled end of operational shift. |
| `Description`| `NVARCHAR(250)` | YES | None | None | Operational notes or break description. |
| `IsActive` | `BIT` | NO | `1` | `DF_Shifts_IsActive` | Active status flag. |
| `CreatedAtUtc`| `DATETIME2`| NO | `SYSUTCDATETIME()` | `DF_Shifts_CreatedAtUtc` | Record creation timestamp (UTC). |

---

## 4. Table: `dbo.Employees`

Workforce records linked to `dbo.Users` and assigned to a `dbo.Shifts`.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `EmployeeId` | `INT IDENTITY(1,1)` | NO | Auto (1,1) | `PK_Employees` (Clustered) | Primary Key. |
| `UserId` | `INT` | NO | None | `FK_Employees_Users`, `UQ_Employees_UserId` | FK referencing `dbo.Users(UserId)`. |
| `ShiftId` | `INT` | YES | None | `FK_Employees_Shifts` | FK referencing `dbo.Shifts(ShiftId)`. |
| `FirstName` | `NVARCHAR(50)` | NO | None | None | Employee first name. |
| `LastName` | `NVARCHAR(50)` | NO | None | None | Employee last name. |
| `Email` | `NVARCHAR(100)` | NO | None | `UQ_Employees_Email` (Unique) | Unique corporate email address. |
| `PhoneNumber`| `NVARCHAR(20)` | YES | None | None | Optional telephone contact. |
| `Department` | `NVARCHAR(50)` | NO | None | None | Assigned department. |
| `Position` | `NVARCHAR(50)` | NO | None | None | Position / job title. |
| `Salary` | `DECIMAL(18,2)` | NO | None | `CK_Employees_Salary` (`>= 0`) | Base compensation. |
| `HireDate` | `DATE` | NO | None | None | Joining date. |
| `IsActive` | `BIT` | NO | `1` | `DF_Employees_IsActive` | Soft-delete status flag. |
| `CreatedAtUtc`| `DATETIME2`| NO | `SYSUTCDATETIME()` | `DF_Employees_CreatedAtUtc` | Record creation timestamp (UTC). |
| `UpdatedAtUtc`| `DATETIME2`| YES | None | None | Last modification timestamp (UTC). |

---

## 5. Table: `dbo.Attendances`

Daily employee punch records, timestamp evaluations, and shift compliance logs.

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `AttendanceId` | `INT IDENTITY(1,1)` | NO | Auto (1,1) | `PK_Attendances` | Primary Key. |
| `EmployeeId` | `INT` | NO | None | `FK_Attendances_Employees` | FK referencing `dbo.Employees(EmployeeId)`. |
| `ShiftId` | `INT` | NO | None | `FK_Attendances_Shifts` | FK referencing `dbo.Shifts(ShiftId)`. |
| `AttendanceDate` | `DATE` | NO | None | `UQ_Attendances_Employee_Date` | Date of attendance (1 record per day per employee). |
| `PunchInTime` | `DATETIME2` | YES | None | None | Timestamp of arrival punch. |
| `PunchOutTime` | `DATETIME2` | YES | None | None | Timestamp of departure punch. |
| `Status` | `NVARCHAR(20)` | NO | None | `CK_Attendances_Status` | `Present`, `Late`, `Absent`, or `Pending`. |
| `ShiftStartTimeSnapshot` | `TIME(0)` | NO | None | None | Shift start time at time of punch. |
| `ShiftEndTimeSnapshot` | `TIME(0)` | NO | None | None | Shift end time at time of punch. |
| `TotalHours` | `DECIMAL(5,2)` | YES | None | None | Total hours worked between punch in and punch out. |
| `Remarks` | `NVARCHAR(250)` | YES | None | None | Calculated arrival early/late notes and departure notes. |
| `MarkedByUserId` | `INT` | NO | None | `FK_Attendances_Users` | FK referencing User who marked the punch. |
| `CreatedAtUtc` | `DATETIME2` | NO | `SYSUTCDATETIME()` | `DF_Attendances_CreatedAtUtc` | Audit creation timestamp (UTC). |
| `UpdatedAtUtc` | `DATETIME2` | YES | None | None | Audit update timestamp (UTC). |

---

## 6. Script Execution Order
1. `001_CreateDatabase.sql` — Initializes `RosterraHRDB`.
2. `002_CreateUsersTable.sql` — Creates `dbo.Users`.
3. `003_SeedInitialHRAccount.sql` — Seeds `hradmin` account.
4. `004_CreateEmployeesTable.sql` — Creates `dbo.Employees`.
5. `005_CreateShiftsAndAttendanceTables.sql` — Creates `dbo.Shifts` and `dbo.Attendances`, adds `ShiftId` to `dbo.Employees`.
6. `006_SeedDefaultShifts.sql` — Seeds baseline shifts and maps active employees.
