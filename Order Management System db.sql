-- create the database
create database OrderManagementSystem;

use OrderManagementSystem;

-- create user table
create table Users (
    userId int primary key identity,
    username varchar(255) not null,
    password varchar(255) not null,
    role varchar(50) check (role in ('Admin', 'User'))
);

-- create product table
create table Products (
    productId int primary key identity,
    productName varchar(255) not null,
    description varchar(255),
    price decimal(10, 2) not null,
    quantityInStock int not null,
    type varchar(50) check (type in ('Electronics', 'Clothing')),
    brand varchar(255), -- for Electronics
    warrantyPeriod int,  -- for Electronics
    size varchar(50),    -- for Clothing
    color varchar(50)    -- for Clothing
);

-- create orders table
create table Orders (
    orderId int primary key identity,
    userId int foreign key references Users(userId),
    orderDate datetime default getdate(),
    status varchar(50)
);

-- create orderitems table
create table OrderItems (
    orderItemId int primary key identity,
    orderId int foreign key references Orders(orderId),
    productId int foreign key references Products(productId),
    quantity int not null
);

-- Insert sample users
INSERT INTO Users (username, password, role) VALUES
('thiyanesh', 'thiyanesh@123', 'Admin'),
('ram', 'r@123', 'User'),
('jaanu', 'ram123', 'User'),
('barath', 'barath123', 'Admin'),
('Gokul', 'gokul123', 'User');

-- Insert sample products
INSERT INTO Products (productName, description, price, quantityInStock, type, brand, warrantyPeriod, size, color) VALUES
('Samsung Galaxy S21', 'Latest Samsung smartphone', 69999.99, 50, 'Electronics', 'Samsung', 24, NULL, NULL),
('Nike Running Shoes', 'Comfortable running shoes', 4999.99, 100, 'Clothing', NULL, NULL, '10', 'Blue'),
('Sony WH-1000XM4', 'Noise-cancelling headphones', 29999.99, 30, 'Electronics', 'Sony', 12, NULL, NULL),
('Levis Jeans', 'Stylish denim jeans', 3499.99, 75, 'Clothing', NULL, NULL, '32', 'Black'),
('LG 55 inch 4K TV', 'Ultra HD Smart TV', 89999.99, 20, 'Electronics', 'LG', 36, NULL, NULL),
('Adidas T-shirt', '100% cotton t-shirt', 1499.99, 150, 'Clothing', NULL, NULL, 'L', 'White');

-- Insert sample orders
INSERT INTO Orders (userId, orderDate, status) VALUES
(2, GETDATE(), 'Pending'),
(3, GETDATE(), 'Completed'),
(1, GETDATE(), 'Shipped');

-- Insert sample order items
INSERT INTO OrderItems (orderId, productId, quantity) VALUES
(1, 1, 1),  -- User 2 orders 1 Samsung Galaxy S21
(1, 2, 2),  -- User 2 orders 2 Nike Running Shoes
(2, 3, 1),  -- User 3 orders 1 Sony WH-1000XM4
(3, 5, 1),  -- Admin 1 orders 1 LG 55 inch 4K TV
(2, 4, 1);  -- User 2 orders 1 Levi's Jeans


select * from Products;
select * from Users;