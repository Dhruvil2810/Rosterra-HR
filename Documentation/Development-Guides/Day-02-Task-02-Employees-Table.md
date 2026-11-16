# Day 2 — Task 2: Employees Table Design & DDL

---

## 1. Task Information
- **Task ID**: `D02-T02`
- **Task Name**: Employees Table Creation, Constraints & Basic CRUD Understanding
- **Development Day**: Day 2
- **Estimated Completion Time**: 40–50 minutes
- **Dependencies**: Day 2 Task 1 (`RosterraHRDB` database created and running)
- **Completion Criteria**:
  1. Understand every SQL data type and constraint used for the `Employees` table.
  2. Write the DDL script in `ROSTERRA HR/Scripts/Database/002_CreateEmployeesTable.sql`.
  3. Execute the script against `RosterraHRDB` on LocalDB.
  4. Verify table structure and constraints using system views.
  5. Practice and verify sample `INSERT`, `SELECT`, `UPDATE`, and `DELETE` queries.

---

## 2. Objective
Create the primary **`Employees`** table in `RosterraHRDB`. The table will store employee identity, contact information, department, position, salary, hire date, status (active/inactive), and audit timestamps. You will write simple, readable SQL using explicitly named constraints.

---

## 3. Concepts to Learn: SQL Table Architecture from Scratch

### A. What is a Table?
A **table** is a collection of related data stored in rows and columns.
- **Column (Attribute / Field)**: A vertical entity representing a single property (e.g., `FirstName`, `Salary`). Every column has a strict **data type**.
- **Row (Record / Tuple)**: A horizontal entity representing one individual employee.

### B. What is the `USE` Statement?
Before creating a table, you must tell SQL Server which database to create it in.
```sql
USE DatabaseName;
GO
```
This switches the active database context so your table is not accidentally created inside system databases like `master`.

### C. SQL Server Data Types Explained

| Data Type in SQL | C# Equivalent | Storage Size | Why We Use It in Rosterra HR |
| :--- | :--- | :--- | :--- |
| `INT` | `int` | 4 bytes | Standard 32-bit integer. Perfect for employee IDs (supports up to 2.14 billion rows). |
| `NVARCHAR(n)` | `string` | 2 bytes per char | Variable-length Unicode string. The `N` prefix stands for **National** (Unicode/UTF-16), supporting accents and international character sets. |
| `DECIMAL(18,2)` | `decimal` | 9 bytes | Exact fixed-point numeric. `18` is precision (total digits), `2` is scale (digits after decimal point). Eliminates rounding errors for currency/salaries. |
| `DATE` | `DateTime` | 3 bytes | Stores year, month, and day (`YYYY-MM-DD`) without time of day. Ideal for `HireDate`. |
| `BIT` | `bool` | 1 bit (stored as byte) | Stores boolean values: `1` (True / Active) or `0` (False / Inactive). |
| `DATETIME2` | `DateTime` | 6–8 bytes | Modern high-precision date and time (`YYYY-MM-DD hh:mm:ss.nnnnnnn`). Superior to legacy `DATETIME`. |

### D. Identity Column (`IDENTITY(seed, increment)`)
```sql
EmployeeId INT IDENTITY(1,1)
```
- `seed = 1`: The first record inserted gets ID 1.
- `increment = 1`: Every new record automatically increments by 1 (1, 2, 3...).
- SQL Server manages this value automatically; you do not supply an ID during `INSERT`.

### E. Database Constraints & Why Each is Required

Constraints enforce business and data integrity rules directly in the database engine:

1. **`NOT NULL`**:
   - *Meaning*: The column cannot be left empty.
   - *Why*: An employee cannot exist without a first name, last name, email, department, or hire date.
2. **`PRIMARY KEY` (`PK_Employees`)**:
   - *Meaning*: Uniquely identifies each row in the table. Cannot contain `NULL`.
   - *Why*: Ensures every employee has a guaranteed unique identifier (`EmployeeId`).
   - *Clustered Index*: In SQL Server, a primary key automatically creates a **Clustered Index**, physically organizing data pages on disk in numerical order of `EmployeeId` for instant lookups.
3. **`UNIQUE` Constraint (`UQ_Employees_Email`)**:
   - *Meaning*: Guarantees that no two rows can have the same value in this column.
   - *Why*: Two employees must never share the same work email address.
4. **`CHECK` Constraint (`CK_Employees_Salary`)**:
   - *Meaning*: Validates a boolean expression before any row is inserted or updated.
   - *Why*: `Salary >= 0` prevents accidental negative salaries from being entered.
5. **`DEFAULT` Constraint (`DF_Employees_IsActive`, `DF_Employees_CreatedAtUtc`)**:
   - *Meaning*: Supplies a default value if none is specified during `INSERT`.
   - *Why*: New employees are Active by default (`1`), and creation time is automatically stamped with the server's current UTC time (`SYSUTCDATETIME()`).

