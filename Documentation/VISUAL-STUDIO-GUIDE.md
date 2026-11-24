# Visual Studio 2022 Execution & Debugging Guide

This reference explains how to open, build, run, and debug Rosterra HR on any Windows development machine.

---

## 1. Opening the Solution
1. Launch **Visual Studio 2022**.
2. Select **Open a project or solution**.
3. Browse to the workspace directory and select `ROSTERRA HR.sln`.

---

## 2. Setting Startup Project & Restoring Packages
1. In **Solution Explorer**, right-click `ROSTERRA HR` (the web project) and select **Set as Startup Project**.
2. Right-click the top-level solution node `Solution 'ROSTERRA HR'` → select **Restore NuGet Packages**.
3. In the top toolbar, ensure the configuration is set to **Debug** and the target is **IIS Express**.

---

## 3. Building and Running
- **Build Solution**: Press `Ctrl + Shift + B` (verify 0 Errors in the Error List).
- **Run with Debugger**: Press `F5` (launches browser and attaches debugger).
- **Run without Debugger**: Press `Ctrl + F5` (faster execution for everyday UI checks).

---

## 4. How to Debug a Controller Action
1. Open the controller file (e.g., `Controllers/HomeController.cs`).
2. Click in the left margin next to the line inside the action method (e.g., on line `return View();`) to set a red breakpoint (or press `F9`).
3. Press `F5` to start debugging.
4. Navigate to the matching URL in the browser (e.g., `http://localhost:port/Home/Index`).
5. Visual Studio will pause execution at the breakpoint.
6. Use debugging controls:
   - `F10`: Step Over (execute next line).
   - `F11`: Step Into (step into called method).
   - `Shift + F11`: Step Out.
   - `F5`: Continue running.
7. Hover over variables to inspect their runtime values or check the **Locals** and **Watch** windows.

---

## 5. Connecting to LocalDB & Inspecting Database Records
1. In Visual Studio 2022, open **View** &rarr; **SQL Server Object Explorer**.
2. Expand **SQL Server** &rarr; `(localdb)\MSSQLLocalDB` &rarr; **Databases** &rarr; `RosterraHRDB`.
3. Expand **Tables** &rarr; right-click `dbo.Users` &rarr; select **View Data**.
4. You will see the `hradmin` record with `PasswordHash`, `PasswordSalt`, and `Role = 'HR'`.
5. After logging in through the browser, refresh the table view to observe `LastLoginAtUtc` updated automatically.

---

## 6. Phase 1 Login Test Credentials
- **URL**: `http://localhost:port/Account/Login`
- **Username**: `hradmin`
- **Password**: `HR@2026`
- **Expected Outcome**: Redirects to `/Dashboard` and renders the welcome banner.


