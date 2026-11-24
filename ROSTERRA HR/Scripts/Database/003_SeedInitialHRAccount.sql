-- ============================================================================
-- Script: 003_SeedInitialHRAccount.sql
-- Project: Rosterra HR - Universal HR & Workforce Management System
-- Phase: 1 - Login and HR Dashboard
-- Description: Seeds the initial HR administrator account.
-- Credentials:
--   Username: hradmin
--   Password: HR@2026
-- Hashing Algorithm: PBKDF2 with HMAC-SHA256, 100,000 iterations, 128-bit salt
-- ============================================================================

USE [RosterraHRDB];
GO

-- Prevent duplicate insertion if re-run
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'hradmin')
BEGIN
    INSERT INTO dbo.Users (
        Username,
        PasswordHash,
        PasswordSalt,
        Role,
        IsActive,
        CreatedAtUtc
    )
    VALUES (
        N'hradmin',
        N'DUZf1bpp/j3mRftRJg08IUcK6Lh2YOqv9edR80PISFI=',
        N'piUs8ZW6vD9TkoBiSO3X9A==',
        N'HR',
        1,
        SYSUTCDATETIME()
    );

    PRINT 'Initial HR administrator account (hradmin) seeded successfully.';
END
ELSE
BEGIN
    PRINT 'Account hradmin already exists. No action taken.';
END
GO

-- Verification query
SELECT UserId, Username, Role, IsActive, CreatedAtUtc, LastLoginAtUtc
FROM dbo.Users;
GO
