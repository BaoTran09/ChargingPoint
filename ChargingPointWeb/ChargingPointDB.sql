create database ChargingPoint
go
use   ChargingPoint;
go
create table Department(

id BIGINT IDENTITY PRIMARY KEY,  
name nvarchar(200), --tên cơ sở, danh mục cha là cơ các cái chi nhánh của công ty theo 3 miền Nam,  Trung, Bắc // còn danh mục con thì là phòng kỹ thuật,...
address nvarchar(300), -- null nếu là phòng ban 
parent_id int, ---nối lại với mã danh mục cha
);
create table Account 
(
  id  BIGINT IDENTITY PRIMARY KEY, 
  Password   VARCHAR(100) ,							  ---hash
  Email VARCHAR(100),                             -- 
  SDT NVARCHAR(50),                       -- 
  Address NVARCHAR(500),						  --
  Name DECIMAL(10,6) NULL,                    --họ tên nv
  Position NVARCHAR(100),
  department_id int,
  CONSTRAINT FK_Account_Department FOREIGN KEY (department_id) REFERENCES Department(id)
);
select * from Station;
delete from Station where StationId <=2;
 INSERT INTO Station (Tag, Name, StationType, Address, Latitude, Longitude, Notes, CreatedAt, BuiltDate)
VALUES 
    (N'TTTM', N'Trạm Sạc Vincom Mega Mall', N'Public', N'72A Nguyễn Trãi, Thanh Xuân, Hà Nội', 21.0045, 105.8132, N'Gần trung tâm thương mại Vincom', SYSUTCDATETIME(), '2023-06-15'),
    (N'Khách sạn', N'Trạm Sạc Sofitel Legend', N'Public', N'15 Ngô Quyền, Hoàn Kiếm, Hà Nội', 21.0314, 105.8557, N'Dành cho khách sạn và khách vãng lai', SYSUTCDATETIME(), '2022-11-20'),
    (N'Bãi đỗ xe', N'Trạm Sạc Cầu Giấy', N'Depot', N'176 Cầu Giấy, Cầu Giấy, Hà Nội', 21.0338, 105.7991, N'Bãi đỗ xe công cộng', SYSUTCDATETIME(), '2024-01-10'),
    (N'Siêu thị', N'Trạm Sạc Big C Thăng Long', N'Public', N'222 Trần Duy Hưng, Cầu Giấy, Hà Nội', 21.0132, 105.7905, N'Gần siêu thị Big C', SYSUTCDATETIME(), '2023-09-05'),
    (N'TTTM', N'Trạm Sạc AEON Mall Long Biên', N'Public', N'27 Cổ Linh, Long Biên, Hà Nội', 21.0537, 105.8936, N'Trong khu vực TTTM AEON', SYSUTCDATETIME(), '2023-12-01'),
    (N'Công viên', N'Trạm Sạc Công viên Thống Nhất', N'Public', N'354A Lê Duẩn, Đống Đa, Hà Nội', 21.0179, 105.8412, N'Gần cổng công viên', SYSUTCDATETIME(), '2022-08-30'),
    (N'Bãi đỗ xe', N'Trạm Sạc Royal City', N'Depot', N'72 Nguyễn Trãi, Thanh Xuân, Hà Nội', 21.0028, 105.8139, N'Bãi đỗ xe tầng hầm', SYSUTCDATETIME(), '2023-03-22'),
    (N'Trường học', N'Trạm Sạc ĐH Bách Khoa', N'Public', N'1 Đại Cồ Việt, Hai Bà Trưng, Hà Nội', 21.0051, 105.8465, N'Dành cho sinh viên và giảng viên', SYSUTCDATETIME(), '2024-02-15'),
    (N'Bệnh viện', N'Trạm Sạc BV Bạch Mai', N'Public', N'78 Giải Phóng, Đống Đa, Hà Nội', 21.0009, 105.8401, N'Gần cổng bệnh viện', SYSUTCDATETIME(), '2023-07-10'),
    (N'TTTM', N'Trạm Sạc Lotte Center', N'Public', N'54 Liễu Giai, Ba Đình, Hà Nội', 21.0325, 105.8135, N'Tầng hầm Lotte Center', SYSUTCDATETIME(), '2022-12-05'),
    (N'TTTM', N'Trạm Sạc Vincom Bà Triệu', N'Public', N'191 Bà Triệu, Hai Bà Trưng, Hà Nội', 21.0136, 105.8498, N'Gần khu vực gửi xe', SYSUTCDATETIME(), '2023-04-20'),
    (N'Siêu thị', N'Trạm Sạc Co.opmart Hà Đông', N'Public', N'Khu đô thị mới Hà Đông, Hà Đông, Hà Nội', 20.9745, 105.7702, N'Khu vực siêu thị', SYSUTCDATETIME(), '2024-03-01'),
    (N'Khách sạn', N'Trạm Sạc InterContinental', N'Public', N'5 Từ Hoa, Tây Hồ, Hà Nội', 21.0578, 105.8291, N'Dành cho khách sạn và khách vãng lai', SYSUTCDATETIME(), '2023-10-15'),
    (N'TTTM', N'Trạm Sạc Vincom Thủ Đức', N'Public', N'216 Võ Văn Ngân, Thủ Đức, TP.HCM', 10.8501, 106.7718, N'Gần khu TTTM Vincom', SYSUTCDATETIME(), '2023-05-12'),
    (N'Bãi đỗ xe', N'Trạm Sạc Quận 7', N'Depot', N'15 Nguyễn Lương Bằng, Quận 7, TP.HCM', 10.7298, 106.7205, N'Bãi đỗ xe công cộng', SYSUTCDATETIME(), '2024-01-25'),
    (N'Siêu thị', N'Trạm Sạc Big C Trường Chinh', N'Public', N'1 Trường Chinh, Tân Bình, TP.HCM', 10.7992, 106.6567, N'Gần siêu thị Big C', SYSUTCDATETIME(), '2023-08-10'),
    (N'Công viên', N'Trạm Sạc Công viên Tao Đàn', N'Public', N'55C Trương Định, Quận 3, TP.HCM', 10.7745, 106.6912, N'Gần lối vào công viên', SYSUTCDATETIME(), '2022-09-15'),
    (N'Bệnh viện', N'Trạm Sạc BV Chợ Rẫy', N'Public', N'201B Nguyễn Chí Thanh, Quận 5, TP.HCM', 10.7563, 106.6689, N'Khu vực gửi xe bệnh viện', SYSUTCDATETIME(), '2023-11-20'),
    (N'TTTM', N'Trạm Sạc Takashimaya', N'Public', N'92 Nam Kỳ Khởi Nghĩa, Quận 1, TP.HCM', 10.7739, 106.7008, N'Tầng hầm TTTM', SYSUTCDATETIME(), '2023-06-30'),
    (N'Bãi đỗ xe', N'Trạm Sạc Đà Nẵng', N'Depot', N'38 Nguyễn Văn Linh, Hải Châu, Đà Nẵng', 16.0598, 108.2097, N'Bãi đỗ xe công cộng', SYSUTCDATETIME(), '2024-02-20');


