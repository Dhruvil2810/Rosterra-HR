# Rosterra HR — Master Project Status & Tracker

**Project Name**: Rosterra HR (Universal HR & Workforce Management System)  
**Architecture**: ASP.NET MVC 5, .NET Framework 4.8, ADO.NET, SQL Server LocalDB  
**Primary Developer**: Student  
**AI Assistant / Implementation Partner**: Antigravity  

---

## Overall Roadmap Status

| Phase | Focus Area | Status | Verified Tests | Documentation Guide |
| :---: | :--- | :---: | :---: | :--- |
| **Phase 1** | **HR Login & Dashboard (Restarted Clean)** | 🟢 **Completed & Verified** | **8 / 8 Passed** | `Phase-01-Login-And-HR-Dashboard.md` |
| **Phase 2** | Employee CRUD & ADO.NET Repositories | ⚪ Pending Sign-off | 0 / 8 | `Pending Phase 1 Sign-off` |
| **Phase 3** | Multi-Role RBAC (HR & Employee Portal) | ⚪ Not Started | 0 / 6 | `Scheduled` |
| **Phase 4** | AuditLogs & ApplicationErrorLogs | ⚪ Not Started | 0 / 6 | `Scheduled` |
| **Phase 5** | Hardening, Review & Viva Defense Prep | ⚪ Not Started | 0 / 5 | `Scheduled` |

---

## Phase 1 Verification Checklist (Completed)

- [x] **Database Clean Restart**: Fresh `RosterraHRDB` created on `(localdb)\MSSQLLocalDB`.
- [x] **Minimal DDL**: `dbo.Users` table with PK, Unique, Check, and Default constraints.
- [x] **Initial Administrator**: `hradmin` seeded with PBKDF2 HMAC-SHA256 hashed password (`HR@2026`).
- [x] **ADO.NET Connection**: `ConnectionFactory.cs` managing `SqlConnection` with `Web.config`.
- [x] **Data Repository**: `UserRepository.cs` with parameterized SQL queries.
- [x] **Security Service**: `PasswordHasher.cs` with PBKDF2 SHA-256 and constant-time byte verification.
- [x] **Authentication Flow**: `AccountController` with `Login` / `Logout`, session, and forms auth cookies.
- [x] **Server-Side Authorization**: `AuthorizeHRAttribute` guarding protected actions and blocking direct URL access.
- [x] **HR Dashboard**: `DashboardController` & `Views/Dashboard/Index.cshtml` displaying greeting and module placeholders.
- [x] **Automated Test Suite**: All 8 test scenarios passed (Valid Login, Invalid Password, Invalid Username, Empty Fields, Direct Access Block, Logout, Post-Logout Denial).
