# Day 2 — Task 1: Database Creation (RosterraHRDB)

---

## 1. Task Information
- **Task ID**: `D02-T01`
- **Task Name**: SQL Server Database Creation & Execution Setup
- **Development Day**: Day 2
- **Estimated Completion Time**: 20–30 minutes
- **Dependencies**: Day 1 completion, SQL Server LocalDB (`MSSQLLocalDB`) running
- **Completion Criteria**:
  1. Complete understanding of what a relational database, SQL, and LocalDB are.
  2. SQL script file `001_CreateDatabase.sql` created and written inside `ROSTERRA HR/Scripts/Database/`.
  3. Script successfully executed against `(localdb)\MSSQLLocalDB` using Visual Studio SQL Server Object Explorer or SSMS.
  4. Database `RosterraHRDB` verified to exist in SQL Server system catalog.

---

## 2. Objective
Write a clean, readable, standard SQL script to create the relational database named **`RosterraHRDB`** on your SQL Server LocalDB instance. You will learn the fundamental keywords of SQL, how query execution works, and how to verify database creation.

---

## 3. Concepts to Learn: SQL from Scratch

### A. What is a Database?
A **database** is an organized container of structured data stored electronically. Think of a database as a secure filing cabinet. Inside this cabinet, you will later place individual "folders" called **tables** (like `Employees`, `AuditLogs`), which hold rows of records.

### B. What is SQL and T-SQL?
- **SQL (Structured Query Language)**: The universal standard language used to communicate with relational databases.
- **T-SQL (Transact-SQL)**: Microsoft's dialect of SQL used by Microsoft SQL Server. It includes standard SQL plus procedural enhancements, transaction control, and built-in functions.

### C. SQL Command Categories
1. **DDL (Data Definition Language)**: Statements that define, create, or alter the structure of database objects (e.g., `CREATE DATABASE`, `CREATE TABLE`, `ALTER TABLE`, `DROP TABLE`).
2. **DML (Data Manipulation Language)**: Statements that work with the data itself (e.g., `INSERT`, `SELECT`, `UPDATE`, `DELETE`).

### D. The `CREATE DATABASE` Statement
To create a new database container in SQL Server, the foundational statement is:
```sql
CREATE DATABASE DatabaseName;
```
When this statement executes, SQL Server creates two physical files on your disk:
1. **Primary Data File (`.mdf`)**: Stores the schema, tables, indexes, and row data.
2. **Transaction Log File (`.ldf`)**: Records all modifications and transactions for ACID compliance and recovery.

### E. The `GO` Command (Batch Separator)
`GO` is not an official SQL keyword; it is a **command recognized by SQL Server client tools** (Visual Studio, SQLCMD, SSMS). It signals: *"Send the batch of SQL statements written above this line to SQL Server for execution right now."*

---

## 4. Files and Locations

- **File to Write**: `001_CreateDatabase.sql`
- **Folder Location**: `ROSTERRA HR/Scripts/Database/`
- **File Purpose**: Provides a clean, reproducible SQL script to create the `RosterraHRDB` database container.
- **Current State**: File exists as an empty file in your workspace.

---

## 5. Requirements

Your `001_CreateDatabase.sql` script must:
1. Target the database name **`RosterraHRDB`**.
2. Be simple, clean, and readable (no complicated dynamic SQL or complex nested scripts).
3. Include clear header comments documenting the script name and purpose.
4. Use standard uppercase keywords for SQL commands (`CREATE DATABASE`, `GO`) to maintain professional readability.

---

## 6. Implementation Logic

### How to Structure Your SQL Script
When writing your script in `001_CreateDatabase.sql`:

1. **Header Comment**:
   In SQL, comments start with `--` for a single line or `/* ... */` for multiple lines.
   ```sql
   -- ============================================================================
   -- Script: 001_CreateDatabase.sql
   -- Description: Creates the primary RosterraHRDB database
   -- ============================================================================
   ```
2. **The Creation Statement**:
   Write the `CREATE DATABASE` command followed by the target database name:
   `RosterraHRDB`
3. **The Batch Separator**:
   Follow the creation command with `GO` on a new line to instruct the query tool to send the command to the server engine.

---

## 7. Application Execution Flow

How does this database connect to our ASP.NET MVC 5 application?

```
┌──────────────────────────────────────────────┐
│  Web.config (<connectionStrings>)            │
│  Data Source=(localdb)\MSSQLLocalDB;         │
│  Initial Catalog=RosterraHRDB;               │
└───────────────────────┬──────────────────────┘
                        │
                        ▼ Reads database name "RosterraHRDB"
┌──────────────────────────────────────────────┐
│  ADO.NET (SqlConnection)                     │
│  Connects to LocalDB engine                  │
└───────────────────────┬──────────────────────┘
                        │
                        ▼ Locates physical container
┌──────────────────────────────────────────────┐
│  SQL Server LocalDB                          │
│  [RosterraHRDB] Database Container           │
│  ├── RosterraHRDB.mdf (Data)                 │
│  └── RosterraHRDB_log.ldf (Log)              │
└──────────────────────────────────────────────┘
```

