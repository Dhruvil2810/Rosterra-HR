# Database Design Specification — Rosterra HR

## 1. Database Overview
- **Database Engine**: Microsoft SQL Server LocalDB (`(localdb)\MSSQLLocalDB`)
- **Database Name**: `RosterraHRDB`
- **Design Philosophy**: Database-First with versioned SQL DDL scripts, manual ADO.NET, and strict relational integrity.
- **Auditing Baseline**: UTC timestamps (`CreatedAtUtc`, `LastLoginAtUtc`, `UpdatedAtUtc`) and soft status flags (`IsActive`).

---

## 2. Table: `dbo.Users`

The `Users` table manages authentication and role assignments across the system.

### Schema Definition

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

## 3. Table: `dbo.Employees`

The `Employees` table stores core workforce records and links directly to `dbo.Users`.

### Schema Definition

| Column | Data Type | Nullable | Default | Constraints | Description |
| :--- | :--- | :---: | :--- | :--- | :--- |
| `EmployeeId` | `INT IDENTITY(1,1)` | NO | Auto (1,1) | `PK_Employees` (Clustered) | Primary Key; auto-incrementing surrogate identifier. |
| `UserId` | `INT` | NO | None | `FK_Employees_Users`, `UQ_Employees_UserId` | Foreign Key referencing `dbo.Users(UserId)`. |
| `FirstName` | `NVARCHAR(50)` | NO | None | None | Employee first name. |
| `LastName` | `NVARCHAR(50)` | NO | None | None | Employee last name. |
| `Email` | `NVARCHAR(100)` | NO | None | `UQ_Employees_Email` (Unique) | Unique employee corporate email address. |
| `PhoneNumber` | `NVARCHAR(20)` | YES | None | None | Optional contact telephone number. |
| `Department` | `NVARCHAR(50)` | NO | None | None | Department name (e.g. Engineering, HR). |
| `Position` | `NVARCHAR(50)` | NO | None | None | Job title / designation. |
| `Salary` | `DECIMAL(18,2)` | NO | None | `CK_Employees_Salary` (`>= 0`) | Exact fixed-precision compensation. |
| `HireDate` | `DATE` | NO | None | None | Date of hire. |
| `IsActive` | `BIT` | NO | `1` | `DF_Employees_IsActive` | Soft-delete status flag. |
| `CreatedAtUtc` | `DATETIME2` | NO | `SYSUTCDATETIME()` | `DF_Employees_CreatedAtUtc` | High-precision record creation timestamp (UTC). |
| `UpdatedAtUtc` | `DATETIME2` | YES | None | None | Timestamp of last modification (UTC). |

---

## 4. Relational Constraints & Integrity Rules

| Constraint Name | Type | Target Column(s) | Business Rule / Purpose |
| :--- | :--- | :--- | :--- |
| `PK_Users` | Primary Key | `UserId` | Enforces entity uniqueness and clustered index structure. |
| `UQ_Users_Username` | Unique | `Username` | Prevents duplicate accounts with the same username. |
| `CK_Users_Role` | Check | `Role` | Ensures role value is restricted to: `Role IN ('HR', 'Employee')`. |
| `PK_Employees` | Primary Key | `EmployeeId` | Enforces employee record uniqueness. |
| `FK_Employees_Users`| Foreign Key | `UserId` | Enforces referential integrity with `dbo.Users(UserId)`. |
| `UQ_Employees_UserId`| Unique | `UserId` | Enforces 1-to-1 account-to-employee mapping. |
| `UQ_Employees_Email` | Unique | `Email` | Prevents multiple employee records with the same email. |
| `CK_Employees_Salary`| Check | `Salary` | Guarantees non-negative compensation (`Salary >= 0`). |

---

## 5. Script Execution Order
1. `001_CreateDatabase.sql` — Cleanly drops legacy DB if present and initializes fresh `RosterraHRDB`.
2. `002_CreateUsersTable.sql` — Creates `dbo.Users` table with all integrity constraints.
3. `003_SeedInitialHRAccount.sql` — Seeds initial HR administrator account (`hradmin` / `HR@2026`) with PBKDF2 hash.
4. `004_CreateEmployeesTable.sql` — Creates `dbo.Employees` table with foreign key to `dbo.Users`.
