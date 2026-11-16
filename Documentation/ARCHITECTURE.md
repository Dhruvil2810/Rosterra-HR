# Application Architecture — Rosterra HR

## 1. Architectural Philosophy: Separation of Concerns (SoC)

Rosterra HR follows a classic multi-tier layered architecture where each layer has a single, strictly bounded responsibility. No layer is allowed to bypass an adjacent layer.

```
┌─────────────────────────────────────────────────────────────┐
│                       Client Browser                        │
└──────────────────────────────┬──────────────────────────────┘
                               │  HTTP Request (GET / POST)
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                    ASP.NET MVC 5 Routing                    │
│            RouteConfig.cs & Global Action Filters           │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                       Controllers                           │
│  - Accepts HTTP requests                                    │
│  - Performs Model Binding & ModelState validation           │
│  - Delegates business workflows to Services                 │
│  - Selects View or redirects                                │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                         Services                            │
│  - Business logic validation (e.g. duplicate email checks)  │
│  - Orchestrates multi-repository workflows                  │
│  - Generates Business Audit events                          │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                       Repositories                          │
│  - Executes SQL commands via ADO.NET                        │
│  - Maps SqlDataReader rows into C# Model objects            │
│  - Manages database parameters to prevent SQL Injection     │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                    ConnectionFactory                        │
│  - Reads connection string from Web.config                  │
│  - Returns an open SqlConnection within a `using` block     │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                 SQL Server LocalDB Engine                   │
│  - Enforces Primary Keys, Foreign Keys, Unique & Checks     │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Layer Responsibilities & Rules

### A. Controllers (`ROSTERRA HR/Controllers/`)
- **Do:** Handle incoming HTTP parameters, check `ModelState.IsValid`, call service methods, pass ViewModels to Views, handle redirect logic.
- **Don't:** Write SQL statements, open `SqlConnection`, perform password hashing, or execute direct business calculations.

### B. ViewModels & Domain Models (`ROSTERRA HR/Models/`)
- **Domain Models**: Direct representations of database entities (e.g., `Employee`).
- **ViewModels**: Data structures tailored specifically for the view (e.g., `EmployeeCreateViewModel`, `LoginViewModel`). Contains DataAnnotation validation rules (`[Required]`, `[EmailAddress]`).

### C. Services (`ROSTERRA HR/Services/`)
- **Do:** Encapsulate business rules (e.g., "Cannot deactivate an employee who is the head of HR"), hash passwords before storage, coordinate audit log entries when state changes.
- **Don't:** Depend on `System.Web.Mvc` or return HTML/ActionResult.

### D. Repositories (`ROSTERRA HR/Data/Repositories/`)
- **Do:** Use pure ADO.NET (`SqlCommand`, `SqlDataReader`, `SqlParameter`) to interact with the database. Always use parameterized queries.
- **Don't:** Place business validation inside the repository.

### E. Cross-Cutting Filters (`ROSTERRA HR/Filters/`)
- **AuditActionFilter**: Records user page visits and navigation without polluting controller action bodies.
- **AuthorizeRoleAttribute**: Verifies user session and role authorization before action execution.

---

## 3. ADO.NET Connection Management Lifecycle
Every database operation follows the strict `using` pattern for deterministic disposal:

```csharp
using (SqlConnection conn = ConnectionFactory.CreateConnection())
{
    using (SqlCommand cmd = new SqlCommand("SELECT ... WHERE Id = @Id", conn))
    {
        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
        conn.Open();
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                // Map reader data to object
            }
        }
    }
}
```
*Because `SqlConnection` implements `IDisposable`, the `using` block guarantees that the connection is closed and returned to the connection pool even if an unhandled exception occurs.*

