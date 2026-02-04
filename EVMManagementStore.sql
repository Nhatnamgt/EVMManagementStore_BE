-- Tạo database
CREATE DATABASE EVMManagementStore;
GO
USE EVMManagementStore;
GO

-- Bảng Role
CREATE TABLE Role (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL
);

-- Bảng Users
CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50) UNIQUE NOT NULL,
    email NVARCHAR(100) UNIQUE NOT NULL,
    password_hash NVARCHAR(200) NOT NULL,
    role_id INT NOT NULL,
    full_name NVARCHAR(100),
    phone NVARCHAR(20),
    address NVARCHAR(200),
    company_name NVARCHAR(200),
    FOREIGN KEY (role_id) REFERENCES Role(role_id)
);

--Bảng Discounts
CREATE TABLE Discounts (
    discount_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,                          
    discount_code NVARCHAR(100) NOT NULL UNIQUE,
    discount_name NVARCHAR(200) NOT NULL,
    discount_type NVARCHAR(50) NOT NULL CHECK (discount_type IN ('amount', 'percent')),  
    discount_value DECIMAL(18,2) NOT NULL,         
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE', 
    FOREIGN KEY (user_id) REFERENCES Users(user_id)
);


-- Bảng Vehicles
CREATE TABLE Vehicles (
    vehicle_id INT IDENTITY(1,1) PRIMARY KEY,
    type NVARCHAR(50),
    model NVARCHAR(100) NOT NULL,
    version NVARCHAR(50),
	distance NVARCHAR(50),
	timecharging NVARCHAR(50),
	speed NVARCHAR(50),
	image1 NVARCHAR(500),
	image2 NVARCHAR(500),
	image3 NVARCHAR(500),
    color NVARCHAR(100),
	discount_id INT NULL,
    price DECIMAL(18,2) NOT NULL,
	final_price DECIMAL(18,2) NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
	FOREIGN KEY (discount_id) REFERENCES Discounts(discount_id)
);


-- Bảng Inventory
CREATE TABLE Inventory (
    inventory_id INT IDENTITY(1,1) PRIMARY KEY,
    vehicle_id INT NOT NULL,
    quantity INT NOT NULL,
	color NVARCHAR(100) NOT NULL,
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id)
);

-- Bảng Quotations
CREATE TABLE Quotations (
    quotation_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    vehicle_id INT NOT NULL,
	color nvarchar(50) NULL,
    quotation_date DATETIME DEFAULT NULL,
    base_price DECIMAL(18,2) NOT NULL,
    final_price DECIMAL(18,2) NOT NULL,
	attachment_image NVARCHAR(500) NULL,   
    attachment_file NVARCHAR(500) NULL,
	promotion_code NVARCHAR(200) NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'DRAFT',
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id)
);

-- Bảng Orders
CREATE TABLE Orders (
    order_id INT IDENTITY(1,1) PRIMARY KEY,
    quotation_id INT,
    user_id INT NOT NULL,
    vehicle_id INT NOT NULL,
    color NVARCHAR(50) NULL,
    order_date DATETIME DEFAULT NULL,
    delivery_address NVARCHAR(200) NULL,
    attachment_image NVARCHAR(500) NULL,   
    attachment_file NVARCHAR(500) NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
    promotion_code NVARCHAR(200) NULL,
    quotation_price DECIMAL(18,2) NOT NULL,
    final_price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (quotation_id) REFERENCES Quotations(quotation_id),
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id)
);

--DealerOrders
CREATE TABLE DealerOrders (
    dealer_order_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    order_id INT NOT NULL,
    vehicle_id INT NOT NULL,
    quantity INT NOT NULL,
    color NVARCHAR(50) NULL,
    order_date DATETIME DEFAULT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
    payment_status NVARCHAR(50) NOT NULL,
    total_amount DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (order_id) REFERENCES Orders(order_id),
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id)
);


