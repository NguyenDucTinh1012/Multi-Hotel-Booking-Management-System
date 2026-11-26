
-- =============================================
-- 1. CREATE DATABASE
-- =============================================

CREATE DATABASE AccountManagementDB;
GO

USE AccountManagementDB;
GO

-- =============================================
-- 2. CREATE TABLE ROLES
-- Quan ly vai tro nguoi dung
-- =============================================

CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL
);
GO

-- =============================================
-- 3. CREATE TABLE ACCOUNTS
-- Quan ly thong tin dang nhap
-- =============================================

CREATE TABLE Accounts (
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    RoleId INT NOT NULL,

    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    IsEmailVerified BIT NOT NULL DEFAULT 0,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT FK_Accounts_Roles
        FOREIGN KEY (RoleId)
        REFERENCES Roles(RoleId)
);

GO

ALTER TABLE Accounts
ADD VerificationTokenHash VARCHAR(255) NULL,
    VerificationTokenExpiry DATETIME2 NULL;

-- =============================================
-- 4. CREATE TABLE USERS
-- Quan ly thong tin ca nhan
-- =============================================

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT NOT NULL UNIQUE,

    FullName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NULL,
    Avatar NVARCHAR(500) NULL,
    Gender NVARCHAR(20) NULL,
    DateOfBirth DATE NULL,
    Address NVARCHAR(255) NULL,

    CONSTRAINT FK_Users_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId),

    CONSTRAINT CK_Users_Gender
        CHECK (
            Gender IS NULL
            OR Gender IN (N'Nam', N'Nữ', N'Khác')
        )
);
GO

-- =============================================
-- 5. CREATE TABLE PASSWORD RESET TOKENS
-- Quan ly ma dat lai mat khau
-- =============================================

CREATE TABLE PasswordResetTokens (
    TokenId BIGINT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT NOT NULL,

    TokenHash VARCHAR(255) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    UsedAt DATETIME2 NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_PasswordResetTokens_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId)
);
GO

-- =============================================
-- 6. CREATE INDEXES
-- =============================================

CREATE INDEX IX_Accounts_RoleId
ON Accounts(RoleId);

CREATE INDEX IX_PasswordResetTokens_AccountId
ON PasswordResetTokens(AccountId);
GO


-- =============================================
-- 7. INSERT DEFAULT ROLES
-- =============================================

INSERT INTO Roles (RoleName, Description)
VALUES
('Admin', N'Quản trị viên hệ thống'),
('Customer', N'Khách hàng'),
('HotelManager', N'Quản lý khách sạn');
GO

-- =============================================
-- 8. CHECK DATA
-- =============================================

SELECT * FROM Roles;

SELECT * FROM Accounts;

SELECT * FROM Users;

SELECT * FROM PasswordResetTokens;
GO
