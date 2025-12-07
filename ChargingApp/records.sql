USE ChargingPoint;
GO
alter table ChargingSession add OverDueMinutes int NULL;

select * from  customer;
drop table [dbo].[RecordDetailLog];
drop table [dbo].[Record_detail];
drop table [dbo].[Revenue_recognition];
drop table [dbo].[RevenueRecognitionLog];
drop table [dbo].[Invoices];
drop table [dbo].[InvoiceLog];
drop table [dbo].[Receipt];
drop table [dbo].[Transactions];
CREATE TABLE Receipt (
    ReceiptId bigint IDENTITY(1,1) NOT NULL,
    ReceiptNumber nvarchar(50) NOT NULL, -- Số phiếu thu (VD: PT001/2025)
    ReceiptDate datetime2(7) NOT NULL DEFAULT GETDATE(), -- Ngày lập phiếu
    
    -- Thông tin người nộp (Snapshot để lưu lại lịch sử nếu danh mục thay đổi)
    PayerName nvarchar(200) NULL, 
    PayerAddress nvarchar(500) NULL,
    Description nvarchar(1000) NULL, -- Lý do thu (Diễn giải chung)
    
    TotalAmount decimal(18,2) NOT NULL DEFAULT 0, -- Tổng tiền thu
    PaymentMethod nvarchar(100) NULL, -- Tiền mặt/Chuyển khoản
    Status nvarchar(50) NOT NULL DEFAULT 'Posted', -- Draft (Nháp), Posted (Đã ghi sổ), Cancelled (Hủy)
    
    -- Khóa ngoại
    CustomerId bigint NULL, -- Thu của khách hàng nào (nếu có)
    EmployeeId bigint NULL, -- Nhân viên nào lập phiếu thu
    InvoiceId bigint NULL, -- (Tùy chọn) Nếu phiếu này thu đích danh cho 1 hóa đơn cụ thể

    -- Metadata
    CreatedAt datetime2(7) NOT NULL DEFAULT GETDATE(),
    CreatedBy nvarchar(100) NULL,
	TransactionId varchar(100) NULL,

    CONSTRAINT PK_Receipt PRIMARY KEY (ReceiptId),
    CONSTRAINT FK_Receipt_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
    CONSTRAINT FK_Receipt_Employee FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId),
    CONSTRAINT FK_Receipt_Invoices FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId),
	CONSTRAINT FK_Receipt_Transaction FOREIGN KEY (TransactionId) REFERENCES Transactions(Id) 

);
CREATE TABLE ReceiptDetail (
    ReceiptId bigint NOT NULL,
    STT int NOT NULL, -- Số thứ tự dòng
    
    Description nvarchar(max) NULL, -- Diễn giải chi tiết cho dòng này
    
    -- Định khoản Kế toán (Liên kết với bảng COA)
    Debit_Account varchar(10) NULL, -- Tài khoản Nợ (VD: 111 - Tiền mặt)
    Credit_Account varchar(10) NULL, -- Tài khoản Có (VD: 131 - Phải thu KH)
    
    Amount decimal(18,2) NOT NULL DEFAULT 0, -- Số tiền của dòng này
    
    -- (Tùy chọn) Nếu 1 phiếu thu trả cho nhiều hóa đơn, cột này dùng để tham chiếu dòng này trả cho hóa đơn nào
    Ref_InvoiceId bigint NULL, 

    CONSTRAINT PK_ReceiptDetail PRIMARY KEY (ReceiptId, STT),
    CONSTRAINT FK_ReceiptDetail_Receipt FOREIGN KEY (ReceiptId) REFERENCES Receipt(ReceiptId) ON DELETE CASCADE,
    CONSTRAINT FK_ReceiptDetail_DebitAcc FOREIGN KEY (Debit_Account) REFERENCES COA(Acc_Code2),
    CONSTRAINT FK_ReceiptDetail_CreditAcc FOREIGN KEY (Credit_Account) REFERENCES COA(Acc_Code2),
    CONSTRAINT FK_ReceiptDetail_RefInvoice FOREIGN KEY (Ref_InvoiceId) REFERENCES Invoices(InvoiceId)
);
CREATE TABLE Invoices(
    InvoiceId bigint IDENTITY NOT NULL,
    SessionId bigint NULL,
    Snashot_CustomerName nvarchar(200) NULL,
    Snashot_CustomerPhone nvarchar(20) NULL,
    Snashot_CustomerEmail nvarchar(200) NULL,
    InvoiceNumber bigint NOT NULL,
    InvoiceTemplate nvarchar(50) NOT NULL,
    InvoiceSymbol nvarchar(50) NOT NULL,
    CreatedAt datetime2(7) NOT NULL DEFAULT GETDATE(),
    ExpireDate datetime2(7) NULL,
    --- 3 cột này tạo view 
	TotalAmountService decimal(10,2) null, -- = sum (InvoiceDetail(Amount))
	TotalAmountTax decimal(10,2) null,-- = sum (InvoiceDetail(TaxAmount))
    TotalAmountDiscount decimal(10,2) null, -- =  SUM(InvoiceDetail(DiscountAmount))
	Total  decimal(10,2) null, -- = sum  (TotalAmountService-TotalAmountDiscount)+TotalAmountTax
	-----
    PaymentLink nvarchar(500) NULL,
    QRCodeData nvarchar(500) NULL,
    Status nvarchar(50) NOT NULL,
    PaidAt datetime2(7) NULL,
    PaymentMethod nvarchar(100) NULL,
    PdfFilePath nvarchar(500) NULL,
    EmailSent bit NOT NULL DEFAULT 0,
    EmailSentAt datetime2(7) NULL,
    Customer_Signature nvarchar(500) NULL,
    Signature nvarchar(500) NULL,
    Notes nvarchar(1000) NULL,
    CustomerId bigint NULL,
    
	 CONSTRAINT PK_Invoices PRIMARY KEY (InvoiceId),
	 CONSTRAINT FK_Invoices_ChargingSession FOREIGN KEY (SessionId ) REFERENCES ChargingSession(SessionId),
	 CONSTRAINT FK_Invoices_Customer FOREIGN KEY (CustomerID ) REFERENCES Customer(CustomerID)


);


