USE ChargingPoint;
GO
drop table Report;
drop table ReportType;

drop table JournalEntries;
select * from ReportType;
create table ReportType
(
ReportTypeCode varchar(50) PRIMARY KEY, -- B01a-DN, B02a-DN, B03a-DN, B04a-DN
ReportTypeName varchar(max) null,
ReportCategory  varchar(50) null, --Category, FINANCIAL, JOURNAL, LEDGER, MANAGEMENT
EntryType varchar(20), -- SUMMARY	bút toán gộp (G01-DN) / DETAIL	bút toán chi tiết (G02-DN)
/*
G01-DN: Sổ nhật ký chung (General Journal) bản gộp bút toán
B01a-DN: Bảng cân đối kế toán giữa niên độ 
B02a-DN: Báo cáo kết quả hoạt động kinh doanh giữa niên độ 
B03a-DN: Báo cáo lưu chuyển tiền tệ giữa niên độ (Dạng đầy đủ - PP trực tiếp)
B03a-DN-GT: Báo cáo lưu chuyển tiền tệ giữa niên độ (Dạng đầy đủ - PP gián tiếp)
*/
);
alter table Report add Version int null;
create table  Report
(
id varchar(100) PRIMARY KEY , -- ReportTypeCode+năm+tháng
Title nvarchar(max) null,
ReportTypeCode varchar(50) null, -- B01a-DN, B02a-DN, B03a-DN, B04a-DN
IsClosed bit default 0,      -- đã khóa sổ hay chưa
CreatedByRole nvarchar(max) null,
CreatedBy nvarchar(max) null,       -- ai tạo report viết tên + chọn role  rồi sẽ truy vấn lại về bảng Employee để kiếm id của người đó rồi insert fk EmployeeId
CreatedDate datetime default getdate(),
Period nvarchar(100) null, --quý 1,2,3,4/ tháng 1,2,3..../ Từ đầu năm đến hiện tại
FromDate date null,
ToDate date null,
EmployeeId bigint null,
Notes  nvarchar(max),
    CONSTRAINT FK_Report_Employee FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId),
	CONSTRAINT FK_Report_ReportType FOREIGN KEY (ReportTypeCode) REFERENCES ReportType(ReportTypeCode),


);
alter table Report add CreatedByRole nvarchar(max) null;
/* Journal Entries: các chứng từ như hóa đơn, phiếu thu (trước mắt là vậy, mốt sẽ có thêm về mua hàng,...) sau khi được hạch toán ở 
bảng [dbo].[Revenue_recognition] và [dbo].[Record_detail] mới có thể ghi vào nhật ký chung, chứng từ đối chiếu sẽ là mã hóa đơn, hoặc phiếu thu,.. 
không phải là RecordId nha. Hạch toán Tổng hợp (Gộp bút toán)
---*/
CREATE TABLE JournalEntries(

  STT  BIGINT PRIMARY KEY, --tăng dần từ 1
  ReportId  varchar(100) not null,
  RecordDate date null, -- ngày ghi sổ
  DocumentDate date null, -- ngày chứng từ

  DocumentNumber nvarchar(50) null, -- số chứng từ đối chiếu
  Interpretation nvarchar(max) null,
  -- diễn giải: vì ghi gộp bút toán của 1 chứng từ nên sẽ là công thức chung như: Ghi nhận công nợ tổng hợp Hóa đơn HD250001,... phiếu thu,...

  InvoiceId bigint null,
  ReceiptId bigint null,
  RecordId bigint null,
  RecordDetail_STT int null,

  Account varchar(10) null, 
  CorrespondingAccount varchar(10) null, --- tài khoản đối ứng

  DebitAmount decimal(10,2) null,
  CrebitAmount decimal(10,2) null,


    CONSTRAINT FK_JournalEntries_Report FOREIGN KEY (ReportId) REFERENCES Report(Id),
  CONSTRAINT FK_JournalEntries_Record_detail FOREIGN KEY (RecordDetail_STT,RecordId) REFERENCES Record_detail(STT, Recognition_id),
  CONSTRAINT FK_JournalEntries_COA_Account  FOREIGN KEY (Account) REFERENCES COA(Acc_Code2),
  CONSTRAINT FK_JournalEntries_COA_CorrespondingAccount FOREIGN KEY (CorrespondingAccount) REFERENCES COA(Acc_Code2),

  
);
alter table JournalEntries add constraint PK_JournalEntries primary key (ReportId,STT);
select * from [dbo].[Record_detail];
UPDATE Invoices
SET 
   Snashot_CustomerEmail = 'old.garment.22@gmail.com'
    