### F. Explicit Constraint Naming Convention
Instead of letting SQL Server generate random names like `PK__Employee__7AD04FF1`, always use clear names:
- `PK_TableName` → `PK_Employees`
- `UQ_TableName_Column` → `UQ_Employees_Email`
- `CK_TableName_Rule` → `CK_Employees_Salary`
- `DF_TableName_Column` → `DF_Employees_IsActive`

---

## 4. Files and Locations

- **File to Edit**: `002_CreateEmployeesTable.sql`
- **Folder Location**: `ROSTERRA HR/Scripts/Database/`
- **File Purpose**: DDL script defining the schema of the `Employees` table with all constraints.

---

## 5. Requirements (Column Specifications)

Your table schema must contain these exact 12 columns:

| Column Name | Data Type | Nullable? | Constraints / Default | Purpose |
| :--- | :--- | :---: | :--- | :--- |
| `EmployeeId` | `INT IDENTITY(1,1)` | NO | `CONSTRAINT PK_Employees PRIMARY KEY CLUSTERED` | Unique primary key |
| `FirstName` | `NVARCHAR(50)` | NO | None (Required) | Employee first name |
| `LastName` | `NVARCHAR(50)` | NO | None (Required) | Employee last name |
| `Email` | `NVARCHAR(100)` | NO | `CONSTRAINT UQ_Employees_Email UNIQUE` | Work email address |
| `PhoneNumber` | `NVARCHAR(20)` | YES | None (Optional) | Contact telephone number |
| `Department` | `NVARCHAR(50)` | NO | None (Required) | Department name |
| `Position` | `NVARCHAR(50)` | NO | None (Required) | Job title / designation |
| `Salary` | `DECIMAL(18,2)` | NO | `CONSTRAINT CK_Employees_Salary CHECK (Salary >= 0)` | Monthly/Annual salary |
| `HireDate` | `DATE` | NO | None (Required) | Date of joining |
| `IsActive` | `BIT` | NO | `CONSTRAINT DF_Employees_IsActive DEFAULT (1)` | Soft-delete status |
| `CreatedAtUtc` | `DATETIME2` | NO | `CONSTRAINT DF_Employees_CreatedAtUtc DEFAULT (SYSUTCDATETIME())` | Creation audit timestamp |
| `UpdatedAtUtc` | `DATETIME2` | YES | None (Optional) | Last update audit timestamp |

---

## 6. Implementation Logic (How to Construct the Script)

In `002_CreateEmployeesTable.sql`, follow this readable structure:

1. **Header comment**: Document script name, purpose, and table created.
2. **Database Context**:
   ```sql
   USE RosterraHRDB;
   GO
   ```
3. **Table Creation Block**:
   ```sql
   CREATE TABLE Employees
   (
       -- Columns with inline nullability and defaults
       EmployeeId    INT IDENTITY(1,1) NOT NULL,
       FirstName     NVARCHAR(50)      NOT NULL,
       ...
       IsActive      BIT               NOT NULL CONSTRAINT DF_Employees_IsActive DEFAULT (1),
       CreatedAtUtc  DATETIME2         NOT NULL CONSTRAINT DF_Employees_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
       UpdatedAtUtc  DATETIME2         NULL,

       -- Table-level constraints
       CONSTRAINT PK_Employees PRIMARY KEY CLUSTERED (EmployeeId),
       CONSTRAINT UQ_Employees_Email UNIQUE (Email),
       CONSTRAINT CK_Employees_Salary CHECK (Salary >= 0)
   );
   GO
   ```

---

## 7. Sample CRUD Queries (How to Interact with Your Table)

Once your table is created, here is how the 4 basic SQL operations (CRUD) work:

### 1. `INSERT` (Create)
Adds a new row to the table. Notice that `EmployeeId`, `IsActive`, and `CreatedAtUtc` do not need to be specified because of `IDENTITY` and `DEFAULT`:
```sql
INSERT INTO Employees (FirstName, LastName, Email, PhoneNumber, Department, Position, Salary, HireDate)
VALUES ('John', 'Doe', 'john.doe@rosterra.local', '555-1234', 'Engineering', 'Software Developer', 70000.00, '2024-01-15');
```

### 2. `SELECT` (Read)
Retrieves rows from the table:
```sql
-- Read all active employees:
SELECT EmployeeId, FirstName, LastName, Email, Department, Position, Salary 
FROM Employees 
WHERE IsActive = 1;

-- Read a single employee by primary key:
SELECT * 
FROM Employees 
WHERE EmployeeId = 1;
```

### 3. `UPDATE` (Update)
Modifies existing data in a row. **Always use a `WHERE` clause** to avoid updating every row in the table!
```sql
UPDATE Employees 
SET Position = 'Lead Developer', 
    Salary = 80000.00,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE EmployeeId = 1;
```