alter table [dbo].[Revenue_recognition] add  ReceiptId bigint null;
alter table [dbo].[Revenue_recognition] alter column  ReceiptDetailSTT int null;
alter table [dbo].[Revenue_recognition] add  
 CONSTRAINT FK_Revenue_recognition_ReceiptDetail FOREIGN KEY (ReceiptId, ReceiptDetailSTT) REFERENCES ReceiptDetail(ReceiptId, STT)

CREATE TABLE InvoiceDetail
(
    InvoiceId BIGINT NOT NULL,
    STT INT NOT NULL,
    ItemId BIGINT NULL,
    Quantities INT NULL DEFAULT 0,
    Unit NVARCHAR(50) NULL,
    UnitPrice DECIMAL(18,4) NULL DEFAULT 0,
    Amount AS (ISNULL(Quantities,0) * ISNULL(UnitPrice,0)) PERSISTED,
    DiscountPercent DECIMAL(5,2) NULL DEFAULT 0,
    DiscountAmount DECIMAL(18,4) NULL DEFAULT 0,
    AmountAfterDiscount AS (ISNULL(Quantities,0) * ISNULL(UnitPrice,0) - ISNULL(DiscountAmount,0)) PERSISTED,
    Tax DECIMAL(5,2) NULL DEFAULT 10,
    TaxAmount AS (ROUND((ISNULL(Quantities,0) * ISNULL(UnitPrice,0) - ISNULL(DiscountAmount,0)) * ISNULL(Tax,0)/100.0, 2)) PERSISTED,
    TotalLine AS (ROUND((ISNULL(Quantities,0) * ISNULL(UnitPrice,0) - ISNULL(DiscountAmount,0)) * (1 + ISNULL(Tax,0)/100.0), 2)) PERSISTED,

    CONSTRAINT PK_InvoiceDetail PRIMARY KEY (InvoiceId, STT),
    CONSTRAINT FK_InvoiceDetail_Invoice FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId) ON DELETE CASCADE,
    CONSTRAINT FK_InvoiceDetail_Item FOREIGN KEY (ItemId) REFERENCES RevenueItem(ItemId)
);
Create table RevenueItem (
ItemId BIGINT IDENTITY PRIMARY KEY,
ItemName nvarchar(max) null,  -- dịch vụ sạc điện tại trạm, mua sạc cầm tay, phí phạt 
Unit nvarchar(50) null,
ItemType nvarchar(300) null, -- dịch vụ sạc, thu phí, ...
Discription nvarchar(max) null,
IsActive bit DEFAULT 1 null
-- Thêm ràng buộc mặc định (Default Constraint)
 CONSTRAINT DF_Transaction_Code 
DEFAULT UPPER(SUBSTRING(REPLACE(NEWID(), '-', ''), 1, 8)) FOR Transaction_Code
);
-- 1. Nếu Id đang là khóa chính (Primary Key), phải xóa khóa chính trước
-- Thay 'PK_Transaction' bằng tên khóa chính thực tế của bạn (nếu không biết tên, xem trong Object Explorer)
ALTER TABLE [Transactions] DROP CONSTRAINT PK__Transact__3214EC071E2ED673; -- (Tên này thường ngẫu nhiên nếu bạn không đặt)