-- Deliveries
CREATE TABLE Deliveries (
    delivery_id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT NOT NULL,
    user_id INT NOT NULL,
    vehicle_id INT NOT NULL,
    delivery_date DATETIME NULL,
    delivery_status NVARCHAR(50) ,
    notes NVARCHAR(255),
    FOREIGN KEY (order_id) REFERENCES Orders(order_id),
	FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id)
);


-- Bảng Payments
CREATE TABLE Payments (
    payment_id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT NOT NULL,
    payment_date DATETIME DEFAULT NULL,
    amount DECIMAL(18,2) NOT NULL,
    method NVARCHAR(50) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
    FOREIGN KEY (order_id) REFERENCES Orders(order_id)
);

-- Bảng Promotions
CREATE TABLE Promotions (
    promotion_id INT IDENTITY(1,1) PRIMARY KEY,
	user_id INT NOT NULL,
    promotion_code NVARCHAR(100) NOT NULL,
	stock INT NOT NULL,
	option_name NVARCHAR(100) NOT NULL,
    option_value DECIMAL(18,2),
    start_date DATE,
    end_date DATE,
    FOREIGN KEY (user_id) REFERENCES Users(user_id)
);

-- Bảng SalesContracts
CREATE TABLE SalesContracts (
    sales_contract_id INT IDENTITY(1,1) PRIMARY KEY,
	user_id INT NOT NULL,
    order_id INT NOT NULL,
    contract_date DATETIME DEFAULT NULL,
    terms NVARCHAR(500),
    signed_by_dealer NVARCHAR(100),
    payment_method NVARCHAR(50) NULL,
    cccd NVARCHAR(20) NULL,  
    contract_file NVARCHAR(500) NULL,
    contract_image NVARCHAR(500) NULL,
    FOREIGN KEY (order_id) REFERENCES Orders(order_id),
	FOREIGN KEY (user_id) REFERENCES Users(user_id)
);

-- Bảng Reports
CREATE TABLE Reports (
    report_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    order_id INT,
    report_type NVARCHAR(50) NOT NULL,
    createdDate DATE,
    resolvedDate DATE,
    content NVARCHAR(MAX),
	status NVARCHAR(MAX),
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (order_id) REFERENCES Orders(order_id)
);

-- Bảng TestDriveAppointments
CREATE TABLE TestDriveAppointments (
    appointment_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
	username nvarchar(50),
	address nvarchar(150),
    vehicle_id INT NOT NULL,
    appointment_date DATETIME NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id)
);

----------------------------------------------------
-- Insert Sample Data
----------------------------------------------------

-- Role
INSERT INTO Role (role_name) VALUES 
('admin'), ('dealer'), ('evm_staff'), ('customer');

-- Users
INSERT INTO Users (username, email, password_hash, role_id, full_name, phone, address, company_name)
VALUES 
('admin', 'admin@gmail.com', '123456', 1, 'admin One', '0901234567', 'HCM', 'Company A'),
('dealer1', 'dealer1@gmail.com', '123456', 2, 'dealer One', '0912345678', 'Hanoi', 'Company B'),
('dealer2', 'dealer2@gmail.com', '123456', 2, 'dealer Two', '0912345678', 'HCM', 'Company B'),
('evm_staff1', 'staff1@gmail.com', '123456', 3, 'staff One', '0901234567', 'HCM', 'Company A'),
('evm_staff2', 'staff2@gmail.com', '123456', 3, 'staff Two', '0901234567', 'HCM', 'Company A'),
('customer1', 'customer1@gmail.com', '123456', 4, 'customer One', '0912345678', 'Hanoi', NULL),
('customer2', 'customer2@gmail.com', '123456', 4, 'customer Two', '0987676542', 'HCM', NULL);


