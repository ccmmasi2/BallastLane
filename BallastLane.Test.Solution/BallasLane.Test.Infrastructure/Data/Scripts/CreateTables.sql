IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
	CREATE TABLE Users (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		Username NVARCHAR(100) NOT NULL,
		Email NVARCHAR(200) NOT NULL UNIQUE,
		PasswordHash NVARCHAR(500) NOT NULL
	);
END 




IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
BEGIN
	CREATE TABLE Categories (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		Name NVARCHAR(100) NOT NULL,
		Description NVARCHAR(300)
	);
END 




IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
BEGIN
	CREATE TABLE Products (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		CategoryId INT NOT NULL,
		Name NVARCHAR(150) NOT NULL,
		Description NVARCHAR(500),
		Price DECIMAL(18,2) NOT NULL,

		CONSTRAINT FK_Products_Categories
		FOREIGN KEY (CategoryId)
		REFERENCES Categories(Id)
	);
END 




IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customers' AND xtype='U')
BEGIN
	CREATE TABLE Customers (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		FullName NVARCHAR(200) NOT NULL,
		DocumentNumber NVARCHAR(50),
		Email NVARCHAR(200),
		Phone NVARCHAR(50),
		Address NVARCHAR(300)
	);
END 




IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Invoices' AND xtype='U')
BEGIN
	CREATE TABLE Invoices (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		InvoiceDate DATETIME NOT NULL DEFAULT GETDATE(),
		Total DECIMAL(18,2) NOT NULL,
		CreatedByUserId INT NOT NULL,

		CustomerId INT NOT NULL,
		CustomerFullName NVARCHAR(200) NOT NULL,
		CustomerDocumentNumber NVARCHAR(50),
		CustomerEmail NVARCHAR(200),
		CustomerPhone NVARCHAR(50),
		CustomerAddress NVARCHAR(300),

		CONSTRAINT FK_Invoices_Customers
		FOREIGN KEY (CustomerId)
		REFERENCES Customers(Id),

		CONSTRAINT FK_Invoices_Users
		FOREIGN KEY (CreatedByUserId)
		REFERENCES Users(Id)
	);
END 




IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceDetails' AND xtype='U')
BEGIN
	CREATE TABLE InvoiceDetails (
		Id INT IDENTITY(1,1) PRIMARY KEY,

		InvoiceId INT NOT NULL,

		ProductId INT NOT NULL,
		ProductName NVARCHAR(200) NOT NULL,
		CategoryName NVARCHAR(100),

		UnitPrice DECIMAL(18,2) NOT NULL,
		Quantity INT NOT NULL,
		Subtotal DECIMAL(18,2) NOT NULL,

		CONSTRAINT FK_InvoiceDetails_Invoices
		FOREIGN KEY (InvoiceId)
		REFERENCES Invoices(Id),

		CONSTRAINT FK_InvoiceDetails_Products
		FOREIGN KEY (ProductId)
		REFERENCES Products(Id)
	);
END 

 