-- HOẶC: Nếu chưa set Key thì chạy thẳng dòng này để xóa cột Id cũ
ALTER TABLE [Transactions] DROP COLUMN Id;

-- 2. Thêm lại cột Id mới với chế độ tự tăng và làm khóa chính luôn
ALTER TABLE [Transactions] 
ADD Id BIGINT IDENTITY(1,1) PRIMARY KEY;
ALTER TABLE [Transaction] alter column Id BIGINT IDENTITY PRIMARY KEY;
INSERT INTO RevenueItem (ItemName, Unit, ItemType, Discription, IsActive)
VALUES 
-- 1. Dịch vụ sạc (Tính theo số điện)
--(N'Dịch vụ sạc tại trạm', N'kWh', N'CHARGING_FEE', N'Phí sạc điện tính theo năng lượng tiêu thụ', 1),

-- 2. Phí phạt quá giờ (Tính theo thời gian chiếm chỗ)
(N'Phí phạt quá thời gian', N'Phút', N'OVERSTAY_FEE', N'Phí phạt khi xe đỗ chiếm chỗ sau khi đã sạc đầy (Idle fee)', 1),

-- 3. Phí đặt chỗ trước (Booking/Reservation)
(N'Phí đặt chỗ trước', N'Lượt', N'RESERVATION_FEE', N'Phí dịch vụ để giữ chỗ sạc trong khoảng thời gian nhất định', 1),
-- 4. Gói thành viên (Subscription)
(N'Gói thành viên tháng', N'Tháng', N'SUBSCRIPTION', N'Phí đăng ký hội viên ưu đãi hàng tháng', 1);



select * from [dbo].[AspNetUsers];
select * from [dbo].[Record_detail];


