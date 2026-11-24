-- ============================================================================
-- Script: 001_CreateDatabase.sql
-- Project: Rosterra HR - Universal HR & Workforce Management System
-- Phase: 1 - Login and HR Dashboard
-- Description: Clean restart script to create fresh RosterraHRDB database.
-- Target: SQL Server LocalDB ((localdb)\MSSQLLocalDB)
-- ============================================================================

USE master;
GO

-- Safely terminate connections and drop only RosterraHRDB if it already exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'RosterraHRDB')
BEGIN
    ALTER DATABASE [RosterraHRDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [RosterraHRDB];
    PRINT 'Existing RosterraHRDB database dropped successfully.';
END
GO

-- Create fresh RosterraHRDB database
CREATE DATABASE [RosterraHRDB];
PRINT 'Fresh RosterraHRDB database created successfully.';
GO
