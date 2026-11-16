# ADO.NET Architecture & Execution Guide — Rosterra HR

## 1. Overview
Rosterra HR uses pure **ADO.NET** (`System.Data.SqlClient`) for all data access operations. This ensures:
- High performance and minimal memory footprint.
- Explicit lifecycle control over database connections.
- Parameterized SQL execution to prevent SQL Injection vulnerabilities.
- Complete transparency for educational and viva defense purposes.

---

## 2. Core Architecture Pipeline

```
[Service Layer]
       │
       │ Invokes Repository method (e.g., GetActiveEmployees())
       ▼
[Repository Layer (Data/Repositories/)]
       │
       │ 1. Requests new SqlConnection from ConnectionFactory
       ▼
[ConnectionFactory (Data/ConnectionFactory.cs)]
       │ Reads Web.config: "RosterraHRConnection"
       │ Returns unopened SqlConnection
       ▼
[using (SqlConnection conn = ConnectionFactory.GetConnection())]
       │
       ├──> conn.Open()
       │
       ├──> using (SqlCommand cmd = new SqlCommand(sql, conn))
       │        ├── cmd.Parameters.Add(new SqlParameter(...))
       │        │
       │        └── using (SqlDataReader reader = cmd.ExecuteReader())
       │                 └── while (reader.Read()) -> MapToEntity(reader)
       │
       └──> conn.Dispose() [Automatically returned to connection pool]
```

---

## 3. Best Practices Enforced
1. **Always Use Parameterized Queries**: Never concatenate strings into SQL commands (`"WHERE Email = '" + email + "'"` is strictly forbidden).
2. **Deterministic Disposal**: Every `SqlConnection`, `SqlCommand`, and `SqlDataReader` MUST be wrapped in a `using` block.
3. **Explicit Data Type Binding**: Use `cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = ... })`.
4. **Handling NULLs Safely**: When reading from `SqlDataReader`, use `reader.IsDBNull(colIndex) ? null : reader.GetString(colIndex)`.
