# Database Setup Guide — Rosterra HR

This document explains how any team member can reproduce the `RosterraHRDB` database on their machine using SQL Server LocalDB.

---

## 1. Prerequisites
- SQL Server LocalDB installed (included with Visual Studio SSDT / Data storage and processing workload).
- Instance name: `(localdb)\MSSQLLocalDB`.

Verify LocalDB is running:
```powershell
sqllocaldb start MSSQLLocalDB
sqllocaldb info MSSQLLocalDB
```

---

## 2. Execution Sequence
Always execute the SQL migration scripts in strict numerical sequence:

### Step 1: Database Creation
- **Script**: `ROSTERRA HR/Scripts/Database/001_CreateDatabase.sql`
- **Action**: Creates the `RosterraHRDB` database container.

### Step 2: Employees Table Creation
- **Script**: `ROSTERRA HR/Scripts/Database/002_CreateEmployeesTable.sql`
- **Action**: Creates `dbo.Employees` with:
  - Primary Key: `PK_Employees` (Clustered on `EmployeeId`)
  - Unique Constraint: `UQ_Employees_Email` (Unique on `Email`)
  - Check Constraint: `CK_Employees_Salary` (`Salary >= 0`)
  - Default Constraints: `DF_Employees_IsActive` (`1`), `DF_Employees_CreatedAtUtc` (`SYSUTCDATETIME()`)
  - Nullability: Enforces `NOT NULL` on all required fields.

### Step 3: Seed Baseline Workforce Data
- **Script**: `ROSTERRA HR/Scripts/Database/003_SeedEmployeesData.sql`
- **Action**: Seeds 3 baseline active employee records:
  1. Alice Johnson (HR Manager)
  2. Bob Smith (Senior Developer)
  3. Charlie Davis (Financial Analyst)

---

## 3. How to Execute in Visual Studio 2022
1. Open **View** → **SQL Server Object Explorer**.
2. Connect to `(localdb)\MSSQLLocalDB`.
3. Open `001_CreateDatabase.sql` → Execute.
4. Open `002_CreateEmployeesTable.sql` → Execute.
5. Open `003_SeedEmployeesData.sql` → Execute.

---

## 4. Verification Queries
```sql
USE RosterraHRDB;
GO

-- 1. Verify rows exist
SELECT * FROM Employees;

-- 2. Verify constraints
SELECT CONSTRAINT_NAME, CONSTRAINT_TYPE 
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE TABLE_NAME = 'Employees';
```
