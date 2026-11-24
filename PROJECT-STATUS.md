# Rosterra HR — Master Project Status & Tracker

**Project Name**: Rosterra HR (Universal HR & Workforce Management System)  
**Architecture**: ASP.NET MVC 5, .NET Framework 4.8, ADO.NET, SQL Server LocalDB  
**Primary Developer**: Student  
**AI Assistant / Implementation Partner**: Antigravity  

---

## Overall Roadmap Status

| Phase | Focus Area | Status | Verified Tests | Documentation Guide |
| :---: | :--- | :---: | :---: | :--- |
| **Phase 1** | **HR Login & Dashboard** | 🟢 **Completed & Verified** | **8 / 8 Passed** | `Phase-01-Login-And-HR-Dashboard.md` |
| **Phase 2** | **Employee CRUD & Account Provisioning** | 🟢 **Completed & Verified** | **14 / 14 Passed** | `Phase-02-Employee-CRUD-And-Portal.md` |
| **Phase 3** | Multi-Role RBAC & Profile Management | 🟢 **Integrated Baseline** | **Verified** | Covered in Phase 1 & 2 Guides |
| **Phase 4** | AuditLogs & ApplicationErrorLogs | ⚪ Pending | 0 / 6 | `Scheduled` |
| **Phase 5** | Hardening, Review & Viva Defense Prep | ⚪ Not Started | 0 / 5 | `Scheduled` |

---

## Phase 2 Verification Checklist (Completed)

- [x] **Relational Schema**: `dbo.Employees` created with foreign key to `dbo.Users(UserId)`.
- [x] **ADO.NET Transaction Support**: `CreateEmployeeWithUser` and `SetActiveStatus` using `SqlTransaction`.
- [x] **Automated Credential Generation**: Each employee registration creates a linked login account with role `Employee`.
- [x] **Data Repository**: `EmployeeRepository.cs` with parameterized search, retrieval, updates, and soft-delete.
- [x] **HR Employee Management Views**: Directory index, registration form, detail inspection, and profile editing.
- [x] **Employee Self-Service Portal**: `EmployeeDashboardController` and view with personal profile display.
- [x] **Server-Side RBAC Enforcement**: `AuthorizeEmployeeAttribute` and `AuthorizeHRAttribute` strictly isolating portals.
- [x] **Complete Integration Suite**: All 14 tests passed (Create, Read, Details, Edit, Deactivate, Direct Login, Access Block).