### 4. `DELETE` vs. Soft Delete
- **Hard Delete** (Permanently removes data from disk):
  ```sql
  DELETE FROM Employees WHERE EmployeeId = 1;
  ```
- **Soft Delete** (The Rosterra HR approach: preserves historical integrity):
  ```sql
  UPDATE Employees 
  SET IsActive = 0,
      UpdatedAtUtc = SYSUTCDATETIME()
  WHERE EmployeeId = 1;
  ```

---

## 8. Testing Instructions

### How to Execute in Visual Studio 2022
1. Open Visual Studio.
2. Open **SQL Server Object Explorer** (`Ctrl+\, Ctrl+S`).
3. Expand `(localdb)\MSSQLLocalDB` → expand `Databases` → right-click `RosterraHRDB` → select **New Query...**.
4. Paste your completed `002_CreateEmployeesTable.sql` script.
5. Click **Execute** (green triangle or `Ctrl+Shift+E`).
6. Verify output says: `Commands completed successfully.`

### How to Verify Table & Constraints
Run this query to inspect your new table columns and constraints:
```sql
-- Check columns
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Employees';

-- Check constraints
SELECT CONSTRAINT_NAME, CONSTRAINT_TYPE
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME = 'Employees';
```

| Test ID | Test Action | Expected Result |
| :---: | :--- | :--- |
| **TC-T01** | Execute `002_CreateEmployeesTable.sql` | Table created with 0 errors. |
| **TC-T02** | Refresh `Tables` under `RosterraHRDB` | `dbo.Employees` appears in the list. |
| **TC-T03** | Insert a test employee | Row inserted with `EmployeeId = 1`. |
| **TC-T04** | Insert another employee with the **same email** | SQL Server rejects with error: `Violation of UNIQUE KEY constraint 'UQ_Employees_Email'`. |
| **TC-T05** | Insert an employee with `Salary = -500` | SQL Server rejects with error: `The INSERT statement conflicted with the CHECK constraint 'CK_Employees_Salary'`. |

---

## 9. Common Mistakes to Avoid

1. **Missing `USE RosterraHRDB;`**:
   - If omitted, your query window might execute in the `master` database by default.
2. **Forgetting `N` in Unicode Types**:
   - Using `VARCHAR` instead of `NVARCHAR` will prevent international names from storing properly.
3. **Using `FLOAT` for Salary**:
   - Never use `FLOAT` for financial data; always use `DECIMAL(18,2)`.
4. **Omitting Constraint Names**:
   - Writing `PRIMARY KEY` without `CONSTRAINT PK_Employees` causes SQL Server to generate an unreadable, random name.

---

## 10. Viva Preparation Questions

1. **Why do we use `NVARCHAR` instead of `VARCHAR`?**
   - `NVARCHAR` stores 2 bytes per character using Unicode (UTF-16), supporting international alphabets and special symbols. `VARCHAR` is 1 byte per character (ASCII) and cannot store non-Western characters.
2. **What is a Clustered Index and how is it related to a Primary Key?**
   - A Clustered Index determines the physical order of data rows in a table. In SQL Server, creating a Primary Key creates a clustered index on that column by default, making primary key lookups extremely fast (`O(log N)` via B-Tree).
3. **What is a Check Constraint and when does it execute?**
   - A Check constraint is a rule that validates column data whenever an `INSERT` or `UPDATE` statement is executed. If the expression evaluates to FALSE (e.g., `Salary < 0`), the transaction is aborted and rolled back.
4. **What is the difference between a Hard Delete and a Soft Delete?**
   - A *Hard Delete* physically removes the row from disk (`DELETE FROM ...`), which can cause foreign key violations or erase historical records. A *Soft Delete* sets a flag (e.g., `IsActive = 0`), hiding the record from active UI lists while preserving audit and relationship history.
5. **Why do we explicitly name constraints like `PK_Employees` and `UQ_Employees_Email`?**
   - If not named explicitly, SQL Server assigns an auto-generated random string. Explicit naming makes error messages immediately understandable in application logs and allows automated scripts to alter or drop constraints reliably.

---

## 11. Completion Checklist

- [ ] Ensure `001_CreateDatabase.sql` has your `CREATE DATABASE RosterraHRDB;` statement and is saved.
- [ ] Write the complete `CREATE TABLE Employees (...)` DDL in `002_CreateEmployeesTable.sql`.
- [ ] Execute the script against `RosterraHRDB`.
- [ ] Run the constraint verification tests (TC-T04 unique email rejection, TC-T05 negative salary rejection).
- [ ] Share your script in chat for review!

---

## 12. GitHub Instructions

After the code review is approved:
```powershell
git add "ROSTERRA HR/Scripts/Database/001_CreateDatabase.sql"
git add "ROSTERRA HR/Scripts/Database/002_CreateEmployeesTable.sql"
git commit -m "Day 2 Task 2: Create Employees table schema with constraints"
```

