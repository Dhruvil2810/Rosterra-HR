-- ============================================================================
-- Script: 002_CreateUsersTable.sql
-- Project: Rosterra HR - Universal HR & Workforce Management System
-- Phase: 1 - Login and HR Dashboard
-- Description: Creates the minimal Users table for authentication and RBAC.
-- ============================================================================

USE [RosterraHRDB];
GO

-- Drop table if it already exists (idempotency)
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Users;
    PRINT 'Existing dbo.Users table dropped.';
END
GO

CREATE TABLE dbo.Users (
    UserId          INT IDENTITY(1,1) NOT NULL,
    Username        NVARCHAR(50)      NOT NULL,
    PasswordHash    NVARCHAR(256)     NOT NULL,
    PasswordSalt    NVARCHAR(256)     NOT NULL,
    Role            NVARCHAR(20)      NOT NULL CONSTRAINT DF_Users_Role DEFAULT ('HR'),
    IsActive        BIT               NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedAtUtc    DATETIME2         NOT NULL CONSTRAINT DF_Users_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    LastLoginAtUtc  DATETIME2         NULL,

    -- Constraints
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('HR', 'Employee'))
);
GO

PRINT 'dbo.Users table created successfully.';
GO
