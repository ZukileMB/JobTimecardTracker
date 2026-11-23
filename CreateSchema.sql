IF DB_ID('JobTimecardTrackerDb') IS NULL
BEGIN
    CREATE DATABASE JobTimecardTrackerDb;
END
GO

USE JobTimecardTrackerDb;
GO

IF OBJECT_ID('dbo.Timecards', 'U') IS NOT NULL DROP TABLE dbo.Timecards;
IF OBJECT_ID('dbo.Jobs', 'U') IS NOT NULL DROP TABLE dbo.Jobs;
GO

CREATE TABLE dbo.Jobs
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    JobNumber NVARCHAR(50) NOT NULL UNIQUE,
    JobType INT NOT NULL CHECK (JobType IN (1, 2, 3)), -- 1=Repair, 2=Support, 3=Warranty
    JobDate DATE NOT NULL,
    CompanyName NVARCHAR(200) NOT NULL,
    ContactName NVARCHAR(150) NOT NULL,
    TelephoneNumber NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE dbo.Timecards
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    Employee NVARCHAR(150) NOT NULL,
    DateWorked DATE NOT NULL,
    HoursWorked DECIMAL(5, 2) NOT NULL CHECK (HoursWorked > 0 AND HoursWorked <= 24),
    JobId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_Timecards_Jobs FOREIGN KEY (JobId) REFERENCES dbo.Jobs(Id) ON DELETE CASCADE
);
GO

-- Supports looking up multiple jobs worked by an employee on a given day
CREATE INDEX IX_Timecards_Employee_DateWorked ON dbo.Timecards (Employee, DateWorked);
GO