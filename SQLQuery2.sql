create database DARKTHESTORE
use DARKTHESTORE
create table KHACHHANG(
ID_KH int IDENTITY(1,1) primary key,
TenKH nvarchar(50),
DiaChi nvarchar(255),
SDT nvarchar(10),
GioTinh nvarchar(5),
NgaySinh date,
Email nvarchar(50),
TK nvarchar(20),
MK nvarchar(20),
PhanQuyen int
)

create table GIOHANG(
ID_GH int IDENTITY(1,1) PRIMARY KEY,
ID_KH int,
constraint fk1 foreign key (ID_KH) references KHACHHANG(ID_KH)
)
create table DANHMUC(
ID_DM int identity(1,1) primary key,
TenDM nvarchar(255)
)
create table SANPHAM(
ID_SP int identity(1,1) primary key,
MaSP nvarchar(20),
TenSP nvarchar(255),
Gia int,
GiaBan int,
Mota nvarchar(255),
Status_SP int,
NgayTao date,
SoLuong int,
SoLuongBan int,
Images_url nvarchar(255),
ID_DM int,
constraint fk2 foreign key (ID_DM) references DANHMUC(ID_DM)
)
create table GIOHANG_SANPHAM(
ID_GH int,
ID_SP int,
SoLuong int,
constraint pk primary key(ID_GH,ID_SP),
constraint fk3 foreign key (ID_GH) references GIOHANG(ID_GH),
constraint fk4 foreign key (ID_SP) references SANPHAM(ID_SP)
)
create table DANHGIA(
ID_GH int identity(1,1) primary key,
NoiDung nvarchar(255),
Diem int,
ID_KH int,
ID_SP int,
constraint fk5 foreign key (ID_KH) references KHACHHANG(ID_KH),
constraint fk6 foreign key (ID_SP) references SANPHAM(ID_SP)
)
create table KHUYENMAI(
ID_KM int identity(1,1) primary key,
GiamGia int
)
create table DONHANG(
ID_DH int identity(1,1) primary key,
NgayLap date,
GhiChu nvarchar(255),
TrangThai nvarchar(30),
ID_KH int,
ID_KM int,
constraint fk7 foreign key (ID_KH) references KHACHHANG(ID_KH),
constraint fk8 foreign key (ID_KM) references KHUYENMAI(ID_KM)
)
create table DONHANG_SANPHAM(
ID_DH int,
ID_SP int,
SoLuong int,
constraint pk1 primary key(ID_DH,ID_SP),
constraint fk9 foreign key (ID_DH) references DONHANG(ID_DH),
constraint fk10 foreign key (ID_SP) references SANPHAM(ID_SP)
)
ALTER TABLE DONHANG
ADD Ten nvarchar(255),
 DiaChiGiaoHang nvarchar(255),
 SDT nvarchar(10),
 PhuongthucTT nvarchar(255)

ALTER TABLE KHUYENMAI
ADD Mota nvarchar(25)


ALTER TABLE DONHANG
ADD PhuongThucNhanHang nvarchar(1000)

ALTER TABLE KHUYENMAI
ALTER COLUMN Mota NVARCHAR(4000);

ALTER TABLE DONHANG
ALTER COLUMN DiaChiGiaoHang NVARCHAR(4000);

ALTER TABLE SANPHAM
ALTER COLUMN Mota NVARCHAR(MAX);

ALTER TABLE DANHGIA
ALTER COLUMN NoiDung NVARCHAR(2000);

ALTER TABLE DONHANG_SANPHAM
ADD DonGia int

ALTER TABLE KHUYENMAI
ADD TrangThai int

select * from DANHGIA
select * from SANPHAM
select * from DANHMUC
select * from DONHANG_SANPHAM
select * from GIOHANG
select * from GIOHANG_SANPHAM
select * from DONHANG
select * from KHUYENMAI
select * from KHACHHANG
insert into KHACHHANG values
('Admin','HaNoi','','Nam','','','admin','abc123','3')
insert into KHACHHANG values
('tung','ThaiBinh','','Nam','','','tung2406','abc123','1')
('Admin','HaNoi','','Nam','','','admin','abc123','3')
insert into KHACHHANG values
('tung','ThaiBinh','','Nam','','','tung2406','abc123','1')

delete KHACHHANG
where(ID_KH=5)

SELECT * FROM KHACHHANG WHERE TK = 'tung2406'
SELECT * FROM GIOHANG WHERE ID_KH = 2
SELECT * FROM GIOHANG_SANPHAM
select * from SANPHAM