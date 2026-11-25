-- ============================================================================
-- Script: 006_SeedDefaultShifts.sql
-- Project: Rosterra HR - Universal HR & Workforce Management System
-- Feature: Shift Management & Attendance Tracking
-- Description: Seeds initial standard shifts and sets baseline shift assignments.
-- ============================================================================

USE [RosterraHRDB];
GO

-- Seed standard shifts if not already present
IF NOT EXISTS (SELECT 1 FROM dbo.Shifts WHERE ShiftName = N'Morning Shift')
BEGIN
    INSERT INTO dbo.Shifts (ShiftName, StartTime, EndTime, Description, IsActive)
    VALUES (N'Morning Shift', '08:00:00', '15:00:00', N'Standard morning shift (08:00 AM - 03:00 PM)', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Shifts WHERE ShiftName = N'General Shift')
BEGIN
    INSERT INTO dbo.Shifts (ShiftName, StartTime, EndTime, Description, IsActive)
    VALUES (N'General Shift', '09:00:00', '17:00:00', N'Corporate standard office hours (09:00 AM - 05:00 PM)', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Shifts WHERE ShiftName = N'Afternoon Shift')
BEGIN
    INSERT INTO dbo.Shifts (ShiftName, StartTime, EndTime, Description, IsActive)
    VALUES (N'Afternoon Shift', '13:00:00', '21:00:00', N'Afternoon / second operational shift (01:00 PM - 09:00 PM)', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Shifts WHERE ShiftName = N'Night Shift')
BEGIN
    INSERT INTO dbo.Shifts (ShiftName, StartTime, EndTime, Description, IsActive)
    VALUES (N'Night Shift', '21:00:00', '05:00:00', N'Overnight shift (09:00 PM - 05:00 AM)', 1);
END
GO

-- Assign Morning Shift as default for any existing employees without a shift
DECLARE @MorningShiftId INT = (SELECT TOP 1 ShiftId FROM dbo.Shifts WHERE ShiftName = N'Morning Shift');

UPDATE dbo.Employees
SET ShiftId = @MorningShiftId
WHERE ShiftId IS NULL;
GO

PRINT 'Default shifts seeded and unassigned employees mapped to Morning Shift successfully.';
GO

-- Verification query
SELECT ShiftId, ShiftName, StartTime, EndTime, Description, IsActive 
FROM dbo.Shifts;
GO