drop TABLE Station;
CREATE TABLE Station (
StationId BIGINT IDENTITY PRIMARY KEY, 
  Tag  NVARCHAR(200) ,							  ---Nhãn, mô tả nhanh (VD: "Trung tâm thương mại")
  Name NVARCHAR(200),                             -- Tên trạm
  StationType NVARCHAR(50),                       -- Loại trạm: "Public", "Depot"
  Address NVARCHAR(500),						  -- địa chỉ
  Latitude DECIMAL(10,6) NULL,                    -- Vĩ độ
  Longitude DECIMAL(10,6) NULL,                   -- Kinh độ
  Notes NVARCHAR(500),                            -- Ghi chú thêm
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),-- Ngày tạo
  BuiltDate DATE,                                 -- Ngày xây dựng

);

CREATE TABLE Charger (
  ChargerId BIGINT IDENTITY PRIMARY KEY,          -- Khóa chính, định danh duy nhất trụ sạc
  StationId BIGINT NOT NULL,                      -- FK tới bảng Station, trụ thuộc trạm nào
  SerialNumber NVARCHAR(100) UNIQUE,              -- Số serial duy nhất của trụ sạc
  Model NVARCHAR(100),                             -- Mẫu/kiểu trụ sạc
  Manufacturer NVARCHAR(100),                      -- Nhà sản xuất
  ChargerType NVARCHAR(50),                        -- Loại trụ: "AC" hoặc "DC"
  MaxPowerKW DECIMAL(8,2),                        -- Công suất tối đa (kW)
  Phases INT NULL,                                 -- Số pha điện (1 pha hoặc 3 pha)
  OutputVoltageMin DECIMAL(6,2),                  -- Điện áp đầu ra tối thiểu (V)
  OutputVoltageMax DECIMAL(6,2),                  -- Điện áp đầu ra tối đa (V)
  PortCount INT,                                  -- Số cổng sạc trên trụ
  Design NVARCHAR(200),                            -- Kiểu dáng/trang trí trụ sạc
  Protections NVARCHAR(500),                       -- Các tính năng bảo vệ (quá dòng, quá nhiệt, sét,...)
  FirmwareVersion NVARCHAR(50),                   -- Phiên bản firmware
  InstalledAt DATETIME2(3),                       -- Ngày lắp đặt trụ
  Status NVARCHAR(50) DEFAULT 'Online',           -- Trạng thái hiện tại: Online/Offline
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),-- Ngày tạo bản ghi
  CONSTRAINT FK_Charger_Station FOREIGN KEY (StationId) REFERENCES Station(StationId)
);

