-- =========================
-- DỮ LIỆU MẪU CHO CATEGORIES
-- =========================
SET IDENTITY_INSERT Categories ON;

INSERT INTO Categories (Id, Name)
VALUES
(1, N'Bút'),
(2, N'Vở và giấy'),
(3, N'Đồ dùng học sinh'),
(4, N'Hoa phấn'),
(5, N'Sách và tài liệu'),
(6, N'Máy tính');

SET IDENTITY_INSERT Categories OFF;

-- =========================
-- DỮ LIỆU MẪU CHO SUPPLIERS
-- =========================
INSERT INTO Suppliers (MaNCC, TenNCC, ContactName, SDT, Email, DiaChi)
VALUES
('NCC001', N'Công ty TNHH Bút Viết Việt', N'Nguyễn Văn A', '0901234567', 'ncc1@example.com', N'123 Lê Lợi, Hà Nội'),
('NCC002', N'Công ty Giấy An Bình', N'Trần Thị B', '0902345678', 'ncc2@example.com', N'45 Nguyễn Huệ, TP.HCM'),
('NCC003', N'Đồ dùng học sinh Hoa Mai', N'Lê Văn C', '0903456789', 'ncc3@example.com', N'78 Trần Phú, Đà Nẵng'),
('NCC004', N'Hoa Phấn Co., Ltd', N'Phạm Thị D', '0904567890', 'ncc4@example.com', N'12 Pasteur, Cần Thơ'),
('NCC005', N'Công ty Sách & Tài liệu Minh Quang', N'Vũ Văn E', '0905678901', 'ncc5@example.com', N'56 Điện Biên Phủ, Hải Phòng');

-- =========================
-- DỮ LIỆU MẪU CHO WAREHOUSES
-- =========================
SET IDENTITY_INSERT Warehouses ON;

INSERT INTO Warehouses (Id, Name, Location, Region, Province, District, MapCoordinates)
VALUES
(1, N'Kho Hà Nội', N'Khu CN Bắc Thăng Long', N'Bắc', N'Hà Nội', N'Đông Anh', '21.1456,105.8456'),
(2, N'Kho TP.HCM', N'Khu CN Tân Bình', N'Nam', N'TP.HCM', N'Tân Bình', '10.8012,106.6520'),
(3, N'Kho Đà Nẵng', N'Khu CN Hòa Khánh', N'Trung', N'Đà Nẵng', N'Liên Chiểu', '16.0718,108.1527');

SET IDENTITY_INSERT Warehouses OFF;

-- =========================
-- DỮ LIỆU MẪU CHO CUSTOMERS
-- =========================
INSERT INTO Customers (FullName, PhoneNumber, Email, Age, Address, Gender)
VALUES
(N'Nguyễn Hoàng Long', '0912345678', 'longnguyen@example.com', 28, N'25 Lý Thường Kiệt, Hà Nội', N'Nam'),
(N'Trần Thị Mai', '0923456789', 'maitran@example.com', 32, N'102 Nguyễn Trãi, TP.HCM', N'Nữ'),
(N'Lê Văn Minh', '0934567890', 'minhle@example.com', 40, N'88 Hùng Vương, Đà Nẵng', N'Nam'),
(N'Phạm Thu Trang', '0945678901', 'trangpham@example.com', 26, N'12 Phan Bội Châu, Hải Phòng', N'Nữ');

-- =========================
-- DỮ LIỆU MẪU CHO PRODUCTS
-- =========================
INSERT INTO Products (ImagePath, Name, Description, Quantity, Price, CreatedAt, CategoryId, WarehouseId, MaNCC)
VALUES
('images/but_bi.jpg', N'Bút bi Thiên Long', N'Bút bi mực xanh, ngòi 0.5mm', 500, 3000, GETDATE(), 1, 1, 'NCC001'),
('images/vo_96_trang.jpg', N'Vở 96 trang', N'Vở học sinh, giấy trắng mịn', 300, 8000, GETDATE(), 2, 1, 'NCC002'),
('images/hop_but.jpg', N'Hộp bút nhựa', N'Hộp bút học sinh màu xanh', 200, 25000, GETDATE(), 3, 2, 'NCC003'),
('images/hoa_phan.jpg', N'Hoa phấn bảng trắng', N'Phấn không bụi, an toàn sức khỏe', 150, 12000, GETDATE(), 4, 2, 'NCC004'),
('images/sach_tieng_anh.jpg', N'Sách tiếng Anh lớp 6', N'Sách giáo khoa tiếng Anh', 100, 35000, GETDATE(), 5, 3, 'NCC005'),
('images/may_tinh_casio.jpg', N'Máy tính Casio fx-570VN Plus', N'Máy tính khoa học 570 chức năng', 50, 450000, GETDATE(), 6, 3, 'NCC003');
