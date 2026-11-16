# Day 2 — Task 3: Seed Data & Database Verification

---

## 1. Task Information
- **Task ID**: `D02-T03`
- **Task Name**: Seed Employees Data & CRUD Verification
- **Development Day**: Day 2
- **Estimated Completion Time**: 25–35 minutes
- **Dependencies**: Day 2 Task 2 (`Employees` table created with strict `NOT NULL` constraints)
- **Completion Criteria**:
  1. Clean, separate SQL files:
     - `001_CreateDatabase.sql` (Database creation only)
     - `002_CreateEmployeesTable.sql` (Table creation with `NOT NULL` and named constraints)
     - `003_SeedEmployeesData.sql` (Baseline employee insert statements)
  2. Baseline seed data inserted for at least 3 realistic employees across different departments.
  3. Verify data integrity:
     - Duplicate email rejection test passed.
     - Negative salary rejection test passed.
     - NULL value rejection test on required columns passed.

---

## 2. Objective
Populate the `RosterraHRDB` database with realistic baseline workforce records. This seed data will serve as our test foundation for Day 3 (ADO.NET) and Day 4 (Employee List in MVC). You will also run verification checks to confirm all table constraints actively block invalid data.

---

## 3. Concepts to Learn

### A. What is Seed Data?
**Seed data** (or baseline data) is an initial set of data provided with a database to make an application immediately usable and testable after installation. In enterprise systems, seed data typically includes default roles, admin accounts, standard lookup categories, and initial test employees.

### B. The `INSERT INTO` Statement Architecture
```sql
INSERT INTO TableName (Column1, Column2, Column3, ...)
VALUES (Value1, Value2, Value3, ...);
```
- **Column List**: Explicitly list every column being populated.
- **Values List**: Must match the column list in order, count, and data type.
- Columns with `IDENTITY` (like `EmployeeId`) are **omitted** because SQL Server generates them.
- Columns with `DEFAULT` (like `IsActive`, `CreatedAtUtc`) can be **omitted** to allow default values to apply, or explicitly supplied.

### C. String Literals in SQL Server
- In T-SQL, string literals are enclosed in **single quotes**: `'Engineering'`. Double quotes (`"`) are reserved for delimited identifiers.
- For Unicode `NVARCHAR` strings, best practice is to prefix with `N`: `N'Alice'`.

---

## 4. Files and Locations

- **File to Edit**: `003_SeedEmployeesData.sql`
- **Folder Location**: `ROSTERRA HR/Scripts/Database/`
- **File Purpose**: Contains baseline employee records for team members to run after table creation.

---

## 5. Requirements

In `003_SeedEmployeesData.sql`:
1. Include `USE RosterraHRDB;` and `GO` at the top.
2. Insert at least 3 distinct employees representing different departments:
   - **HR Manager** (Human Resources)
   - **Senior Developer** (Engineering)
   - **Financial Analyst** (Finance)
3. Ensure all emails are unique and use realistic corporate domains (e.g., `@rosterra.local`).
4. Ensure all salaries are positive values.
5. Provide a final `SELECT * FROM Employees;` query at the bottom to display the seeded records.

---

## 6. Implementation Logic

1. Switch database context:
   ```sql
   USE RosterraHRDB;
   GO
   ```
2. Write individual `INSERT INTO Employees (...) VALUES (...)` statements for:
   - Alice Johnson (HR, Manager, $75,000)
   - Bob Smith (Engineering, Senior Developer, $85,000)
   - Charlie Davis (Finance, Financial Analyst, $68,000)
3. End with a `SELECT` statement displaying all columns of active employees.

---

## 7. Testing Instructions

### Constraint Verification Tests
Run these intentional failure tests in a query window to prove your constraints are working:

| Test ID | Test Query | Expected Behavior |
| :---: | :--- | :--- |
| **TC-SEED-01** | Execute `003_SeedEmployeesData.sql` | 3 rows inserted successfully. |
| **TC-SEED-02** | Re-run an `INSERT` with an existing email (`alice.johnson@rosterra.local`) | Blocked by `UQ_Employees_Email` with error: `Violation of UNIQUE KEY constraint`. |
| **TC-SEED-03** | Run `INSERT` with `Salary = -100` | Blocked by `CK_Employees_Salary` with error: `conflicted with the CHECK constraint`. |
| **TC-SEED-04** | Run `INSERT` with `FirstName = NULL` | Blocked by column nullability with error: `Cannot insert the value NULL into column 'FirstName'`. |

---

## 8. Viva Preparation Questions

1. **Why do we separate DDL scripts (`CREATE TABLE`) from Seed DML scripts (`INSERT`)?**
   - Separation of concerns: DDL defines structure, while DML populates data. In continuous integration and multi-developer setups, tables need to be created first before seed scripts can run, and seed data may vary between test and production environments.
2. **What happens if you omit the column list in an `INSERT INTO` statement?**
   - Writing `INSERT INTO TableName VALUES (...)` relies on the physical column order in the table schema. If anyone alters the table (adds or moves a column), the query breaks. Always explicitly list target columns.
3. **How does SQL Server handle string literals for `NVARCHAR`?**
   - Prepending the letter `N` (e.g., `N'John'`) tells the SQL engine to treat the string as a National Unicode string (UCS-2 / UTF-16) rather than standard ASCII, preventing character corruption.

---

## 9. Completion Checklist

- [ ] `001_CreateDatabase.sql` contains only database creation.
- [ ] `002_CreateEmployeesTable.sql` contains table definition with `NOT NULL` on required fields.
- [ ] `003_SeedEmployeesData.sql` contains clean seed records for 3 employees.
- [ ] Verification tests (TC-SEED-02, 03, 04) pass successfully.

---

## 10. GitHub Instructions

After verification:
```powershell
git add "ROSTERRA HR/Scripts/Database/001_CreateDatabase.sql"
git add "ROSTERRA HR/Scripts/Database/002_CreateEmployeesTable.sql"
git add "ROSTERRA HR/Scripts/Database/003_SeedEmployeesData.sql"
git commit -m "Day 2: Finalize database schema, constraints, and baseline seed data"
git push origin main
```
