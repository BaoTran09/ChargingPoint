-- Tạo DB
-- drop DATABASE ChargingPoint_Draft1;
CREATE DATABASE ChargingPoint_Draft;
GO
USE ChargingPoint;
GO
select * from [dbo].[Revenue_recognition];
alter table  charger add Note nvarchar(max) null; 


-- Bảng tài khoản
CREATE TABLE Account (
  Account_id  BIGINT IDENTITY PRIMARY KEY,
  Email VARCHAR(100) UNIQUE NOT NULL,
  PasswordHash VARCHAR(200) NOT NULL,           
  AccountType VARCHAR(50) NOT NULL,             
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),
  UpdatedAt DATETIME2(3) NULL,
  Status VARCHAR(50), 
  CONSTRAINT CK_Account_Type CHECK (AccountType IN ('guest','admin','customer'))
);
create table Department
(
Id int primary key not null,
Name nvarchar(max) null,
Branch nvarchar(200) null, -- HCM,HN,..

);
create table Role
(
Id int primary key not null,
Name nvarchar(max) null,
Level nvarchar(100) null,
SalaryBase decimal(10,2) null,


);
alter table Employee add StartingDate date null;
alter table Employee add RoleId int null;
alter table Employee add ExperienceMonth int null;
alter table Employee add CONSTRAINT FK_Employee_Role FOREIGN KEY (RoleId) REFERENCES Role(RoleId);

-- Bảng nhân viên
CREATE TABLE Employee (
  EmployeeId BIGINT IDENTITY PRIMARY KEY,
  FullName NVARCHAR(100) NOT NULL,
  PhoneNumber VARCHAR(20) NULL,
  Birthday Date NULL,
  Address NVARCHAR(255) NULL,
  JobTitle NVARCHAR(200),
  Account_id BIGINT NOT NULL,
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),
  UpdatedAt DATETIME2(3) NULL,
  Status NVARCHAR(200) null,
  CONSTRAINT FK_Employee_Account FOREIGN KEY (Account_id) REFERENCES Account(Account_id)
);
ALTER TABLE Charger
ADD CONSTRAINT DF_Charger_CreatedAt
DEFAULT (SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time') FOR CreatedAt;

select * from [dbo].[AspNetUsers];

-- Bảng khách hàng
CREATE TABLE Customer (
  CustomerID BIGINT IDENTITY PRIMARY KEY,
  FullName NVARCHAR(100) NOT NULL,
  PhoneNumber VARCHAR(20) NULL,
  Birthday Date NULL,
  Address NVARCHAR(255) NULL,
  NationalID VARCHAR(50) UNIQUE NULL,           
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),
  UpdatedAt DATETIME2(3) NULL,
  --Account_id BIGINT NOT NULL,
  --CONSTRAINT FK_Customer_Account FOREIGN KEY (Account_id) REFERENCES Account(Account_id)
);
ALTER TABLE Customer
ADD CONSTRAINT DF_Customer_CreatedAt
DEFAULT (SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time') FOR CreatedAt;
FirmwareVersion
select * from Customer;
update Customer set fullname = N'Nguyễn Hảo My' where CustomerID=2;
-- Bảng trạm sạc
CREATE TABLE Station (
  StationId BIGINT IDENTITY PRIMARY KEY, 
  Tag  NVARCHAR(200),							  
  Name NVARCHAR(200),                             
  StationType NVARCHAR(50),      --- trạm sạc ô tô, xe điện, cả 2                 
  Address NVARCHAR(500),						  
  Latitude DECIMAL(10,6) NULL,                    
  Longitude DECIMAL(10,6) NULL,                   
  Notes NVARCHAR(500),                            
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),
  BuiltDate DATE,                                 
 );
 ALTER TABLE Station
