CREATE TABLE users
(
	id INT PRIMARY KEY IDENTITY(1,1),
	username VARCHAR(MAX) NULL,
	password VARCHAR(MAX) NULL,
	profile_image VARCHAR(MAX) NULL,
	role VARCHAR(MAX) NULL,
	status VARCHAR(MAX) NULL,
	date_reg DATE NULL
)

select * from users

INSERT INTO users (username, password, profile_image, role, status, date_reg) VALUES ('admin', 'admin123', '', 'Admin', 'Active', '2024-11-4')


CREATE TABLE products
(
	id INT PRIMARY KEY IDENTITY(1,1),
	prod_id VARCHAR(MAX) NULL,
	prod_name VARCHAR(MAX) NULL,
	prod_type VARCHAR(MAX) NULL,
	prod_stock INT NULL,
	prod_price FLOAT NULL,
	prod_status VARCHAR(MAX) NULL,
	prod_image VARCHAR(MAX) NULL,
	date_insert DATE NULL,
	date_update DATE NULL,
	date_delete DATE NULL,
)

SELECT * FROM products

CREATE TABLE orders
(
	id INT PRIMARY KEY IDENTITY(1,1),
	customer_id INT NULL,
	prod_id VARCHAR(MAX) NULL,
	prod_name VARCHAR(MAX) NULL,
	prod_type VARCHAR(MAX) NULL,
	prod_price FLOAT NULL,
	order_date DATE NULL,
	delete_order DATE NULL
)

ALTER TABLE orders
ADD qty INT NULL

SELECT * FROM orders



SELECT MAX(id) FROM orders

CREATE TABLE customers
(
	id INT PRIMARY KEY IDENTITY(1,1),
	customer_id INT NULL,
	total_price FLOAT NULL,
	date DATE NULL,
	amount FLOAT NULL,
	change FLOAT NULL
)

ALTER TABLE customers
ALTER COLUMN amount FLOAT

ALTER TABLE customers
ADD change FLOAT NULL

SELECT * FROM customers

SELECT * FROM orders WHERE customer_id = 1

SELECT SUM(prod_price * qty) FROM orders WHERE customer_id = 1

SELECT * FROM products WHERE prod_name LIKE 'or' OR prod_type LIKE 'or' AND date_delete IS NULL

SELECT id FROM users WHERE username = 'admin';

UPDATE users
SET profile_image = ''
WHERE id = 1;