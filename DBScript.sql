-- Create Database
CREATE DATABASE CorporationSecurity;
GO

-- Use the BinaryRisk database
USE CorporationSecurity;
GO

-- Create Role table
CREATE TABLE Role (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL UNIQUE
);

-- Create User table
CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    RoleId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    LastLogin DATETIME,
    FOREIGN KEY (RoleId) REFERENCES Role(Id)
);

-- Create Category table (for assets)
CREATE TABLE Category (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

-- Create RiskCategory table (for risks)
CREATE TABLE RiskCategory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

-- Create Asset table
CREATE TABLE Asset (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CategoryId INT NOT NULL,
    Description NVARCHAR(500),
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Category(Id),
    FOREIGN KEY (CreatedBy) REFERENCES [User](Id)
);

-- Create Risk table
CREATE TABLE Risk (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AssetId INT NOT NULL,
    RiskCategoryId INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Likelihood FLOAT NOT NULL CHECK (Likelihood >= 0 AND Likelihood <= 1),
    Impact FLOAT NOT NULL CHECK (Impact >= 0 AND Impact <= 1),
    Mitigation NVARCHAR(500),
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (AssetId) REFERENCES Asset(Id),
    FOREIGN KEY (RiskCategoryId) REFERENCES RiskCategory(Id),
    FOREIGN KEY (CreatedBy) REFERENCES [User](Id)
);

-- Create Control table
CREATE TABLE Control (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RiskId INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Effectiveness NVARCHAR(20) NOT NULL CHECK (Effectiveness IN ('Pass', 'Fail', 'Pending')),
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RiskId) REFERENCES Risk(Id),
    FOREIGN KEY (CreatedBy) REFERENCES [User](Id)
);

-- Create AuditLog table
CREATE TABLE AuditLog (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Action NVARCHAR(200) NOT NULL,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES [User](Id)
);

-- Insert roles
INSERT INTO Role (Name) VALUES 
('System Administrator'),
('Risk Manager'),
('Operations Staff');

-- Insert users (PasswordHash is a placeholder; use bcrypt in app)
INSERT INTO [User] (Username, PasswordHash, FirstName, LastName, Email, RoleId, IsActive) VALUES 
('admin', 'admin', 'John', 'Smith', 'john.smith@company.com', 1, 1),
('ESanders', 'hashed_password_2', 'Emma', 'Sanders', 'emma.sanders@company.com', 2, 1),
('MLee', 'hashed_password_3', 'Michael', 'Lee', 'michael.lee@company.com', 3, 1),
('ALopez', 'hashed_password_4', 'Anna', 'Lopez', 'anna.lopez@company.com', 2, 1),
('DWilson', 'hashed_password_5', 'David', 'Wilson', 'david.wilson@company.com', 3, 0),
('SChen', 'hashed_password_6', 'Sarah', 'Chen', 'sarah.chen@company.com', 1, 1);

-- Insert asset categories
INSERT INTO Category (Name) VALUES 
('IT'),
('Finance'),
('Operations'),
('HR'),
('Marketing');

-- Insert risk categories
INSERT INTO RiskCategory (Name) VALUES 
('Cybersecurity'),
('Financial'),
('Operational'),
('Compliance'),
('Reputational');

-- Insert assets
INSERT INTO Asset (Name, CategoryId, Description, CreatedBy) VALUES 
('Cloud Migration Project', 1, 'Migration to AWS cloud infrastructure', 1),
('ERP System Upgrade', 2, 'Upgrade to SAP S/4HANA', 1),
('Production Line A', 3, 'Main factory production line', 2),
('Employee Onboarding Portal', 4, 'HR system for new hires', 2),
('Data Warehouse', 1, 'Centralized data storage solution', 1),
('Payroll System', 2, 'Automated payroll processing', 3),
('Marketing Campaign Platform', 5, 'Digital marketing automation tool', 4);

-- Insert risks
INSERT INTO Risk (AssetId, RiskCategoryId, Description, Likelihood, Impact, Mitigation, CreatedBy) VALUES 
(1, 1, 'Data breach during cloud migration', 0.3, 0.8, 'Implement encryption and MFA', 2),
(1, 3, 'Service downtime during migration', 0.5, 0.6, 'Schedule off-peak migration', 2),
(2, 2, 'System compatibility issues', 0.4, 0.7, 'Conduct pre-upgrade testing', 1),
(3, 3, 'Equipment failure', 0.6, 0.5, 'Regular maintenance checks', 3),
(4, 1, 'Unauthorized access to portal', 0.2, 0.9, 'Role-based access controls', 2),
(5, 1, 'Data corruption', 0.3, 0.8, 'Daily backups and validation', 1),
(6, 2, 'Payroll calculation errors', 0.4, 0.6, 'Automated error checking', 3),
(7, 5, 'Negative campaign feedback', 0.3, 0.7, 'Monitor social media sentiment', 4),
(7, 4, 'Non-compliance with ad regulations', 0.2, 0.8, 'Legal review of campaign content', 4);

-- Insert controls
INSERT INTO Control (RiskId, Description, Effectiveness, CreatedBy) VALUES 
(1, 'Two-factor authentication', 'Pass', 2),
(1, 'Firewall configuration', 'Pending', 2),
(2, 'Backup scheduling', 'Pass', 2),
(3, 'Compatibility testing suite', 'Pass', 1),
(4, 'Preventive maintenance', 'Pass', 3),
(5, 'Access control policies', 'Pending', 2),
(6, 'Backup validation process', 'Pass', 1),
(7, 'Error detection algorithm', 'Fail', 3),
(8, 'Sentiment analysis tool', 'Pending', 4),
(9, 'Compliance checklist', 'Pass', 4);

-- Insert audit logs
INSERT INTO AuditLog (UserId, Action, Timestamp) VALUES 
(1, 'Created Asset: Cloud Migration Project', '2025-07-13 09:00:12'),
(2, 'Added Risk: Data breach during cloud migration', '2025-07-13 09:15:30'),
(3, 'Viewed Asset: Production Line A', '2025-07-13 09:30:45'),
(1, 'Updated Asset: ERP System Upgrade', '2025-07-13 10:00:00'),
(2, 'Added Control: Two-factor authentication', '2025-07-13 10:10:22'),
(3, 'Viewed Risk: Payroll calculation errors', '2025-07-13 10:20:15'),
(4, 'Updated Risk: Unauthorized access to portal', '2025-07-13 10:30:00'),
(1, 'Deactivated User: DWilson', '2025-07-13 11:00:00'),
(4, 'Created Asset: Marketing Campaign Platform', '2025-07-13 11:15:00'),
(6, 'Logged in', '2025-07-13 11:30:00');