ADD CONSTRAINT DF_Station_CreatedAt
DEFAULT (SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time') FOR CreatedAt;
 ALTER TABLE Charger
ADD CONSTRAINT DF_Charger_CreatedAt
DEFAULT (SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time') FOR CreatedAt;
 select * from Station;
 -- Charger
CREATE TABLE Charger (
  ChargerId BIGINT IDENTITY PRIMARY KEY,          -- Mã định danh tự tăng duy nhất cho mỗi bộ sạc (charger)
  StationId BIGINT NOT NULL,                      -- Khóa ngoại liên kết đến trạm sạc (Station) mà charger này thuộc về
  SerialNumber NVARCHAR(100) UNIQUE,              -- Số sê-ri của thiết bị (độc nhất, dùng để nhận diện vật lý)
  Model NVARCHAR(100),                            -- Tên hoặc mã model của bộ sạc (ví dụ: ABB Terra 54)
  Manufacturer NVARCHAR(100),                     -- Hãng sản xuất (ví dụ: ABB, Siemens, VinFast)
  ChargerType NVARCHAR(50),                       -- Loại sạc: 'AC' (xoay chiều) hoặc 'DC' (một chiều)
  MaxPowerKW DECIMAL(8,2),                        -- Công suất tối đa (kW) mà bộ sạc có thể cung cấp
  Phases INT NULL,                                -- Số pha điện (1 pha hoặc 3 pha, thường chỉ áp dụng cho AC)
  OutputVoltageMin DECIMAL(6,2),                  -- Điện áp đầu ra nhỏ nhất (V)
  OutputVoltageMax DECIMAL(6,2),                  -- Điện áp đầu ra lớn nhất (V)
  PortCount INT,                                  -- Số cổng sạc (số lượng đầu nối vật lý trên charger)
  Design NVARCHAR(200),                           -- Kiểu thiết kế hoặc form factor (ví dụ: wall-mounted, pedestal)
  Protections NVARCHAR(500),                      -- Danh sách các tính năng bảo vệ (ví dụ: overcurrent, overheat)
  FirmwareVersion NVARCHAR(50),                   -- Phiên bản phần mềm điều khiển của thiết bị
  InstalledAt DATETIME2(3),                       -- Ngày/giờ thiết bị được lắp đặt tại trạm
  Status NVARCHAR(50) DEFAULT 'Active',           -- Trạng thái hiện tại: 'Active', 'PowerOff', 'Maintenance'...
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),-- Ngày/giờ bản ghi được tạo (theo UTC)
  PicturePath varchar(500),
  UseFor nvarchar(50),							  -- Trụ sạc xe ô tô hay xe máy
  CONSTRAINT FK_Charger_Station 
    FOREIGN KEY (StationId) REFERENCES Station(StationId)  -- Ràng buộc khóa ngoại tới bảng Station
);

-- Connector
CREATE TABLE Connector (
  ConnectorId BIGINT IDENTITY PRIMARY KEY,        -- Mã định danh duy nhất cho từng đầu nối (connector)
  ChargerId BIGINT NOT NULL,                      -- Khóa ngoại liên kết đến thiết bị sạc (Charger)
  ConnectorIndex INT NOT NULL,                    -- Số thứ tự của đầu nối trên cùng một charger (ví dụ: 1, 2, 3)
  ConnectorType NVARCHAR(50) NOT NULL,            -- Loại đầu nối (ví dụ: CCS2, Type2, CHAdeMO, Tesla)
--  MaxPowerKW DECIMAL(8,2) NULL,                   -- Công suất tối đa của riêng đầu nối này (nếu khác với charger)
  Status NVARCHAR(50) DEFAULT 'Available',        -- Tình trạng hiện tại: 'Available', 'InUse', 'Faulted'...
  CONSTRAINT FK_Connector_Charger 
    FOREIGN KEY (ChargerId) REFERENCES Charger(ChargerId) -- Liên kết đến thiết bị sạc chứa đầu nối này
);


-- Vehicle
-- =========================
-- Bảng Vehicle (Phương tiện)
-- Lưu thông tin xe của khách hàng, chỉ chấp nhận xe thuộc thương hiệu VinFast
-- =========================
CREATE TABLE Vehicle (
  VehicleId BIGINT IDENTITY PRIMARY KEY,             -- Mã định danh tự tăng của xe
  CustomerID BIGINT NOT NULL,                        -- Khóa ngoại tham chiếu khách hàng sở hữu xe
  VehicleType NVARCHAR(20) NOT NULL,                 -- Loại xe (xe máy điện, xe ô tô điện)
  Model NVARCHAR(100) NOT NULL,                      -- Tên/mẫu xe (VD: VF8, Klara S)
  ProductionDate DATE NULL,                          -- Ngày sản xuất xe
  BatteryType NVARCHAR(50) NULL,                     -- Loại pin (SDI, CATL,...)
  Version varchar(8) ,                               -- phiên bản như ECO, PLUS, 
  BatteryGrossKWh DECIMAL(9,3) NULL,                -- Dung lượng pin tổng (kWh)
  BatteryUsableKWh DECIMAL(9,3) NULL,                -- Dung lượng pin khả dụng (kWh)
  AcChargingSupport BIT DEFAULT 1,                   -- Hỗ trợ sạc AC (1: có, 0: không)
  DcChargingSupport BIT DEFAULT 1,                   -- Hỗ trợ sạc DC (1: có, 0: không)
  MaxAcChargeKW DECIMAL(6,2) NULL,                   -- Công suất sạc tối đa qua AC (kW)
  MaxDcChargeKW DECIMAL(6,2) NULL,                   -- Công suất sạc tối đa qua DC (kW)
  LicensePlate VARCHAR(30) UNIQUE NULL,              -- Biển số xe (duy nhất)
  VIN NVARCHAR(50) UNIQUE NULL,                      -- Số khung xe (Vehicle Identification Number)
 -- CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),   -- Thời điểm tạo bản ghi
  CONSTRAINT FK_Vehicle_Customer 
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
 /* 
  -- Ràng buộc: chỉ cho phép xe VinFast
  CONSTRAINT CK_Vehicle_VinFast 
    CHECK (
      Model LIKE 'VF%' 
      OR Model LIKE 'Feliz%' 
      OR Model LIKE 'Klara%' 
      OR Model LIKE 'Theon%'
    )*/
);
ALTER TABLE Vehicle
ADD Manufacturer NVARCHAR(100)  NULL DEFAULT N'VinFast';

