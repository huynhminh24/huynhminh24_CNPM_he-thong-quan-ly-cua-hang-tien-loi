--======================================================================================
--CREATE DATABASE QL_SieuThiMiNi ON PRIMARY
--(
--name= QLDDH_Primary,
--Filename= 'E:\LuuDuLieuSinhVien\2001216332_TruongThanhY\QL_SieuThiMiNi_primary.mdf',
--Size=3MB,
--Maxsize=10MB,
--Filegrowth=10%
--)
--LOG ON
--(
--name= QLDDH_Log,
--Filename= 'E:\LuuDuLieuSinhVien\2001216332_TruongThanhY\QL_SieuThiMiNi_Log.ldf',
--Size=1MB,
--Maxsize=5MB,
--Filegrowth=10%
--)
--======================================================================================
CREATE DATABASE QL_SieuThiMiNi

drop DATABASE QL_SieuThiMiNi

GO

USE QL_SieuThiMiNi;
GO

-- Tạo các bảng và insert trong cơ sở dữ liệu QL_SieuThiMiNi
SET DATEFORMAT ymd;

CREATE TABLE NHANVIEN (
    MANV VARCHAR(10) NOT NULL,
    TENNV NVARCHAR(50) NOT NULL,
    Email NVARCHAR(50),
    CCCD NCHAR(20),
    NGAYSINH datetime NULL,
    GIOITINH NVARCHAR(50),
    DANTOC NVARCHAR(50),
    DIACHI NVARCHAR(50) NOT NULL,
    SDT NCHAR(15) NOT NULL,
    CONSTRAINT NV_MANV_PK PRIMARY KEY (MANV)
);


INSERT INTO NHANVIEN 
VALUES 
('NV001', N'Nguyễn Ngọc Lan', N'Lan@gmail.com', N'63453464573745', N'2001-05-05', N'Nữ', N'Kinh', N'98 Cầu Giấy', N'111222339'),
('NV002', N'Trịnh Thanh Tú', N'Tu@gmail.com', NULL, NULL, NULL, NULL, N'68 Đường Láng', N'991222312'),
('NV003', N'Nguyễn Thị Bình', N'Binh@gmail.com', NULL, NULL, NULL, NULL, N'29 Đại La', N'982362111'),
('NV004', N'Trần Thị Hồng', N'Hung@gmail.com', NULL, NULL, NULL, NULL, N'123 Tam Trinh', N'344237654'),
('NV005', N'Đỗ Văn Ngân', N'Ngan@gmail.com', NULL, NULL, NULL, NULL, N'342 Đường Láng', N'345653763')

CREATE TABLE HOADON (
    MAHD VARCHAR(10) NOT NULL,
    NGAYHD datetime NOT NULL,
    MANV VARCHAR(10) NOT NULL,
    THANHTIEN FLOAT NULL,
    CONSTRAINT HD_MAHD_PK PRIMARY KEY (MAHD),
    CONSTRAINT FK_HOADON_MANV FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV)
);

insert into HOADON values
('HD2308', '2023-12-12', 'NV001', 390000),
('HD5492', '2023-12-12', 'NV002', 20000),
('HD6845', '2023-12-12', 'NV003', 800000)


CREATE TABLE HANGHOA (
    MAHH VARCHAR(10) NOT NULL,
    TENHH NVARCHAR(50) NOT NULL,
    GIABAN INT NOT NULL,
    DVT VARCHAR(10) NOT NULL,
    HSD datetime NOT NULL,
    SoLuongTon INT NULL,
    CONSTRAINT HH_MAHH_PK PRIMARY KEY (MAHH)
);