WHERE [InvoiceNumber] =20250001;
		select * from [dbo].[Revenue_recognition];
select * from [dbo].[Record_detail];
UPDATE Invoices
SET 
    snashot_CustomerEmail = 'tranne2k4@gmail.com'
    
WHERE InvoiceNumber = 20250001;
alter table invoices ;
exec sp_rename '[dbo].[invoices].[Signature]', 'SignatureFile','Column';
	select * from invoices a join invoicedetail b on a.InvoiceId=b.InvoiceId;
		select * from invoices;
		SELECT * FROM Invoices WHERE InvoiceId = 2
SELECT * FROM [dbo].[COA] ;

CREATE TABLE Transactions(
  Id varchar(100) PRIMARY KEY,
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
  Content varchar(max),
  URL varchar(100),
  InvoiceId bigint null,
      CONSTRAINT FK_Transactions_Invoices  FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId)

);
select * from invoices;



select * from transactions;
 select * from chargingsession a join connector c on a.ConnectorId=c.ConnectorId;
-- ***************************************************************
-- 1. Xóa các Khóa Ngoại hiện có (Như bạn đã cung cấp)
-- ***************************************************************

-- Connector table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Connector_Charger')
    ALTER TABLE Connector DROP CONSTRAINT FK_Connector_Charger;

-- ChargingSession table
-- Connector table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChargingSession_Connector')
    ALTER TABLE ChargingSession DROP CONSTRAINT FK_ChargingSession_Connector;

-- Transactions table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Transactions_ChargingSession')
    ALTER TABLE Transactions DROP CONSTRAINT FK_Transactions_ChargingSession;

-- Invoices table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Invoices_ChargingSession_SessionId')
    ALTER TABLE Invoices DROP CONSTRAINT FK_Invoices_ChargingSession_SessionId;


	IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Revenue_recognition_Invoices')
    ALTER TABLE Revenue_recognition DROP CONSTRAINT FK_Revenue_recognition_Invoices;

	-- 1. Xóa tất cả các ràng buộc khóa ngoại liên quan
-- Đầu tiên xóa các ràng buộc từ các bảng con xa nhất
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_InvoiceDetail_Invoices')
    ALTER TABLE InvoiceDetail DROP CONSTRAINT FK_InvoiceDetail_Invoices;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Revenue_recognition_InvoiceDetail')
    ALTER TABLE Revenue_recognition DROP CONSTRAINT FK_Revenue_recognition_InvoiceDetail;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Record_detail_Revenue_recognition')
    ALTER TABLE Record_detail DROP CONSTRAINT FK_Record_detail_Revenue_recognition;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ReceiptDetail_Receipt')
    ALTER TABLE ReceiptDetail DROP CONSTRAINT FK_ReceiptDetail_Receipt;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Receipt_Invoices')
    ALTER TABLE Receipt DROP CONSTRAINT FK_Receipt_Invoices;




IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Invoices_ChargingSession')
    ALTER TABLE Invoices DROP CONSTRAINT FK_Invoices_ChargingSession;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChargingSession_Connector')
    ALTER TABLE ChargingSession DROP CONSTRAINT FK_ChargingSession_Connector;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Connector_Charger_ChargerId')
    ALTER TABLE Connector DROP CONSTRAINT FK_Connector_Charger_ChargerId;

-- 2. Thêm lại tất cả các ràng buộc với CASCADE
-- Từ Connector đến Charger
ALTER TABLE Connector
ADD CONSTRAINT FK_Connector_Charger
FOREIGN KEY (ChargerId) 
REFERENCES Charger(ChargerId)
ON DELETE CASCADE
ON UPDATE CASCADE;

-- Từ ChargingSession đến Connector
ALTER TABLE ChargingSession
ADD CONSTRAINT FK_ChargingSession_Connector
FOREIGN KEY (ConnectorId) 
REFERENCES Connector(ConnectorId)
ON DELETE CASCADE
ON UPDATE CASCADE;

-- Từ Invoices đến ChargingSession
ALTER TABLE Invoices
ADD CONSTRAINT FK_Invoices_ChargingSession
FOREIGN KEY (SessionId) 
REFERENCES ChargingSession(SessionId)
ON DELETE CASCADE
ON UPDATE CASCADE;