---

## 8. Testing Instructions (How to Execute and Verify)

### Option A: Using Visual Studio 2022 (Built-in)
1. Open `ROSTERRA HR.sln` in Visual Studio 2022.
2. Open **SQL Server Object Explorer** via menu: **View** → **SQL Server Object Explorer** (or press `Ctrl+\, Ctrl+S`).
3. Expand **SQL Server** → expand `(localdb)\MSSQLLocalDB`.
4. Right-click on `(localdb)\MSSQLLocalDB` → select **New Query...**.
5. Copy your script from `001_CreateDatabase.sql` into the query window.
6. Click the green **Execute** button (or press `Ctrl+Shift+E`).
7. Inspect the **Messages** tab at the bottom: It should report `Commands completed successfully.`

### Option B: Using SQL Server Management Studio (SSMS)
1. Launch SSMS.
2. In the Server Name field, enter: `(localdb)\MSSQLLocalDB`.
3. Set Authentication to **Windows Authentication** → Click **Connect**.
4. Click **New Query** on the top toolbar.
5. Paste your SQL script and press **F5** (or click **Execute**).

### How to Verify the Database Exists
Open a new query window and run this standard verification query:
```sql
SELECT name, database_id, create_date 
FROM sys.databases 
WHERE name = 'RosterraHRDB';
```
| Test ID | Action | Expected Result | How to Verify |
| :---: | :--- | :--- | :--- |
| **TC-DB-01** | Execute `001_CreateDatabase.sql` | `Commands completed successfully.` in query output. | Output pane. |
| **TC-DB-02** | Refresh `Databases` in SQL Server Object Explorer | `RosterraHRDB` node appears under Databases. | Visual tree view. |
| **TC-DB-03** | Run `SELECT name FROM sys.databases WHERE name = 'RosterraHRDB'` | Returns 1 row containing `RosterraHRDB`. | Results grid. |

---

## 9. Common Mistakes to Avoid

1. **Executing while connected to the wrong server instance**:
   - Always ensure your query window is connected to `(localdb)\MSSQLLocalDB`, not another instance like `SQLEXPRESS` or a remote server.
2. **Executing `CREATE DATABASE` multiple times without checking**:
   - If you run `CREATE DATABASE RosterraHRDB` when it already exists, SQL Server will return error: *Database 'RosterraHRDB' already exists*. If that happens, verify the database is already there!
3. **Misspelling the Database Name**:
   - Case sensitivity depends on server collation, but keep the casing exact: `RosterraHRDB` everywhere (in SQL and later in `Web.config`).

---

## 10. Viva Preparation Questions

1. **What is SQL Server LocalDB and how does it differ from SQL Server Express?**
   * LocalDB is an execution mode of SQL Server Express that runs as a dedicated process on demand under the developer's user account. It requires minimal configuration, creates no background Windows service, and shares the exact same T-SQL capabilities as full SQL Server Express.
2. **What are `.mdf` and `.ldf` files?**
   * `.mdf` (Master Data File) holds table structures, data rows, and indexes. `.ldf` (Log Data File) is the transaction log, storing all transaction steps to ensure ACID properties (Atomicity, Consistency, Isolation, Durability) and allow rollback if an error occurs.
3. **What is `sys.databases` in SQL Server?**
   * It is a system catalog view that returns one row for each database residing on the SQL Server instance, providing metadata such as database ID, creation date, and collation.
4. **What does the `GO` command do in SQL Server?**
   * `GO` is not a T-SQL keyword; it is a batch separator recognized by SQL Server client tools (SSMS, Visual Studio, sqlcmd). It instructs the client tool to compile and send the current batch of statements to the SQL Server engine.
5. **Why do we manage database creation with SQL scripts instead of the Visual Studio GUI?**
   * Scripts are version-controlled in Git, repeatable, auditable, and allow every developer on the team to recreate the identical database schema on their own local machine.

---

## 11. Completion Checklist

Before moving to Task 2 (Employees Table Design):
- [ ] Write the clean `CREATE DATABASE` statement inside `001_CreateDatabase.sql`.
- [ ] Execute the query in Visual Studio SQL Server Object Explorer or SSMS.
- [ ] Run the verification query `SELECT name FROM sys.databases WHERE name = 'RosterraHRDB'` and verify the result.
- [ ] Share your script in chat for review.

---

## 12. GitHub Instructions

After your review is approved:
1. Stage the file:
   ```powershell
   git add "ROSTERRA HR/Scripts/Database/001_CreateDatabase.sql"
   ```
2. Commit:
   ```powershell
   git commit -m "Day 2 Task 1: Add database creation script for RosterraHRDB"
   ```
*(Do not commit yet until you write and test the script!)*