--Discounts
INSERT INTO Discounts (user_id, discount_code, discount_name, discount_type, discount_value, start_date, end_date, status)
VALUES
(1, 'GIAM100K', N'Giảm 100.000đ ', 'amount', 100000, '2025-11-01', '2025-12-30', 'ACTIVE'),
(1, 'GIAM500K', N'Giảm 500.000đ ', 'amount', 500000, '2025-11-01', '2025-12-30', 'ACTIVE'),
(3, 'GIAM10%', N'Giảm 10% ', 'percent', 10, '2025-11-01', '2025-12-30', 'ACTIVE'),
(3, 'GIAM15%', N'Giảm 15% ', 'percent', 15, '2025-11-05', '2025-12-30', 'ACTIVE'),
(3, 'GIAMCU', N'Chương trình khuyến mãi đã hết hạn', 'amount', 300000, '2025-09-01', '2025-09-30', 'EXPIRED');

-- Vehicles
INSERT INTO Vehicles (type, model, version, distance, timecharging, speed, image1, image2, image3, color, price)
VALUES 
('SUV', 'Vinfast VF3', '2025','210km','5-hours','~100 km/h','https://vinfasttimescity.vn/wp-content/uploads/2024/08/vf31.png','https://product.hstatic.net/200000960063/product/vf_3_zenith_grey_with_wheel_cover__4__2e2b770321f1476586ea87c0e79b00a2_master.png','https://vinfastvietnam.com.vn/wp-content/uploads/2023/09/Do-min.png','White', 249000000.00),
('SUV', 'Vinfast VF5', '2025','326km','5-hours','~130km/h',
 'https://vinfastotosaigon.com/OTO3602400600/files/mau_xe/VF5_plus/181Y.webp','https://vinfastotosaigon.com/OTO3602400600/files/mau_xe/VF5_plus/181X.webp','https://vinfast3sthanhhoa.com/wp-content/uploads/2022/12/vf5-1.png','Black', 529000000.00),
('SUV', 'Vinfast VF6', '2025','400km','6-hours','200km/h',
 'https://vinfastninhbinh.com.vn/wp-content/uploads/2024/06/vinfast-vf6-6.png','https://vinfast-cars.vn/wp-content/uploads/2024/09/vinfastvf6-1.png','https://vinfastotothanhhoa.vn/OTO3602400618/files/san_pham/VF6/mau_xe/CE18.webp','Blue', 668000000.00),
('SUV', 'Vinfast VF7', '2025','450km','6-hours','250km/h',
 'https://vinfastphantrongtue.com/wp-content/uploads/2023/11/tai-xuong-8.png','https://vinfastphantrongtue.com/wp-content/uploads/2023/11/tai-xuong-1.png','https://vinfastdanang.net/wp-content/uploads/2023/06/vf7icon.png','Red', 799000000.00),
('SUV', 'Vinfast VF8', '2025','412km','6-hours','270km/h',
 'https://vinfasttruongchinh.net/wp-content/uploads/2023/10/vf8-do.png','https://vinfastdienchau.com/wp-content/uploads/2013/08/VF-8-Plus_EU-VN_20inch_Crimson-Red-4-scaled.png','https://vinfastdienchau.com/wp-content/uploads/2013/08/VF-8-Plus_EU-VN_20inch_Crimson-Red-4-scaled.png','White', 1019000000.00),
('SUV', 'Vinfast VF9', '2025','531km','10%-70% ~35 min','~270km/h',
 'https://vinfastvinh-nghean.com/wp-content/uploads/2022/02/vf9-1.png','https://vinfast-cars.vn/wp-content/uploads/2024/09/VinFast-VF-9-mau-Den.png','','Silver', 1499000000.00);

-- Inventory
INSERT INTO Inventory (vehicle_id, color, quantity)
VALUES
(1,'White',10),
(2,'Black',5),
(3,'Blue',0),
(4,'Red',0),
(5,'White',0),
(6,'Silver',0);