-- Từ InvoiceDetail đến Invoices
ALTER TABLE InvoiceDetail
ADD CONSTRAINT FK_InvoiceDetail_Invoices
FOREIGN KEY (InvoiceId) 
REFERENCES Invoices(InvoiceId)
ON DELETE CASCADE
ON UPDATE CASCADE;

-- Từ Revenue_recognition đến InvoiceDetail
ALTER TABLE Revenue_recognition
ADD CONSTRAINT FK_Revenue_recognition_InvoiceDetail
FOREIGN KEY (InvoiceId, InvoiceDetailId) 
REFERENCES InvoiceDetail(InvoiceId, STT)
ON DELETE CASCADE
ON UPDATE CASCADE;

-- Từ Record_detail đến Revenue_recognition
ALTER TABLE Record_detail
ADD CONSTRAINT FK_Record_detail_Revenue_recognition
FOREIGN KEY (Recognition_id) 
REFERENCES Revenue_recognition(Id)
ON DELETE CASCADE
ON UPDATE CASCADE;

-- Từ Receipt đến Invoices
ALTER TABLE Receipt
ADD CONSTRAINT FK_Receipt_Invoices
FOREIGN KEY (InvoiceId) 
REFERENCES Invoices(InvoiceId)
ON DELETE CASCADE
ON UPDATE CASCADE;

-- Từ ReceiptDetail đến Receipt
ALTER TABLE ReceiptDetail
ADD CONSTRAINT FK_ReceiptDetail_Receipt
FOREIGN KEY (ReceiptId) 
REFERENCES Receipt(ReceiptId)
ON DELETE CASCADE
ON UPDATE CASCADE;

ALTER TABLE JournalEntries
ADD CONSTRAINT FK_JournalEntries_Record_detail
FOREIGN KEY (RecordDetail_STT,RecordId) 
REFERENCES Record_detail(STT,Recognition_id)
ON DELETE CASCADE
ON UPDATE CASCADE;
-- ***************************************************************
-- 2. Thêm lại Khóa Ngoại với ON DELETE CASCADE và ON UPDATE CASCADE
-- ***************************************************************

-- 1. Thêm FK_Connector_Charger
-- Khi Charger bị xóa/cập nhật ID, các Connector liên quan sẽ bị xóa/cập nhật ID theo.
ALTER TABLE Connector
ADD CONSTRAINT FK_Connector_Charger
FOREIGN KEY (ChargerId) REFERENCES Charger(ChargerId)
ON DELETE CASCADE
ON UPDATE CASCADE;


-- 2. Thêm FK_ChargingSession_Charger
-- Khi Charger bị xóa/cập nhật ID, các ChargingSession liên quan sẽ bị xóa/cập nhật ID theo.
ALTER TABLE ChargingSession
ADD CONSTRAINT FK_ChargingSession_Connector
FOREIGN KEY (ConnectorId) REFERENCES Connector(ConnectorId)
ON DELETE CASCADE
ON UPDATE CASCADE;


-- 3. Thêm FK_Transactions_ChargingSession
-- Khi ChargingSession bị xóa/cập nhật ID, các Transactions liên quan sẽ bị xóa/cập nhật ID theo.
ALTER TABLE Transactions
add InvoiceId nvarchar(50) null;
ALTER TABLE Transactions
ADD CONSTRAINT FK_Transactions_Invoices
FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId)
ON DELETE CASCADE
ON UPDATE CASCADE;


-- 4. Thêm FK_Invoices_ChargingSession_SessionId
-- Khi ChargingSession bị xóa/cập nhật ID, các Invoices liên quan sẽ bị xóa/cập nhật ID theo.
ALTER TABLE Invoices
ADD CONSTRAINT FK_Invoices_ChargingSession_SessionId
FOREIGN KEY (SessionId) REFERENCES ChargingSession(SessionId)
ON DELETE CASCADE
ON UPDATE CASCADE;


-- ***************************************************************
-- 3. Thêm lại Khóa Ngoại cho Revenue_recognition (Sử dụng NO ACTION/RESTRICT)
-- ***************************************************************

-- 5. Thêm FK_Revenue_recognition_Session (Không CASCADE theo yêu cầu)
-- Nếu Session bị xóa, thao tác này sẽ bị ngăn chặn (RESTRICT/NO ACTION là mặc định nếu không chỉ định).
ALTER TABLE Revenue_recognition
ADD CONSTRAINT FK_Revenue_recognition_Session
FOREIGN KEY (SessionId) REFERENCES ChargingSession(SessionId)
ON DELETE CASCADE ON UPDATE CASCADE ;


