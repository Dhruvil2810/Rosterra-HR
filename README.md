# Rosterra HR — Universal HR & Workforce Management System

A centralized Human Resource and Workforce Management System built with **ASP.NET MVC 5**, **.NET Framework 4.8**, **ADO.NET**, and **SQL Server LocalDB**.

Designed for enterprise-grade separation of concerns, complete audit instrumentation, role-based access control, and educational clarity for B.Tech Semester 5.

---

## 🚀 Technology Stack

- **Framework**: .NET Framework 4.8
- **Platform**: ASP.NET MVC 5 (Razor View Engine)
- **Data Access**: ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlParameter`)
- **Database**: Microsoft SQL Server LocalDB (`(localdb)\MSSQLLocalDB`)
- **Language**: C#
- **IDE**: Visual Studio 2022
- **Version Control**: Git & GitHub

> **Strict Architectural Rules:**
> - No ASP.NET Core / .NET 6+
> - No Entity Framework (v1 is pure ADO.NET Database-First)
> - No Code-First migrations (all schema changes managed via numbered SQL scripts)

---

## 📁 Repository Structure

```
ROSTERRA HR/
│
├── ROSTERRA HR.sln               # Visual Studio Solution
├── .gitignore                    # Visual Studio git ignore rules
├── README.md                     # Project overview and setup instructions
│
├── ROSTERRA HR/                  # ASP.NET MVC 5 Web Project
│   ├── App_Start/                # RouteConfig, FilterConfig
│   ├── Controllers/              # MVC Controllers (HTTP handlers)
│   ├── Models/                   # Data transfer objects & ViewModels
│   ├── Views/                    # Razor .cshtml templates
│   ├── Data/                     # ConnectionFactory & ADO.NET Repositories
│   ├── Services/                 # Business logic & domain workflows
│   ├── Filters/                  # Custom action & authorization filters
│   ├── Scripts/                  # Database migration scripts
│   │   ├── Database/             # Numbered SQL DDL scripts
│   │   └── Seed/                 # Sample seed data
│   ├── Content/                  # CSS and static assets
│   ├── Web.config                # Application configuration & connection strings
│   └── Global.asax               # Application lifecycle events
│
└── Documentation/                # Architectural & viva reference guides
    ├── PROJECT-OVERVIEW.md
    ├── ARCHITECTURE.md
    ├── PROJECT-STRUCTURE.md
    ├── GITHUB-WORKFLOW.md
    └── VISUAL-STUDIO-GUIDE.md
```

---

## 🛠️ Getting Started (Prerequisites & Setup)

### 1. Prerequisites
- **Visual Studio 2022** with workload:
  - *ASP.NET and web development*
  - *.NET Framework 4.8 targeting pack*
- **SQL Server LocalDB** (installed with VS SSDT tools).
- **Git**

### 2. Verify LocalDB
Open PowerShell / Command Prompt and verify LocalDB is running:
```powershell
sqllocaldb start MSSQLLocalDB
sqllocaldb info MSSQLLocalDB
```

### 3. Open and Run in Visual Studio 2022
1. Clone the repository:
   ```powershell
   git clone https://github.com/Dhruvil2810/Rosterra-HR.git
   ```
2. Open `ROSTERRA HR.sln` in **Visual Studio 2022**.
3. Right-click the solution in Solution Explorer → **Restore NuGet Packages**.
4. Set `ROSTERRA HR` as the Startup Project.
5. Press `Ctrl + F5` to run with IIS Express.

---

## 📖 Documentation Index

Full documentation is located in the `Documentation/` folder:
- [Project Overview](Documentation/PROJECT-OVERVIEW.md)
- [Architecture & Layering](Documentation/ARCHITECTURE.md)
- [Project Structure Guide](Documentation/PROJECT-STRUCTURE.md)
- [Visual Studio Execution Guide](Documentation/VISUAL-STUDIO-GUIDE.md)
- [Git & Team Workflow](Documentation/GITHUB-WORKFLOW.md)