-- Connector (cổng trên trụ)
CREATE TABLE Connector (
  ConnectorId BIGINT IDENTITY PRIMARY KEY,        -- Khóa chính
  ChargerId BIGINT NOT NULL,                      -- FK tới bảng Charger
  ConnectorIndex INT NOT NULL,                    -- Thứ tự cổng (1,2,...)
  ConnectorType NVARCHAR(50) NOT NULL,            -- Loại cổng: "Type2", "CCS2", "CHAdeMO"
  MaxPowerKW DECIMAL(8,2) NULL,                   -- Công suất tối đa của cổng
  Status NVARCHAR(50) DEFAULT 'Available',        -- Trạng thái: Available/Occupied/Error
--  CurrentTransactionId BIGINT NULL,               -- ID phiên giao dịch đang chạy (nếu có)
  CONSTRAINT FK_Connector_Charger FOREIGN KEY (ChargerId) REFERENCES Charger(ChargerId)
);

-- Vehicle
CREATE TABLE Vehicle (
  VehicleId BIGINT IDENTITY PRIMARY KEY,          -- Khóa chính
  OwnerID BIGINT NOT NULL,                        -- FK tới bảng Owner
  VehicleType NVARCHAR(20) NOT NULL,              -- Loại xe: 'Ô tô' / 'Xe máy'
  Model NVARCHAR(100) NOT NULL,                   -- Tên xe (VD: VF e34, Feliz S)
  ProductionDate DATE NULL,                       -- Ngày sản xuất
  BatteryType NVARCHAR(50) NULL,                  -- Loại pin (LFP, NMC,...)
  BatteryUsableKWh DECIMAL(9,3) NULL,             -- Dung lượng pin khả dụng (kWh)
  MaxAcChargeKW DECIMAL(6,2) NULL,                -- Công suất sạc AC tối đa
  AcChargingSupport BIT DEFAULT 1,                -- Hỗ trợ sạc AC (0/1)
  DcChargingSupport BIT DEFAULT 1,                -- Hỗ trợ sạc DC (0/1)
  MaxDcChargeKW DECIMAL(6,2) NULL,                -- Công suất sạc DC tối đa
  LicensePlate VARCHAR(30) UNIQUE NULL,           -- Biển số xe
  VIN NVARCHAR(50) UNIQUE NULL,                   -- Số khung xe (VIN)
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),-- Ngày tạo
  CONSTRAINT FK_Vehicle_Owner FOREIGN KEY (OwnerID) REFERENCES Owner(OwnerID),
  CONSTRAINT CK_Vehicle_VinFast CHECK (Model LIKE 'VF%' OR Model LIKE 'Feliz%' OR Model LIKE 'Klara%' OR Model LIKE 'Theon%')
);

CREATE TABLE Owner (
  OwnerID BIGINT IDENTITY PRIMARY KEY,            -- Khóa chính
  FullName NVARCHAR(100) NOT NULL,                -- Họ tên
  PhoneNumber VARCHAR(20) NULL,                   -- Số điện thoại
  Email VARCHAR(100) NULL UNIQUE,                 -- Email (unique)
  Address NVARCHAR(255) NULL,                     -- Địa chỉ
  NationalID VARCHAR(50) NULL UNIQUE,             -- CCCD/Hộ chiếu
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),-- Ngày tạo
  UpdatedAt DATETIME2(3) NULL                     -- Ngày cập nhật gần nhất
);