-- 6. Thêm FK_Revenue_recognition_Invoices
-- Nếu Invoice bị xóa, thao tác này sẽ bị ngăn chặn.
ALTER TABLE Record_detail
ADD CONSTRAINT FK_Record_detail_Revenue_recognition
FOREIGN KEY (Recognition_id) REFERENCES [dbo].[Revenue_recognition](Id)
ON DELETE CASCADE ON UPDATE CASCADE;


SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn,
    fk.delete_referential_action_desc AS DeleteAction
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.referenced_object_id) = 'Charger';
-- Xóa bảng Log cũ nếu tồn tại
IF OBJECT_ID('RevenueRecognitionLog', 'U') IS NOT NULL DROP TABLE RevenueRecognitionLog;
IF OBJECT_ID('RecordDetailLog', 'U') IS NOT NULL DROP TABLE RecordDetailLog;
IF OBJECT_ID('InvoiceLog', 'U') IS NOT NULL DROP TABLE InvoiceLog;
IF OBJECT_ID('ChargingSessionLog', 'U') IS NOT NULL DROP TABLE ChargingSessionLog;
IF OBJECT_ID('ConnectorLog', 'U') IS NOT NULL DROP TABLE ConnectorLog;

-- 1. Tạo lại RevenueRecognitionLog (Id là bigint bình thường, KHÔNG IDENTITY)
SELECT TOP 0 * INTO RevenueRecognitionLog FROM Revenue_recognition;
-- Sửa cột Id nếu nó đang là Identity (Do SELECT INTO copy cả thuộc tính Identity)
ALTER TABLE RevenueRecognitionLog DROP COLUMN Id;
ALTER TABLE RevenueRecognitionLog ADD Id bigint NOT NULL; -- Thêm lại cột Id thường
ALTER TABLE RevenueRecognitionLog ADD LogReason NVARCHAR(MAX), LogTime DATETIME DEFAULT GETDATE();

-- 2. Tạo lại RecordDetailLog
SELECT TOP 0 * INTO RecordDetailLog FROM Record_Detail;
-- Giả sử STT là Identity
ALTER TABLE RecordDetailLog DROP COLUMN STT;
ALTER TABLE RecordDetailLog ADD STT int NOT NULL; 
ALTER TABLE RecordDetailLog ADD LogReason NVARCHAR(MAX), LogTime DATETIME DEFAULT GETDATE();

-- 3. Tạo lại InvoiceLog
SELECT TOP 0 * INTO InvoiceLog FROM Invoices;
ALTER TABLE InvoiceLog ADD LogReason NVARCHAR(MAX), LogTime DATETIME DEFAULT GETDATE();

-- 4. Tạo lại ChargingSessionLog
SELECT TOP 0 * INTO ChargingSessionLog FROM ChargingSession;
ALTER TABLE ChargingSessionLog DROP COLUMN SessionId;
ALTER TABLE ChargingSessionLog ADD SessionId bigint NOT NULL; -- SessionId thường
ALTER TABLE ChargingSessionLog ADD LogReason NVARCHAR(MAX), LogTime DATETIME DEFAULT GETDATE();

-- 5. Tạo lại ConnectorLog
SELECT TOP 0 * INTO ConnectorLog FROM Connector;
ALTER TABLE ConnectorLog DROP COLUMN ConnectorId;
ALTER TABLE ConnectorLog ADD ConnectorId bigint NOT NULL; -- ConnectorId thường
ALTER TABLE ConnectorLog ADD LogReason NVARCHAR(MAX), LogTime DATETIME DEFAULT GETDATE();
UPDATE i
SET 
    i.status = 'paid',
    i.paidat = t.trans_date,
    i.paymentmethod = 'Bank Transfer',
    i.transaction_code = t.transaction_code
FROM invoices i
INNER JOIN transactions t
    ON i.sessionid = t.sessionid;
	select * from invoices where invoiceid='INV-20251117-16384';


	select *   from [dbo].[Revenue_recognition] a join [dbo].[Record_detail] b on a.id=b.[Recognition_id]
	;
	
	select * from [dbo].[ReportType] e join [dbo].[Report] c on c.[ReportTypeCode]=e.[ReportTypeCode] join [dbo].[JournalEntries] d on c.id=d.ReportId;


	-- Khai báo biến để lưu ID vừa tạo của bảng Revenue_recognition
