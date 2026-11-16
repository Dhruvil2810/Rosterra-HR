# Day 1 — Task 1: Environment Setup & Project Baseline Verification

---

## 1. Task Information
- **Task ID**: `D01-T01`
- **Task Name**: Environment Verification and ASP.NET MVC 5 Baseline Setup
- **Development Day**: Day 1
- **Estimated Completion Time**: 30–45 minutes
- **Dependencies**: Windows OS with Administrator access
- **Completion Criteria**:
  1. Visual Studio 2022 confirmed with ASP.NET & .NET Framework 4.8 components.
  2. SQL Server LocalDB instance `MSSQLLocalDB` verified and running.
  3. Git verified and working from terminal.
  4. ASP.NET MVC 5 solution (`ROSTERRA HR.sln`) opens and builds cleanly without compiler errors.
  5. Default Controller (`HomeController`) and Razor View (`Views/Home/Index.cshtml`) load in browser under IIS Express.

---

## 2. Objective
Verify the complete development toolchain required for Rosterra HR. You will confirm your installation of Visual Studio 2022, .NET Framework 4.8 developer targeting pack, SQL Server LocalDB, and Git. You will ensure the base ASP.NET MVC 5 solution compiles and launches successfully via IIS Express without external dependencies or broken references.

---

## 3. Concepts to Learn

### A. .NET Framework vs. .NET Core
- **.NET Framework 4.8**: A mature, Windows-integrated runtime. Web applications run on top of `System.Web.dll` and are hosted in Microsoft Internet Information Services (IIS) or IIS Express.
- **.NET Core / .NET 8**: A cross-platform, modular rewrite that does not use `System.Web`.
- *Why we use .NET Framework 4.8*: It is the prescribed syllabus technology, teaching classic ASP.NET lifecycle architecture, Web.config management, and IIS hosting mechanics.

### B. Common Language Runtime (CLR)
The CLR is the heart of .NET. When you compile C# code, the compiler does not produce native machine code directly; it produces **MSIL** (Microsoft Intermediate Language) stored inside an **Assembly** (`.dll` or `.exe`). When you run the application, the CLR's **JIT (Just-In-Time) Compiler** converts MSIL into machine code for your CPU, while managing memory through automated **Garbage Collection (GC)**.

### C. ASP.NET MVC 5 Architecture
- **Model**: Represents application data, business entities, and ViewModels.
- **View**: The presentation layer rendered using the Razor view engine (`.cshtml`), generating HTML sent to the client.
- **Controller**: The orchestrator. Receives HTTP requests, coordinates with services and repositories, and returns an `ActionResult` (such as a View or a Redirect).

### D. SQL Server LocalDB
A lightweight, zero-configuration execution mode of SQL Server Express designed for developers. It runs on demand as a user process (not a heavy background service), using the exact same T-SQL syntax and database engine as production SQL Server instances.

### E. IIS Express
A lightweight, self-contained development web server included with Visual Studio. It handles HTTP requests locally (e.g., `http://localhost:44300/`) and simulates production IIS behavior.

---

## 4. Files and Locations

| File / Folder | Exact Path | Purpose |
| :--- | :--- | :--- |
| **Solution File** | `ROSTERRA HR.sln` | Visual Studio master solution binding projects together. |
| **Project File** | `ROSTERRA HR/ROSTERRA HR.csproj` | MSBuild configuration defining target framework (`v4.8`) and project references. |
| **Startup Routing** | `ROSTERRA HR/App_Start/RouteConfig.cs` | Defines default URL patterns (`{controller}/{action}/{id}`). |
| **Application Lifecycle** | `ROSTERRA HR/Global.asax.cs` | Handles application start, session initialization, and unhandled errors. |
| **App Configuration** | `ROSTERRA HR/Web.config` | Root XML configuration file containing connection strings and compilation options. |
| **Test Controller** | `ROSTERRA HR/Controllers/HomeController.cs` | Baseline controller handling root requests. |
| **Test Razor View** | `ROSTERRA HR/Views/Home/Index.cshtml` | Baseline view presenting HTML markup to the browser. |

