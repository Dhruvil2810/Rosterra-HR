# ROSTERRA HR &mdash; Human Resource & Workforce Management System

[![ASP.NET MVC 5](https://img.shields.io/badge/ASP.NET-MVC%205-blue.svg)](https://dotnet.microsoft.com/)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4.svg)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/SQL%20Server-LocalDB-CC292B.svg)](https://www.microsoft.com/sql-server)
[![Architecture](https://img.shields.io/badge/Data%20Access-Manual%20ADO.NET-success.svg)](#technology-stack)
[![License](https://img.shields.io/badge/License-MIT-lightgrey.svg)](LICENSE)

**Rosterra HR** is an enterprise-grade Human Resource and Workforce Operations Management web application built with **ASP.NET MVC 5** on **.NET Framework 4.8** using **pure manual ADO.NET** and **Microsoft SQL Server LocalDB**.

The platform provides a dual-portal role-based architecture separating administrative human resource workflows from employee self-service capabilities.

---

## Key Modules & Features

### 1. Authentication & Role-Based Access Control (RBAC)
- **Forms Authentication** with secure session management.
- **Cryptographic Security:** Industry-standard **PBKDF2 with HMAC-SHA256** password hashing with 10,000 iterations and unique cryptographic salts (zero plain-text storage).
- **Custom Authorization Filters:** `[AuthorizeHR]` and `[AuthorizeEmployee]` prevent direct URL tampering and cross-portal access.

### 2. Employee Lifecycle Management
- **Directory & Search:** Real-time multi-field search across employee names, email addresses, and departments.
- **Auto-Provisioning:** HR can register an employee and simultaneously provision their user account within an atomic SQL transaction.
- **Profile Management:** Edit personal data, designations, assigned workplace shifts, and toggle active/inactive account status.

### 3. Shift Management System
- **Custom Operating Windows:** HR can configure standard or custom shifts (e.g., Morning `08:00 - 15:00`, General `09:00 - 17:00`, Afternoon `13:00 - 21:00`, Night `21:00 - 05:00`).
- **Overnight Shift Engine:** Seamlessly handles shifts crossing midnight with automatic overnight punch-in/out tracking and duration calculation.
- **Shift Assignments:** Assign or transfer employees between shifts.

### 4. Attendance & Daily Punch Monitoring
- **Lateness & Punctuality Engine:** Automatically evaluates arrival timestamp against the employee's scheduled shift start:
  - Arrived on or before shift start $\rightarrow$ `Present (On-Time / Early)`.
  - Arrived after shift start $\rightarrow$ `Late (calculated by X minutes)`.
  - No punch-in $\rightarrow$ `Absent`.
- **Departure Tracking:** Calculates total hours worked and flags early departures vs completed full shifts.
- **HR Day-Wise Roster:** Interactive date picker with instant aggregate statistics (Present, Late, Absent, Pending).

### 5. Leave Management & Holiday System
- **Sunday Holiday Policy:** Sundays are recognized as official weekly offs. Multi-day leave requests (e.g., Friday to Tuesday) automatically exclude Sundays from leave counts.
- **Calendar Conflict Prevention:** Active (approved and pending) leave dates are dynamically locked on the employee's calendar, preventing duplicate requests.
- **HR Approval Workflow:** HR reviews, filters, and approves or rejects leave applications with remarks.

### 6. Payroll & Compensation Engine
- **Salary Structures:** Configurable per employee with Basic Salary, Allowances (HRA, transport), and Standard Deductions (Tax, PF).
- **Attendance-Integrated Loss of Pay (LOP):**
  - Monthly working days automatically calculated excluding Sundays.
  - Automatically factors in attendance absences and approved unpaid leaves:
    $$\text{Per Day Salary} = \frac{\text{Basic Salary} + \text{Allowances}}{\text{Total Working Days}}$$
    $$\text{Loss of Pay} = \text{Absent Days} \times \text{Per Day Salary}$$
    $$\text{Net Pay} = \text{Gross Salary} - (\text{Standard Deductions} + \text{Loss of Pay})$$
- **Printable Digital Payslips:** Official printable HTML payslips formatted for physical printing or PDF saving (`window.print()`).

---

## Technology Stack

| Layer | Technology | Details |
|---|---|---|
| **Platform** | .NET Framework 4.8 | High-performance Windows application stack |
| **Framework** | ASP.NET MVC 5 (v5.2.9) | Traditional Model-View-Controller architecture |
| **Language** | C# (Roslyn Compiler) | Strongly typed business logic |
| **Data Access** | **Manual ADO.NET** | `SqlConnection`, `SqlCommand`, `SqlDataReader`, parameterized queries (zero ORM/EF overhead) |
| **Database** | SQL Server LocalDB | `(localdb)\MSSQLLocalDB`, database `RosterraHRDB` |
| **Security** | PBKDF2 HMAC-SHA256 | Cryptographically salted password hashing |
| **Frontend** | Razor Views + Bootstrap 5 | Responsive UI with custom clean layouts |

---

## Database Architecture

The application uses versioned SQL scripts located in `ROSTERRA HR/Scripts/Database/`:

1. `001_CreateDatabase.sql` &mdash; Creates `RosterraHRDB`
2. `002_CreateUsersTable.sql` &mdash; User credentials, roles, and password hashes
3. `003_SeedInitialHRAccount.sql` &mdash; Initializes default HR Admin account
4. `004_CreateEmployeesTable.sql` &mdash; Employee profiles linked to users
5. `005_CreateShiftsAndAttendanceTables.sql` &mdash; Shifts and daily attendance tracking
6. `006_SeedDefaultShifts.sql` &mdash; Default Morning, General, Afternoon, and Night shifts
7. `007_CreateLeavesTable.sql` &mdash; Employee time-off requests and approvals
8. `008_CreatePayrollTables.sql` &mdash; Salary structures and monthly payroll records

---

## Getting Started

### Prerequisites
- Visual Studio 2022 (with ASP.NET and web development workload)
- .NET Framework 4.8 Developer Pack
- SQL Server LocalDB

### Setup & Run
1. **Clone the repository:**
   ```bash
   git clone https://github.com/Dhruvil2810/Rosterra-HR.git
   cd Rosterra-HR
   ```
2. **Execute Database Scripts:**
   Run scripts `001` through `008` in `ROSTERRA HR/Scripts/Database/` using `sqlcmd` or SQL Server Management Studio (SSMS) against `(localdb)\MSSQLLocalDB`.
3. **Open Solution:**
   Open `ROSTERRA HR.sln` in Visual Studio 2022.
4. **Run Application:**
   Press `Ctrl + F5` or run via IIS Express.

### Default Credentials

| Portal | Username | Password | Role |
|---|---|---|---|
| **HR Administration** | `hradmin` | `HR@2026` | Full administrative control |
| **Employee Self-Service** | `johndoe` | `Password@2026` | Employee profile, attendance, leaves, payslips |

---

## Project Structure

```
ROSTERRA HR/
├── App_Start/
│   └── RouteConfig.cs              # ASP.NET MVC routing configuration
├── Controllers/
│   ├── AccountController.cs        # Login, logout, session init
│   ├── DashboardController.cs      # HR Admin main hub
│   ├── EmployeeController.cs       # Employee CRUD operations
│   ├── ShiftController.cs          # Shift management & assignments
│   ├── AttendanceController.cs     # HR daily attendance monitor
│   ├── LeaveController.cs          # HR leave approvals
│   ├── PayrollController.cs        # HR payroll runs & structures
│   ├── EmployeeDashboardController.cs   # Employee profile portal
│   ├── EmployeeAttendanceController.cs  # Employee punch log history
│   ├── EmployeeLeaveController.cs       # Employee leave application
│   └── EmployeePayrollController.cs     # Employee payslips & salary
├── Data/
│   └── ConnectionFactory.cs        # ADO.NET connection provider
├── Filters/
│   ├── AuthorizeHRAttribute.cs     # Role barrier for HR routes
│   └── AuthorizeEmployeeAttribute.cs # Role barrier for Employee routes
├── Models/                         # ViewModels and data transfer objects
├── Repositories/                   # ADO.NET query execution layers
├── Scripts/Database/               # Versioned SQL migration scripts
└── Views/                          # Razor layout and feature views
```

---

## Author & Academic Note
Developed as an academic software engineering project demonstrating enterprise-grade manual ADO.NET architecture, role-based authorization, and real-world workforce business rules without relying on high-level ORM abstractions.