DECLARE @NewRevenueId bigint;

-- 1. INSERT VÀO BẢNG Revenue_recognition
INSERT INTO [dbo].[Revenue_recognition] (
    Sell_type, 
    Invoice_Delivery, 
    Money_received, 
    Methods_payment, 
    RecordDate, 
    Documentdate, 
    Customerid, 
    Customer_name, 
    Customer_Phone, 
    Customer_Email, 
    Customer_Address, 
    Customer_TaxCode, 
    ExpireDate, 
    InvoiceId, -- Chú ý: dùng Invoiceld (chữ L thường) theo schema của bạn
    Employeeid, 
    EmployeeName
) 
VALUES (
    N'Dịch vụ sạc xe điện', -- Sell_type
    1, -- Invoice_Delivery (True)
    1, -- Money_received (True - Đã nhận tiền)
    N'Chuyển khoản', -- Methods_payment
    GETDATE(), -- RecordDate
    GETDATE(), -- Documentdate
    2, -- Customerid (Giả định)
    NULL, -- Customer_name
    NULL, -- Customer_Phone
    NULL, -- Customer_Email
    NULL, -- Customer_Address
    NULL, -- Customer_TaxCode
    DATEADD(day, 30, GETDATE()), -- ExpireDate
    'INV-20251117-16384', -- Invoiceld (MÃ HÓA ĐƠN BẠN YÊU CẦU)
    2, -- Employeeid (Giả định)
    NULL -- EmployeeName
);
select * from employee;
select * from [dbo].[Revenue_recognition];

-- Lấy ID tự sinh ra từ lệnh Insert trên
SET @NewRevenueId = SCOPE_IDENTITY();

-- 2. INSERT VÀO BẢNG Record_Detail (Sử dụng @NewRevenueId)
-- Giả sử đây là chi tiết cho việc sạc điện
INSERT INTO [dbo].[record_Detail] (
    STT, 
    Recognition_id, 
    ServiceName, 
    Interpretation, 
    Debit_account, 
    Credit_account, 
    Tax_account, 
    UnitPrice, 
    Quantities, 
    Amount, 
    Total, 
    Tax_Percentae, 
    TaxAmount, 
    Unit_measure
) 
VALUES 
(
    1, -- STT
    14, -- Recognition_id (Lấy từ bảng trên)
    N'Phí sạc điện DC', -- ServiceName
    N'Sạc tại trạm V-Green 01', -- Interpretation
    '1111', -- Debit_account (Phải thu)
    '511', -- Credit_account (Doanh thu)
    '333', -- Tax_account (Thuế)
    3500.00, -- UnitPrice (Đơn giá)
    50, -- Quantities (Số lượng kWh)
    175000.00, -- Amount (Thành tiền chưa thuế)
    192500.00, -- Total (Tổng cộng sau thuế)
    10.00, -- Tax_Percentae (10%)
    17500.00, -- TaxAmount
    N'kWh' -- Unit_measure
);

-- Kiểm tra lại kết quả
SELECT * FROM [dbo].[Revenue_recognition] WHERE Invoiceld = 'INV-20251117-16384';
SELECT * FROM [dbo].[record_Detail] WHERE Recognition_id = @NewRevenueId;

	select r.ChargerId  from station s join charger er  on s.stationid=er.stationid
	join connector r on er.ChargerId=r.ChargerId
	join chargingsession c   on c.ConnectorId=r.connectorid
	join invoices i on i.sessionid=c.sessionid
	join Revenue_recognition e on e.invoiceid=i.invoiceid
	join Record_detail d on e.Id=d.Recognition_id;
	select * from Revenue_recognition;

    select *  from invoices i
	join Revenue_recognition e on e.invoiceid=i.invoiceid
	join Record_detail d on e.Id=d.Recognition_id;
	
	-- (Bỏ join với invoices i vì bạn đang chèn DỮ LIỆU MỚI vào invoices)