---

## 5. Requirements

### Toolchain Requirements
1. **Visual Studio 2022** (Community, Professional, or Enterprise).
2. **Workload Installed**: `ASP.NET and web development`.
3. **Individual Components**:
   - `.NET Framework 4.8 targeting pack`
   - `.NET Framework project and item templates`
   - `SQL Server Data Tools (SSDT)`
4. **LocalDB Instance**: `(localdb)\MSSQLLocalDB` must exist and be startable.
5. **Git**: Installed and accessible via CLI (`git --version`).

### Project Baseline Requirements
- Target Framework: `.NETFramework,Version=v4.8`.
- Controller: A class named `HomeController` inheriting from `System.Web.Mvc.Controller`.
- Action Method: An action method `Index()` returning `ActionResult` with `View()`.
- View: `Views/Home/Index.cshtml` rendering basic HTML confirmation.

---

## 6. Implementation Logic

### Step 1: Check Visual Studio Workloads
1. Open the **Visual Studio Installer** from your Start Menu.
2. Locate **Visual Studio 2022** and click **Modify**.
3. Under the **Workloads** tab, verify **ASP.NET and web development** is selected.
4. Under the **Individual components** tab, search for `4.8` and verify:
   - *.NET Framework 4.8 targeting pack* (or SDK).
5. Click **Close** or **Modify** if changes are needed.

### Step 2: Verify SQL Server LocalDB via Terminal
Open PowerShell or Command Prompt and run:
```powershell
sqllocaldb info
```
*Expected*: Lists `MSSQLLocalDB`.

Start the instance to verify it works:
```powershell
sqllocaldb start MSSQLLocalDB
sqllocaldb info MSSQLLocalDB
```
*Expected*: Reports `State: Running`.

### Step 3: Verify Git Configuration
Run in PowerShell:
```powershell
git --version
git status
```
*Expected*: Displays Git version and confirms you are on branch `main`.

### Step 4: Verify Solution in Visual Studio
1. Launch Visual Studio 2022.
2. Open `ROSTERRA HR.sln`.
3. Check **Solution Explorer**:
   - Solution `ROSTERRA HR` contains the project `ROSTERRA HR`.
   - Right-click project `ROSTERRA HR` → **Properties** → Verify **Target framework** is set to `.NET Framework 4.8`.
4. Restore NuGet packages:
   - Right-click Solution → **Restore NuGet Packages**.
5. Build solution: Press `Ctrl + Shift + B`. Ensure 0 errors.

---

## 7. Application Execution Flow

When you press `Ctrl + F5` to run the baseline project:

```
[Browser Request]
       │  HTTP GET http://localhost:<port>/
       ▼
[IIS Express]
       │  Intercepts request, routes to ASP.NET Pipeline
       ▼
[RouteConfig.cs]
       │  Matches URL against routes.MapRoute():
       │  Defaults: controller = "Home", action = "Index", id = Optional
       ▼
[HomeController.cs]
       │  Activates HomeController instance
       │  Executes public ActionResult Index()
       │  Returns View() -> tells ASP.NET to find matching view
       ▼
[Razor View Engine]
       │  Looks in: /Views/Home/Index.cshtml
       │  Executes Razor code and produces pure HTML
       ▼
[HTTP 200 OK Response sent to Browser]
       │
       ▼
[Browser renders page]
```

---

## 8. Testing Instructions

