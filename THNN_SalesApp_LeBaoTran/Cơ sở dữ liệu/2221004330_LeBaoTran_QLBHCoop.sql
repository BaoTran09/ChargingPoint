create database QLBH_Coop
use  QLBH_Coop
drop database QLBH_Coop
SELECT DAY(NgayLapBT) AS Ngay, SUM(SoTien) AS DoanhThu
FROM ButToan
WHERE TKCo = '511' AND MONTH(NgayLapBT) = 9
GROUP BY DAY(NgayLapBT)
ORDER BY Ngay

select * from ButToan
select * from DmNhanVien
sp_rename 'ChungTu.NgayinHD','Ngayin','column';
drop table DmNghiepVu
----------------------------
---SLBan của hàng hóa trong bảng CTHD luôn lớn hơn 0.
CREATE TRIGGER tg_SLB
ON CtChungTu
FOR INSERT, UPDATE 
AS
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM inserted 
        WHERE SLBan <= 0
    )
    BEGIN
        PRINT  N'Bạn nhớ thêm số lượng hàng hóa nhé!'
	ROLLBACK
	END
END

 INSERT INTO CtChungTu(MaCT, MaHH, GhiChu, DVT, DGia, SLBan) 
VALUES
-- Hóa đơn HD001
('HD042', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 0);



---TẠO TRIGGER GIỚI HẠN SỐ LƯỢNG BÁN BẰNG HOẶC NHỎ HƠN SỐ LƯỢNG TỒN CỦA HÀNG HÓA
CREATE TRIGGER tg_uiSLBan_Ton
ON CtChungTu
FOR INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN TonKho h ON i.MaHH = h.MaHH
        WHERE i.SLBan > h.SLTon 
		)
    BEGIN
        PRINT N'Số lượng bán không được vượt quá số lượng tồn của hàng hóa.'
        ROLLBACK 
    END
END

select * from DmHangHoa where Ma = 'HH002';

 INSERT INTO CtChungTu(MaCT, MaHH, GhiChu, DVT, DGia, SLBan) 
VALUES

('HD042', 'HH002', N'Nước ngọt Coca-Cola', N'Kg', 150000, 300);



----TẠO TRIGGER KIỂM TRA TRÙNG LẶP DỮ LIỆU CỦA KHÁCH HÀNG TRƯỚC KHI THÊM 
CREATE TRIGGER tg_KiemTraTrungLap
ON DmKhachHang
FOR INSERT
AS
BEGIN
    DECLARE  @SDT NVARCHAR(10);
    SELECT @SDT = SDT FROM inserted;
    IF EXISTS (SELECT * FROM DmKhachHang WHERE SDT = @SDT)
    BEGIN
	PRINT N'Số điện thoại đã được đăng ký!'
	ROLLBACK
    END
    INSERT INTO DmKhachHang (Ma, Ten, GioiTinh, SDT, DiaChi)
    SELECT * FROM inserted
END
delete from DmKhachHang where SDT='0123456789';
INSERT INTO 
VALUES
('KH020', N'Nguyễn Văn An', 0, '0123456789', N'12 Nguyễn Trãi, Q.1, TP.HCM');


RESTORE HEADERONLY FROM DISK = 'D:\Downloads\Backup_1G108VXTSalaUFM_20250319_090846.bak'

SELECT name FROM sys.databases;

SELECT name, state_desc FROM sys.databases WHERE name = 'QLBH_Coop';

CREATE TABLE DmCoSo
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,  
    Ten NVARCHAR(64) NULL,               
    DiaChi NVARCHAR(256) NULL             
);
alter table TKCT alter column MoTa nvarchar(128) null
CREATE TABLE DmLoaiHH
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    Ten NVARCHAR(128) NULL                
);
select * from Account
alter table DmHangHoa drop column DVT
EXEC sp_help 'DmHangHoa'
select* from TonKho

select* from DmHangHoa
CREATE TABLE DmHangHoa
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    Ten NVARCHAR(128) NULL,                 
    HinhAnh NVARCHAR(128) NULL,           
    MoTa NVARCHAR(128) NULL,             
    MaLoaiHH NVARCHAR(8) NOT NULL,
    MaCS NVARCHAR(8) NOT NULL,
    
    CONSTRAINT FK_DMHH_MaLHH FOREIGN KEY (MaLoaiHH) REFERENCES DmLoaiHH(Ma),
    CONSTRAINT FK_DMHH_MaCS FOREIGN KEY (MaCS) REFERENCES DmCoSo(Ma)
);


create table TonKho
(
	MaHH NVARCHAR(8)  NOT NULL,
	SLTon INT NULL,
	TgCapNhat DATETIME NULL,

	CONSTRAINT FK_TonKHo_MaHH FOREIGN KEY (MaHH) REFERENCES DmHangHoa(Ma)
);



CREATE TABLE DmKhachHang
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    Ten NVARCHAR(64) NULL,
    GioiTinh BIT,
    SDT NVARCHAR(16) NULL,               
    DiaChi NVARCHAR(256) NULL,
    ThoiGianDK DATETIME NULL
);


CREATE TABLE Account 
(
    UserName NVARCHAR(16) PRIMARY KEY NOT NULL,  
    PassW NVARCHAR(16) NOT NULL                 
);

CREATE TABLE DmNhanVien
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    Ten NVARCHAR(64) NULL,
    GioiTinh BIT,
    SDT NVARCHAR(16) NULL,
    DiaChi NVARCHAR(128) NULL,
    ChucVu NVARCHAR(64) NULL,                  
    MaCS NVARCHAR(8) NOT NULL,
    UserName NVARCHAR(16) NOT NULL,
    
    CONSTRAINT FK_NhanVien_MaCS FOREIGN KEY (MaCS) REFERENCES DmCoSo(Ma),
    CONSTRAINT FK_NhanVien_UserName FOREIGN KEY (UserName) REFERENCES Account(UserName)
);
CREATE TABLE DmLoaiCT
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    Ten NVARCHAR(128) NULL                
);
alter table ChungTu add  GiamGia float null,
CREATE TABLE DmHangHoa
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    Ten NVARCHAR(128) NULL,                 
    HinhAnh NVARCHAR(128) NULL,           
    MoTa NVARCHAR(128) NULL,             
    MaLoaiHH NVARCHAR(8) NOT NULL,
    MaCS NVARCHAR(8) NOT NULL,
    
    CONSTRAINT FK_DMHH_MaLHH FOREIGN KEY (MaLoaiHH) REFERENCES DmLoaiHH(Ma),
    CONSTRAINT FK_DMHH_MaCS FOREIGN KEY (MaCS) REFERENCES DmCoSo(Ma)
);

CREATE TABLE DmKhachHang
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    Ten NVARCHAR(64) NULL,
    GioiTinh BIT,
    SDT NVARCHAR(16) NULL,               
    DiaChi NVARCHAR(256) NULL,
    ThoiGianDK DATETIME NULL
);



CREATE TABLE DmNhanVien
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    Ten NVARCHAR(64) NULL,
    GioiTinh BIT,
    SDT NVARCHAR(16) NULL,
    DiaChi NVARCHAR(128) NULL,
    ChucVu NVARCHAR(64) NULL,                  
    MaCS NVARCHAR(8) NOT NULL,
    UserName NVARCHAR(16) NOT NULL,
    
    CONSTRAINT FK_NhanVien_MaCS FOREIGN KEY (MaCS) REFERENCES DmCoSo(Ma),
    CONSTRAINT FK_NhanVien_UserName FOREIGN KEY (UserName) REFERENCES Account(UserName)
);
CREATE TABLE ChungTu
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
	MaLoaiCT NVARCHAR(8) NOT NULL,
	MaNV NVARCHAR(8) NOT NULL,
    MaKH NVARCHAR(8) NOT NULL,
	PTTT NVARCHAR(32) NULL,
    Ngayin DATE,
    GiamGia float null,

    CONSTRAINT FK_ChungTu_MaLoaiCT FOREIGN KEY (MaLoaiCT) REFERENCES DmLoaiCT(Ma),    
    CONSTRAINT FK_ChungTu_MaNV FOREIGN KEY (MaNV) REFERENCES DmNhanVien(Ma),
    CONSTRAINT FK_ChungTu_MaKH FOREIGN KEY (MaKH) REFERENCES DmKhachHang(Ma)
);

CREATE TABLE CtChungTu
(
    MaCT NVARCHAR(8) NOT NULL,
    MaHH NVARCHAR(8) NOT NULL,
    GhiChu NVARCHAR(128) NULL,               
    DVT NVARCHAR(32) NULL,
    DGia FLOAT NULL,
    SLban INT NULL,

    CONSTRAINT PK_CtCT PRIMARY KEY (MaCT, MaHH),
    CONSTRAINT FK_CtCT_MaCT FOREIGN KEY (MaCT) REFERENCES ChungTu(Ma),
    CONSTRAINT FK_CtCT_MaHH FOREIGN KEY (MaHH) REFERENCES DmHangHoa(Ma)
);
alter table DmHangHoa add DVT NVARCHAR(32) NULL,
--------------------
create table TKTH
(
Ma nvarchar(8) primary key not null,
Ten nvarchar (128) null,
)


create table TKCT
(
Ma nvarchar(8) primary key not null,
Ten nvarchar(128) null,
MaTKTH nvarchar(8) not null,
MoTa nvarchar(150) null,
constraint FK_TKCT_MaTKTH foreign key (MaTKTH) references TKTH(Ma)
)