-- Bảng tài khoản
CREATE TABLE User_App (
  UserId_App  BIGINT IDENTITY PRIMARY KEY,
  Email VARCHAR(100) UNIQUE  NULL,
  Username nvarchar(max) null,
  PasswordHash VARCHAR(200) NOT NULL,           
  AccountType VARCHAR(50)  NULL,             
  CreatedAt DATETIME2(3) DEFAULT SYSUTCDATETIME(),
  UpdatedAt DATETIME2(3) NULL,
  Status VARCHAR(50) NULL, 
-- CONSTRAINT CK_Account_Type CHECK (AccountType IN ('guest','admin','customer'))
);
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
-- Chart of Accounts (COA)
CREATE TABLE COA(
  Acc_Code2 varchar(10) PRIMARY KEY, 
  Acc_Code1 varchar(10) NULL, 
  Acc_Name nvarchar(100),
  Acc_Type nvarchar(100)
);
update COA set Acc_Type =N'Tài sản ngắn hạn' where Acc_Code2 = 111;
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name, Acc_Type)
VALUES 
-- LOẠI 1: TÀI SẢN NGẮN HẠN (Current Assets)
('131', '131', N'Phải thu của khách hàng', N'Tài sản ngắn hạn'),
('1331', '133', N'Thuế GTGT được khấu trừ', N'Tài sản ngắn hạn'),
('152', '152', N'Nguyên liệu, vật liệu', N'Tài sản ngắn hạn'),
('154', '154', N'Chi phí sản xuất, kinh doanh dở dang', N'Tài sản ngắn hạn'),
('156', '156', N'Hàng hóa', N'Tài sản ngắn hạn'),

-- LOẠI 2: TÀI SẢN DÀI HẠN (Non-current Assets)
('2111', '211', N'Nhà cửa, vật kiến trúc (Trạm sạc)', N'Tài sản dài hạn'),
('2113', '211', N'Máy móc, thiết bị (Thiết bị sạc)', N'Tài sản dài hạn'),
('2141', '214', N'Hao mòn TSCĐ hữu hình', N'Tài sản dài hạn'),
('242', '242', N'Chi phí trả trước dài hạn', N'Tài sản dài hạn'),

-- LOẠI 3: NỢ PHẢI TRẢ (Liabilities)
('331', '331', N'Phải trả người bán (Tiền điện, vật tư)', N'Nợ phải trả'),
('33311', '3331', N'Thuế GTGT phải nộp', N'Nợ phải trả'),
('3341', '334', N'Phải trả người lao động', N'Nợ phải trả'),

-- LOẠI 4: VỐN CHỦ SỞ HỮU (Equity)
('411', '411', N'Vốn đầu tư của chủ sở hữu', N'Vốn chủ sở hữu'),
('421', '421', N'Lợi nhuận sau thuế chưa phân phối', N'Vốn chủ sở hữu'),

-- LOẠI 5: DOANH THU (Revenues)
('5111', '511', N'Doanh thu bán hàng và cung cấp dịch vụ (Dịch vụ sạc)', N'Doanh thu'),
('515', '515', N'Doanh thu hoạt động tài chính', N'Doanh thu'),

-- LOẠI 6: GIÁ VỐN & CHI PHÍ KINH DOANH (COGS & Operating Expenses)
('632', '632', N'Giá vốn hàng bán (Giá vốn tiền điện đầu vào)', N'Chi phí kinh doanh'),
('641', '641', N'Chi phí bán hàng', N'Chi phí kinh doanh'),
('642', '642', N'Chi phí quản lý doanh nghiệp', N'Chi phí kinh doanh'),

-- LOẠI 7 & 8: THU NHẬP/CHI PHÍ KHÁC
('711', '711', N'Thu nhập khác', N'Thu nhập khác'),
('811', '811', N'Chi phí khác', N'Chi phí khác');
alter table COA alter column Acc_Code2 varchar(10) ;
alter table COA alter column Acc_Code1 varchar(10) null ;
select * from COA;
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (111, NULL, N'Tiền mặt');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (1111,111, N'Tiền Việt Nam');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (1112,111, N'Ngoại Tệ');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (1113,111, N'Vàng tiền tệ');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (711,NULL, N'Thu nhập khác');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (511,NULL, N'Doanh thu ');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (5113,511, N'Doanh Thu từ cung cấp dịch vụ');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (331,NULL, N'Thuế GTGT');
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (33311,333, N'Thuế GTGT đầu ra');

update COA set Acc_Name =N'Doanh Thu từ cung cấp dịch vụ' where Acc_Code2=5113
INSERT INTO COA (Acc_Code2, Acc_Code1, Acc_Name) VALUES (5118,511, N'Doanh thu khác'); --- Phí phụ thu chiếm dụng trụ (Idle Fee) 

