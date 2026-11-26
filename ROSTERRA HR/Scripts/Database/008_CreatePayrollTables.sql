-- ============================================================================
-- Script: 008_CreatePayrollTables.sql
-- Description: Creates dbo.SalaryStructures and dbo.Payrolls tables.
-- Stores monthly salary configurations (Basic, Allowances, Deductions) per employee
-- and tracks monthly generated payroll records with attendance-based loss of pay (LOP).
-- ============================================================================

USE [RosterraHRDB];
GO

-- 1. Table: dbo.SalaryStructures
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalaryStructures' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.SalaryStructures (
        SalaryStructureId INT IDENTITY(1,1) NOT NULL,
        EmployeeId INT NOT NULL,
        BasicSalary DECIMAL(18,2) NOT NULL DEFAULT 0.00,
        Allowances DECIMAL(18,2) NOT NULL DEFAULT 0.00, -- HRA, Travel, Special
        Deductions DECIMAL(18,2) NOT NULL DEFAULT 0.00, -- Fixed monthly deductions (Tax, PF)
        CreatedAtUtc DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc DATETIME2(7) NULL,

        CONSTRAINT PK_SalaryStructures PRIMARY KEY CLUSTERED (SalaryStructureId ASC),
        CONSTRAINT UQ_SalaryStructures_Employee UNIQUE (EmployeeId),
        CONSTRAINT FK_SalaryStructures_Employees FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(EmployeeId) ON DELETE CASCADE,
        CONSTRAINT CK_SalaryStructures_Basic CHECK (BasicSalary >= 0),
        CONSTRAINT CK_SalaryStructures_Allowances CHECK (Allowances >= 0),
        CONSTRAINT CK_SalaryStructures_Deductions CHECK (Deductions >= 0)
    );

    PRINT 'Table dbo.SalaryStructures created successfully.';
END
GO

-- 2. Table: dbo.Payrolls
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payrolls' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Payrolls (
        PayrollId INT IDENTITY(1,1) NOT NULL,
        EmployeeId INT NOT NULL,
        Month INT NOT NULL,        -- 1 to 12
        Year INT NOT NULL,         -- e.g. 2026
        TotalWorkingDays INT NOT NULL, -- Total working days in month (excluding Sundays)
        PresentDays INT NOT NULL,      -- Punched present or on approved paid leave
        AbsentDays INT NOT NULL,       -- Unpaid leave or unexplained absence
        BasicSalary DECIMAL(18,2) NOT NULL,
        Allowances DECIMAL(18,2) NOT NULL,
        GrossSalary DECIMAL(18,2) NOT NULL,
        StandardDeductions DECIMAL(18,2) NOT NULL,
        LossOfPay DECIMAL(18,2) NOT NULL, -- (Gross / TotalWorkingDays) * AbsentDays
        NetSalary DECIMAL(18,2) NOT NULL,  -- Gross - (StandardDeductions + LossOfPay)
        Status NVARCHAR(20) NOT NULL DEFAULT 'Generated', -- 'Generated', 'Paid'
        PaymentDate DATE NULL,
        GeneratedByUserId INT NULL,
        CreatedAtUtc DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc DATETIME2(7) NULL,

        CONSTRAINT PK_Payrolls PRIMARY KEY CLUSTERED (PayrollId ASC),
        CONSTRAINT UQ_Payrolls_Employee_Month_Year UNIQUE (EmployeeId, Month, Year),
        CONSTRAINT FK_Payrolls_Employees FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(EmployeeId) ON DELETE CASCADE,
        CONSTRAINT FK_Payrolls_Users_GeneratedBy FOREIGN KEY (GeneratedByUserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT CK_Payrolls_Month CHECK (Month BETWEEN 1 AND 12),
        CONSTRAINT CK_Payrolls_Year CHECK (Year >= 2020),
        CONSTRAINT CK_Payrolls_Status CHECK (Status IN ('Generated', 'Paid'))
    );

    CREATE NONCLUSTERED INDEX IX_Payrolls_Month_Year ON dbo.Payrolls (Year, Month);
    CREATE NONCLUSTERED INDEX IX_Payrolls_Employee ON dbo.Payrolls (EmployeeId);

    PRINT 'Table dbo.Payrolls created successfully.';
END
GO

-- 3. Seed default salary structures for existing employees if not present
INSERT INTO dbo.SalaryStructures (EmployeeId, BasicSalary, Allowances, Deductions)
SELECT e.EmployeeId, 35000.00, 5000.00, 1500.00
FROM dbo.Employees e
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.SalaryStructures s WHERE s.EmployeeId = e.EmployeeId
);
GO