-- Quotations
INSERT INTO Quotations (user_id, vehicle_id, quotation_date, base_price, final_price, attachment_file, attachment_image,promotion_code,color)
VALUES 
(1, 2, GETDATE(), 1500000000.00, 150000000.00, 'uploads/hop-dong-mua-ban-xe-may.doc', 'uploads/hoa-don-ban-le.jpg','tang chai nuoc mui thom','red'),
(2, 3, GETDATE(), 1000000000.00, 90000000.00, 'uploads/hop-dong-mua-ban-xe-may.doc', 'uploads/hoa-don-ban-le.jpg','tang decal dan xe','blue');
-- Orders
INSERT INTO Orders (quotation_id, user_id, vehicle_id, final_price,order_date,attachment_file, attachment_image,promotion_code,color,quotation_price,status)
VALUES 
(1, 2, 1, 1500000000.00,GETDATE(),'uploads/hop-dong-mua-ban-xe-may.doc','uploads/hoa-don-ban-le.jpg','tang chai nuoc mui thom','red',1500000000,'approved'),
(2, 2, 2, 90000000.00,GETDATE(),'uploads/hop-dong-mua-ban-xe-may.doc','uploads/hoa-don-ban-le.jpg','tang chai nuoc mui thom','blue',90000000,'approved'),
(2, 3, 3, 90000000.00,GETDATE(),'uploads/hop-dong-mua-ban-xe-may.doc','uploads/hoa-don-ban-le.jpg','tang decal dan xe','blue',90000000,'denied');

-- Deliveries
INSERT INTO Deliveries (vehicle_id,user_id,order_id, delivery_date, delivery_status,notes)
VALUES 
( 1, 2, 1, GETDATE(),'Deliverd','da giao dung gio'),
( 2, 3, 2, GETDATE(),'ON THE WAY','ket xe den tre');

-- DealerOrders
INSERT INTO DealerOrders (user_id,order_id ,vehicle_id ,quantity, order_date, total_amount,payment_status,color,status)
VALUES 
(2, 1, 1, 5, GETDATE(), 1000000000.00,'unpaid','red','PENDING'),
(3, 2, 2, 2, GETDATE(), 500000000.00,'paid','blue','Approved');

-- Payments
INSERT INTO Payments (order_id, payment_date, amount, method)
VALUES 
(1, GETDATE(), 1500000000.00, 'CREDIT_CARD'),
(2, GETDATE(), 90000000.00, 'BANK_TRANSFER'),
(3, GETDATE(), 90000000.00, 'CASH');

-- Promotions
INSERT INTO Promotions (user_id, promotion_code, start_date, end_date,option_name,option_value,stock)
VALUES 
(2,'DECAL','2025-06-01', '2025-06-30', 'tang decal dan xe',100000,2),
(3,'MUITHOM','2025-01-01', '2025-01-15','tang chai nuoc mui thom',200000,1);

-- SalesContracts
INSERT INTO SalesContracts (order_id,user_id,contract_date, terms, signed_by_dealer,contract_file,contract_image)
VALUES 
(1,2,GETDATE(), 'Standard Terms', 'Dealer One','uploads/hop-dong-mua-ban-xe-may.doc','uploads/hoa-don-ban-le.jpg'),
(2,3, GETDATE(), 'Premium Terms', 'Dealer Two','uploads/hop-dong-mua-ban-xe-may.doc','uploads/hoa-don-ban-le.jpg');

-- Reports
INSERT INTO Reports (user_id, order_id, report_type, createdDate, resolvedDate,content,status)
VALUES 
(6, 1, 'Sales', '2025-01-01', '2025-03-31', 'xe bi loi gat mua','Da Xu li'),
(7, 2, 'Sales', '2025-04-01', '2025-06-30', 'xe loi pin','Dang Xu li');

-- TestDriveAppointments
INSERT INTO TestDriveAppointments (user_id, vehicle_id, appointment_date,username,address)
VALUES 
(6, 1, GETDATE(),'nam','123 phan dinh phung'),
(7, 2, GETDATE(),'hung anh','vinhome ');