create table Revenue_recognition --hạch toán doanh thu
(
Id BIGINT IDENTITY PRIMARY KEY,
Sell_type nvarchar (500) null, --- 1. bán hàng trong nước/ 2. Bán hàng xuất khẩu / 3. Bán hàng đại lý bán đúng giá / 4. Bán hàng ủy thác xuất khẩu2. 
Invoice_Delivery bit null, -- xuất kèm hóa đơn invoice và Receipt_Delivery dạng bit, tickbox
Receipt_Delivery bit null, -- xuất kèm phiếu thu 
Money_received bit null, -- chưa thu tiền, đã thu tiền 
Methods_payment nvarchar(100) null,--- only 2 tiền mặt, tiền ngân hàng (dropbox)
RecordDate datetime null,
Documentdate datetime null,
ExpireDate date null,

--Document_reference ,
--Transactionid varchar(100)null,-- tham chiếu mã giao dịch
InvoiceId bigint  null,
InvoiceDetailId int,

CustomerId bigint null,
-- nhân viên hạch toán, lấy id từ user_app thực hiện trên form, suy ra thông tin nhân viên
Employeeid bigint null,

   CONSTRAINT FK_Revenue_recognition_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
   CONSTRAINT FK_Revenue_recognition_Employee FOREIGN KEY (Employeeid) REFERENCES Employee(EmployeeId),
	CONSTRAINT FK_Revenue_recognition_InvoiceDetail FOREIGN KEY (InvoiceId, InvoiceDetailId) REFERENCES InvoiceDetail(InvoiceId, STT)


);
create table Record_detail
(
STT int , --tăng dần từ 1
Recognition_id bigint ,

ItemId bigint null,
Interpretation nvarchar(max) null, -- diễn giải mặc định: bán dịch vụ theo số hóa đơn + invoice_id 
Unit_measure nvarchar(20) null, -- lần, phút
Debit_account varchar(10) null, -- tài khoản nợ 
/*xét theo 2 điều kiện: nếu money_received =  true => debit_account = 1111 hoặc 1112,
xét tiếp methods_payment là tiền mặt hay ngân hàng, false thì mặc định là "131" 
*/
Credit_account varchar(10) null, --- tài khoản ghi doanh thu (tk có )  "5111" hoặc hạch toán tiền phí phạt "711"
Tax_account varchar(10) null, -- mặc định: 33311
Discount_account varchar(10) null, --

----
UnitPrice decimal(10,2) null,
Quantities int null,
Amount decimal(10,2) null,


TaxPercentage decimal(5,2) null,
TaxAmount decimal(10,2) null,


DiscountPercent DECIMAL(5,2) NULL DEFAULT 0,
DiscountAmount DECIMAL(18,4) NULL DEFAULT 0,


  CONSTRAINT FK_Record_detail_Revenue_recognition FOREIGN KEY (Recognition_id) REFERENCES Revenue_recognition(Id),
  CONSTRAINT PK_Record_detail PRIMARY KEY (STT, Recognition_id),
  CONSTRAINT FK_Record_detail_Recognition FOREIGN KEY (Recognition_id) REFERENCES Revenue_recognition(Id),

    CONSTRAINT FK_Record_detail_DebitAccount  FOREIGN KEY (Debit_account) REFERENCES COA(Acc_Code2),

    CONSTRAINT FK_Record_detail_CreditAccount  FOREIGN KEY (Credit_account) REFERENCES COA(Acc_Code2),

    CONSTRAINT FK_Record_detail_TaxAccount  FOREIGN KEY (Tax_account) REFERENCES COA(Acc_Code2)
);
-- PHẦN KHÓA NGOẠI (FOREIGN KEYS) - Bỏ comment để chạy nếu cần

-- ALTER TABLE Invoices ADD CONSTRAINT FK_Invoices_Sessions FOREIGN KEY(SessionId) REFERENCES Sessions(Id);
-- ALTER TABLE Invoices ADD CONSTRAINT FK_Invoices_Customers FOREIGN KEY(CustomerId) REFERENCES Customers(Id);
-- ALTER TABLE Invoices ADD CONSTRAINT FK_Invoices_Stations FOREIGN KEY(StationId) REFERENCES Stations(Id);
select * from invoices;
exec sp_rename '[dbo].[invoices].[Invoiceid]', 'InvoiceId','Column';
alter table customer add  TaxCode varchar(200) null;
INSERT INTO Transactions
(
 Id, Trans_Date, From_Bank_Code, From_Bank_Name, From_Acc_Name,
 To_Bank_Code, To_Bank_Name, To_Acc_Name,
 Transaction_Code, Trans_Type, Amount, URL, SessionId
)
VALUES
('T001', GETDATE(), 970415, 'Vietcombank', 'Nguyen A',
        970407, 'Techcombank', 'Cong ty X',
        10001, 'received', 60000,  'url/1',4 ),

('T002', GETDATE(), 970423, 'MB Bank', 'Le B',
        970415, 'Vietcombank', 'Cong ty X',
        10002, 'received', 75000, 'url/2', 5),

('T003', GETDATE(), 970436, 'TPBank', 'Cong ty C',
        970415, 'Vietcombank', 'Cong ty X',
        10003, 'received', 50000,  'url/3', 7),

('T004', GETDATE(), 970415, 'Vietcombank', 'Nguyen D',
        970407, 'Techcombank', 'Cong ty X',
        10004, 'received', 112000,  'url/4', 8),

