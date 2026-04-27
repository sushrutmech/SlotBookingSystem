
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FirstName NVARCHAR(100),
    MiddleName NVARCHAR(100),
    LastName NVARCHAR(100),
    Email NVARCHAR(150) UNIQUE,
    MobileNo NVARCHAR(20),
    PasswordHash NVARCHAR(MAX),
    DateOfBirth DATETIME,
    Gender NVARCHAR(20)
);

CREATE TABLE Roles (
    Id INT PRIMARY KEY,
    Name NVARCHAR(50)
);

CREATE TABLE UserRoles (
    UserId UNIQUEIDENTIFIER,
    RoleId INT,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE Facilities (
    Id INT PRIMARY KEY,
    Name NVARCHAR(100)
);

CREATE TABLE BookingStatusMaster (
    Id INT PRIMARY KEY,
    Name NVARCHAR(50)
);


CREATE TABLE SlotBookings (
    Id UNIQUEIDENTIFIER PRIMARY KEY,

    UserId UNIQUEIDENTIFIER NOT NULL,
    FacilityId INT NOT NULL,
    StatusId INT NOT NULL,

    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,

    CreatedAt DATETIME DEFAULT GETUTCDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (FacilityId) REFERENCES Facilities(Id),
    FOREIGN KEY (StatusId) REFERENCES BookingStatusMaster(Id)
);


--INSERT INTO Roles (Id, Name) VALUES
--(1, 'User'),
--(2, 'Admin'),
--(3, 'StaffPerson');


--INSERT INTO Facilities (Id, Name) VALUES
--(1, 'Cricket Turf'),
--(2, 'Swimming Pool'),
--(3, 'Cricket Coaching');

--INSERT INTO BookingStatusMaster (Id, Name) VALUES
--(1, 'Pending'),
--(2, 'Booked'),
--(3, 'Cancelled');


--INSERT INTO Users (
--    Id, FirstName, LastName, Email, MobileNo, PasswordHash, DateOfBirth, Gender
--)
--VALUES (
--    NEWID(), 'Test', 'User', 'test@mail.com', '9999999999',
--    'TEST_HASH', GETDATE(), 'Male'
--);