-- =========================
-- Bảng ChargingSession (Phiên sạc)
-- Ghi nhận thông tin chi tiết của mỗi lần sạc giữa xe và trạm
-- =========================
CREATE TABLE ChargingSession (
  SessionId BIGINT IDENTITY PRIMARY KEY,             -- Mã định danh tự tăng cho phiên sạc
  ConnectorId BIGINT NOT NULL,                       -- Khóa ngoại tham chiếu đầu sạc (connector)
  VehicleId BIGINT NULL,                             -- Xe được sạc (tham chiếu bảng Vehicle)
  StartTime DATETIME2(3) NULL,                       -- Thời gian bắt đầu sạc

  ExpectTime DATETIME2(3) NULL,
  PowerOffTime DATETIME2(3) NULL,
  EndTime DATETIME2(3) NULL,                         -- Thời gian kết thúc sạc
  MeterStartKWh DECIMAL(10,4) NULL,                  -- Chỉ số điện năng trước khi sạc (kWh)
  MeterStopKWh DECIMAL(10,4) NULL,                   -- Chỉ số điện năng sau khi sạc (kWh)
  EnergyDeliveredKWh AS (MeterStopKWh - MeterStartKWh) PERSISTED, -- Điện năng tiêu thụ (kWh)
  StartSOC DECIMAL(5,2) NULL,                        -- SOC (State of Charge) khi bắt đầu (%)
  EndSOC DECIMAL(5,2) NULL,                          -- SOC khi kết thúc (%)
  TargetSOC DECIMAL(5,2) NULL default 80,            -- SOC mục tiêu mong muốn (%)/ mặc định là 80% nếu không điền 
  Status NVARCHAR(50) DEFAULT 'Charging',            -- Trạng thái phiên sạc (Charging, Completed, Error, Rejected.v.)
  LastUpdated DATETIME2(3) DEFAULT SYSUTCDATETIME(), -- Lần cập nhật cuối của bản ghi
  OverDueTime AS (DATEDIFF(MINUTE, PowerOffTime,EndTime )), -- Số phút trễ từ lúc kết thúc sạc đến khi cập nhật
  
  
  
  CONSTRAINT FK_Session_Connector 
    FOREIGN KEY (ConnectorId) REFERENCES Connector(ConnectorId),
  CONSTRAINT FK_Session_Vehicle 
    FOREIGN KEY (VehicleId) REFERENCES Vehicle(VehicleId)
);
ALTER TABLE Vehicle
ADD TargetSOC DECIMAL(5,2) NULL;                     -- SOC mục tiêu mong muốn (%)
ALTER TABLE ChargingSession
ADD ExpectTime DATETIME2(3) NULL;						-- Thời gian dự kiến xong
ALTER TABLE ChargingSession
ADD PowerOffTime DATETIME2(3) NULL;						-- Thời gian ngắt điện 
ALTER TABLE ChargingSession
ADD OverDueTime AS (DATEDIFF(MINUTE, PowerOffTime,EndTime )); -- Số phút trễ từ lúc kết thúc sạc đến khi cập nhật
 ALTER TABLE ChargingSession
