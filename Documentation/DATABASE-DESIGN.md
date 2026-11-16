# Database Design Specification — Rosterra HR

## 1. Database Overview
- **Database Engine**: Microsoft SQL Server LocalDB (`(localdb)\MSSQLLocalDB`)
- **Database Name**: `RosterraHRDB`
- **Design Philosophy**: Database-First with versioned DDL scripts and strict relational constraints.
- **Auditing Baseline**: UTC timestamps (`CreatedAtUtc`, `UpdatedAtUtc`) and soft-delete flags (`IsActive`).

---

## 2. Table: `dbo.Employees`

The `Employees` table is the central workforce entity for Version 1 of Rosterra HR.

### Schema Definition

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `EmployeeId` | `INT IDENTITY(1,1)` | NO | Auto (1,1) | `PK_Employees` (Clustered) | Primary Key; auto-incrementing surrogate identifier. |
| `FirstName` | `NVARCHAR(50)` | NO | None | None | Employee first name (Unicode). |
| `LastName` | `NVARCHAR(50)` | NO | None | None | Employee last name (Unicode). |
| `Email` | `NVARCHAR(100)` | NO | None | `UQ_Employees_Email` (Unique) | Unique employee corporate email address. |
| `PhoneNumber` | `NVARCHAR(20)` | YES | None | None | Optional contact telephone number. |
| `Department` | `NVARCHAR(50)` | NO | None | None | Assigned department (e.g. IT, HR, Finance). |
| `Position` | `NVARCHAR(50)` | NO | None | None | Job title / designation. |
| `Salary` | `DECIMAL(18,2)` | NO | None | `CK_Employees_Salary` (`>= 0`) | Exact fixed-precision compensation. |
| `HireDate` | `DATE` | NO | None | None | Date employee joined the company. |
| `IsActive` | `BIT` | NO | `1` | `DF_Employees_IsActive` | Soft-delete flag (1 = Active, 0 = Inactive). |
| `CreatedAtUtc` | `DATETIME2` | NO | `SYSUTCDATETIME()` | `DF_Employees_CreatedAtUtc` | Unambiguous UTC record creation timestamp. |
| `UpdatedAtUtc` | `DATETIME2` | YES | None | None | Unambiguous UTC last modification timestamp. |

---

## 3. Explicit Constraint Documentation

| Constraint Name | Type | Target Column(s) | Business Rule / Purpose |
| :--- | :--- | :--- | :--- |
| `PK_Employees` | Primary Key | `EmployeeId` | Guarantees uniqueness and establishes physical clustered index order. |
| `UQ_Employees_Email` | Unique | `Email` | Prevents duplicate employee registrations with the same email address. |
| `CK_Employees_Salary` | Check | `Salary` | Guarantees salaries are non-negative (`Salary >= 0`). |
| `DF_Employees_IsActive` | Default | `IsActive` | Default new employees to active (`1`). |
| `DF_Employees_CreatedAtUtc`| Default | `CreatedAtUtc`| Auto-populates creation timestamp using high-precision UTC. |

---

## 4. Script Execution Order
1. `001_CreateDatabase.sql` — Creates `RosterraHRDB`.
2. `002_CreateEmployeesTable.sql` — Creates `dbo.Employees` and constraints.
3. `003_SeedEmployeesData.sql` — Seeds baseline records for testing.

