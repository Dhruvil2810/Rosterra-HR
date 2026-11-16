# Day 3 — Task 1: Web.config Connection String & ConnectionFactory

---

## 1. Task Information
- **Task ID**: `D03-T01`
- **Task Name**: Centralized Database Connection Architecture (`ConnectionFactory`)
- **Development Day**: Day 3
- **Estimated Completion Time**: 30–40 minutes
- **Dependencies**: Day 2 complete (`RosterraHRDB` database and `Employees` table live)
- **Completion Criteria**:
  1. Connection string named `RosterraHRConnection` configured inside `Web.config`.
  2. Folder `ROSTERRA HR/Data/` created.
  3. Class `ConnectionFactory.cs` created inside `ROSTERRA HR/Data/`.
  4. Method `GetConnection()` implemented to return an unopened `SqlConnection`.
  5. Deterministic connection disposal pattern (`using` statement) understood.

---

## 2. Objective
Establish the centralized database connection mechanism for Rosterra HR. Instead of scattering hardcoded connection strings throughout your controllers and repositories, you will store the connection string once in `Web.config` and expose a thread-safe, static `ConnectionFactory` class that provides fresh `SqlConnection` instances.

---

## 3. Concepts to Learn: ADO.NET from Scratch

### A. What is ADO.NET?
**ADO.NET (ActiveX Data Objects for .NET)** is Microsoft's foundational data access technology included in the .NET Framework (`System.Data.dll` and `System.Data.SqlClient.dll`). It provides direct, high-performance communication between your C# application and SQL Server.

### B. Core ADO.NET Objects

| Class | Namespace | Purpose in Rosterra HR |
| :--- | :--- | :--- |
| **`SqlConnection`** | `System.Data.SqlClient` | Represents an open physical or pooled socket connection to SQL Server. |
| **`SqlCommand`** | `System.Data.SqlClient` | Represents a SQL statement (`SELECT`, `INSERT`, `UPDATE`, `DELETE`) or stored procedure to execute against SQL Server. |
| **`SqlDataReader`** | `System.Data.SqlClient` | A forward-only, read-only stream of rows returned from SQL Server (ultra fast, minimal memory). |
| **`SqlParameter`** | `System.Data.SqlClient` | Binds C# variables to SQL parameters (`@Email`, `@Salary`) to eliminate **SQL Injection**. |

### C. The Connection String Anatomy
A connection string tells ADO.NET how to find, authenticate, and connect to the database:
```xml
<add name="RosterraHRConnection" 
     connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RosterraHRDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True" 
     providerName="System.Data.SqlClient" />
```
- **`Data Source=(localdb)\MSSQLLocalDB`**: The SQL Server instance address.
- **`Initial Catalog=RosterraHRDB`**: The target database name.
- **`Integrated Security=True`**: Uses your current Windows user credentials (Windows Authentication) so no database passwords are hardcoded.
- **`Encrypt=True;TrustServerCertificate=True`**: Encrypts the transport connection between the application and LocalDB.

### D. Why a `ConnectionFactory`? (Single Responsibility Principle)
If you hardcode connection strings inside multiple repository files and your database server changes, you would have to edit dozens of files. With `ConnectionFactory`:
- The connection string name is defined in **one place**.
- Repositories simply ask: *"Give me a connection"*.

### E. Deterministic Disposal and the `using` Statement
`SqlConnection` holds native, unmanaged Windows socket handles. If connections are not properly closed, your server will suffer from **Connection Pool Exhaustion** (crashing the application with `Timeout expired` errors).

By wrapping `SqlConnection` in a `using` block:
```csharp
using (SqlConnection conn = ConnectionFactory.GetConnection())
{
    conn.Open();
    // Execute queries...
} // conn.Dispose() is ALWAYS called here, even if an exception is thrown!
```
The C# `using` block compiles into a `try ... finally` block, ensuring `conn.Dispose()` (and `conn.Close()`) is always executed.

---

## 4. Files and Locations

| File | Exact Path | Purpose |
| :--- | :--- | :--- |
| **Configuration** | `ROSTERRA HR/Web.config` | Define `<connectionStrings>` block. |
| **Connection Factory** | `ROSTERRA HR/Data/ConnectionFactory.cs` | Provides helper methods to read connection string and return `SqlConnection`. |

---

## 5. Requirements

### 1. `Web.config` Requirements
- Inside `<configuration>`, locate or add the `<connectionStrings>` element.
- Add an entry named **`RosterraHRConnection`**.
- Provider must be `System.Data.SqlClient`.

### 2. `ConnectionFactory.cs` Requirements
- Namespace: `ROSTERRA_HR.Data` (matching your project root namespace).
- Class: `public static class ConnectionFactory`
- Responsibilities:
  1. Read the connection string named `"RosterraHRConnection"` using `ConfigurationManager.ConnectionStrings["RosterraHRConnection"].ConnectionString`.
  2. Throw a meaningful `InvalidOperationException` if the connection string is missing or empty.
  3. Expose a public method `GetConnection()` returning a new `SqlConnection(connectionString)`.