| Test ID | Action | Expected Result | How to Verify |
| :---: | :--- | :--- | :--- |
| **TC-01** | Run `sqllocaldb info MSSQLLocalDB` | Output displays `State: Running` or `State: Stopped` with version details. | Terminal output inspection. |
| **TC-02** | Press `Ctrl + Shift + B` in Visual Studio | Build completes with `0 Errors, 0 Warnings`. | Visual Studio Output window / Error List. |
| **TC-03** | Press `Ctrl + F5` in Visual Studio | IIS Express launches default browser at `http://localhost:<port>/`. | Browser displays the Rosterra HR verification heading. |
| **TC-04** | Navigate to `http://localhost:<port>/Home/Index` | Exactly the same page loads as the root URL. | Verifies default routing conventions. |

---

## 9. Common Mistakes to Avoid

1. **Selecting ASP.NET Core instead of ASP.NET (.NET Framework)**:
   - When creating or modifying projects, ensure you pick the template with `.NET Framework` in its title. ASP.NET Core has a different project format (`<Project Sdk="...">`) and does not use `Web.config` or `Global.asax`.
2. **Missing .NET Framework 4.8 Targeting Pack**:
   - If missing, Visual Studio will complain that the project target framework is not installed and ask to retarget. Install the 4.8 targeting pack via the Visual Studio Installer.
3. **Leaving LocalDB Corrupted or Unregistered**:
   - If `sqllocaldb info` returns nothing, LocalDB can be recreated with `sqllocaldb create MSSQLLocalDB`.
4. **Committing Build Binaries to Git**:
   - Never commit `bin/`, `obj/`, or `.vs/`. Ensure `.gitignore` is present before staging files.

---

## 10. Viva Preparation Questions

1. **What is the difference between .NET Framework 4.8 and modern .NET (Core/8)?**
   - *.NET Framework 4.8* is Windows-only, tightly coupled to IIS via `System.Web.dll`, and comes pre-installed with Windows. Modern .NET is cross-platform, modular, uses Kestrel web server, and is independent of Windows system files.
2. **What role does `Global.asax` play in an ASP.NET MVC application?**
   - It is the application lifecycle configuration file. In `Application_Start()`, global routes (`RouteConfig.RegisterRoutes`), global filters (`FilterConfig.RegisterGlobalFilters`), and bundles are registered once when IIS worker process starts.
3. **How does ASP.NET MVC find the View when a Controller returns `View()` without parameters?**
   - By convention, the Razor View Engine looks inside `Views/{ControllerName}/{ActionName}.cshtml`. For `HomeController.Index()`, it automatically looks for `Views/Home/Index.cshtml`.
4. **What is the difference between Debug mode (`F5`) and Release/Start Without Debugging (`Ctrl + F5`)?**
   - `F5` attaches the Visual Studio debugger process, loading symbol files (`.pdb`) and allowing breakpoints, but runs slower. `Ctrl + F5` runs the web application directly in IIS Express without debugger overhead.
5. **What is an Assembly in .NET?**
   - An Assembly (`.dll` or `.exe`) is the compiled output of a .NET project. It contains MSIL instructions, type metadata, and a manifest detailing security, versioning, and referenced dependencies.

---

## 11. Completion Checklist

Before requesting the next task, ensure you have:
- [ ] Confirmed Visual Studio 2022 has the ASP.NET workload and .NET Framework 4.8 targeting pack.
- [ ] Started LocalDB via `sqllocaldb start MSSQLLocalDB` and verified its running status.
- [ ] Opened `ROSTERRA HR.sln` and built it cleanly (`0 Errors`).
- [ ] Launched the project in browser via IIS Express (`Ctrl + F5`) and verified the page loads.
- [ ] Checked `git status` to ensure working directory is clean.

---

## 12. GitHub Instructions

Once the baseline verification is complete:
1. Verify status:
   ```powershell
   git status
   ```
2. Pull latest changes if working across computers:
   ```powershell
   git pull origin main
   ```
3. Push your verified baseline:
   ```powershell
   git push origin main
   ```
*(Note: Do not commit temporary `.vs/`, `bin/`, or `obj/` folders; our `.gitignore` protects against this.)*

