# Database Setup & Initialization — Rosterra HR

## 1. Prerequisites
- **Visual Studio 2022** with SQL Server Data Tools / LocalDB installed.
- **SQL Server LocalDB**: Instance `(localdb)\MSSQLLocalDB`.

---

## 2. Automated Script Execution

Run the versioned SQL scripts in numerical order using `sqlcmd` from the terminal or Visual Studio SQL Server Object Explorer:

### Step 1: Create Database
```cmd
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "ROSTERRA HR\Scripts\Database\001_CreateDatabase.sql"
```

### Step 2: Create Users Table
```cmd
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "ROSTERRA HR\Scripts\Database\002_CreateUsersTable.sql"
```

### Step 3: Seed Initial HR Account
```cmd
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "ROSTERRA HR\Scripts\Database\003_SeedInitialHRAccount.sql"
```

### Step 4: Create Employees Table (Phase 2)
```cmd
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "ROSTERRA HR\Scripts\Database\004_CreateEmployeesTable.sql"
```

---

## 3. Connection String Configuration

Located in `ROSTERRA HR/Web.config`:
```xml
<connectionStrings>
  <add name="RosterraHRConnection" 
       connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RosterraHRDB;Integrated Security=True;TrustServerCertificate=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

---

## 4. Default Seeded Accounts
- **HR Administrator**: `hradmin` / `HR@2026`
- **Employee Accounts**: Created through the HR Portal at `/Employee/Create` (each employee receives an automated user account with role `'Employee'`).