('T005', GETDATE(), 970437, 'VIB', 'Cong ty E',
        970415, 'Vietcombank', 'Cong ty X',
        10005, 'received', 80000,  'url/5', 37);
------------CHARGER
USE ChargingPoint;
GO
INSERT INTO Charger (
    StationId, Name, SerialNumber, Model, ChargerType, MaxPowerKW, Phases, 
    OutputVoltageMin, OutputVoltageMax, PortCount, Design, Protections, 
    FirmwareVersion, InstalledAt, Status, CreatedAt, PicturePath, UseFor
) VALUES
(23, 'VF-DC-300KW-01', 'VGD300-051', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(30, 'VF-DC-30KW-02', 'VGD030-052', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(41, 'VF-AC-11KW-03', 'VGA011-053', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 1, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(35, 'VF-MOTO-1.2KW-04', 'VGM1.2-054', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 4, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(27, 'VF-DC-300KW-05', 'VGD300-055', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(32, 'VF-AC-11KW-06', 'VGA011-056', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 1, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(39, 'VF-DC-30KW-07', 'VGD030-057', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'poweroff', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(24, 'VF-MOTO-1.2KW-08', 'VGM1.2-058', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 2, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(40, 'VF-DC-300KW-09', 'VGD300-059', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(38, 'VF-AC-11KW-10', 'VGA011-060', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 2, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(25, 'VF-DC-30KW-11', 'VGD030-061', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(31, 'VF-MOTO-1.2KW-12', 'VGM1.2-062', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 2, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(26, 'VF-DC-300KW-13', 'VGD300-063', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'poweroff', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(33, 'VF-AC-11KW-14', 'VGA011-064', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 1, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(37, 'VF-DC-30KW-15', 'VGD030-065', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(28, 'VF-MOTO-1.2KW-16', 'VGM1.2-066', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 4, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(34, 'VF-DC-300KW-17', 'VGD300-067', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(36, 'VF-AC-11KW-18', 'VGA011-068', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 2, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(29, 'VF-DC-30KW-19', 'VGD030-069', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(41, 'VF-MOTO-1.2KW-20', 'VGM1.2-070', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 4, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(23, 'VF-DC-300KW-21', 'VGD300-071', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(30, 'VF-AC-11KW-22', 'VGA011-072', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 1, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'poweroff', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(40, 'VF-DC-30KW-23', 'VGD030-073', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(35, 'VF-MOTO-1.2KW-24', 'VGM1.2-074', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 2, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(27, 'VF-DC-300KW-25', 'VGD300-075', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(32, 'VF-DC-30KW-26', 'VGD030-076', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(39, 'VF-AC-11KW-27', 'VGA011-077', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 1, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(24, 'VF-MOTO-1.2KW-28', 'VGM1.2-078', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 2, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'poweroff', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(40, 'VF-DC-300KW-29', 'VGD300-079', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(38, 'VF-DC-30KW-30', 'VGD030-080', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(25, 'VF-AC-11KW-31', 'VGA011-081', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 2, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(31, 'VF-MOTO-1.2KW-32', 'VGM1.2-082', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 4, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(26, 'VF-DC-300KW-33', 'VGD300-083', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(33, 'VF-DC-30KW-34', 'VGD030-084', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'poweroff', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(37, 'VF-AC-11KW-35', 'VGA011-085', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 1, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(28, 'VF-MOTO-1.2KW-36', 'VGM1.2-086', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 4, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(34, 'VF-DC-300KW-37', 'VGD300-087', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(36, 'VF-DC-30KW-38', 'VGD030-088', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(29, 'VF-AC-11KW-39', 'VGA011-089', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 2, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(41, 'VF-MOTO-1.2KW-40', 'VGM1.2-090', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 2, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(23, 'VF-DC-300KW-41', 'VGD300-091', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'poweroff', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(30, 'VF-AC-11KW-42', 'VGA011-092', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 1, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(40, 'VF-DC-30KW-43', 'VGD030-093', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(35, 'VF-MOTO-1.2KW-44', 'VGM1.2-094', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 4, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'Active', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(27, 'VF-DC-300KW-45', 'VGD300-095', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(32, 'VF-AC-11KW-46', 'VGA011-096', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 1, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô'),
(39, 'VF-DC-30KW-47', 'VGD030-097', 'DC-Fast', 'DC', 30.0, NULL, 200, 1000, 1, 'Treo tường', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.8.1', '2024-09-15', 'Active', '2024-09-01', '/img/vgreen/d030.jpg', 'Ô tô'),
(24, 'VF-MOTO-1.2KW-48', 'VGM1.2-098', 'MOTO-Basic', 'AC', 1.2, 1, 220, 220, 2, 'Tủ đứng', 'Qúa tải/nhiệt/ngắn mạch/chống giật', '1.0.0', '2024-07-01', 'poweroff', '2024-06-25', '/img/vgreen/moto.jpg', 'Xe máy'),
(40, 'VF-DC-300KW-49', 'VGD300-099', 'DC-Ultra', 'DC', 300.0, NULL, 200, 1000, 1, 'Tủ đứng', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '3.5.0', '2024-10-01', 'Active', '2024-09-20', '/img/vgreen/d300.jpg', 'Ô tô'),
(38, 'VF-AC-11KW-50', 'VGA011-100', 'AC-Normal', 'AC', 11.0, 3, 220, 400, 2, 'Treo tường/trụ', 'Qúa tải/nhiệt/dòng rò/ngắn mạch/IP54', '1.2.5', '2024-08-20', 'Active', '2024-08-10', '/img/vgreen/a011.jpg', 'Ô tô');



-----CONNECTOR
INSERT INTO Connector (
    ChargerId, ConnectorIndex, ConnectorType, Status
) VALUES
-- ChargerId 2 - 50 (Từ dữ liệu ban đầu)
-- Trụ 002 (ID 2): DC, Port=1 (CCS2)
(2, 1, 'CCS2', 'Available'),
-- Trụ 003 (ID 3): AC, Port=1 (AC-2P cho Xe máy)
(3, 1, 'AC-2P', 'Available'),
-- Trụ 004 (ID 4): AC, Port=2 (Type 2)
(4, 1, 'Type 2', 'Available'), (4, 2, 'Type 2', 'Available'),
-- Trụ 005 (ID 5): DC, Port=2 (CCS2)
(5, 1, 'CCS2', 'Available'), (5, 2, 'CCS2', 'Available'),
-- Trụ 006 (ID 6): DC, Port=1 (CCS2)
(6, 1, 'CCS2', 'Available'),
-- Trụ 007 (ID 7): AC, Port=2 (Type 2)
(7, 1, 'Type 2', 'Available'), (7, 2, 'Type 2', 'Available'),
-- Trụ 008 (ID 8): AC, Port=1 (AC-2P cho Xe máy)
(8, 1, 'AC-2P', 'Available'),
-- Trụ 009 (ID 9): DC, Port=2 (CCS2)
(9, 1, 'CCS2', 'Available'), (9, 2, 'CCS2', 'Available'),
-- Trụ 010 (ID 10): DC, Port=1 (CCS2)
(10, 1, 'CCS2', 'Available'),
-- Trụ 011 (ID 11): AC, Port=1 (AC-2P cho Xe máy)
(11, 1, 'AC-2P', 'Available'),
-- Trụ 012 (ID 12): AC, Port=2 (Type 2)
(12, 1, 'Type 2', 'Available'), (12, 2, 'Type 2', 'Available'),
-- Trụ 013 (ID 13): DC, Port=2 (CCS2)
(13, 1, 'CCS2', 'Available'), (13, 2, 'CCS2', 'Available'),
-- Trụ 014 (ID 14): DC, Port=1 (CCS2)
(14, 1, 'CCS2', 'Available'),
-- Trụ 015 (ID 15): AC, Port=2 (Type 2)
(15, 1, 'Type 2', 'Available'), (15, 2, 'Type 2', 'Available'),
-- Trụ 016 (ID 16): AC, Port=1 (AC-2P cho Xe máy)
(16, 1, 'AC-2P', 'Available'),
-- Trụ 017 (ID 17): DC, Port=1 (CCS2)
(17, 1, 'CCS2', 'Available'),
-- Trụ 018 (ID 18): DC, Port=2 (CCS2)
(18, 1, 'CCS2', 'Available'), (18, 2, 'CCS2', 'Available'),
-- Trụ 019 (ID 19): AC, Port=2 (Type 2)
(19, 1, 'Type 2', 'Available'), (19, 2, 'Type 2', 'Available'),
-- Trụ 020 (ID 20): AC, Port=1 (AC-2P cho Xe máy)
(20, 1, 'AC-2P', 'Available'),
-- Trụ 021 (ID 21): DC, Port=1 (CCS2)
(21, 1, 'CCS2', 'Available'),
-- Trụ 022 (ID 22): DC, Port=1 (CCS2)
(22, 1, 'CCS2', 'Available'),
-- Trụ 023 (ID 23): AC, Port=2 (Type 2)
(23, 1, 'Type 2', 'Available'), (23, 2, 'Type 2', 'Available'),
-- Trụ 024 (ID 24): AC, Port=1 (AC-2P cho Xe máy)
(24, 1, 'AC-2P', 'Available'),
-- Trụ 025 (ID 25): DC, Port=2 (CCS2)
(25, 1, 'CCS2', 'Available'), (25, 2, 'CCS2', 'Available'),
-- Trụ 026 (ID 26): DC, Port=1 (CCS2)
(26, 1, 'CCS2', 'Available'),
-- Trụ 027 (ID 27): AC, Port=1 (AC-2P cho Xe máy)
(27, 1, 'AC-2P', 'Available'),
-- Trụ 028 (ID 28): AC, Port=2 (Type 2)
(28, 1, 'Type 2', 'Available'), (28, 2, 'Type 2', 'Available'),
-- Trụ 029 (ID 29): DC, Port=1 (CCS2)
(29, 1, 'CCS2', 'Available'),
-- Trụ 030 (ID 30): DC, Port=2 (CCS2)
(30, 1, 'CCS2', 'Available'), (30, 2, 'CCS2', 'Available'),
-- Trụ 031 (ID 31): AC, Port=2 (Type 2)
(31, 1, 'Type 2', 'Available'), (31, 2, 'Type 2', 'Available'),
-- Trụ 032 (ID 32): AC, Port=1 (AC-2P cho Xe máy)
(32, 1, 'AC-2P', 'Available'),
-- Trụ 033 (ID 33): DC, Port=1 (CCS2)
(33, 1, 'CCS2', 'Available'),
-- Trụ 034 (ID 34): DC, Port=1 (CCS2)
(34, 1, 'CCS2', 'Available'),
-- Trụ 035 (ID 35): AC, Port=1 (AC-2P cho Xe máy)
(35, 1, 'AC-2P', 'Available'),
-- Trụ 036 (ID 36): AC, Port=2 (Type 2)
(36, 1, 'Type 2', 'Available'), (36, 2, 'Type 2', 'Available'),
-- Trụ 037 (ID 37): DC, Port=2 (CCS2)
(37, 1, 'CCS2', 'Available'), (37, 2, 'CCS2', 'Available'),
-- Trụ 038 (ID 38): DC, Port=1 (CCS2)
(38, 1, 'CCS2', 'Available'),
-- Trụ 039 (ID 39): AC, Port=1 (AC-2P cho Xe máy)
(39, 1, 'AC-2P', 'Available'),
-- Trụ 040 (ID 40): AC, Port=2 (Type 2)
(40, 1, 'Type 2', 'Available'), (40, 2, 'Type 2', 'Available'),
-- Trụ 041 (ID 41): DC, Port=1 (CCS2)
(41, 1, 'CCS2', 'Available'),
-- Trụ 042 (ID 42): DC, Port=2 (CCS2)
(42, 1, 'CCS2', 'Available'), (42, 2, 'CCS2', 'Available'),
-- Trụ 043 (ID 43): AC, Port=2 (Type 2)
(43, 1, 'Type 2', 'Available'), (43, 2, 'Type 2', 'Available'),
-- Trụ 044 (ID 44): AC, Port=1 (AC-2P cho Xe máy)
(44, 1, 'AC-2P', 'Available'),
-- Trụ 045 (ID 45): DC, Port=1 (CCS2)
(45, 1, 'CCS2', 'Available'),
-- Trụ 046 (ID 46): DC, Port=1 (CCS2)
(46, 1, 'CCS2', 'Available'),
-- Trụ 047 (ID 47): AC, Port=2 (Type 2)
(47, 1, 'Type 2', 'Available'), (47, 2, 'Type 2', 'Available'),
-- Trụ 048 (ID 48): AC, Port=1 (AC-2P cho Xe máy)
(48, 1, 'AC-2P', 'Available'),
-- Trụ 049 (ID 49): DC, Port=2 (CCS2)
(49, 1, 'CCS2', 'Available'), (49, 2, 'CCS2', 'Available'),
-- Trụ 050 (ID 50): DC, Port=1 (CCS2)
(50, 1, 'CCS2', 'Available'),

-- ChargerId 51 - 57 (Từ dữ liệu V-Green mới nhất)
-- VF-DC-300KW-01 (ID 51): DC, Port=1 (CCS2)
(51, 1, 'CCS2', 'Available'),
-- VF-DC-30KW-02 (ID 52): DC, Port=1 (CCS2)
(52, 1, 'CCS2', 'Available'),
-- VF-AC-11KW-03 (ID 53): AC, Port=1 (Type 2)
(53, 1, 'Type 2', 'Available'),
-- VF-MOTO-1.2KW-04 (ID 54): AC, Port=4 (AC-2P)
(54, 1, 'AC-2P', 'Available'), (54, 2, 'AC-2P', 'Available'), (54, 3, 'AC-2P', 'Available'), (54, 4, 'AC-2P', 'Available'),
-- VF-DC-300KW-05 (ID 55): DC, Port=1 (CCS2)
(55, 1, 'CCS2', 'Available'),
-- VF-AC-11KW-06 (ID 56): AC, Port=1 (Type 2)
(56, 1, 'Type 2', 'Available'),
-- VF-DC-30KW-07 (ID 57): DC, Port=1 (CCS2)
(57, 1, 'CCS2', 'Available');
use ChargingPoint
select * from ReportType;
alter table ReportType alter column  ReportTypeName nvarchar(max) null;
delete from ReportType;
INSERT INTO ReportType (ReportTypeCode, ReportTypeName, ReportCategory, EntryType)
VALUES 
-- Báo cáo Tài chính (FINANCIAL REPORTS - Thường là SUMMARY)
('B01a-DN', N'Bảng cân đối kế toán giữa niên độ', 'FINANCIAL', 'SUMMARY'),
('B02a-DN', N'Báo cáo kết quả hoạt động kinh doanh', 'FINANCIAL', 'SUMMARY'),
('B03a-DN', N'Báo cáo lưu chuyển tiền tệ (Trực tiếp)', 'FINANCIAL', 'SUMMARY'),
('B03a-DN-GT', N'Báo cáo lưu chuyển tiền tệ (Gián tiếp)', 'FINANCIAL', 'SUMMARY'),

-- Sổ sách Kế toán (JOURNAL / LEDGER)
('G01-DN', N'Sổ Nhật ký chung (Bản tổng hợp bút toán)', 'JOURNAL', 'SUMMARY'),
('G02-DN', N'Sổ Nhật ký chung (Bản chi tiết bút toán)', 'JOURNAL', 'DETAIL');
INSERT INTO ReportType (ReportTypeCode, ReportTypeName, ReportCategory, EntryType)
VALUES 
('S131', N'Sổ chi tiết Phải thu khách hàng (TK 131)', 'LEDGER', 'DETAIL'),
('S511', N'Sổ chi tiết Doanh thu bán hàng (TK 511)', 'LEDGER', 'DETAIL'),

-- Báo cáo Quản trị (MANAGEMENT)
('MGMT-01', N'Báo cáo Tuổi nợ phải thu (Aging Report)', 'MANAGEMENT', 'SUMMARY'),
('MGMT-02', N'Báo cáo Doanh thu theo thời gian', 'MANAGEMENT', 'SUMMARY'),
('MGMT-03', N'Báo cáo Doanh thu theo Trạm sạc', 'MANAGEMENT', 'SUMMARY');