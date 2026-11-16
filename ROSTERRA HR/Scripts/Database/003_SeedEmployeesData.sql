
USE RosterraHRDB;
GO

INSERT INTO Employees (FirstName, LastName, Email, PhoneNumber, Department, Position, Salary, HireDate)
VALUES 
('Alice', 'Johnson', 'alice.johnson@rosterra.local', '555-0101', 'Human Resources', 'HR Manager', 75000.00, '2023-01-15'),
('Bob', 'Smith', 'bob.smith@rosterra.local', '555-0102', 'Engineering', 'Senior Developer', 85000.00, '2023-03-01'),
('Charlie', 'Davis', 'charlie.davis@rosterra.local', '555-0103', 'Finance', 'Financial Analyst', 68000.00, '2023-06-10');
GO

SELECT EmployeeId, FirstName, LastName, Email, Department, Position, Salary, IsActive 
FROM Employees;
GO
