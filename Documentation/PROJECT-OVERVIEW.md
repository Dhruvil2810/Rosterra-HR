# Project Overview — Rosterra HR

## 1. Executive Summary
**Rosterra HR** is a centralized Human Resource and Workforce Management System built for an educational B.Tech Semester 5 Capstone project. It bridges fundamental enterprise software principles with practical database and application design:
- Multi-tier layered architecture (Separation of Concerns).
- Handcrafted database schema in SQL Server LocalDB.
- Raw ADO.NET data access without ORM abstractions.
- Role-Based Access Control (HR vs. Employee).
- Deep audit trails and segregated application error logging.

---

## 2. Problem Statement
Many small-to-medium enterprises rely on disparate spreadsheets or fragmented tools for managing workforce data. This leads to:
1. Inconsistent employee records.
2. Lack of accountability (no tracking of who modified what).
3. Security vulnerabilities from lack of strict role-based access.
4. Difficulty scaling when new HR modules (payroll, attendance, shifts) are introduced.

Rosterra HR solves this by establishing a disciplined database-first architecture with clear modular boundaries.

---

## 3. Technology Stack & Constraints

| Component | Selected Technology | Syllabus / Engineering Justification |
| :--- | :--- | :--- |
| **Language** | C# | Core language for .NET development. |
| **Web Framework** | ASP.NET MVC 5 | Classic MVC pattern teaching separation of Model, View, and Controller. |
| **Runtime** | .NET Framework 4.8 | Enterprise foundation running on Windows IIS/IIS Express. |
| **Data Access** | ADO.NET | Directly teaches SQL connection management, parameterization, and readers without ORM magic. |
| **Database** | SQL Server LocalDB | Light, zero-configuration local SQL instance reproducible on all developer machines. |
| **View Engine** | Razor (HTML5) | Clean, typed server-side rendering. |
| **Styling** | Modern CSS + Bootstrap | Clean and responsive user interface. |
| **VCS** | Git & GitHub | Industry-standard branching, code review, and teamwork. |

---

## 4. Initial Scope (Version 1)
The 10-day initial development cycle focuses on:
1. **Employee Management (CRUD)**: Personal data, contact info, job title, department assignment, salary, hiring date, status (Active/Inactive).
2. **Authentication**: Form-based session authentication with securely hashed passwords.
3. **Authorization & RBAC**:
   - `HR`: Full CRUD access, audit log viewing, employee deactivation.
   - `Employee`: Self-service view of own profile; restricted from modifying sensitive HR data or accessing others' data.
4. **Audit Logging & Error Logging**:
   - `AuditLogs`: Append-only recording of user actions, navigation, employee updates.
   - `ApplicationErrorLogs`: Segregated recording of unhandled exceptions and database failures.

---

## 5. Viva / Evaluation Points
- **Why ADO.NET over Entity Framework for this project?**
  - Entity Framework automates SQL generation, which hides connection lifecycles, SQL parameterization, and transaction details from students. ADO.NET provides complete, explicit control over every query, connection, and data reader.
- **Why SQL Server LocalDB?**
  - It runs on-demand as a user-level process with the exact same SQL engine as enterprise SQL Server, without requiring a permanent background service.
