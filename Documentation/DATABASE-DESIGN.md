# Database Design Specification — Rosterra HR

## 1. Database Overview
- **Database Engine**: Microsoft SQL Server LocalDB (`(localdb)\MSSQLLocalDB`)
- **Database Name**: `RosterraHRDB`
- **Design Philosophy**: Database-First with versioned SQL DDL scripts, manual ADO.NET, and strict relational integrity.
- **Auditing Baseline**: UTC timestamps (`CreatedAtUtc`, `LastLoginAtUtc`) and soft status flags (`IsActive`).

---

## 2. Table: `dbo.Users` (Phase 1 Baseline)

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

## 3. Explicit Constraint Documentation

| Constraint Name | Type | Target Column(s) | Business Rule / Purpose |
| :--- | :--- | :--- | :--- |
| `PK_Users` | Primary Key | `UserId` | Enforces entity uniqueness and clustered index structure. |
| `UQ_Users_Username` | Unique | `Username` | Prevents duplicate accounts with the same username. |
| `CK_Users_Role` | Check | `Role` | Ensures role value is restricted to valid values: `Role IN ('HR', 'Employee')`. |
| `DF_Users_Role` | Default | `Role` | Defaults role to `'HR'` if omitted during creation. |
| `DF_Users_IsActive` | Default | `IsActive` | Defaults new accounts to active status (`1`). |
| `DF_Users_CreatedAtUtc` | Default | `CreatedAtUtc` | Auto-populates creation timestamp using server UTC time (`SYSUTCDATETIME()`). |

---

## 4. Script Execution Order
1. `001_CreateDatabase.sql` — Cleanly drops legacy DB if present and initializes fresh `RosterraHRDB`.
2. `002_CreateUsersTable.sql` — Creates `dbo.Users` table with all integrity constraints.
3. `003_SeedInitialHRAccount.sql` — Seeds initial HR administrator account (`hradmin` / `HR@2026`) with PBKDF2 hash.