create table SoDuDK
(
MaTKCT nvarchar(8) not null,
MoTa nvarchar(128) null,
NgayDK date not null,
DuNo float null,
DuCo float null,
constraint PK_SoDuDK primary key (MaTKCT,NgayDK),
constraint FK_SoDuDK_MaTKCT foreign key (MaTKCT) references TKCt(Ma)


create table ButToan
(
MaBT nvarchar(8) primary key not null,
MaTKCT nvarchar(8) not null,
MaCT nvarchar(8) not null,
TKNo nvarchar(8) null,
TKCo nvarchar(8) null,
NgayLapBT date null,
DienGiai nvarchar(128) null,
SoTien float null,
constraint FK_ButToan_MaTKCT foreign key (MaTKCT) references TKCT(Ma),
constraint FK_ButToan_MaCT foreign key (MaCT) references ChungTu(Ma)
)
drop database QLBH_Coop

-- Lấy danh sách tài khoản tổng hợp
SELECT TKTH.Ma AS SHTK, TKTH.Ten AS TenTaiKhoan
FROM TKTH
ORDER BY TKTH.Ma;
use QLBH_Coop
-- Lấy số dư đầu kỳ từ SoDuDK
SELECT TKTH.Ma AS SHTK, 
       SUM(ISNULL(SoDuDK.DuNo, 0)) AS DuNoDauKy, 
       SUM(ISNULL(SoDuDK.DuCo, 0)) AS DuCoDauKy
FROM TKTH
LEFT JOIN TKCT ON TKTH.Ma = TKCT.MaTKTH
LEFT JOIN SoDuDK ON TKCT.Ma = SoDuDK.MaTKCT
GROUP BY TKTH.Ma;

-- Lấy tổng phát sinh trong kỳ từ ButToan



-- Tính số dư cuối kỳ
SELECT SHTK, 
       DuNoDauKy + TongPhatSinhNo - TongPhatSinhCo AS DuNoCuoiKy,
       DuCoDauKy + TongPhatSinhCo - TongPhatSinhNo AS DuCoCuoiKy
FROM (
    -- Kết hợp số dư đầu kỳ và phát sinh
    SELECT TKTH.Ma AS SHTK, 
           SUM(ISNULL(SoDuDK.DuNo, 0)) AS DuNoDauKy, 
           SUM(ISNULL(SoDuDK.DuCo, 0)) AS DuCoDauKy,
           SUM(CASE WHEN ButToan.TKNo IS NOT NULL THEN 1 ELSE 0 END * ISNULL(ButToan.DienGiai, 0)) AS TongPhatSinhNo,
           SUM(CASE WHEN ButToan.TKCo IS NOT NULL THEN 1 ELSE 0 END * ISNULL(ButToan.DienGiai, 0)) AS TongPhatSinhCo
    FROM TKTH
    LEFT JOIN TKCT ON TKTH.Ma = TKCT.MaTKTH
    LEFT JOIN SoDuDK ON TKCT.Ma = SoDuDK.MaTKCT
    LEFT JOIN ButToan ON TKCT.Ma = ButToan.MaTKCT
    GROUP BY TKTH.Ma
) AS BangCanDoi;

UPDATE B
SET SoTien = CTT.TongTien
FROM ButToan B
JOIN (
    SELECT CT.MaCT, SUM(CT.SLBan * CT.DGia) - COALESCE(C.GiamGia, 0) AS TongTien
    FROM CtChungTu CT
    LEFT JOIN ChungTu C ON CT.MaCT = C.Ma
    GROUP BY CT.MaCT, C.GiamGia
) CTT ON B.MaCT = CTT.MaCT;
select * from  ButToan;
EXEC sp_help ButToan;

SELECT 
    C.Ma AS MaCT,
    '5211' AS TKNo,
    B.TKCo,  
    CAST(ISNULL(C.GiamGia, 0) AS FLOAT) AS SoTien,  
    N'Ghi nhận giảm giá bán hàng' AS DienGiai,
    GETDATE() AS NgayLapBT
FROM ChungTu C
LEFT JOIN ButToan B ON C.Ma = B.MaCT
WHERE C.GiamGia > 0;

-- Hóa đơn bán hàng (CT1): Ghi nhận doanh thu và thuế GTGT
INSERT INTO ButToan (MaCT, NgayLapBT, TKNo, TKCo, SoTien, DienGiai)
SELECT 
    ct.MaCT,
    c.NgayLapCT,
    '131' AS TKNo,   -- Phải thu khách hàng
    '5111' AS TKCo,  -- Doanh thu bán hàng
    ct.ThanhTien AS SoTien,
    'Doanh thu bán hàng theo hóa đơn ' + ct.MaCT AS DienGiai
FROM CtChungTu ct
JOIN ChungTu c ON ct.MaCT = c.MaCT
WHERE c.LoaiCT = 'CT1';

-- Ghi nhận thuế GTGT đầu ra
INSERT INTO ButToan (MaCT, NgayLapBT, TKNo, TKCo, SoTien, DienGiai)
SELECT 
    ct.MaCT,
    c.NgayLapCT,
    '131' AS TKNo,   -- Phải thu khách hàng
    '3331' AS TKCo,  -- Thuế GTGT đầu ra
    ct.ThueGTGT AS SoTien,
    'Thuế GTGT đầu ra theo hóa đơn ' + ct.MaCT AS DienGiai
FROM CtChungTu ct
JOIN ChungTu c ON ct.MaCT = c.MaCT
WHERE c.LoaiCT = 'CT1';

-- Phiếu thu tiền (CT2): Nhận tiền từ khách hàng
INSERT INTO ButToan (MaCT, NgayLapBT, TKNo, TKCo, SoTien, DienGiai)
SELECT 
    c.MaCT,
    c.NgayLapCT,
    '1121' AS TKNo,  -- Tiền mặt hoặc ngân hàng
    '131' AS TKCo,   -- Phải thu khách hàng
    c.TongTien AS SoTien,
    'Thu tiền từ khách hàng theo phiếu thu ' + c.MaCT AS DienGiai
FROM ChungTu c
WHERE c.LoaiCT = 'CT2';


select * from TKTH where TKNo='5211'

exec sp_rename KhachHang,DmKhachHang

--------------------

create view vw_BanHang112
as
select hd.Ma,ct.MaHH,hd.PTTT,nv.TKNo,nv.TKCo 
from HoaDon hd join ctHoaDon ct on hd.Ma=ct.MaHD
join DmNghiepVu nv on nv.Ma=ct.MaNVu
where hd.PTTT= 'Tiền mặt'

 
------
create proc TriGia @MaHD nvarchar(5)
as begin
select SLBan*DonGia
from CTHD ct join DMHangHoa
---------------------
-----CỞ SỞ 
INSERT INTO DmCoSo (Ma, Ten, DiaChi)
VALUES
('CS001', N'Cơ sở Co.opmart Biên Hòa', N'121 Võ Thị Sáu, Biên Hòa, Đồng Nai'),
('CS002', N'Cơ sở Co.opmart Bình Dương', N'500 Đại Lộ Bình Dương, Thủ Dầu Một, Bình Dương'),
('CS003', N'Cơ sở Co.opmart Bình Dương', N'500 Đại Lộ Bình Dương, Thủ Dầu Một, Bình Dương');
------LOẠI HÀNG HÓA
INSERT INTO DmLoaiHH (Ma, Ten)
VALUES
('LHH01', N'Thực phẩm tươi sống'),
('LHH02', N'Đồ uống'),
('LHH03', N'Hóa mỹ phẩm'),
('LHH04', N'Thực phẩm khô'),
('LHH05', N'Sữa và các sản phẩm từ sữa'),
('LHH06', N'Đồ gia dụng'),
('LHH07', N'Quần áo và thời trang'),
('LHH08', N'Điện gia dụng'),
('LHH09', N'Sách và văn phòng phẩm'),
('LHH10', N'Đồ chơi trẻ em');

------DANH MỤC HÀNG HÓA
---------NẾU CÓ CHỈNH CỘT "MOTA" TỪ NVARCHAR(200) -> NVARCHAR(MAX) = JSON THÌ NHỚ CHỈNH SỬA DỮ LIỆU
INSERT INTO DmHangHoa (Ma, Ten,  HinhAnh,MoTa,MaLoaiHH, MaCS) 
VALUES
('HH001', N'Thịt heo ba rọi', N'Kg', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\thit_heo_ba_roi.jpg', '', 'LHH01', 'CS001'),
('HH002', N'Nước ngọt Coca-Cola', N'Chai', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\coca_cola.jpg', N'{"Loại": "Chai"; "Size":"20ml"}', 'LHH02', 'CS001'),
('HH003', N'Bột giặt OMO 3kg', N'Kg', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\bot_giat_omo.jpg', '', 'LHH03', 'CS001'),
('HH004', N'Mì gói Hảo Hảo', N'Gói', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\mi_goi_hao_hao.jpg', '', 'LHH04', 'CS001'),
('HH005', N'Sữa Vinamilk 1L', N'Chai', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\sua_vinamilk.jpg', '', 'LHH05', 'CS001'),
('HH006', N'Nồi cơm điện Sharp', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\noi_com_dien_sharp.jpg', '', 'LHH06', 'CS001'),
('HH007', N'Áo sơ mi nam', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\ao_so_mi_nam.jpg', N'Size L, màu xanh', 'LHH07', 'CS001'),
('HH008', N'Quạt điện Toshiba', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\quat_dien_toshiba.jpg', '', 'LHH08', 'CS001'),
('HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\sach_giao_khoa_lop_12.jpg', '', 'LHH09', 'CS001'),
('HH010', N'Lego Creator 3in1', N'Bộ', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\lego_creator.jpg', '', 'LHH10', 'CS001'),

('HH011', N'Cá hồi phi lê', N'Kg', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\ca_hoi_phi_le.jpg', '', 'LHH01', 'CS002'),
('HH012', N'Nước tăng lực Red Bull', N'Lon', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\red_bull.jpg', '', 'LHH02', 'CS002'),
('HH013', N'Kem đánh răng Colgate', N'Tuýp', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\kem_danh_rang_colgate.jpg', '', 'LHH03', 'CS002'),
('HH014', N'Đậu phộng rang muối', N'Gói', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\dau_phong_rang_muoi.jpg', '', 'LHH04', 'CS002'),
('HH015', N'Phô mai con bò cười', N'Hộp', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\pho_mai_con_bo_cuoi.jpg', '', 'LHH05', 'CS002'),
('HH016', N'Chảo chống dính Sunhouse', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\chao_chong_dinh_sunhouse.jpg', '', 'LHH06', 'CS002'),
('HH017', N'Áo khoác nữ mùa đông', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\ao_khoac_nu_mua_dong.jpg', N'Size L, màu hồng', 'LHH07', 'CS002'),
('HH018', N'Máy ép trái cây Panasonic', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\may_ep_trai_cay_panasonic.jpg', '', 'LHH08', 'CS002'),
('HH019', N'Bút bi Thiên Long', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\but_bi_thien_long.jpg', '', 'LHH09', 'CS002'),
('HH020', N'Búp bê Barbie', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\bup_be_barbie.jpg', '', 'LHH10', 'CS002');
-------------------
insert TonKHo (MaHH, SLTon,TgCapNhat ) values
('HH001', 100, '2024-09-09'),
('HH002', 200, '2024-09-09'),
('HH003', 150, '2024-09-09'),
('HH004', 120, '2024-09-09'),
('HH005', 180, '2024-09-09'),
('HH006', 80, '2024-09-09'),
('HH007', 90, '2024-09-09'),
('HH008', 70, '2024-09-09'),
('HH009', 50, '2024-09-09'),
('HH010', 110, '2024-09-09'),

('HH011', 130, '2024-09-09'),
('HH012', 210, '2024-09-09'),
('HH013', 160, '2024-09-09'),
('HH014', 140, '2024-09-09'),
('HH015', 190, '2024-09-09'),
('HH016', 85, '2024-09-09'),
('HH017', 95, '2024-09-09'),
('HH018', 75, '2024-09-09'),
('HH019', 55, '2024-09-09'),
('HH020', 115, '2024-09-09');


alter table TonKHo add TgCapNhat datetime null;
update TgCapNhat = '2024-09-09' from  TonKHo where MaHH='HH001';


INSERT INTO DmKhachHang (Ma, Ten, GioiTinh, SDT, DiaChi)
VALUES
('KH001', N'Nguyễn Văn An', 0, '0909123456', N'12 Nguyễn Trãi, Q.1, TP.HCM'),
('KH002', N'Trần Thị Bình', 1, '0912123456', N'25 Lê Lợi, Q.3, TP.HCM'),
('KH003', N'Lê Hoàng Công', 0, '0903123456', N'56 Lý Tự Trọng, Q.1, TP.HCM'),
('KH004', N'Phạm Ngọc Diễm', 1, '0907123456', N'78 Nguyễn Đình Chiểu, Q.3, TP.HCM'),
('KH005', N'Võ Thị Thảo', 1, '0932123456', N'89 Điện Biên Phủ, Q.10, TP.HCM'),
('KH006', N'Hoàng Văn Thắng', 0, '0933123456', N'23 Cách Mạng Tháng Tám, Q.1, TP.HCM'),
('KH007', N'Nguyễn Thị Giang', 1, '0943123456', N'45 Võ Văn Tần, Q.3, TP.HCM'),
('KH008', N'Đỗ Văn Hoàng', 0, '0917123456', N'67 Nguyễn Văn Trỗi, Q.Phú Nhuận, TP.HCM'),
('KH009', N'Phan Thị Thúy', 1, '0908123456', N'89 Pasteur, Q.1, TP.HCM'),
('KH010', N'Lý Văn Dũng', 0, '0928123456', N'123 Nam Kỳ Khởi Nghĩa, Q.3, TP.HCM');

SELECT * FROM sys.procedures WHERE name = 'ThemNhanVien'

--------ACCOUNT
INSERT INTO Account (UserName, PassW)
VALUES
('QL_Mai','123'),
('TN_Tung','1234'),
('TN_Lan','1234'),
('TN_Huy','1234'),
('TN_Hanh','1234'),
('TN_Tai','1234'),
('TN_Tran','1234'),
('KT_An','12345'),
('AD_Huong','123456'),
('TN_Khue','1234');
INSERT INTO Account (UserName, PassW)
VALUES
('TN_Tam','123');
INSERT INTO Account (UserName, PassW)
VALUES
('TN_Hang','123');

----NHÂN VIÊN
INSERT INTO DmNhanVien (Ma, Ten, GioiTinh, SDT, DiaChi, ChucVu, MaCS, UserName)
VALUES
('NV001', N'Nguyễn Thị Mai', 1, '0908234567', N'36 Lý Thái Tổ, Q.10, TP.HCM', N'Nhân viên thu ngân', 'CS001', 'QL_Mai'),
('NV002', N'Trần Văn An', 0, '0919234567', N'01 Nguyễn Kiệm, Q.Phú Nhuận, TP.HCM', N'Nhân viên thu ngân', 'CS001', 'AD_Huong'),
('NV003', N'Lê Thị Hương', 1, '0931234567', N'189C Cống Quỳnh, Q.1, TP.HCM', N'Nhân viên thu ngân', 'CS001', 'AD_Huong'),
('NV004', N'Phạm Văn Tùng', 0, '0941234567', N'69 Đinh Tiên Hoàng, Q.Bình Thạnh, TP.HCM', N'Nhân viên thu ngân', 'CS001', 'TN_Tung'),
('NV005', N'Võ Thị Lan', 1, '0951234567', N'191 Xa Lộ Hà Nội, Q.2, TP.HCM', N'Nhân viên thu ngân', 'CS001', 'TN_Lan'),

('NV006', N'Nguyễn Văn Huy', 0, '0961234567', N'385 Nguyễn Ảnh Thủ, Q.12, TP.HCM', N'Quản lý', 'CS002', 'TN_Huy'),
('NV007', N'Đặng Thị Hạnh', 1, '0971234567', N'97 Cách Mạng Tháng Tám, Bà Rịa', N'Kế toán', 'CS002', 'TN_Hanh'),
('NV008', N'Hoàng Văn Tài', 0, '0981234567', N'36 Nguyễn Thái Học, TP.Vũng Tàu', N'Nhân viên kỹ thuật', 'CS002', 'TN_Tai');
----------
/*
INSERT INTO DmNghiepVu (Ma, MoTa, TKNo, TKCo) VALUES
('NVU01', 'Bán hàng thu tiền mặt', 1111, 5111),
('NVU02', 'Bán hàng thu tiền qua tài khoản ngân hàng', 1121, 5111),
('NVU03', 'Bán hàng chưa thu tiền', 131, 5111);
*/
use QLBH_Coop
delete from DmLoaiCT

insert into DmLoaiCT(Ma, Ten) values
('CT1',N'Hóa đơn bán lẻ'),
('CT2',N'Phiếu thu tiền');
select * from ButToan
delete from ChungTu where Ma like 'PT%'
delete from DmLoaiCT
delete from ChungTu
delete from ButToan
DELETE FROM CtChungTu;
sp_help 'DmLoaiCT'
INSERT INTO DmHangHoa (Ma, Ten,  HinhAnh,MoTa,MaLoaiHH, MaCS) 
VALUES
('HH001', N'Thịt heo ba rọi', N'Kg', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\thit_heo_ba_roi.jpg', '', 'LHH01', 'CS001'),
('HH002', N'Nước ngọt Coca-Cola', N'Chai', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\coca_cola.jpg', N'{"Loại": "Chai"; "Size":"20ml"}', 'LHH02', 'CS001'),
('HH003', N'Bột giặt OMO 3kg', N'Kg', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\bot_giat_omo.jpg', '', 'LHH03', 'CS001'),
('HH004', N'Mì gói Hảo Hảo', N'Gói', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\mi_goi_hao_hao.jpg', '', 'LHH04', 'CS001'),
('HH005', N'Sữa Vinamilk 1L', N'Chai', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\sua_vinamilk.jpg', '', 'LHH05', 'CS001'),
('HH006', N'Nồi cơm điện Sharp', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\noi_com_dien_sharp.jpg', '', 'LHH06', 'CS001'),
('HH007', N'Áo sơ mi nam', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\ao_so_mi_nam.jpg', N'Size L, màu xanh', 'LHH07', 'CS001'),
('HH008', N'Quạt điện Toshiba', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\quat_dien_toshiba.jpg', '', 'LHH08', 'CS001'),
('HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\sach_giao_khoa_lop_12.jpg', '', 'LHH09', 'CS001'),
('HH010', N'Lego Creator 3in1', N'Bộ', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\lego_creator.jpg', '', 'LHH10', 'CS001'),

('HH011', N'Cá hồi phi lê', N'Kg', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\ca_hoi_phi_le.jpg', '', 'LHH01', 'CS002'),
('HH012', N'Nước tăng lực Red Bull', N'Lon', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\red_bull.jpg', '', 'LHH02', 'CS002'),
('HH013', N'Kem đánh răng Colgate', N'Tuýp', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\kem_danh_rang_colgate.jpg', '', 'LHH03', 'CS002'),
('HH014', N'Đậu phộng rang muối', N'Gói', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\dau_phong_rang_muoi.jpg', '', 'LHH04', 'CS002'),
('HH015', N'Phô mai con bò cười', N'Hộp', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\pho_mai_con_bo_cuoi.jpg', '', 'LHH05', 'CS002'),
('HH016', N'Chảo chống dính Sunhouse', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\chao_chong_dinh_sunhouse.jpg', '', 'LHH06', 'CS002'),
('HH017', N'Áo khoác nữ mùa đông', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\ao_khoac_nu_mua_dong.jpg', N'Size L, màu hồng', 'LHH07', 'CS002'),
('HH018', N'Máy ép trái cây Panasonic', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\may_ep_trai_cay_panasonic.jpg', '', 'LHH08', 'CS002'),
('HH019', N'Bút bi Thiên Long', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\but_bi_thien_long.jpg', '', 'LHH09', 'CS002'),
('HH020', N'Búp bê Barbie', N'Cái', 'C:\\Users\\trann\\Downloads\\SaleCoop\\MediaCoop\\bup_be_barbie.jpg', '', 'LHH10', 'CS002');
-------------------

INSERT INTO ChungTu (Ma, MaLoaiCT, MaNV, MaKH, PTTT, Ngayin, GiamGia) VALUES
-- Hóa đơn bán hàng (CT1)
('HD001', 'CT1', 'NV001', 'KH001', N'CK Agribank', '2024-09-09', 0),
('HD002', 'CT1', 'NV001', 'KH002', N'Tiền mặt', '2024-09-09', 0),
('HD003', 'CT1', 'NV002', 'KH003', N'Tiền mặt', '2024-09-10', 0),
('HD004', 'CT1', 'NV002', 'KH004', N'Tiền mặt', '2024-09-10', 0),
('HD005', 'CT1', 'NV003', 'KH005', N'Tiền mặt', '2024-09-11', 0),
('HD006', 'CT1', 'NV003', 'KH006', N'Tiền mặt', '2024-09-11', 0),
('HD007', 'CT1', 'NV004', 'KH007', N'CK Vietcombank', '2024-09-12', 0),
('HD008', 'CT1', 'NV004', 'KH008', N'CK Vietcombank', '2024-09-12', 0),
('HD009', 'CT1', 'NV005', 'KH009', N'CK Vietcombank', '2024-09-13', 0),
('HD010', 'CT1', 'NV005', 'KH010', N'CK Agribank', '2024-09-13', 0);
INSERT INTO ChungTu (Ma, MaLoaiCT, MaNV, MaKH, PTTT, Ngayin, GiamGia) VALUES

-- Phiếu thu tiền (CT2)
('PT001', 'CT2', 'NV001', 'KH001', N'CK Agribank', '2024-09-09', 0),
('PT002', 'CT2', 'NV001', 'KH002', N'Tiền mặt', '2024-09-09', 0),
('PT003', 'CT2', 'NV002', 'KH003', N'Tiền mặt', '2024-09-10', 0),
('PT004', 'CT2', 'NV002', 'KH004', N'Tiền mặt', '2024-09-10', 0),
('PT005', 'CT2', 'NV003', 'KH005', N'Tiền mặt', '2024-09-11', 0),
('PT006', 'CT2', 'NV003', 'KH006', N'Tiền mặt', '2024-09-11', 0),
('PT007', 'CT2', 'NV004', 'KH007', N'CK Vietcombank', '2024-09-12', 0),
('PT008', 'CT2', 'NV004', 'KH008', N'CK Vietcombank', '2024-09-12', 0),
('PT009', 'CT2', 'NV005', 'KH009', N'CK Vietcombank', '2024-09-13', 0),
('PT010', 'CT2', 'NV005', 'KH010', N'CK Agribank', '2024-09-13', 0);


 INSERT INTO CtChungTu(MaCT, MaHH, GhiChu, DVT, DGia, SLBan) 
VALUES
-- Hóa đơn HD001
('HD001', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('HD001', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),
('HD001', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('HD001', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 4),
('HD001', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 3),

-- Hóa đơn HD002
('HD002', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('HD002', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 1),
('HD002', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 5),
('HD002', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 3),

-- Hóa đơn HD003
('HD003', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),
('HD003', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 2),
('HD003', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 4),
('HD003', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 2),

-- Hóa đơn HD004
('HD004', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('HD004', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 2),
('HD004', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 1),
('HD004', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 4),

-- Hóa đơn HD005
('HD005', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 1),
('HD005', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 3),
('HD005', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 2),
('HD005', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 5),

-- Hóa đơn HD006
('HD006', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),
('HD006', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 2),
('HD006', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 3),
('HD006', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 4),

-- Hóa đơn HD007
('HD007', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('HD007', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 1),
('HD007', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 5),
('HD007', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 3),

-- Hóa đơn HD008
('HD008', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 2),
('HD008', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 1),
('HD008', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 4),
('HD008', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 5),

-- Hóa đơn HD009
('HD009', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('HD009', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 1),
('HD009', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 3),
('HD009', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 4),

-- Hóa đơn HD010
('HD010', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 2),
('HD010', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 1),
('HD010', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 3),
('HD010', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 4);
INSERT INTO CtChungTu(MaCT, MaHH, GhiChu, DVT, DGia, SLBan) 
VALUES
-- Phiếu thu PT001
('PT001', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('PT001', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),
('PT001', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('PT001', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 4),
('PT001', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 3),

-- Phiếu thu PT002
('PT002', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('PT002', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 1),
('PT002', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 5),
('PT002', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 3),

-- Phiếu thu PT003
('PT003', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),
('PT003', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 2),
('PT003', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 4),
('PT003', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 2),

-- Phiếu thu PT004
('PT004', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('PT004', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 2),
('PT004', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 1),
('PT004', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 4),

-- Phiếu thu PT005
('PT005', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 1),
('PT005', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 3),
('PT005', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 2),
('PT005', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 5),

-- Phiếu thu PT006
('PT006', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),
('PT006', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 2),
('PT006', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 3),
('PT006', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 4),

-- Phiếu thu PT007
('PT007', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('PT007', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 1),
('PT007', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 5),
('PT007', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 3),

-- Phiếu thu PT008
('PT008', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 2),
('PT008', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 1),
('PT008', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 4),
('PT008', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 5),

-- Phiếu thu PT009
('PT009', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('PT009', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 1),
('PT009', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 3),
('PT009', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 4),

-- Phiếu thu PT010
('PT010', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 2),
('PT010', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 1),
('PT010', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 3),
('PT010', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 4);


-----

INSERT INTO TKTH (Ma, Ten)
VALUES
('111', N'Tiền mặt'),
('112', N'Tiền gửi ngân hàng'),
('113', N'Tiền đang chuyển'),
('131', N'Phải thu khách hàng'),
('133', N'Thuế GTGT được khấu trừ'),
('156', N'Hàng hóa'),
('157', N'Hàng gửi đi bán'),
('331', N'Phải trả người bán'),
('333', N'Thuế và các khoản phải nộp Nhà nước'),
('334', N'Phải trả người lao động'),
('335', N'Chi phí phải trả'),
('511', N'Doanh thu bán hàng và cung cấp dịch vụ'),
('515', N'Doanh thu hoạt động tài chính'),
('521', N'Các khoản giảm trừ doanh thu'),
('641', N'Chi phí bán hàng'),
('642', N'Chi phí quản lý doanh nghiệp'),
('711', N'Thu nhập khác'),
('811', N'Chi phí khác'),
('821', N'Chi phí thuế TNDN'),
('911', N'Xác định kết quả kinh doanh');

-----------
EXEC sp_dbcmptlevel 'QLBH_Coop', 110; -- Thay 'TênDatabase' bằng tên cơ sở dữ liệu của bạn

----------------


INSERT INTO TKCT (Ma, Ten, MaTKTH)
VALUES
-- Tiền mặt (111)
('1111', N'Tiền Việt Nam', '111'),
('1112', N'Ngoại tệ', '111'),

-- Tiền gửi ngân hàng (112)
('1121', N'Tiền Việt Nam gửi ngân hàng', '112'),
('1122', N'Ngoại tệ gửi ngân hàng', '112'),

-- Tiền đang chuyển (113)
('1131', N'Tiền chuyển qua bưu điện', '113'),
('1132', N'Tiền chuyển qua ngân hàng', '113'),

-- Phải thu khách hàng (131)
('1311', N'Phải thu khách hàng ngắn hạn', '131'),
('1312', N'Phải thu khách hàng dài hạn', '131'),

-- Thuế GTGT được khấu trừ (133)
('1331', N'Thuế GTGT của hàng hóa, dịch vụ', '133'),

-- Hàng hóa (156)
('1561', N'Giá mua hàng hóa', '156'),

-- Hàng gửi đi bán (157)
('1571', N'Hàng gửi đi bán chưa thu tiền', '157'),
('1572', N'Hàng gửi đi bán đã thu tiền', '157'),

-- Phải trả người bán (331)
('3311', N'Phải trả nhà cung cấp ngắn hạn', '331'),
('3312', N'Phải trả nhà cung cấp dài hạn', '331'),

-- Thuế và các khoản phải nộp Nhà nước (333)
('3331', N'Thuế GTGT phải nộp', '333'),
('3332', N'Thuế thu nhập doanh nghiệp phải nộp', '333'),
('3333', N'Thuế khác phải nộp', '333'),

-- Phải trả người lao động (334)
('3341', N'Lương nhân viên', '334'),
('3343', N'Các khoản phải trả khác cho nhân viên', '334'),

-- Doanh thu bán hàng và cung cấp dịch vụ (511)
('5111', N'Doanh thu bán hàng hóa', '511'),
('5112', N'Doanh thu bán thành phẩm', '511'),
('5113', N'Doanh thu cung cấp dịch vụ', '511'),
('5118', N'Doanh thu khác', '511'),

-- Các khoản giảm trừ doanh thu (521)
('5211', N'Giảm giá hàng bán', '521'),
('5212', N'Chiết khấu thương mại', '521'),

-- Chi phí quản lý doanh nghiệp (642)
('6421', N'Chi phí nhân viên quản lý', '642'),
('6422', N'Chi phí văn phòng phẩm', '642'),

-- Xác định kết quả kinh doanh (911)
('9111', N'Doanh thu và thu nhập', '911');
------
use QLBH_Coop
select * from TKCT 
INSERT INTO TKCT (Ma, Ten, MaTKTH, MoTa) VALUES
-- Tiền mặt (1111)
('1111.01', N'Tiền mặt tại quầy 1', '111', N'Tiền mặt lưu tại quầy bán hàng 1'),
('1111.02', N'Tiền mặt tại quầy 2', '111', N'Tiền mặt lưu tại quầy bán hàng 2'),

-- Tiền gửi ngân hàng (1121)
('1121.01', N'Tiền Việt Nam gửi tại Vietcombank', '112', N'Tiền VNĐ gửi tại ngân hàng Vietcombank'),
('1121.02', N'Tiền Việt Nam gửi tại BIDV', '112', N'Tiền VNĐ gửi tại ngân hàng BIDV'),
('1121.03', N'Tiền Việt Nam gửi tại Techcombank', '112', N'Tiền VNĐ gửi tại ngân hàng Techcombank'),

('1121.04', N'Tiền Việt Nam gửi tại AgriBank', '112', N'Tiền VNĐ gửi tại ngân hàng AgriBank'),


-- Phải thu khách hàng trong nước (1311)
('1311.01', N'Phải thu khách hàng mua lẻ', '131', N'Tiền phải thu từ khách hàng lẻ'),
-- Thuế GTGT được khấu trừ (1331)
('1331.01', N'Thuế GTGT của hàng hóa', '133', N'Thuế GTGT được khấu trừ từ hàng hóa bán ra'),
('1331.02', N'Thuế GTGT của dịch vụ', '133', N'Thuế GTGT được khấu trừ từ dịch vụ cung cấp'),

-- Hàng hóa (156)
('156.01', N'Hàng hóa bán ra', '156', N'Hàng hóa đã bán cho khách hàng'),


-- Phải trả người bán (331)
('331.01', N'Phải trả người bán MaSan', '331', N'Tiền phải trả cho nhà cung cấp MaSan'),
('331.02', N'Phải trả người bán Omaji', '331', N'Tiền phải trả cho nhà cung cấp Omaji'),
('331.03', N'Phải trả người bán Vinamilk', '331', N'Tiền phải trả cho nhà cung cấp Vinamilk'),

-- Doanh thu bán hàng và cung cấp dịch vụ (511)
('511.01', N'Doanh thu bán hàng hóa', '511', N'Doanh thu từ việc bán hàng hóa'),
('511.02', N'Doanh thu cung cấp dịch vụ', '511', N'Doanh thu từ việc cung cấp dịch vụ');
select * from TKCT
select * from SoDuDK
alter table SoDuDK drop column TenTKCT;
INSERT INTO SoDuDK (MaTKCT, NgayDK, DuNo, DuCo) VALUES
-- Tiền mặt (1111)
('1111.01', '2024-09-01', 50000000, 0),
('1111.02', '2024-09-01', 30000000, 0),

-- Tiền gửi ngân hàng (1121)
('1121.01', '2024-09-01', 200000000, 0),
('1121.02', '2024-09-01', 150000000, 0),
('1121.03','2024-09-01', 100000000, 0),
('1121.04', '2024-09-01', 25000000, 0),
-- Hàng hóa (156)
('156.01', '2024-09-01',  0,0);
INSERT INTO SoDuDK (MaTKCT, NgayDK, DuNo, DuCo) VALUES
-- Phải thu khách hàng (1311)
('1311.01', '2024-09-01', 120000000, 0),

-- Thuế GTGT được khấu trừ (1331)
('1331.01', '2024-09-01', 5000000, 0),
('1331.02', '2024-09-01', 7000000, 0),



-- Phải trả người bán (331)
('331.01', '2024-09-01', 0, 80000000),
('331.02', '2024-09-01', 0, 50000000),
('331.03', '2024-09-01', 0, 30000000),

-- Doanh thu bán hàng và cung cấp dịch vụ (511)
('511.01', '2024-09-01', 0, 200000000),
('511.02', '2024-09-01', 0, 150000000);

select *  from DmKhachHang;
----------------
-- Xóa dữ liệu cũ (nếu cần)
DELETE FROM ButToan;
UPDATE ButToan
SET SoTien = (
    SELECT SUM(CtChungTu.SLBan * CtChungTu.DGia) - ChungTu.GiamGia
    FROM CtChungTu
    JOIN ChungTu ON CtChungTu.MaCT = ChungTu.Ma
    WHERE ButToan.MaCT = ChungTu.Ma
    GROUP BY ChungTu.Ma, ChungTu.GiamGia
)
WHERE EXISTS (
    SELECT 1
    FROM CtChungTu
    JOIN ChungTu ON CtChungTu.MaCT = ChungTu.Ma
    WHERE ButToan.MaCT = ChungTu.Ma

insert into DmLoaiCT(Ma, Ten) values
('CT1',N'Hóa đơn bán lẻ'),
('CT2',N'Phiếu thu tiền');


select * from DmNhanVien;
INSERT INTO ChungTu (Ma, MaLoaiCT, MaNV, MaKH, PTTT, Ngayin, GiamGia) VALUES
('HD011', 'CT1', 'NV001', 'KH001', N'Tiền mặt tại quầy 1', '2024-09-01', 50000),
('PT011', 'CT2', 'NV001', 'KH001', N'Tiền mặt tại quầy 1', '2024-09-01', 50000),

('HD012', 'CT1', 'NV002', 'KH002', N'CK qua Vietcombank', '2024-09-02', 30000),
('PT012', 'CT2', 'NV002', 'KH002', N'CK qua Vietcombank', '2024-09-02', 30000),

('HD013', 'CT1', 'NV003', 'KH003', N'Tiền mặt tại quầy 2', '2024-09-03', 20000),
('PT013', 'CT2', 'NV003', 'KH003', N'Tiền mặt tại quầy 2', '2024-09-03', 20000),

('HD014', 'CT1', 'NV004', 'KH004', N'CK qua BIDV', '2024-09-04', 15000),
('PT014', 'CT2', 'NV004', 'KH004', N'CK qua BIDV', '2024-09-04', 15000),

('HD015', 'CT1', 'NV005', 'KH005', N'Tiền mặt tại quầy 1', '2024-09-05', 18000),
('PT015', 'CT2', 'NV005', 'KH005', N'Tiền mặt tại quầy 1', '2024-09-05', 18000),

('HD016', 'CT1', 'NV001', 'KH006', N'CK qua Techcombank', '2024-09-06', 22000),
('PT016', 'CT2', 'NV001', 'KH006', N'CK qua Techcombank', '2024-09-06', 22000),

('HD017', 'CT1', 'NV001', 'KH007', N'Tiền mặt tại quầy 2', '2024-09-07', 12000),
('PT017', 'CT2', 'NV001', 'KH007', N'Tiền mặt tại quầy 2', '2024-09-07', 12000),

('HD018', 'CT1', 'NV001', 'KH008', N'CK qua AgriBank', '2024-09-08', 25000),
('PT018', 'CT2', 'NV001', 'KH008', N'CK qua AgriBank', '2024-09-08', 25000),

('HD019', 'CT1', 'NV001', 'KH009', N'CK qua Vietcombank', '2024-09-09', 17000),
('PT019', 'CT2', 'NV001', 'KH009', N'CK qua Vietcombank', '2024-09-09', 17000),

('HD020', 'CT1', 'NV002', 'KH010', N'Tiền mặt tại quầy 1', '2024-09-10', 23000),
('PT020', 'CT2', 'NV002', 'KH010', N'Tiền mặt tại quầy 1', '2024-09-10', 23000),

('HD021', 'CT1', 'NV003', 'KH001', N'CK qua BIDV', '2024-09-11', 16000),
('PT021', 'CT2', 'NV003', 'KH001', N'CK qua BIDV', '2024-09-11', 16000),

('HD022', 'CT1', 'NV004', 'KH002', N'Tiền mặt tại quầy 2', '2024-09-12', 28000),
('PT022', 'CT2', 'NV004', 'KH002', N'Tiền mặt tại quầy 2', '2024-09-12', 28000),

('HD023', 'CT1', 'NV005', 'KH003', N'CK qua Techcombank', '2024-09-13', 19000),
('PT023', 'CT2', 'NV005', 'KH003', N'CK qua Techcombank', '2024-09-13', 19000),

('HD024', 'CT1', 'NV002', 'KH004', N'Tiền mặt tại quầy 1', '2024-09-14', 14000),
('PT024', 'CT2', 'NV002', 'KH004', N'Tiền mặt tại quầy 1', '2024-09-14', 14000),

('HD025', 'CT1', 'NV002', 'KH005', N'CK qua AgriBank', '2024-09-15', 26000),
('PT025', 'CT2', 'NV002', 'KH005', N'CK qua AgriBank', '2024-09-15', 26000),

('HD026', 'CT1', 'NV002', 'KH006', N'Tiền mặt tại quầy 2', '2024-09-16', 31000),
('PT026', 'CT2', 'NV002', 'KH006', N'Tiền mặt tại quầy 2', '2024-09-16', 31000),
('HD027', 'CT1', 'NV003', 'KH007', N'Tiền mặt', '2024-09-22', 0),
('PT027', 'CT2', 'NV003', 'KH007', N'Tiền mặt', '2024-09-22', 0),

('HD028', 'CT1', 'NV002', 'KH005', N'CK Vietcombank', '2024-09-22', 50000),


('PT028', 'CT2', 'NV002', 'KH005', N'CK Vietcombank', '2024-09-22', 50000),
('HD029', 'CT1', 'NV001', 'KH007', N'CK qua Vietcombank', '2024-09-17', 18000),
('PT029', 'CT2', 'NV001', 'KH007', N'CK qua Vietcombank', '2024-09-17', 18000),

('HD030', 'CT1', 'NV002', 'KH008', N'CK qua BIDV', '2024-09-18', 20000),
('PT030', 'CT2', 'NV002', 'KH008', N'CK qua BIDV', '2024-09-18', 20000);
INSERT INTO ChungTu (Ma, MaLoaiCT, MaNV, MaKH, PTTT, Ngayin, GiamGia) VALUES
('HD042',  'CT1', 'NV003', 'KH009', N'Tiền mặt tại quầy 1', '2024-09-10', 24000);
delete from ChungTu where Ma='HD042';
INSERT INTO ChungTu (Ma, MaLoaiCT, MaNV, MaKH, PTTT, Ngayin, GiamGia) VALUES

('HD031',  'CT1', 'NV003', 'KH009', N'Tiền mặt tại quầy 1', '2024-09-19', 24000),
('PT031', 'CT2', 'NV003', 'KH009', N'Tiền mặt tại quầy 1', '2024-09-19', 24000),

('HD032',  'CT1', 'NV004', 'KH010', N'CK qua Techcombank', '2024-09-20', 21000),
('PT032',  'CT2', 'NV004', 'KH010', N'CK qua Techcombank', '2024-09-20', 21000),


('HD033', 'CT1', 'NV001', 'KH009', N'Tiền mặt', '2024-09-22', 100000),

-- Phiếu thu tiền (CT2)

('PT033', 'CT2', 'NV001', 'KH009', N'Tiền mặt', '2024-09-22', 100000),
('HD034', 'CT1', 'NV001', 'KH002', N'CK qua Vietcombank', '2024-09-21', 18000),
('PT034', 'CT2', 'NV001', 'KH002', N'CK qua Vietcombank', '2024-09-21', 18000),

-- Ngày 22
('HD035', 'CT1', 'NV002', 'KH003', N'Tiền mặt tại quầy 1', '2024-09-22', 22000),
('PT035', 'CT2', 'NV002', 'KH003', N'Tiền mặt tại quầy 1', '2024-09-22', 22000),

-- Ngày 23
('HD036', 'CT1', 'NV003', 'KH004', N'CK qua BIDV', '2024-09-23', 19500),
('PT036', 'CT2', 'NV003', 'KH004', N'CK qua BIDV', '2024-09-23', 19500),

-- Ngày 24
('HD037', 'CT1', 'NV004', 'KH005', N'CK qua Techcombank', '2024-09-24', 25000),
('PT037', 'CT2', 'NV004', 'KH005', N'CK qua Techcombank', '2024-09-24', 25000),

-- Ngày 25
('HD038', 'CT1', 'NV005', 'KH006', N'Tiền mặt tại quầy 2', '2024-09-25', 23000),
('PT038', 'CT2', 'NV005', 'KH006', N'Tiền mặt tại quầy 2', '2024-09-25', 23000),

-- Ngày 26
('HD039', 'CT1', 'NV001', 'KH007', N'CK qua AgriBank', '2024-09-26', 26000),
('PT039', 'CT2', 'NV001', 'KH007', N'CK qua AgriBank', '2024-09-26', 26000),

-- Ngày 29
('HD040', 'CT1', 'NV002', 'KH008', N'CK qua Vietcombank', '2024-09-29', 28000),
('PT040', 'CT2', 'NV002', 'KH008', N'CK qua Vietcombank', '2024-09-29', 28000),

-- Ngày 30
('HD041', 'CT1', 'NV003', 'KH009', N'Tiền mặt tại quầy 1', '2024-09-30', 20000),
('PT041', 'CT2', 'NV003', 'KH009', N'Tiền mặt tại quầy 1', '2024-09-30', 20000);


select *from DmNhanVien where ChucVu like '%Thu ngân';

 INSERT INTO CtChungTu(MaCT, MaHH, GhiChu, DVT, DGia, SLBan) 
VALUES
-- Hóa đơn HD001
('HD001', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('HD001', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),
('HD001', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('HD001', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 4),
('HD001', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 3),

-- Hóa đơn HD002
('HD002', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('HD002', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 1),
('HD002', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 5),
('HD002', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 3),

-- Hóa đơn HD003
('HD003', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),
('HD003', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 2),
('HD003', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 4),
('HD003', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 2),

-- Hóa đơn HD004
('HD004', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('HD004', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 2),
('HD004', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 1),
('HD004', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 4),

-- Hóa đơn HD005
('HD005', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 1),
('HD005', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 3),
('HD005', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 2),
('HD005', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 5),

-- Hóa đơn HD006
('HD006', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),
('HD006', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 2),
('HD006', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 3),
('HD006', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 4),

-- Hóa đơn HD007
('HD007', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('HD007', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 1),
('HD007', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 5),
('HD007', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 3),

-- Hóa đơn HD008
('HD008', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 2),
('HD008', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 1),
('HD008', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 4),
('HD008', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 5),

-- Hóa đơn HD009
('HD009', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('HD009', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 1),
('HD009', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 3),
('HD009', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 4),

('HD010', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 2),
('HD010', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 1),
('HD010', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 3),
('HD010', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 4),

-- Phiếu thu PT001
('PT001', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('PT001', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),
('PT001', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('PT001', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 4),
('PT001', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 3),

-- Phiếu thu PT002
('PT002', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('PT002', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 1),
('PT002', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 5),
('PT002', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 3),

-- Phiếu thu PT003
('PT003', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),
('PT003', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 2),
('PT003', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 4),
('PT003', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 2),

-- Phiếu thu PT004
('PT004', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('PT004', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 2),
('PT004', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 1),
('PT004', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 4),

-- Phiếu thu PT005
('PT005', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 1),
('PT005', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 3),
('PT005', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 2),
('PT005', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 5),

-- Phiếu thu PT006
('PT006', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),
('PT006', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 2),
('PT006', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 3),
('PT006', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 4),

-- Phiếu thu PT007
('PT007', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('PT007', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 1),
('PT007', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 5),
('PT007', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 3),

-- Phiếu thu PT008
('PT008', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 2),
('PT008', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 1),
('PT008', 'HH006', N'Nồi cơm điện Sharp', N'Cái', 1200000, 4),
('PT008', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 5),

-- Phiếu thu PT009
('PT009', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('PT009', 'HH010', N'Lego Creator 3in1', N'Bộ', 1200000, 1),
('PT009', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 3),
('PT009', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 50000, 4),

-- Phiếu thu PT010
('PT010', 'HH005', N'Sữa Vinamilk 1L', N'Chai', 30000, 2),
('PT010', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 1),
('PT010', 'HH007', N'Áo sơ mi nam', N'Cái', 250000, 3),
('PT010', 'HH008', N'Quạt điện Toshiba', N'Cái', 850000, 4);
INSERT INTO CtChungTu (MaCT, MaHH, GhiChu, DVT, DGia, SLBan) 
VALUES
-- HD011 - PT011
('HD011', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('HD011', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),
('PT011', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('PT011', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 1),

-- HD012 - PT012
('HD012', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('HD012', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 4),
('PT012', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('PT012', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 4),

-- HD013 - PT013
('HD013', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 2),
('HD013', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 1),
('PT013', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 2),
('PT013', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 1),

-- HD014 - PT014
('HD014', 'HH007', N'Gạo ST25', N'Kg', 32000, 5),
('HD014', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 2),
('PT014', 'HH007', N'Gạo ST25', N'Kg', 32000, 5),
('PT014', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 2),

-- HD015 - PT015
('HD015', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 3),
('HD015', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 1),
('PT015', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 3),
('PT015', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 1),

-- HD016 - PT016
('HD016', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 10),
('HD016', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 2),
('PT016', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 10),
('PT016', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 2),

-- HD017 - PT017
('HD017', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),
('HD017', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 4),
('PT017', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),
('PT017', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 4),
-- HD018 - PT018
('HD018', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('HD018', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 1),
('PT018', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 2),
('PT018', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 1),

-- HD019 - PT019
('HD019', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 8),
('HD019', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 3),
('PT019', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 8),
('PT019', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 3),

-- HD020 - PT020
('HD020', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 2),
('HD020', 'HH007', N'Gạo ST25', N'Kg', 32000, 5),
('PT020', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 2),
('PT020', 'HH007', N'Gạo ST25', N'Kg', 32000, 5),

-- HD021 - PT021
('HD021', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 1),
('HD021', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 2),
('PT021', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 1),
('PT021', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 2),

-- HD022 - PT022
('HD022', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 1),
('HD022', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('PT022', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 1),
('PT022', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),

-- HD023 - PT023
('HD023', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 4),
('HD023', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 2),
('PT023', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 4),
('PT023', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 2),

-- HD024 - PT024
('HD024', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 6),
('HD024', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 3),
('PT024', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 6),
('PT024', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 3),

-- HD025 - PT025
('HD025', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 5),
('HD025', 'HH007', N'Gạo ST25', N'Kg', 32000, 2),
('PT025', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 5),
('PT025', 'HH007', N'Gạo ST25', N'Kg', 32000, 2),

-- HD026 - PT026
('HD026', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 4),
('HD026', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 3),
('PT026', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 4),
('PT026', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 3),

-- HD027 - PT027
('HD027', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 2),
('HD027', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),
('PT027', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 2),
('PT027', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),

-- HD028 - PT028
('HD028', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 3),
('HD028', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 2),
('PT028', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 3),
('PT028', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 2),

-- HD029 - PT029
('HD029', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 7),
('HD029', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 4),
('PT029', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 7),
('PT029', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 4),

-- HD030 - PT030
('HD030', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 3),
('HD030', 'HH007', N'Gạo ST25', N'Kg', 32000, 6),
('PT030', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 3),
('PT030', 'HH007', N'Gạo ST25', N'Kg', 32000, 6),

-- HD031 - PT031
('HD031', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 2),
('HD031', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 4),
('PT031', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 2),
('PT031', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 4),

-- HD032 - PT032
('HD032', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 1),
('HD032', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),
('PT032', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 1),
('PT032', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 2),

-- HD033 - PT033
('HD033', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 5),
('HD033', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),
('PT033', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 5),
('PT033', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 3),

-- HD034 - PT034
('HD034', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 8),
('HD034', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 5),
('PT034', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 8),
('PT034', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 5),

-- HD035 - PT035
('HD035', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 2),
('HD035', 'HH007', N'Gạo ST25', N'Kg', 32000, 3),
('PT035', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 2),
('PT035', 'HH007', N'Gạo ST25', N'Kg', 32000, 3),

-- HD036 - PT036
('HD036', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 1),
('HD036', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 3),
('PT036', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 1),
('PT036', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 3),

-- HD037 - PT037
('HD037', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 2),
('HD037', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),
('PT037', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 2),
('PT037', 'HH001', N'Thịt heo ba rọi', N'Kg', 150000, 1),

-- HD038 - PT038
('HD038', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 3),
('HD038', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 4),
('PT038', 'HH002', N'Nước ngọt Coca-Cola', N'Chai', 12000, 3),
('PT038', 'HH003', N'Bột giặt OMO 3kg', N'Kg', 95000, 4),

-- HD039 - PT039
('HD039', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 9),
('HD039', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 2),
('PT039', 'HH004', N'Mì gói Hảo Hảo', N'Gói', 5000, 9),
('PT039', 'HH005', N'Sữa tươi Vinamilk 1L', N'Hộp', 35000, 2),
-- HD040 - PT040
('HD040', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 2),
('HD040', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 3),
('PT040', 'HH006', N'Bánh quy Cosy', N'Hộp', 45000, 2),
('PT040', 'HH008', N'Nước mắm Nam Ngư', N'Chai', 25000, 3),


-- Tiếp tục đến HD041 - PT041
('HD041', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 3),
('HD041', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 1),
('PT041', 'HH009', N'Sách giáo khoa lớp 12', N'Cuốn', 120000, 3),
('PT041', 'HH010', N'Lego Creator 3in1', N'Bộ', 850000, 1);



-- Chèn dữ liệu mới
INSERT INTO ButToan (MaBT, MaTKCT, MaCT, TKNo, TKCo, NgayLapBT, DienGiai,SoTien ) VALUES
('BT001', '1121.04', 'PT001', '112', '511', '2024-09-09', N'Ghi nhận thu tiền chuyển khoản Agribank'),
('BT002', '1121.04', 'HD001', '112', '511', '2024-09-09', N'Ghi nhận thu tiền chuyển khoản Agribank'),
('BT003', '1111.01', 'HD002', '111', '511', '2024-09-09', N'Ghi nhận thu tiền mặt từ khách hàng KH002'),
('BT004', '1111.02', 'HD003', '111', '511', '2024-09-10', N'Ghi nhận thu tiền mặt từ khách hàng KH003'),
('BT005', '1111.01', 'HD004', '111', '511', '2024-09-10', N'Ghi nhận thu tiền mặt từ khách hàng KH004'),
('BT006', '1111.02', 'HD005', '111', '511', '2024-09-11', N'Ghi nhận thu tiền mặt từ khách hàng KH005'),
('BT007', '1111.01', 'HD006', '111', '511', '2024-09-11', N'Ghi nhận thu tiền mặt từ khách hàng KH006'),
('BT008', '1121.01', 'HD007', '112', '511', '2024-09-12', N'Ghi nhận thu tiền chuyển khoản Vietcombank từ KH007'),
('BT009', '1121.01', 'HD008', '112', '511', '2024-09-12', N'Ghi nhận thu tiền chuyển khoản Vietcombank từ KH008'),
('BT010', '1121.01', 'HD009', '112', '511', '2024-09-13', N'Ghi nhận thu tiền chuyển khoản Vietcombank từ KH009'),
('BT011', '1121.04', 'HD010', '112', '511', '2024-09-13', N'Ghi nhận thu tiền chuyển khoản Agribank từ KH010'),
('BT012', '1111.01', 'PT002', '111', '131', '2024-09-09', N'Nhận tiền mặt từ khách hàng KH002'),
('BT013', '1111.02', 'PT003', '111', '131', '2024-09-10', N'Nhận tiền mặt từ khách hàng KH003'),
('BT014', '1111.01', 'PT004', '111', '131', '2024-09-10', N'Nhận tiền mặt từ khách hàng KH004'),
('BT015', '1111.02', 'PT005', '111', '131', '2024-09-11', N'Nhận tiền mặt từ khách hàng KH005'),
('BT016', '1111.01', 'PT006', '111', '131', '2024-09-11', N'Nhận tiền mặt từ khách hàng KH006'),
('BT017', '1121.01', 'PT007', '112', '131', '2024-09-12', N'Nhận tiền chuyển khoản Vietcombank từ KH007'),
('BT018', '1121.01', 'PT008', '112', '131', '2024-09-12', N'Nhận tiền chuyển khoản Vietcombank từ KH008'),
('BT019', '1121.01', 'PT009', '112', '131', '2024-09-13', N'Nhận tiền chuyển khoản Vietcombank từ KH009'),
('BT020', '1121.04', 'PT010', '112', '131', '2024-09-13', N'Nhận tiền chuyển khoản Agribank từ KH010');




INSERT INTO ButToan (MaBT, MaTKCT, MaCT, TKNo, TKCo, NgayLapBT, DienGiai, SoTien) VALUES
-- HD031
('BT021', '1111.01', 'HD031', '111', '511', '2024-09-19', N'Thu tiền mặt tại quầy 1', 506000), 
-- (25,000*2 + 120,000*4) - 24,000 = 50,000 + 480,000 - 24,000 = 506,000

-- HD032
('BT022', '1121.03', 'HD032', '112', '511', '2024-09-20', N'Thu chuyển khoản qua Techcombank', 1009000), 
-- (850,000*1 + 150,000*2) - 21,000 = 850,000 + 300,000 - 21,000 = 1,009,000

-- HD033
('BT023', '1111.01', 'HD033', '111', '511', '2024-09-22', N'Thu tiền mặt', 245000), 
-- (12,000*5 + 95,000*3) - 100,000 = 60,000 + 285,000 - 100,000 = 245,000

-- HD034
('BT024', '1121.01', 'HD034', '112', '511', '2024-09-21', N'Thu chuyển khoản qua Vietcombank', 197000), 
-- (5,000*8 + 35,000*5) - 18,000 = 40,000 + 175,000 - 18,000 = 197,000

-- HD035
('BT025', '1111.01', 'HD035', '111', '511', '2024-09-22', N'Thu tiền mặt tại quầy 1', 164000), 
-- (45,000*2 + 32,000*3) - 22,000 = 90,000 + 96,000 - 22,000 = 164,000

-- HD036
('BT026', '1121.02', 'HD036', '112', '511', '2024-09-23', N'Thu chuyển khoản qua BIDV', 364500), 
-- (25,000*1 + 120,000*3) - 19,500 = 25,000 + 360,000 - 19,500 = 364,500

-- HD037
('BT027', '1121.03', 'HD037', '112', '511', '2024-09-24', N'Thu chuyển khoản qua Techcombank', 1825000), 
-- (850,000*2 + 150,000*1) - 25,000 = 1,700,000 + 150,000 - 25,000 = 1,825,000

-- HD038
('BT028', '1111.02', 'HD038', '111', '511', '2024-09-25', N'Thu tiền mặt tại quầy 2', 393000), 
-- (12,000*3 + 95,000*4) - 23,000 = 36,000 + 380,000 - 23,000 = 393,000

-- HD039
('BT029', '1121.04', 'HD039', '112', '511', '2024-09-26', N'Thu chuyển khoản qua AgriBank', 110000), 
-- (5,000*9 + 35,000*2) - 26,000 = 45,000 + 70,000 - 26,000 = 110,000

-- HD040
('BT030', '1121.01', 'HD040', '112', '511', '2024-09-29', N'Thu chuyển khoản qua Vietcombank', 137000), 
-- (45,000*2 + 25,000*3) - 28,000 = 90,000 + 75,000 - 28,000 = 137,000

-- HD041
('BT031', '1111.01', 'HD041', '111', '511', '2024-09-30', N'Thu tiền mặt tại quầy 1', 1190000), 
-- (120,000*3 + 850,000*1) - 20,000 = 360,000 + 850,000 - 20,000 = 1,190,000
-- PT011
-- PT031
('BT032', '1111.01', 'PT031', '111', '511', '2024-09-19', N'Thu tiền mặt tại quầy 1', 506000), 
-- (25,000*2 + 120,000*4) - 24,000 = 50,000 + 480,000 - 24,000 = 506,000

-- PT032
('BT033', '1121.03', 'PT032', '112', '511', '2024-09-20', N'Thu chuyển khoản qua Techcombank', 1009000), 
-- (850,000*1 + 150,000*2) - 21,000 = 850,000 + 300,000 - 21,000 = 1,009,000

-- PT033
('BT034', '1111.01', 'PT033', '111', '511', '2024-09-22', N'Thu tiền mặt', 245000), 
-- (12,000*5 + 95,000*3) - 100,000 = 60,000 + 285,000 - 100,000 = 245,000

-- PT034
('BT035', '1121.01', 'PT034', '112', '511', '2024-09-21', N'Thu chuyển khoản qua Vietcombank', 197000), 
-- (5,000*8 + 35,000*5) - 18,000 = 40,000 + 175,000 - 18,000 = 197,000

-- PT035
('BT036', '1111.01', 'PT035', '111', '511', '2024-09-22', N'Thu tiền mặt tại quầy 1', 164000), 
-- (45,000*2 + 32,000*3) - 22,000 = 90,000 + 96,000 - 22,000 = 164,000

-- PT036
('BT037', '1121.02', 'PT036', '112', '511', '2024-09-23', N'Thu chuyển khoản qua BIDV', 364500), 
-- (25,000*1 + 120,000*3) - 19,500 = 25,000 + 360,000 - 19,500 = 364,500

-- PT037
('BT038', '1121.03', 'PT037', '112', '511', '2024-09-24', N'Thu chuyển khoản qua Techcombank', 1825000), 
-- (850,000*2 + 150,000*1) - 25,000 = 1,700,000 + 150,000 - 25,000 = 1,825,000

-- PT038
('BT039', '1111.02', 'PT038', '111', '511', '2024-09-25', N'Thu tiền mặt tại quầy 2', 393000), 
-- (12,000*3 + 95,000*4) - 23,000 = 36,000 + 380,000 - 23,000 = 393,000

-- PT039
('BT040', '1121.04', 'PT039', '112', '511', '2024-09-26', N'Thu chuyển khoản qua AgriBank', 110000), 
-- (5,000*9 + 35,000*2) - 26,000 = 45,000 + 70,000 - 26,000 = 110,000

-- PT040
('BT041', '1121.01', 'PT040', '112', '511', '2024-09-29', N'Thu chuyển khoản qua Vietcombank', 137000), 
-- (45,000*2 + 25,000*3) - 28,000 = 90,000 + 75,000 - 28,000 = 137,000

-- PT041
('BT042', '1111.01', 'PT041', '111', '511', '2024-09-30', N'Thu tiền mặt tại quầy 1', 1190000);
-- (120,000*3 + 850,000*1) - 20,000 = 360,000 + 850,000 - 20,000 = 1,190,000





ALTER TABLE ButToan ADD SoTien FLOAT NULL;
UPDATE ButToan
SET SoTien = (
    SELECT SUM(ct.SLBan * ct.DGia) - SUM(c.GiamGia)
    FROM CtChungTu ct join ChungTu c on ct.MaCT=c.Ma
    WHERE CtChungTu.MaCT = ButToan.MaCT
)
WHERE EXISTS (
    SELECT 1 FROM CtChungTu WHERE CtChungTu.MaCT = ButToan.MaCT
);

------

CREATE PROCEDURE sp_TimKiemHangHoa
    @TenHH NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Ma, Ten, HinhAnh, MoTa, MaLoaiHH, MaCS 
    FROM DmHangHoa
    WHERE Ten LIKE '%' + @TenHH + '%';
END;

-------------------------SP
create proc sp_ThemHD @MaNV nvarchar(8), @MaKH nvarchar(8), @NgayinHD datetime, 
@MaHH nvarchar(8), @Ten nvarchar(128), @DVT nvarchar(32),@DGia float,@SLban int, @MaNVu  nvarchar(8)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @MaHD nvarchar(8);
    
    BEGIN TRANSACTION;

    -- 1️⃣ Thêm hóa đơn mới
    INSERT INTO HoaDon (MaNV, MaKH, NgayinHD)
    VALUES (@MaNV, @MaKH, @NgayinHD);

    -- Lấy Mã Hóa Đơn vừa tạo
    SET @MaHD = SCOPE_IDENTITY(); 

    -- 2️⃣ Thêm chi tiết hóa đơn
    INSERT INTO CtHoaDon (MaHD, MaHH, Ten, DVT, DGia, SLBan, MaNVu)
    VALUES (@MaHD, @MaHH, @Ten, @DVT, @DGia, @SLBan, @MaNVu);

    COMMIT TRANSACTION;
END;
/*
CREATE TABLE HoaDon
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    MaNV NVARCHAR(8) NOT NULL,
    MaKH NVARCHAR(8) NOT NULL,
	PTTT NVARCHAR(32) NULL,
    NgayinHD DATE,
          
    
    CONSTRAINT FK_HoaDon_MaNV FOREIGN KEY (MaNV) REFERENCES DmNhanVien(Ma),
    CONSTRAINT FK_HoaDon_MaKH FOREIGN KEY (MaKH) REFERENCES DmKhachHang(Ma)
);
CREATE TABLE CtHoaDon
(
    MaHD NVARCHAR(8) NOT NULL,
    MaHH NVARCHAR(8) NOT NULL,
    Ten NVARCHAR(128) NULL,               
    DVT NVARCHAR(32) NULL,
    DGia FLOAT NULL,
    SLban INT NULL,

    CONSTRAINT PK_CTHD PRIMARY KEY (MaHD, MaHH),
    CONSTRAINT FK_CTHD_MaHD FOREIGN KEY (MaHD) REFERENCES HoaDon(Ma),
    CONSTRAINT FK_CTHD_MaHH FOREIGN KEY (MaHH) REFERENCES DmHangHoa(Ma)
);

drop table PhieuThuTien
CREATE TABLE PhieuThuTien
(
    Ma NVARCHAR(8) PRIMARY KEY NOT NULL,
    MaHD NVARCHAR(8) NOT NULL,
   
    TGThuTien DATETIME NULL,
 --   MaNVu NVARCHAR(8) NOT NULL,
    MoTa NVARCHAR(256) NULL,

    CONSTRAINT FK_PTTT_MaBT FOREIGN KEY (MaBT) REFERENCES ButToan(Ma),
    CONSTRAINT FK_PTTT_MaHD FOREIGN KEY (MaHD) REFERENCES HoaDon(Ma),

);

CREATE TABLE CtPTT
(
    MaPT NVARCHAR(8) NOT NULL,
    MaHH NVARCHAR(8) NOT NULL,
	Ten NVARCHAR(128) NULL,               
    DVT NVARCHAR(32) NULL,
    DGia FLOAT NULL,
    SLban INT NULL,

	CONSTRAINT PK_CTPT PRIMARY KEY (MaPT, MaHH),
    CONSTRAINT FK_CTPT_MaPT FOREIGN KEY (MaPT) REFERENCES PhieuThuTien(Ma),
    CONSTRAINT FK_CTPT_MaHH FOREIGN KEY (MaHH) REFERENCES DmHangHoa(Ma),

);*/
/*
----------------------------TRIGGER
--1. SLBan của hàng hóa trong bảng CTHD luôn lớn hơn 0.
CREATE TRIGGER tg_SLB
ON CTHD
FOR INSERT, UPDATE 
AS
BEGIN
    -- Kiểm tra nếu có bất kỳ số lượng bán nào <= 0
    IF EXISTS (
        SELECT 1 
        FROM inserted 
        WHERE SLBan <= 0
    )
    BEGIN
        PRINT  N'Bạn nhớ thêm số lượng hàng hóa nhé!'
	ROLLBACK
	END
END 
-- KIỂM THỬ 
--Thêm thông tin hóa đơn 
insert into HoaDon(MaHD, MaNV, MaKH, NgayinHD) values
('HD011','NV005','KH010','2024-09-13');
-- Chi tiết hóa đơn HD011
insert into CTHD(MaHD, MaHH, SLBan) values
('HD011','HH005',2);
-- Chỉnh sửa chi tiết hóa đơn HD011
update CTHD
set SLBan = 0
where MaHD='HD011';
/*
delete from CTHD where MaHD='HD011';
delete from HoaDon where MaHD='HD011';
*/
-- 3. TẠO TRIGGER GIỚI HẠN SỐ LƯỢNG BÁN BẰNG HOẶC NHỎ HƠN SỐ LƯỢNG TỒN CỦA HÀNG HÓA
CREATE TRIGGER tg_uiSLBan_Ton
ON CTHD
FOR INSERT, UPDATE
AS
BEGIN
    -- Kiểm tra nếu số lượng bán vượt quá số lượng tồn kho
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN DMHangHoa h ON i.MaHH = h.MaHH
        WHERE i.SLBan > h.SLTon 
		)
    BEGIN
        PRINT N'Số lượng bán không được vượt quá số lượng tồn của hàng hóa.'
        ROLLBACK 
    END
END
---KIỂM THỬ
--THÔNG TIN CỦA HÀNG HÓA CÓ MÃ 'HH001'
INSERT INTO DMHangHoa (MaHH, MaLoaiHH, TenHH, MaCS, SLTon, DonGia, DVT, HinhAnh) 
VALUES
('HH001', 'LHH01', N'Thịt heo ba rọi', 'CS001', 100, 150000, N'Kg', 'thit_heo_ba_roi.jpg');
--Thêm thông tin hóa đơn 
insert into HoaDon(MaHD, MaNV, MaKH, NgayinHD) values
('HD011','NV005','KH010','2024-09-13');
-- Thêm Chi tiết hóa đơn HD011
insert into CTHD(MaHD, MaHH, SLBan) values
('HD011','HH001',2);
-- Chỉnh sửa chi tiết hóa đơn HD011
update CTHD
set SLBan = 101
where MaHD='HD011';
drop trigger tg_uiSLBan_Ton

-- 4. TẠO TRIGGER KIỂM TRA TRÙNG LẶP DỮ LIỆU CỦA KHÁCH HÀNG TRƯỚC KHI THÊM 
CREATE TRIGGER tg_KiemTraTrungLap
ON KhachHang
FOR INSERT
AS
BEGIN
    DECLARE  @SDT NVARCHAR(10);
    SELECT @SDT = SDT_KH FROM inserted;
    IF EXISTS (SELECT * FROM KhachHang WHERE SDT_KH = @SDT)
    BEGIN
		PRINT 'Khach hang da ton tai voi so dien thoai va email nay!'
		ROLLBACK
    END
    INSERT INTO KhachHang (MaKH, TenKH, Nu,Nam, SDT_KH, DiaChi, HangKHTT) 
    SELECT * FROM inserted
END

-- KIỂM THỬ 
-- TRƯỜNG HỢP KHÔNG TRÙNG SỐ ĐIỆN THOẠI 
INSERT INTO KhachHang (MaKH, TenKH, Nu, Nam, SDT_KH, DiaChi, HangKHTT)
VALUES
('KH011', N'Nguyễn Văn An', 0, 1, '0909123456', N'12 Nguyễn Trãi, Q.1, TP.HCM', N'Vàng'),

*/
select * from DmCoSo


EXEC ThemNhanVien @Ma = 'NV021', @Ten = 'Nguyen Van A', @GioiTinh = 1, 
                  @SDT = '0123456789', @DiaChi = 'Hà Nội', 
                  @ChucVu = 'Nhân viên', @MaCS = 'CS001', @UserName = 'TN_Tam';

DROP PROCEDURE ThemNhanVien;
------------THÊM NHÂN VIÊN
create proc [dbo].[ThemNhanVien] 
@Ma nvarchar(8),
@Ten nvarchar(64),
@GioiTinh bit,
@SDT nvarchar(16),
@DiaChi nvarchar(128),
@ChucVu nvarchar(64),
@MaCS nvarchar(8),
@UserName nvarchar(16)
as 
begin 
	insert into DmNhanVien (Ma, Ten, GioiTinh,SDT, DiaChi, ChucVu, MaCS, UserName)
	values (@Ma,@Ten ,@GioiTinh ,@SDT ,@DiaChi,@ChucVu ,@MaCS ,@UserName )
end 
---------------SỬA NHÂN VIÊN
create proc [dbo].[SuaNhanVien] 
@Ma nvarchar(8),
@Ten nvarchar(64),
@GioiTinh bit,
@SDT nvarchar(16),
@DiaChi nvarchar(128),
@ChucVu nvarchar(64),
@MaCS nvarchar(8),
@UserName nvarchar(16)
as 
begin 
update  DmNhanVien
set Ten=@Ten, GioiTinh=@GioiTinh, SDT=@SDT, DiaChi=@DiaChi, ChucVu=@ChucVu, MaCS=@MaCS, UserName=@UserName
where Ma=@Ma
end 
select * from DmKhachHang
alter table DmKhachHang drop column ThoiGianDK
---------THÊM THÔNG TIN KHÁCH HÀNG
drop proc ThemKhachHang
CREATE PROCEDURE sp_ThemKhachHang
    @MaKH NVARCHAR(8),
    @TenKH NVARCHAR(64),
    @GioiTinh BIT,  -- Thêm giới tính
    @SDT NVARCHAR(16),
    @DiaChi NVARCHAR(256)

AS
BEGIN
    INSERT INTO DmKhachHang (Ma, Ten, GioiTinh, SDT, DiaChi)
    VALUES (@MaKH, @TenKH, @GioiTinh, @SDT, @DiaChi)
END
---------SỬA THÔNG TIN KHÁCH HÀNG
CREATE PROCEDURE sp_SuaKhachHang
    @MaKH NVARCHAR(8),
    @TenKH NVARCHAR(64),
    @GioiTinh BIT,
    @SDT NVARCHAR(16),
    @DiaChi NVARCHAR(256)

AS
BEGIN
    UPDATE DmKhachHang
    SET Ten = @TenKH,
        GioiTinh = @GioiTinh,
        SDT = @SDT,
        DiaChi = @DiaChi

    WHERE Ma = @MaKH
END

-------
CREATE PROCEDURE [dbo].[GetTenHH]
    @MaHH nvarchar(5)
AS
BEGIN
    SELECT TenHH FROM DMHangHoa WHERE MaHH = @MaHH
END
-- Kiểm tra Stored Procedure
EXEC [dbo].[GetTenHH] @MaHH = 'HH001'
-----
create proc [dbo].[ThemHH]
@MaHH nvarchar(5),
@MaLoaiHH nvarchar(5),
@TenHH nvarchar (100),
@MaCS nvarchar(5),
@SLTon int ,
@DonGia float,
@HinhAnh nvarchar(100),
@DVT nvarchar(20)
as begin 
update DMHangHoa 
set 
@MaLoaiHH=MaLoaiHH,
@TenHH =TenHH,
@MaCS =MaCS,
@SLTon=SLTon,
@DonGia=DonGia,
@HinhAnh =HinhAnh,
@DVT =DVT
where  @MaHH =MaHH
end 



-----------------------show HangHang

SELECT C.MaHD, C.MaHH, D.TenHH, D.MaLoaiHH, D.DonGia, D.DVT
FROM CTHD C
INNER JOIN DMHangHoa D ON C.MaHH = D.MaHH
WHERE C.MaHD = @MaHD


-------------THÊM HÓA ĐƠN
create proc [dbo].[sp_ThemHoaDon] @MaHD nvarchar(5), @MaNV nvarchar(5), @MaKH nvarchar(5), @NgayinHD date
as 
begin
insert into HoaDon(MaHD, MaNV, MaKH, NgayinHD) values (@MaHD , @MaNV , @MaKH , @NgayinHD)
end 
-------------SỬA HÓA ĐƠN
create proc [dbo].[sp_SuaHoaDon] @MaHD nvarchar(5), @MaNV nvarchar(5), @MaKH nvarchar(5), @NgayinHD date
as 
begin
update  HoaDon 
set  MaNV=@MaNV, MaKH=@MaKH, NgayinHD=@NgayinHD
where MaHD=@MaHD
end 

----THÊM PHIẾU THU
SELECT * FROM PHIEUTHUTIEN
create proc [dbo].[sp_ThemPTT] @MaPT nvarchar(5), @MaHD nvarchar(5), @TGThuTien date, @PTTT nvarchar(30), @TriGia float , @TrangThaiTT nvarchar(50)
as 
begin 
insert into PhieuThuTien(MaPT,MaHD,TGThuTien,PTTT,TriGia,TrangThaiTT) values 
(@MaPT , @MaHD , @TGThuTien , @PTTT, @TriGia  , @TrangThaiTT)
end 
-----SƯA PHIẾU THU
create proc [dbo].[sp_SuaPTT] @MaPT nvarchar(5), @MaHD nvarchar(5), @TGThuTien date, @PTTT nvarchar(30), @TriGia float , @TrangThaiTT nvarchar(50)
as 
begin 
update  PhieuThuTien
set MaHD=@MaHD,TGThuTien=@TGThuTien,PTTT=@PTTT,TriGia=@TriGia,TrangThaiTT=@TrangThaiTT
where MaPT=@MaPT
end 
-----THÊM BÚT TOÁN
create proc [dbo].[sp_ThemBT] @MaBT nvarchar(5), @MaTKCT nvarchar(10), @MaPT nvarchar(5), @TKNo nvarchar(10), @TKCo nvarchar(10), @NgayLapBT date, @DienGiai nvarchar(100), @Tong float
as begin
insert into ButToan (MaBT, MaTKCT, MaPT, TKNo, TKCo,NgayLapBT, DienGiai,Tong)
values (@MaBT, @MaTKCT , @MaPT , @TKNo, @TKCo, @NgayLapBT , @DienGiai , @Tong)
end 

------SỬA BÚT TOÁN
create proc [dbo].[sp_SuaBT] @MaBT nvarchar(5), @MaTKCT nvarchar(10), @MaPT nvarchar(5), @TKNo nvarchar(10), @TKCo nvarchar(10), @NgayLapBT date, @DienGiai nvarchar(100), @Tong float
as begin
update ButToan
set MaTKCT=@MaTKCT,MaPT=@MaPT,TKNo=@TKNo,TKCo=@TKCo,NgayLapBT=@NgayLapBT,DienGiai=@DienGiai,Tong=@Tong
where MaBT=@MaBT
end 
-----
create proc sp_MaKH @MaKH nvarchar(5)
as begin 
select * from KhachHang
where MaKH=@MaKH
end 

exec sp_MaKH @MaKH='KH001'
select * from KhachHang

create proc sp_TGThuTien @Ngay date
as begin 
select * from PhieuThuTien
where TGThuTien=@Ngay
end 
---
create proc sp_MaDH @MaHD nvarchar(5)
as begin
SELECT *  FROM HoaDon WHERE MaHD = @MaHD
end 
drop proc sp_MaDH
exec sp_MaDH @MaHD='HD001'
-----
create proc sp_DienGiai @DienGiai nvarchar(100)
as begin 
SELECT * FROM ButToan WHERE DienGiai = @DienGiai
end 

----
create proc sp_ReportPTTT @MaHD nvarchar(5)
as begin
select kh.TenKH, kh.DiaChi, kh.SDT_KH, cs.TenCS, cs.DiaChiCS,pt.TriGia, pt.PTTT,pt.TrangThaiTT,nv.TenNV,bt.DienGiai,pt.MaPT,hd.MaHD,bt.TKNo,bt.TKCo
from HoaDon hd join KhachHang kh on hd.MaKH=kh.MaKH
join NhanVien nv on hd.MaNV = nv.MaNV
join COSO cs on cs.MaCS=nv.MaCS
join PhieuThuTien pt on hd.MaHD=pt.MaHD
join ButToan bt on pt.MaPT=bt.MaPT
where hd.MaHD=@MaHD
end 
exec sp_ReportPTTT @MaHD='HD001'
-----