insert into HANGHOA values
('HH001', N'Bánh quy', 20000, N'Gói', '2022-06-12', 300),
('HH002', N'Kẹo cao su', 2000, N'Gói', '2022-10-08', 160),
('HH003', N'Bia', 12000, N'Lon', '2022-11-18', 180),
('HH004', N'Nước lọc', 5000, N'Chai','2022-07-27', 450),
('HH005', N'Sữa chua', 6000, N'Hộp', '2022-04-21', 490),
('HH006', N'Sữa Fami', 4000, N'Hộp', '2022-04-20', 490),
('HH007', N'Mì tôm Hảo Hảo', 5000, N'Gói','2022-06-23', 950),
('HH008', N'Bánh mì', 9000, N'Gói', '2022-01-11', 950)


CREATE TABLE CTHOADON (
    MAHD VARCHAR(10) NOT NULL,
    MAHH VARCHAR(10) NOT NULL,
    SOLUONG INT NOT NULL,
    TongTien FLOAT NULL,
    CONSTRAINT CTHD_PK PRIMARY KEY (MAHD, MAHH),
    CONSTRAINT FK_CTHOADON_MAHD FOREIGN KEY (MAHD) REFERENCES HOADON (MAHD),
    CONSTRAINT FK_CTHOADON_MAHH FOREIGN KEY (MAHH) REFERENCES HANGHOA (MAHH)
);

insert into CTHOADON values
('HD2308', 'HH002', 10, 20000),
('HD2308', 'HH003', 10, 120000),
('HD2308', 'HH004', 50, 250000),
('HD5492', 'HH002', 10, 20000),
('HD6845', 'HH005', 10, 60000),
('HD6845', 'HH006', 10, 40000),
('HD6845', 'HH007', 50, 250000),
('HD6845', 'HH008', 50, 450000)

CREATE TABLE NHACUNGCAP (
    MANCC VARCHAR(10) NOT NULL,
    TENNCC NVARCHAR(30) NOT NULL,
    DIACHI NVARCHAR(50) NOT NULL,
    MST CHAR(10) NOT NULL,
    TKNCC VARCHAR(20) NOT NULL,
    EMAIL VARCHAR(20) NULL,
    CONSTRAINT NCC_MANCC_PK PRIMARY KEY (MANCC)
);

INSERT INTO NHACUNGCAP VALUES
('NCC001', N'HIT 20', '50 Ngọc Hải', '1234845874', '123456', 'HIT_20@gmail.com'),
('NCC002', N'Mai Anh', '101 Lê Thanh Nghị', '0323727432', '174494', 'Mai_Anh@gmail.com'),
('NCC003', N'Tuấn Hưng', '243 Trần Đại Nghĩa', '1323340948', '198343', 'tuanhung@gmail.com'),
('NCC004', N'Tú Uyên', '123 Giải Phóng', '0138283477', '988736', 'tauyen@gmail.com'),
('NCC005', N'Tường An', '121 Tô Quang Bảo', '0987633546', '873432', 'tuongan@gmail.com')

CREATE TABLE PHIEUNHAPHANG (
    MAPN VARCHAR(10) NOT NULL,
    MANCC VARCHAR(10) NOT NULL,
    NGAYNHAP datetime NOT NULL,
    TONGTIEN FLOAT NULL,
    CONSTRAINT PN_MANCC_PK PRIMARY KEY (MAPN),
    CONSTRAINT FK_PHIEUNHAPHANG_MANCC FOREIGN KEY (MANCC) REFERENCES NHACUNGCAP (MANCC)
);

INSERT INTO PHIEUNHAPHANG 
VALUES 
('PN7240', 'NCC003', '2023-12-14', 75000),
('PN7921', 'NCC001', '2023-12-12', 126500),
('PN8653', 'NCC001', '2023-12-12', 5000),
('PN9190', 'NCC001', '2023-12-21', 3000);


CREATE TABLE CTPHIEUNHAP (
    MAPN VARCHAR(10) NOT NULL,
    MAHH VARCHAR(10) NOT NULL,
    SOLUONG INT NOT NULL,
    GIANHAP INT NOT NULL,
    CONSTRAINT CTPN_PK PRIMARY KEY (MAPN, MAHH),
    CONSTRAINT FK_CTPHIEUNHAP_MAHH FOREIGN KEY (MAHH) REFERENCES HANGHOA(MAHH),
    CONSTRAINT FK_CTPHIEUNHAP_CTPN FOREIGN KEY (MAPN) REFERENCES PHIEUNHAPHANG(MAPN)
);