-- Payment
CREATE TABLE Payment(
  PaymentID BIGINT IDENTITY PRIMARY KEY, 
  TransID varchar(20),
  InvoiceID varchar(20),
  Update_Time datetime
);
create table RecognitionType
(
-----------
Code nvarchar(50)  primary key,
Name nvarchar(200)
);

INSERT INTO RecognitionType (Code, Name)
VALUES 
('DT', N'Hạch toán Doanh thu (Bán hàng và Phiếu thu)'), -- mặc định là bảng Revenue_recognition rồi nên không cần 
('MH', N'Hạch toán Mua hàng'),
('CP', N'Hạch toán Chi phí (Chi phí chung)'),
('PC', N'Phiếu chi (Chi tiền mặt/Ngân hàng)'),
('CNPT', N'Công nợ phải thu (Điều chỉnh Nợ phải thu)'),
('CNPP', N'Công nợ phải trả (Điều chỉnh Nợ phải trả)');
CREATE TABLE Recognition
(
    Id BIGINT IDENTITY PRIMARY KEY,

    RecognitionCode  int not null,   -- FK đến JournalType
    DocumentDate datetime not null,
    RecordDate datetime not null,

    CustomerId bigint null,
    SupplierId bigint null,
    EmployeeId bigint null,

    ReferenceTable nvarchar(100) null,
    ReferenceId bigint null,

    Notes nvarchar(max) null,

    FOREIGN KEY (RecognitionCode ) REFERENCES Recognition(Id),
    FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
    FOREIGN KEY (SupplierId) REFERENCES Supplier(SupplierId),
    FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId)
);

alter table Revenue_recognition alter column  JournalCode nvarchar(50) default 'DT';
alter table Revenue_recognition add  CONSTRAINT FK_Revenue_recognition_JournalType FOREIGN KEY (JournalCode) REFERENCES JournalType(Code);
drop column JournalCode
add  JournalCode nvarchar(50) null,

create table Revenue_recognition --hạch toán doanh thu
(
Id BIGINT IDENTITY PRIMARY KEY,
Sell_type nvarchar (500) null, --- 1. bán hàng trong nước/ 2. Bán hàng xuất khẩu / 3. Bán hàng đại lý bán đúng giá / 4. Bán hàng ủy thác xuất khẩu2. 
Invoice_Delivery bit null,
-- Delivery_note bit null, -- phiếu xuất
Money_received bit null, -- chưa thu tiền, đã thu tiền 
Methods_payment nvarchar(100) null,--- CASH | BANK | E_WALLET
RecordDate datetime null,
Documentdate datetime null,

-- invoice và delivery_note dạng bit, tickbox

---- THÔNG TIN KHÁCH HÀNG
Customerid bigint null,


--- THÔNG TIN HÓA ĐƠN
ExpireDate date null,

--Document_reference ,
--Transactionid varchar(100)null,-- tham chiếu mã giao dịch
Invoiceid bigint null,
InvoiceDetailId int null,

ReceiptId bigint null,
ReceiptDetailSTT int null,

-- nhân viên hạch toán, lấy id từ user_app thực hiện trên form, suy ra thông tin nhân viên
Employeeid bigint null,


   CONSTRAINT FK_Revenue_recognition_Customer FOREIGN KEY (Customerid) REFERENCES Customer(CustomerId),
      CONSTRAINT FK_Revenue_recognition_Employee FOREIGN KEY (Employeeid) REFERENCES Employee(EmployeeId),
	        CONSTRAINT FK_Revenue_recognition_Invoices FOREIGN KEY (Invoiceid) REFERENCES Invoices(Invoiceid)

);
create table Record_detail
(
STT int , --tăng dần từ 1
Recognition_id bigint ,
ItemId bigint  null, -- dịch vụ sạc điện tại trạm, tiền phạt 
Interpretation nvarchar(max) null, -- diễn giải mặc định: bán dịch vụ theo số hóa đơn + invoice_id 
Unit_measure nvarchar(20) null, -- lần, phút
Debit_account varchar(10) null, -- tài khoản nợ 
/*xét theo 2 điều kiện: nếu money_received =  true => debit_account = 1111 hoặc 1112,
xét tiếp methods_payment là tiền mặt hay ngân hàng, false thì mặc định là "131" 
*/
Credit_account varchar(10) null, --- tài khoản ghi doanh thu (tk có )  "5111" hoặc hạch toán tiền phí phạt "711"
Tax_account varchar(10) null, -- mặc định: 33311
UnitPrice decimal(10,2) null,
Quantities int null,
Amount decimal(10,2) null,
Total decimal (10,2) null, -- tổng tiền dịch vụ
Tax_Percentae decimal(5,2) null,
TaxAmount decimal(10,2) null,
DiscountPercent decimal(5,2) null,
DiscountAmount decimal(18,2) null,

    CONSTRAINT PK_Record_detail PRIMARY KEY (STT, Recognition_id),

    CONSTRAINT FK_Record_detail_Recognition FOREIGN KEY (Recognition_id) REFERENCES Revenue_recognition(Id),
	    CONSTRAINT FK_Record_detail_RevenueItem FOREIGN KEY (ItemId) REFERENCES RevenueItem(ItemId),

    CONSTRAINT FK_Record_detail_DebitAccount  FOREIGN KEY (Debit_account) REFERENCES COA(Acc_Code2),

    CONSTRAINT FK_Record_detail_CreditAccount  FOREIGN KEY (Credit_account) REFERENCES COA(Acc_Code2),

    CONSTRAINT FK_Record_detail_TaxAccount  FOREIGN KEY (Tax_account) REFERENCES COA(Acc_Code2)
);
drop constraint  FK_Record_detail_Revenue_recognition;
--- log bảng hạch toán

