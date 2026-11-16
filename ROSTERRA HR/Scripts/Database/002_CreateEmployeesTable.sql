
USE RosterraHRDB;
GO

CREATE TABLE Employees (
    EmployeeId    INT IDENTITY(1,1) NOT NULL,
    FirstName     NVARCHAR(50) NOT NULL,
    LastName      NVARCHAR(50)  NOT NULL,
    Email         NVARCHAR(100)  NOT NULL,
    PhoneNumber   NVARCHAR(20) NULL,
    Department    NVARCHAR(50) NOT NULL,
    Position      NVARCHAR(50)  NOT NULL,
    Salary        DECIMAL(18,2)  NOT NULL,
    HireDate      DATE    NOT NULL,
    IsActive      BIT  NOT NULL CONSTRAINT DF_Employees_IsActive DEFAULT (1),
    CreatedAtUtc  DATETIME2 NOT NULL CONSTRAINT DF_Employees_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc  DATETIME2  NULL,

    CONSTRAINT PK_Employees PRIMARY KEY CLUSTERED (EmployeeId),
    CONSTRAINT UQ_Employees_Email UNIQUE (Email),
    CONSTRAINT CK_Employees_Salary CHECK (Salary >= 0)
);
GO
