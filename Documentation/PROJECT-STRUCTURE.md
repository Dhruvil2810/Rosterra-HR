# Project Directory Structure — Rosterra HR

This document details the file and folder layout of the Rosterra HR project and the specific role of each directory.

```
ROSTERRA HR/
│
├── ROSTERRA HR.sln                  # Master Visual Studio solution file
├── .gitignore                       # Git exclusion rules for .vs, bin, obj, packages
├── README.md                        # Quickstart and project setup
│
├── ROSTERRA HR/                     # Primary ASP.NET MVC 5 Web Project
│   │
│   ├── App_Start/                   # Application startup configurations
│   │   ├── RouteConfig.cs           # URL routing patterns ({controller}/{action}/{id})
│   │   └── FilterConfig.cs          # Global MVC filter registrations
│   │
│   ├── Controllers/                 # Web controllers (HTTP handling & navigation)
│   │   ├── HomeController.cs        # Landing page & health check
│   │   ├── EmployeeController.cs    # (Day 4/5) Employee CRUD operations
│   │   ├── AccountController.cs     # (Day 6) Login, logout & session management
│   │   └── AuditController.cs       # (Day 8) HR audit log viewer
│   │
│   ├── Models/                      # Entities, ViewModels, and Data Transfer Objects
│   │   ├── Domain/                  # Core domain models mapping to DB tables
│   │   │   ├── Employee.cs
│   │   │   ├── AuditLog.cs
│   │   │   └── ApplicationErrorLog.cs
│   │   └── ViewModels/              # Specialized UI binding models with DataAnnotations
│   │       ├── LoginViewModel.cs
│   │       └── EmployeeFormViewModel.cs
│   │
│   ├── Views/                       # Razor .cshtml user interface templates
│   │   ├── Shared/                  # Shared layouts (_Layout.cshtml, _Navbar.cshtml)
│   │   ├── Home/                    # Home views (Index.cshtml)
│   │   ├── Employee/                # Employee views (Index, Create, Edit, Details)
│   │   ├── Account/                 # Login views
│   │   └── Audit/                   # Audit log tables & filter views
│   │
│   ├── Data/                        # Data access layer (ADO.NET)
│   │   ├── ConnectionFactory.cs     # Centralized SQL connection provider
│   │   └── Repositories/            # Direct SQL execution classes
│   │       ├── IEmployeeRepository.cs
│   │       ├── EmployeeRepository.cs
│   │       ├── IAuditRepository.cs
│   │       └── AuditRepository.cs
│   │
│   ├── Services/                    # Business rules and application workflows
│   │   ├── EmployeeService.cs       # Validation, duplicate checking, audit triggers
│   │   └── AuthService.cs          # Password hashing, credential verification
│   │
│   ├── Filters/                     # Reusable MVC cross-cutting attributes
│   │   ├── RoleAuthorizeAttribute.cs # RBAC access checking
│   │   └── AuditPageFilter.cs       # Automated page navigation logging
│   │
│   ├── Infrastructure/              # Cross-cutting technical capabilities
│   │   ├── Security/                # Password hasher (PBKDF2 / SHA-256 with salt)
│   │   └── Logging/                 # Error logging helper
│   │
│   ├── Scripts/                     # SQL migration scripts (Version-controlled)
│   │   ├── Database/                # Numbered DDL scripts
│   │   │   ├── 001_CreateDatabase.sql
│   │   │   ├── 002_CreateEmployeesTable.sql
│   │   │   └── 003_CreateAuditAndErrorTables.sql
│   │   └── Seed/                    # Seed data
│   │       └── 001_SeedInitialEmployees.sql
│   │
│   ├── Content/                     # CSS stylesheets, images, fonts
│   │   └── css/                     # Custom application styles
│   │
│   ├── Web.config                   # App configuration & connection strings
│   ├── Global.asax                  # Application lifecycle hooks
│   └── packages.config              # NuGet package manifest
│
└── Documentation/                   # Comprehensive project & viva documentation
    ├── PROJECT-OVERVIEW.md
    ├── ARCHITECTURE.md
    ├── PROJECT-STRUCTURE.md
    ├── DATABASE-DESIGN.md           # (Day 2)
    ├── DATABASE-SETUP.md            # (Day 2)
    ├── ADO-NET-GUIDE.md             # (Day 3)
    ├── EMPLOYEE-MODULE.md           # (Day 4/5)
    ├── AUTHENTICATION.md            # (Day 6)
    ├── AUTHORIZATION-RBAC.md        # (Day 7)
    ├── AUDIT-LOGGING.md             # (Day 8)
    ├── ERROR-LOGGING.md             # (Day 8)
    ├── GITHUB-WORKFLOW.md
    ├── VISUAL-STUDIO-GUIDE.md
    ├── TESTING.md                   # (Day 9)
    ├── TROUBLESHOOTING.md
    ├── VIVA-QUESTIONS.md
    └── FUTURE-ROADMAP.md
```
