-- ============================================================================
-- Script: 004_CreateEmployeesTable.sql
-- Project: Rosterra HR - Universal HR & Workforce Management System
-- Phase: 2 - Employee CRUD & Account Linking
-- Description: Creates the Employees table linked to the Users table.
-- ============================================================================

USE [RosterraHRDB];
GO

-- Drop table if it already exists (idempotency)
IF OBJECT_ID(N'dbo.Employees', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Employees;
    PRINT 'Existing dbo.Employees table dropped.';
END
GO

CREATE TABLE dbo.Employees (
    EmployeeId    INT IDENTITY(1,1) NOT NULL,
    UserId        INT               NOT NULL,
    FirstName     NVARCHAR(50)      NOT NULL,
    LastName      NVARCHAR(50)      NOT NULL,
    Email         NVARCHAR(100)     NOT NULL,
    PhoneNumber   NVARCHAR(20)      NULL,
    Department    NVARCHAR(50)      NOT NULL,
    Position      NVARCHAR(50)      NOT NULL,
    Salary        DECIMAL(18,2)     NOT NULL,
    HireDate      DATE              NOT NULL,
    IsActive      BIT               NOT NULL CONSTRAINT DF_Employees_IsActive DEFAULT (1),
    CreatedAtUtc  DATETIME2         NOT NULL CONSTRAINT DF_Employees_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc  DATETIME2         NULL,

    -- Constraints
    CONSTRAINT PK_Employees PRIMARY KEY CLUSTERED (EmployeeId),
    CONSTRAINT FK_Employees_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE,
    CONSTRAINT UQ_Employees_Email UNIQUE (Email),
    CONSTRAINT UQ_Employees_UserId UNIQUE (UserId),
    CONSTRAINT CK_Employees_Salary CHECK (Salary >= 0)
);
GO

PRINT 'dbo.Employees table created successfully with foreign key to dbo.Users.';
GO
