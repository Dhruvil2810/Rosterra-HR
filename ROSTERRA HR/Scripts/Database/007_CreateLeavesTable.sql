-- ============================================================================
-- Script: 007_CreateLeavesTable.sql
-- Description: Creates the dbo.Leaves table for Employee Leave Management.
-- Includes StartDate, EndDate, TotalDays (excluding Sundays), Status (Pending, Approved, Rejected),
-- Employee FK, Decision-maker (HR) FK, Reason, and Admin Remarks.
-- ============================================================================

USE [RosterraHRDB];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Leaves' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Leaves (
        LeaveId INT IDENTITY(1,1) NOT NULL,
        EmployeeId INT NOT NULL,
        LeaveType NVARCHAR(50) NOT NULL DEFAULT 'Casual Leave', -- 'Casual Leave', 'Sick Leave', 'Paid Leave', 'Unpaid Leave'
        StartDate DATE NOT NULL,
        EndDate DATE NOT NULL,
        TotalDays INT NOT NULL, -- Total working days requested (automatically excludes Sundays)
        Reason NVARCHAR(500) NOT NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- 'Pending', 'Approved', 'Rejected'
        AdminRemarks NVARCHAR(250) NULL,
        DecidedByUserId INT NULL,
        DecidedAtUtc DATETIME2(7) NULL,
        CreatedAtUtc DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc DATETIME2(7) NULL,

        CONSTRAINT PK_Leaves PRIMARY KEY CLUSTERED (LeaveId ASC),
        CONSTRAINT FK_Leaves_Employees FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(EmployeeId) ON DELETE CASCADE,
        CONSTRAINT FK_Leaves_Users_DecidedBy FOREIGN KEY (DecidedByUserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT CK_Leaves_DateOrder CHECK (EndDate >= StartDate),
        CONSTRAINT CK_Leaves_TotalDays CHECK (TotalDays > 0),
        CONSTRAINT CK_Leaves_Status CHECK (Status IN ('Pending', 'Approved', 'Rejected'))
    );

    -- Index for fast lookup by employee and date overlap checks
    CREATE NONCLUSTERED INDEX IX_Leaves_Employee_Dates ON dbo.Leaves (EmployeeId, StartDate, EndDate, Status);
    CREATE NONCLUSTERED INDEX IX_Leaves_Status ON dbo.Leaves (Status);

    PRINT 'Table dbo.Leaves created successfully.';
END
ELSE
BEGIN
    PRINT 'Table dbo.Leaves already exists.';
END
GO
