-- ============================================================================
-- Script: 005_CreateShiftsAndAttendanceTables.sql
-- Project: Rosterra HR - Universal HR & Workforce Management System
-- Feature: Shift Management & Attendance Tracking
-- Description: Creates Shifts and Attendances tables, links Shifts to Employees.
-- ============================================================================

USE [RosterraHRDB];
GO

-- 1. Create Shifts Table
IF OBJECT_ID(N'dbo.Shifts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Shifts (
        ShiftId        INT IDENTITY(1,1) NOT NULL,
        ShiftName      NVARCHAR(50)      NOT NULL,
        StartTime      TIME(0)           NOT NULL,
        EndTime        TIME(0)           NOT NULL,
        Description    NVARCHAR(250)     NULL,
        IsActive       BIT               NOT NULL CONSTRAINT DF_Shifts_IsActive DEFAULT (1),
        CreatedAtUtc   DATETIME2         NOT NULL CONSTRAINT DF_Shifts_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Shifts PRIMARY KEY CLUSTERED (ShiftId),
        CONSTRAINT UQ_Shifts_ShiftName UNIQUE (ShiftName)
    );
    PRINT 'dbo.Shifts table created successfully.';
END
GO

-- 2. Add ShiftId foreign key to Employees table if not present
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'dbo.Employees') AND name = N'ShiftId'
)
BEGIN
    ALTER TABLE dbo.Employees ADD ShiftId INT NULL;
    ALTER TABLE dbo.Employees ADD CONSTRAINT FK_Employees_Shifts 
        FOREIGN KEY (ShiftId) REFERENCES dbo.Shifts(ShiftId);
    PRINT 'ShiftId column and foreign key added to dbo.Employees.';
END
GO

-- 3. Create Attendances Table
IF OBJECT_ID(N'dbo.Attendances', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Attendances (
        AttendanceId            INT IDENTITY(1,1) NOT NULL,
        EmployeeId              INT               NOT NULL,
        ShiftId                 INT               NOT NULL,
        AttendanceDate          DATE              NOT NULL,
        PunchInTime             DATETIME2         NULL,
        PunchOutTime            DATETIME2         NULL,
        Status                  NVARCHAR(20)      NOT NULL,
        ShiftStartTimeSnapshot  TIME(0)           NOT NULL,
        ShiftEndTimeSnapshot    TIME(0)           NOT NULL,
        TotalHours              DECIMAL(5,2)      NULL,
        Remarks                 NVARCHAR(250)     NULL,
        MarkedByUserId          INT               NOT NULL,
        CreatedAtUtc            DATETIME2         NOT NULL CONSTRAINT DF_Attendances_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        UpdatedAtUtc            DATETIME2         NULL,

        CONSTRAINT PK_Attendances PRIMARY KEY CLUSTERED (AttendanceId),
        CONSTRAINT FK_Attendances_Employees FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(EmployeeId),
        CONSTRAINT FK_Attendances_Shifts FOREIGN KEY (ShiftId) REFERENCES dbo.Shifts(ShiftId),
        CONSTRAINT FK_Attendances_Users FOREIGN KEY (MarkedByUserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT UQ_Attendances_Employee_Date UNIQUE (EmployeeId, AttendanceDate),
        CONSTRAINT CK_Attendances_Status CHECK (Status IN ('Present', 'Late', 'Absent', 'Pending'))
    );
    PRINT 'dbo.Attendances table created successfully.';
END
GO
