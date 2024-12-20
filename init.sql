CREATE DATABASE IF NOT EXISTS catalog_db;

USE catalog_db;

CREATE TABLE IF NOT EXISTS Products (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Price DECIMAL(10, 2) NOT NULL,
    Stock INT NOT NULL
);

INSERT INTO Products (Name, Description, Price, Stock)
VALUES
    ('Book A', 'A fascinating book about coding.', 10.99, 50),
    ('Book B', 'An intriguing novel about adventure.', 15.49, 30),
    ('Notebook', 'A plain notebook for writing.', 5.99, 100),
    ('Pen', 'A smooth-writing ballpoint pen.', 1.49, 200),
    ('Marker', 'A permanent marker.', 2.99, 150);


CREATE DATABASE IF NOT EXISTS order_db;

USE order_db;

CREATE TABLE IF NOT EXISTS Orders (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CustomerId INT NOT NULL,
    ProductIds VARCHAR(255) NOT NULL,
    TotalPrice DECIMAL(10, 2) NOT NULL,
    OrderDate DATETIME NOT NULL
);

INSERT INTO Orders (CustomerId, ProductIds, TotalPrice, OrderDate)
VALUES
    (1, '1,2', 26.48, '2024-12-19 10:00:00'),
    (2, '3,4', 7.48, '2024-12-18 15:30:00');

GRANT ALL PRIVILEGES ON order_db.* TO 'order_user'@'%';
GRANT ALL PRIVILEGES ON catalog_db.* TO 'catalog_user'@'%';
FLUSH PRIVILEGES;