-- Transactions

--- phiếu thu tiền
CREATE TABLE Receipt
(
    Id bigint IDENTITY PRIMARY KEY,
    ReceiptNo varchar(50) null,
    ReceiptDate datetime null,
    CustomerId bigint null,
    Invoiceid nvarchar(50) NULL, -- nếu thu theo hóa đơn
    TotalAmount decimal(18,2) null,
    Method_payment varchar(50) null,
    Employeeid bigint,
	TransID varchar(100) null,
	RecordDate date null,
    FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
    FOREIGN KEY (Invoiceid) REFERENCES Invoices(Invoiceid),
    FOREIGN KEY (Employeeid) REFERENCES Employee(EmployeeId),
	FOREIGN KEY (TransID) REFERENCES Transactions(Id)

);

--- giấy báo có


select * from charger;
-- 1. First, drop existing foreign key constraints that need to be modified
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Connector_Charger')
    ALTER TABLE Connector DROP CONSTRAINT FK_Connector_Charger;

-- 2. Recreate the foreign key with CASCADE operations
ALTER TABLE Connector
ADD CONSTRAINT FK_Connector_Charger
FOREIGN KEY (ChargerId) 
REFERENCES Charger(ChargerId)
ON DELETE NO ACTION
ON UPDATE NO ACTION;

-- 3. For tables that reference Charger through Connector, we need to handle them accordingly
--    First, drop the existing foreign key constraints
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChargingSession_Connector')
    ALTER TABLE ChargingSession DROP CONSTRAINT FK_ChargingSession_Connector;

-- 4. Recreate the foreign key with CASCADE operations
ALTER TABLE ChargingSession
ADD CONSTRAINT FK_ChargingSession_Connector
FOREIGN KEY (ConnectorId) 
REFERENCES Connector(ConnectorId)
ON DELETE CASCADE
ON UPDATE CASCADE;

-- 5. For any other tables that reference ChargingSession, you might want to add CASCADE as well
--    For example, if there are tables that reference ChargingSession:
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Invoices_ChargingSession')
    ALTER TABLE Invoices DROP CONSTRAINT FK_Invoices_ChargingSession;

ALTER TABLE Invoices
ADD CONSTRAINT FK_Invoices_ChargingSession
FOREIGN KEY (SessionId) 
REFERENCES ChargingSession(SessionId)
ON DELETE SET NULL  -- Or CASCADE depending on your requirements
ON UPDATE CASCADE;

-- 6. For Revenue_recognition that references ChargingSession
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Revenue_recognition_Session')
    ALTER TABLE Revenue_recognition DROP CONSTRAINT FK_Revenue_recognition_Session;

ALTER TABLE Revenue_recognition
ADD CONSTRAINT FK_Revenue_recognition_Session
FOREIGN KEY (SessionId) 
REFERENCES ChargingSession(SessionId)
ON DELETE SET NULL  -- Or CASCADE depending on your requirements
ON UPDATE CASCADE;