-- Charging Session
CREATE TABLE ChargingSession (
  SessionId BIGINT IDENTITY PRIMARY KEY,          -- Khóa chính
  ConnectorId BIGINT NOT NULL,                    -- FK tới Connector
  VehicleId BIGINT NULL,                          -- FK tới Vehicle
  StartTime DATETIME2(3) NULL,                    -- Thời gian bắt đầu
  EndTime DATETIME2(3) NULL,                      -- Thời gian kết thúc
  OverDueTime AS (DATEDIFF(MINUTE, EndTime, LastUpdated)), -- Phút quá hạn sau khi pin đầy
  MeterStartKWh DECIMAL(10,4) NULL,               -- Số điện (kWh) lúc bắt đầu
  MeterStopKWh DECIMAL(10,4) NULL,                -- Số điện (kWh) lúc kết thúc
  EnergyDeliveredKWh AS (MeterStopKWh - MeterStartKWh) PERSISTED, -- Năng lượng đã nạp
  StartSOC DECIMAL(5,2) NULL,                     -- % pin lúc bắt đầu
  EndSOC DECIMAL(5,2) NULL,                       -- % pin lúc kết thúc (thường nếu không có trục trặc thì pin kết thúc sẽ = pin mục tiêu)
  TargetSOC DECIMAL(5,2) NULL,                    -- % pin mục tiêu
  Status NVARCHAR(50) DEFAULT 'Charging',         -- Trạng thái phiên sạc: Charging/charged/
  FullTime DATETIME2(3)  ,							-- thời gian sạc đầy đến TargetSOC 
  LastUpdated DATETIME2(3) DEFAULT SYSUTCDATETIME(), -- Cập nhật cuối
  CONSTRAINT FK_Session_Connector FOREIGN KEY (ConnectorId) REFERENCES Connector(ConnectorId),
  CONSTRAINT FK_Session_Vehicle FOREIGN KEY (VehicleId) REFERENCES Vehicle(VehicleId)
);
/*
-- Tariff / Pricing rule
CREATE TABLE PriceHistory (
  Id BIGINT IDENTITY PRIMARY KEY,                 -- Khóa chính
  StartDate DATE,                                 -- Ngày áp dụng
  EndDate DATE,                                   -- Ngày hết hạn (NULL = hiện hành)
  Price DECIMAL(9,3)                              -- Giá (VNĐ/kWh)
); */
-- Invoice
CREATE TABLE Invoice (
  InvoiceId BIGINT IDENTITY PRIMARY KEY,          -- Khóa chính
  SessionId BIGINT NOT NULL,                      -- FK tới ChargingSession
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),-- Ngày tạo hóa đơn
  EnergyKWh DECIMAL(10,4),                        -- Số điện đã sạc
  EnergyCost DECIMAL(12,2),                       -- Tiền điện
  IdleFee DECIMAL(12,2),                          -- Phí phạt chờ quá lâu
  ExtraFees DECIMAL(12,2),                        -- Phí bổ sung khác
  TotalAmount DECIMAL(12,2),                      -- Tổng cộng
  Currency NVARCHAR(10) DEFAULT 'VND',            -- Loại tiền
  Status NVARCHAR(50) DEFAULT 'unpaid',           -- unpaid / paid / overdue
  CONSTRAINT FK_Invoice_Session FOREIGN KEY (SessionId) REFERENCES ChargingSession(SessionId)
);

-- Payment
CREATE TABLE Payment (
    PaymentID BIGINT IDENTITY PRIMARY KEY, -- Khóa chính
    InvoiceId BIGINT NOT NULL, -- FK đến ChargingSession
    UnitPrice DECIMAL(10,2) NOT NULL, -- Đơn giá sạc (VNĐ/kWh)
    OverDueMinutes INT NULL, -- Số phút vượt
    IdleFee DECIMAL(10,2) NULL as (IdleUnitPrice*OverDueMinutes)  , -- Phí sạc lâu
    ExtraFee DECIMAL(10,2) NULL  , -- Phí phát sinh
    IdleUnitPrice DECIMAL(10,2) default 1000 NULL, -- Đơn giá phí sạc lâu
    TotalEnergyKWh DECIMAL(10,3) NULL, -- Tổng năng lượng đã sạc (MeterStopKWh - MeterStartKWh)
    TotalCost AS ((TotalEnergyKWh * UnitPrice) + ISNULL(IdleFee,0) + ISNULL(ExtraFee,0)) PERSISTED, -- Tổng tiền
    CONSTRAINT FK_Payment_Session FOREIGN KEY (InvoiceId) REFERENCES Invoice(InvoiceId)
);

-- Activity Log (audit)
CREATE TABLE LogChange (
  LogId BIGINT IDENTITY PRIMARY KEY,              -- Khóa chính
  TableName NVARCHAR(50),                         -- Tên bảng bị thay đổi
  ChangeType NVARCHAR(50),                        -- Loại thay đổi: thêm, xóa, sửa
  RecordId BIGINT,                                -- ID bản ghi bị thay đổi
  ColumnName NVARCHAR(50),                        -- Tên cột bị thay đổi
  OldValue NVARCHAR(MAX),                         -- Giá trị cũ
  NewValue NVARCHAR(MAX),                         -- Giá trị mới
  ChangedUser NVARCHAR(50),                       -- Người thay đổi
  ChangedDate DATETIME2(3) DEFAULT SYSUTCDATETIME() -- Thời gian thay đổi
);