ADD CONSTRAINT DF_ChargingSession_LastUpdated
DEFAULT (SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time') FOR LastUpdated;
 ALTER TABLE ChargingSession
ADD CONSTRAINT DF_ChargingSession_StartTime
DEFAULT (SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time') FOR StartTime;

-- Invoice
CREATE TABLE Invoice (
  InvoiceId varchar(20) PRIMARY KEY,          
  SessionId BIGINT NOT NULL,                      
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),
  
  TotalEnergyKWh DECIMAL(10,4),  -- tổng năng lượng truyền từ ChargingSession(EnergyDeliveredKWh)              
  UnitPrice DECIMAL(10,2) NOT NULL,		--- 3.85VND/Kwh	  
  ExtraFee DECIMAL(10,2) NULL, 
  IdleUnitPrice DECIMAL(10,2) DEFAULT 1000 NULL, 
  OverDueMinutes INT NULL, 
  
  -- Tính phí idle dựa trên phút trễ
  Total_IdleFee AS (IdleUnitPrice * ISNULL(OverDueMinutes,0)), 
  
  -- Tính chi phí điện
  EnergyCost AS (TotalEnergyKWh * UnitPrice) PERSISTED,  
  
  -- Các phí bổ sung
  IdleFee DECIMAL(12,2),                          
  ExtraFees DECIMAL(12,2),                        
  
  Tax DECIMAL(5,2) DEFAULT 10.00,			
  
  -- Tổng chi phí (điện + idle + extra)
  TotalCost AS (
      (TotalEnergyKWh * UnitPrice) 
      + ISNULL(IdleFee,0) 
      + ISNULL(ExtraFee,0)
  ) PERSISTED, 
  
  -- FinalCost phải viết lại công thức gốc, không tham chiếu TotalCost (vi phạm đạt chuẩn csdl)
  FinalCost AS (
      ((TotalEnergyKWh * UnitPrice) + ISNULL(IdleFee,0) + ISNULL(ExtraFee,0))
      + (((TotalEnergyKWh * UnitPrice) + ISNULL(IdleFee,0) + ISNULL(ExtraFee,0)) * ISNULL(Tax,0) / 100)
  ) PERSISTED,
  
  PaymentLink VARCHAR(500),  
  EmployeeId INT,
  Expire_Date DATE, --- ngày 15 tháng sau
  Status NVARCHAR(50) DEFAULT 'unpaid',           
  
  CONSTRAINT FK_Invoice_Session FOREIGN KEY (SessionId) REFERENCES ChargingSession(SessionId)
);

CREATE TABLE SendEmail(
send_id varchar(20) PRIMARY KEY, --- 
InvoiceId varchar(20),
CustomerID int,
Send_time datetime,
subjects nvarchar(200),
contents nvarchar (max),

);
/*
-- Sequence
CREATE SEQUENCE InvoiceSeq
    START WITH 1
    INCREMENT BY 1;

-- Trigger tạo InvoiceId
IF OBJECT_ID('trg_GenInvoiceID', 'TR') IS NOT NULL
    DROP TRIGGER trg_GenInvoiceID;
GO
CREATE TRIGGER trg_GenInvoiceID
ON Invoice
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Invoice (
        InvoiceId, SessionId, TotalEnergyKWh, UnitPrice, ExtraFee, IdleUnitPrice, 
        OverDueMinutes, EmployeeId, Expire_Date, Status, PaymentLink
    )
    SELECT 
        'HD' + RIGHT('000000' + CAST(NEXT VALUE FOR InvoiceSeq AS VARCHAR(6)), 6),
        SessionId, TotalEnergyKWh, UnitPrice, ExtraFee, IdleUnitPrice, 
        OverDueMinutes, EmployeeId, Expire_Date, Status, PaymentLink
    FROM inserted;
END;
GO
*/
select 
-- Payment
CREATE TABLE Payment(
  PaymentID BIGINT IDENTITY PRIMARY KEY, 
  TransID varchar(20),
  InvoiceID varchar(20),
  Update_Time datetime
);

-- Transactions
CREATE TABLE Transactions(
  TransID varchar(20) PRIMARY KEY,
  Trans_Date datetime,
  From_Bank_Code int,
  From_Bank_Name varchar(50),
  From_Acc_Name varchar(50),
  To_Bank_Code int,
  To_Bank_Name varchar(50),
  To_Acc_Name varchar(50),
  Transaction_Code int,
  Trans_Type VARCHAR(50) CHECK (Trans_Type IN ('received', 'sent')), 
  Amount float,
  ReferenceID int,
  URL varchar(100)
);

-- LogChange
CREATE TABLE LogChange (
  LogId BIGINT IDENTITY PRIMARY KEY,              
  TableName NVARCHAR(50),                         
  ChangeType NVARCHAR(50),                        
  RecordId BIGINT,                                
  ColumnName NVARCHAR(50),                        
  OldValue NVARCHAR(MAX),                         
  NewValue NVARCHAR(MAX),                         
  ChangedUser NVARCHAR(50),                       
  ChangedDate DATETIME2(3) DEFAULT SYSUTCDATETIME()
);

-- Chart of Accounts (COA)
CREATE TABLE COA(
  Acc_Code2 int PRIMARY KEY, 
  Acc_Code1 int NULL, 
  Acc_Name nvarchar(100),
  Acc_Type nvarchar(100)
);

INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (111, NULL, N'Tiền mặt');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (1111,111, N'Tiền Việt Nam');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (1112,111, N'Ngoại Tệ');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (1113,111, N'Vàng tiền tệ');

-- Journal Entries
CREATE TABLE Journal_Entries(
  entry_id BIGINT IDENTITY PRIMARY KEY, 
  entry_type varchar(50), 
  Entry_Date date,
  Debit_Acc int,
  Credit_Acc int,
  Amount float,
  Descriptions nvarchar(200),
  document_type VARCHAR(50),  
  document_id varchar(20),

  CONSTRAINT CK_Journal_DocumentType 
  CHECK (document_type IN ('Invoice', 'Transaction', 'Receipt', 'MaintenanceContract'))
);
/*
-- Trigger kiểm tra chứng từ hợp lệ
CREATE TRIGGER trg_ValidateJournalDocument
ON Journal_Entries
AFTER INSERT, UPDATE
AS
BEGIN
    DECLARE @document_type NVARCHAR(50), @document_id varchar(20);
    SELECT TOP 1 @document_type = document_type, @document_id = document_id FROM inserted;

    IF @document_type = 'Invoice'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM Invoice WHERE InvoiceId = @document_id)
            THROW 50001, 'Invalid Invoice ID for Journal Entry.', 1;
    END
    ELSE IF @document_type = 'Transaction'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM Transactions WHERE TransID = @document_id)
            THROW 50002, 'Invalid Transaction ID for Journal Entry.', 1;
    END
END;
GO
*/
-- Opening Balance
CREATE TABLE Opening_Balance(
  Period int,
  Year int,
  Opening_Date date,
  Acc_Code2 int,
  Type nvarchar(20),
  Balance float
);

-- Ending Balance
CREATE TABLE Ending_Balance(
  Period int,
  Year int,
  Ending_Date date,
  Acc_Code2 int,
  Type nvarchar(20),
  Balance float
);
update [dbo].[AspNetUsers] set Username ='Admin' where email='lebaotran.forwork@gmail.com';
select * from [AspNetUsers];
select * from employee;
---------------THÊM DỮ LIỆU
---việc đầu tiên khi bắt đầu sạc là hệ thống sẽ lưu thông tin của xe và chủ xe trước, sau đó mới kiểm tra các điều kiện sạc khác
use [ChargingPoint];

select * from Vehicle;
INSERT INTO Vehicle (
  CustomerID, VehicleType, Model, ProductionDate, BatteryType, Version,
  BatteryGrossKWh, BatteryUsableKWh, AcChargingSupport, DcChargingSupport,
  MaxAcChargeKW, MaxDcChargeKW, LicensePlate, VIN,[Manufacturer]
)
VALUES
(2, N'Ô tô điện', N'VF8', '2024-03-15', 'CATL', 'PLUS',
  90.000, 82.000, 1, 1, 11.00, 150.00, '60A-99999', 'VF8PLUSCATL20240315', 'Toyota'),

(2, N'Xe máy điện', N'Klara S', '2023-09-10', 'LFP', 'ECO',
  3.500, 3.200, 1, 0, 1.20, NULL, '59X2-12345', 'KLARAS20230910LFP','Vinfast');
  select * from Vehicle;
  INSERT INTO Charger
(StationId, SerialNumber, Model, Manufacturer, ChargerType, MaxPowerKW, Phases, 
 OutputVoltageMin, OutputVoltageMax, PortCount, Design, Protections, 
 FirmwareVersion, InstalledAt, Status, CreatedAt, PicturePath, UseFor)
VALUES
(24, 'SN-CHG001', 'Wallbox Pulsar Plus', 'Wallbox', 'AC', 7.4, 1, 220, 240, 1,
 N'Tường treo', N'Quá áp, quá dòng, ngắn mạch', 'v1.2.0',
 GETDATE(), 'Online', SYSUTCDATETIME(), 'https://vgreen.net/themes/custom/vgreen/images/charging-car-1.png', N'Xe máy'),

(24, 'SN-CHG002', 'ABB Terra 54', 'ABB', 'DC', 50, 3, 200, 500, 2,
 N'Đứng sàn', N'Quá nhiệt, chống sét', 'v3.1.5',
 GETDATE(), 'Offline', SYSUTCDATETIME(), 'https://vgreen.net/themes/custom/vgreen/images/charging-car-1.png', N'Ô tô'),

(24, 'SN-CHG003', 'Delta AC Mini Plus', 'Delta', 'AC', 22, 3, 220, 400, 1,
 N'Tường treo', N'Quá dòng, bảo vệ chạm đất', 'v2.0.1',
 GETDATE(), 'Online', SYSUTCDATETIME(), 'https://vgreen.net/themes/custom/vgreen/images/charging-car-1.png', N'Ô tô');
 select * from Connector;

  INSERT INTO Connector (ChargerId, ConnectorIndex, ConnectorType, Status)
VALUES
(5, 1, 'CCS2', 'InUsed'),
(6, 2, 'CHAdeMO', 'Available'),
(7, 1, 'Type2', 'InUsed');

update connector set status ='InUsed' where ConnectorId=1,2;
--- thêm các trigger cập nhật trạng thái như chargingsession của connector có status "Charging" => connector có status "InUsed"
select * from ChargingSession;
INSERT INTO ChargingSession (
  ConnectorId, VehicleId, StartTime, EndTime,
  MeterStartKWh, MeterStopKWh, StartSOC, EndSOC, TargetSOC, Status, FullTime
)
VALUES
(4, 2, '2025-10-09 08:00:00', '2025-10-09 10:30:00',
  12500.5000, 12570.7500, 20.00, 85.00, 90.00, 'Charging', '2025-10-09 10:20:00'),

(5, 3, '2025-10-09 11:00:00', '2025-10-09 12:15:00',
  532.2500, 535.8500, 35.00, 100.00, 100.00, 'Charging', '2025-10-09 12:10:00');
  ALTER TABLE ChargingSession
ADD CONSTRAINT DF_ChargingSession_LastUpdated DEFAULT SYSUTCDATETIME() FOR [LastUpdated];
-- Customer (khách hàng)
INSERT INTO Customer (FullName, PhoneNumber, Birthday, Address, NationalID)
VALUES
(N'Lê Bảo Trân', '0987654321', '2002-11-04', N'45 Trần Hưng Đạo, Biên Hòa, Đồng Nai', '079202110433')
select * from customer;

ALTER TABLE Customer
ADD CONSTRAINT DF_Customer_CreatedAt DEFAULT SYSUTCDATETIME() FOR CreatedAt;
-- Kiểm tra session nào không có connector
SELECT * FROM ChargingSession WHERE ConnectorId NOT IN (SELECT ConnectorId FROM Connector);

-- Kiểm tra connector nào không có charger
SELECT * FROM Connector WHERE ChargerId NOT IN (SELECT ChargerId FROM Charger);

-- Kiểm tra charger nào không có station
SELECT * FROM Charger WHERE StationId NOT IN (SELECT StationId FROM Station);
select * from station

select n.name from connector r  join charger c on r.chargerid=c.ChargerId join station n on c.StationId=n.StationId where r.ConnectorId=4;
DECLARE @VehicleId BIGINT = 4;      -- Xe đang sạc
DECLARE @ConnectorId BIGINT = 5;    -- Đầu nối đang sử dụng
DECLARE @BatteryGrossKWh DECIMAL(9,3);
DECLARE @StartSOC DECIMAL(5,2) = 20.0;   -- Pin hiện tại (%)
DECLARE @TargetSOC DECIMAL(5,2) = 80.0;  -- Pin mục tiêu (%)
DECLARE @PowerKW DECIMAL(6,2) = 11.00;   -- Công suất sạc tối đa (kW)
DECLARE @EnergyToCharge DECIMAL(9,3);
DECLARE @ChargeTimeSeconds DECIMAL(12,2);
DECLARE @StartTime DATETIME2(3) = SYSDATETIME();
DECLARE @ExpectTime DATETIME2(3);

-- 1️⃣ Lấy dung lượng pin tổng từ Vehicle
SELECT @BatteryGrossKWh = BatteryGrossKWh
FROM Vehicle
WHERE VehicleId = @VehicleId;

-- 2️⃣ Dung lượng pin cần sạc (U)
SET @EnergyToCharge = ((@TargetSOC - @StartSOC) / 100) * @BatteryGrossKWh;

-- 3️⃣ Thời gian sạc (giây)
SET @ChargeTimeSeconds = (@EnergyToCharge / @PowerKW) * 3600;

-- 4️⃣ Thời gian dự kiến hoàn thành
SET @ExpectTime = DATEADD(SECOND, @ChargeTimeSeconds, @StartTime);

-- 5️⃣ Thực hiện thêm bản ghi phiên sạc
INSERT INTO ChargingSession
(
    ConnectorId,
    VehicleId,
    StartTime,
    ExpectTime,
    MeterStartKWh,
    MeterStopKWh,
    StartSOC,
    TargetSOC,
    Status,
    LastUpdated
)
VALUES
(
    @ConnectorId,            -- Đầu nối đang sạc
    @VehicleId,              -- Xe đang sạc
    @StartTime,              -- Giờ bắt đầu (giờ VN)
    @ExpectTime,             -- Dự kiến kết thúc
    125.50,                  -- Chỉ số đầu công tơ
    NULL,                    -- Chưa kết thúc
    @StartSOC,               -- SOC ban đầu
    @TargetSOC,              -- SOC mục tiêu
    'Requiring',              -- Trạng thái
    SYSDATETIME()            -- Cập nhật lần cuối
);

-- (Tuỳ chọn) Xem lại bản ghi vừa chèn
SELECT * FROM ChargingSession ORDER BY SessionId DESC;


-- 6️⃣ Xem kết quả
SELECT TOP 1 
    SessionId, ConnectorId, VehicleId, StartTime, ExpectTime, 
    DATEDIFF(MINUTE, StartTime, ExpectTime) AS EstimatedMinutes,
    Status
FROM ChargingSession
ORDER BY SessionId DESC;


-- 6️⃣ Xem lại kết quả
SELECT TOP 1 * 
FROM ChargingSession
ORDER BY SessionId DESC;
use  Chargingpoint;
SELECT name AS TableName
FROM sys.tables
ORDER BY name;
select * from [dbo].[AspNetUsers];
select * from Employee;
EXEC sp_help 'Employee';
DELETE FROM [dbo].[AspNetRoles];

INSERT INTO [dbo].[AspNetRoles] (Id, Name, NormalizedName)
VALUES
(NEWID(), 'Admin', 'ADMIN'),
(NEWID(), 'Customer', 'CUSTOMER'),
(NEWID(), 'Employee', 'EMPLOYEE');

insert into [dbo].[AspNetUserRoles] ([UserId],[RoleId]) select     Id,2 FROM AspNetUsers WHERE Email = 'bao.tran.intern@gmail.com'; 

INSERT INTO Employee (FullName, JobTitle, UserId, CreatedAt)
SELECT 
    N'Lê Bảo Trân', 
    'Manager', 
    Id, 
    SYSDATETIME()
FROM AspNetUsers
WHERE Email = 'lebaotran.forwork@gmail.com';
update [dbo].[AspNetUsers] set EmailConfirmed=1 where email='bao.tran.intern@gmail.com';
select * from [AspNetUsers] u join [dbo].[AspNetUserRoles] j on u.id=j.[UserId] join  [dbo].[AspNetRoles] r on j.roleid=r.id;
delete from [dbo].[AspNetUserRoles] where UserId = (select id from [AspNetUsers]where email= 'tranne2k4@gmail.com');
delete from [dbo].[Customer] where UserId = (select id from [AspNetUsers]where email= 'tranne2k4@gmail.com');

delete from[AspNetUsers] where email='tranne2k4@gmail.com';
update [dbo].[AspNetUsers] set status='Success' where email='tranne2k4@gmail.com';

select * from [dbo].[AspNetRoles];
select * from [dbo].[AspNetUserRoles];
INSERT INTO [dbo].[AspNetUserRoles] (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'bao.tran.intern@gmail.com'
  AND r.Name = 'Admin';
delete from[AspNetUsers] where email='tranne2k4@gmail.com';