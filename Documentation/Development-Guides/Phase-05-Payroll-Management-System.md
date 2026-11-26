# Phase 5 — Simple Payroll Management & Compensation Module

## 1. Overview
The Payroll Management module completes the workforce operations lifecycle by connecting **Salary Structures**, **Shift Attendance records**, and **Approved Time-Off** to automate monthly salary computation.

### Key Capabilities:
1. **Salary Structures**:
   - Configurable per employee with Basic Salary, Monthly Allowances (HRA, transport), and Standard Deductions (Tax, PF).
2. **Attendance-Integrated Loss of Pay (LOP)**:
   - Monthly working days automatically calculated excluding Sundays.
   - Absent days detected from `dbo.Attendances` and unpaid leave in `dbo.Leaves`.
   - $\text{Loss of Pay} = \text{Absent Days} \times \left(\frac{\text{Gross Salary}}{\text{Total Working Days}}\right)$.
   - $\text{Net Take-Home} = \text{Gross Salary} - (\text{Standard Deductions} + \text{Loss of Pay})$.
3. **HR Payroll Dashboard (`/Payroll`)**:
   - Select Month & Year, one-click payroll generation/recalculation, mark payment as Paid, view company-wide disbursement totals.
4. **Digital Printable Payslip**:
   - Clean printable HTML statement with print styles (`window.print()`).
5. **Employee Self-Service (`/EmployeePayroll`)**:
   - Employee can review their active salary package, historical pay slips, and download printable receipts.

---

## 2. Team File Breakdown

### Files for Radhesh (Employee Module):
1. `ROSTERRA HR/Models/EmployeePayrollHistoryViewModel.cs`
2. `ROSTERRA HR/Controllers/EmployeePayrollController.cs`
3. `ROSTERRA HR/Views/EmployeePayroll/Index.cshtml`
4. `ROSTERRA HR/Views/EmployeePayroll/Payslip.cshtml`

### Files for Dhruvil (HR Module & Core):
1. `ROSTERRA HR/Scripts/Database/008_CreatePayrollTables.sql`
2. `ROSTERRA HR/Models/SalaryStructure.cs`
3. `ROSTERRA HR/Models/Payroll.cs`
4. `ROSTERRA HR/Models/HRPayrollIndexViewModel.cs`
5. `ROSTERRA HR/Repositories/PayrollRepository.cs`
6. `ROSTERRA HR/Controllers/PayrollController.cs`
7. `ROSTERRA HR/Views/Payroll/Index.cshtml`
8. `ROSTERRA HR/Views/Payroll/SalaryStructures.cshtml`
9. `ROSTERRA HR/Views/Payroll/EditStructure.cshtml`
10. `ROSTERRA HR/Views/Payroll/Payslip.cshtml`
11. Updated: `ROSTERRA HR.csproj`, `Views/Dashboard/Index.cshtml`, `Views/EmployeeDashboard/Index.cshtml`, `Views/Shared/_Layout.cshtml`