insert into CTPHIEUNHAP values
('PN7240', 'HH002', 100, 50),
('PN7240', 'HH003', 200, 350),
('PN7921', 'HH004', 500, 8),
('PN7921', 'HH005', 500, 50),
('PN7921', 'HH006', 500, 25),
('PN7921', 'HH007', 1000, 80),
('PN7921', 'HH008', 1000, 5),
('PN8653', 'HH002', 100, 50),
('PN9190', 'HH001', 300, 10)


CREATE TABLE tb_TaiKhoan (
    MaTaiKhoan INT IDENTITY(1,1) NOT NULL,
    TenTaiKhoan VARCHAR(50) NOT NULL,
    MatKhau VARCHAR(50) NULL,
    Quyen NVARCHAR(50) NULL,
    MaNV VARCHAR(10) NULL,
    CONSTRAINT PK_tb_TaiKhoan_1 PRIMARY KEY (MaTaiKhoan),
    CONSTRAINT FK_tb_TaiKhoan_MANV FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MANV)
);

SET IDENTITY_INSERT tb_TaiKhoan ON;

INSERT INTO tb_TaiKhoan (MaTaiKhoan, TenTaiKhoan, MatKhau, Quyen, MaNV)
VALUES 
(2, 'Hung@gmail.com', '123', N'Admin', 'NV004'),
(3, 'Binh@gmail.com', '123', N'User', 'NV003'),
(4, 'Lan@gmail.com', '123', N'Admin', 'NV001'),
(5, 'Tu@gmail.com', '123', N'Admin', 'NV002');

SET IDENTITY_INSERT tb_TaiKhoan OFF;

--========================================= ALTER TABLE=======================================
GO
ALTER TABLE tb_TaiKhoan ADD CONSTRAINT DF_tb_TaiKhoan_Quyen DEFAULT (user_name()) FOR Quyen;

ALTER TABLE tb_TaiKhoan DROP CONSTRAINT DF_tb_TaiKhoan_Quyen;
GO
--========================================= TRIGGER============================================
CREATE TRIGGER trg_UpdateSoLuongTon
ON CTHOADON
AFTER INSERT
AS
BEGIN
    UPDATE HANGHOA
    SET SoLuongTon = SoLuongTon - I.SoLuong
    FROM HANGHOA
    INNER JOIN INSERTED I ON HANGHOA.MAHH = I.MaHH;
END;

-- INSERT INTO
--INSERT INTO HOADON (MAHD, NGAYHD, MANV, THANHTIEN) VALUES ('HD123', '2023-12-12', 'NV001', 390000);
--INSERT INTO CTHOADON (MAHD, MAHH, SOLUONG, TongTien) VALUES ('HD123', 'HH001', 10, 2000);

-- SELECT
SELECT * FROM CTHOADON;
SELECT * FROM HANGHOA;
GO
--======================================== FUNCTION =================================================
CREATE FUNCTION dbo.CalculateTotalRevenue (@maHH NVARCHAR(50), @tenHH NVARCHAR(50), @selectedMonth DATE)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @TongDoanhThu DECIMAL(18, 2);

    SELECT @TongDoanhThu = SUM(CTHOADON.SOLUONG * HANGHOA.GIABAN)
    FROM CTHOADON
    JOIN HOADON ON CTHOADON.MAHD = HOADON.MAHD
    JOIN HANGHOA ON CTHOADON.MAHH = HANGHOA.MAHH
    WHERE HANGHOA.MAHH = @maHH AND HANGHOA.TENHH = @tenHH
    AND HOADON.NGAYHD >= DATEADD(MONTH, DATEDIFF(MONTH, '19000101', @selectedMonth), '19000101')
    AND HOADON.NGAYHD < DATEADD(MONTH, DATEDIFF(MONTH, '19000101', @selectedMonth) + 1, '19000101');

    RETURN @TongDoanhThu;