---

## 6. Implementation Logic (Pseudocode & Structure)

### Step 1: Adding the Connection String in `Web.config`
In `ROSTERRA HR/Web.config`, directly under `<configuration>`, add:
```xml
<connectionStrings>
  <add name="RosterraHRConnection" 
       connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RosterraHRDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Step 2: Creating `Data/ConnectionFactory.cs`
1. In Solution Explorer, right-click project `ROSTERRA HR` → **Add** → **New Folder** → name it `Data`.
2. Right-click `Data` → **Add** → **Class...** → name it `ConnectionFactory.cs`.
3. Structure:
   - Required `using` statements:
     ```csharp
     using System;
     using System.Configuration;
     using System.Data.SqlClient;
     ```
   - Class structure:
     ```csharp
     namespace ROSTERRA_HR.Data
     {
         public static class ConnectionFactory
         {
             private static readonly string _connectionString;

             static ConnectionFactory()
             {
                 // Read connection string from Web.config ConfigurationManager
                 // Validate it is not null or empty
             }

             public static SqlConnection GetConnection()
             {
                 // Return a new SqlConnection instance
             }
         }
     }
     ```

---

## 7. Application Execution Flow

```
[Repository / Service Layer]
         │
         │  Calls ConnectionFactory.GetConnection()
         ▼
[ConnectionFactory.cs]
         │  Reads ConfigurationManager.ConnectionStrings["RosterraHRConnection"]
         │  Returns new SqlConnection(...)
         ▼
[using (SqlConnection conn = ...)]
         │  conn.Open()
         ▼
[SQL Server LocalDB Connection Pool]
         │  Borrows an active socket from pool
         ▼
[SQL Command Executes]
         │
         ▼
[End of using block]
         │  Calls conn.Dispose()
         ▼
[Socket returned cleanly to Connection Pool]
```

---

## 8. Testing Instructions

| Test ID | Action | Expected Result |
| :---: | :--- | :--- |
| **TC-CF-01** | Build Solution (`Ctrl + Shift + B`) | Compiles with `0 Errors`. |
| **TC-CF-02** | Add a temporary test call in `HomeController.cs` | Calling `using (var conn = ConnectionFactory.GetConnection()) { conn.Open(); }` opens successfully without throwing an exception. |

---

## 9. Common Mistakes to Avoid

1. **Missing Reference to `System.Configuration`**:
   - In .NET Framework 4.8, `ConfigurationManager` requires `using System.Configuration;`. Ensure this using directive is present at the top of `ConnectionFactory.cs`.
2. **Opening Connection Inside Factory**:
   - Do **NOT** call `.Open()` inside `GetConnection()`. The caller (the repository) must open the connection right before executing commands inside a `using` block so it is not held open prematurely.
3. **Misspelling the Connection String Name**:
   - The name in `Web.config` (`"RosterraHRConnection"`) must match the string passed to `ConfigurationManager.ConnectionStrings["..."]` character for character.

---

## 10. Viva Preparation Questions

1. **What is ADO.NET and how does it differ from Entity Framework?**
   - ADO.NET is low-level, direct data access where the developer explicitly manages connections (`SqlConnection`), writes parameterized SQL (`SqlCommand`), and maps rows (`SqlDataReader`). Entity Framework is an Object-Relational Mapper (ORM) that generates SQL and tracks entities automatically.
2. **What is Connection Pooling in SQL Server?**
   - Opening a physical socket connection to SQL Server is expensive (network handshake, authentication). ADO.NET maintains a pool of open connections. When you call `conn.Close()` or `Dispose()`, the physical connection is not destroyed; it is returned to the pool for reuse, vastly improving performance.
3. **Why should `SqlConnection` always be wrapped in a `using` block?**
   - `using` ensures that `IDisposable.Dispose()` is called even if an unhandled exception occurs inside the block. This guarantees connections are closed and returned to the pool, preventing connection leaks.
4. **What does `Integrated Security=True` mean in a connection string?**
   - It enables Windows Authentication. SQL Server authenticates the user using their active Windows OS security token instead of transmitting a database username and password in plain text.
5. **Why should connection strings never be hardcoded in C# classes?**
   - Hardcoding violates separation of concerns and creates security risks. Storing them in `Web.config` allows database locations to be changed between Development, Staging, and Production environments without recompiling the application.

---

## 11. Completion Checklist

- [ ] Add `<connectionStrings>` with `RosterraHRConnection` in `Web.config`.
- [ ] Create `Data/ConnectionFactory.cs`.
- [ ] Implement `GetConnection()` to return a new `SqlConnection`.
- [ ] Press `Ctrl + Shift + B` to verify clean compilation.
- [ ] Share your `ConnectionFactory.cs` and `Web.config` snippet for review!

---

## 12. GitHub Instructions

```powershell
git status
git add "ROSTERRA HR/Web.config"
git add "ROSTERRA HR/Data/ConnectionFactory.cs"
git commit -m "Day 3 Task 1: Add connection string and ConnectionFactory"
```