END;
GO

CREATE FUNCTION dbo.CalculateTotalRevenueByYear (@maHH NVARCHAR(50), @tenHH NVARCHAR(50), @selectedYear INT)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @TongDoanhThu DECIMAL(18, 2);

    SELECT @TongDoanhThu = SUM(CTHOADON.SOLUONG * HANGHOA.GIABAN)
    FROM CTHOADON
    JOIN HOADON ON CTHOADON.MAHD = HOADON.MAHD
    JOIN HANGHOA ON CTHOADON.MAHH = HANGHOA.MAHH
    WHERE HANGHOA.MAHH = @maHH AND HANGHOA.TENHH = @tenHH
    AND YEAR(HOADON.NGAYHD) = @selectedYear;

    RETURN @TongDoanhThu;
END;
GO
-- =============================================TRIGGER=====================================================
CREATE TRIGGER trg_UpdateSoLuongTon_HangHoa
ON CTPHIEUNHAP
AFTER INSERT
AS
BEGIN
    UPDATE HANGHOA
    SET SoLuongTon = HANGHOA.SoLuongTon + i.SOLUONG
    FROM HANGHOA
    INNER JOIN inserted i ON HANGHOA.MAHH = i.MAHH;
END;

-- INSERT INTO
INSERT INTO PHIEUNHAPHANG (MAPN, MANCC, NGAYNHAP, TONGTIEN) VALUES ('PN0001', 'NCC003', '2023-12-14', 75000);
INSERT INTO CTPHIEUNHAP (MAPN, MAHH, SOLUONG, GIANHAP) VALUES ('PN0001', 'HH001', 10, 5000);

-- SELECT
SELECT * FROM HOADON;

---====================================== back up dữ liệu ===================================================

-- Thực hiện FULL backup
BACKUP DATABASE QL_SieuThiMiNi
TO DISK = 'E:\LuuDuLieuSinhVien\2001216332_TruongThanhY\Backup\Full_QL_SieuThiMiNi.bak'
WITH INIT

-- Thực hiện FULL backup
BACKUP DATABASE QL_SieuThiMiNi
TO DISK = 'E:\LuuDuLieuSinhVien\2001216332_TruongThanhY\Backup\Full_QL_SieuThiMiNi(trn).trn'
WITH INIT

-- Thực hiện Differential backup
BACKUP DATABASE QL_SieuThiMiNi
TO DISK = 'E:\LuuDuLieuSinhVien\2001216332_TruongThanhY\Backup\Diff_QL_SieuThiMiNi.bak'
WITH DIFFERENTIAL

-- Thực hiện Log backup
BACKUP LOG QL_SieuThiMiNi
TO DISK = 'E:\LuuDuLieuSinhVien\2001216332_TruongThanhY\Backup\Log_QL_SieuThiMiNi.bak'

---====================================== Phục hồi cơ sở dữ liệu ===================================================

USE master
go
RESTORE DATABASE QL_SieuThiMiNi
FROM DISK = 'E:\LuuDuLieuSinhVien\2001216332_TruongThanhY\Backup\Full_QL_SieuThiMiNi.bak'
WITH NORECOVERY

-- Phục hồi Differential backup
RESTORE DATABASE QL_SieuThiMiNi
FROM DISK = 'E:\LuuDuLieuSinhVien\2001216332_TruongThanhY\Backup\Diff_QL_SieuThiMiNi.bak'
WITH NORECOVERY

-- Phục hồi Log backup
RESTORE LOG QL_SieuThiMiNi
FROM DISK = 'E:\LuuDuLieuSinhVien\2001216332_TruongThanhY\Backup\Log_QL_SieuThiMiNi.bak'
WITH RECOVERY