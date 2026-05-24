-- =============================================================================
-- SeedData.sql
-- Each section is guarded with IF NOT EXISTS so inserts are idempotent.
-- User is seeded separately in C# (DatabaseInitializer) to allow password
-- hashing via PasswordHasher. All other tables are seeded here.
-- IDENTITY_INSERT is used to keep IDs explicit and FK references predictable.
-- =============================================================================


-- ── Categories (IDs 1-5) ─────────────────────────────────────────────────────

IF NOT EXISTS (SELECT 1 FROM Categories)
BEGIN
    SET IDENTITY_INSERT Categories ON;

    INSERT INTO Categories (Id, Name, Description) VALUES
        (1, 'Electronics',      'High-tech electronic devices and accessories'),
        (2, 'Clothing',         'Fashion apparel for men and women'),
        (3, 'Books',            'Educational and entertainment reading material'),
        (4, 'Home & Garden',    'Products for home improvement and gardening'),
        (5, 'Sports & Fitness', 'Equipment and apparel for sports and fitness');

    SET IDENTITY_INSERT Categories OFF;
END;


-- ── Products (IDs 1-10, 2 per category) ──────────────────────────────────────

IF NOT EXISTS (SELECT 1 FROM Products)
BEGIN
    SET IDENTITY_INSERT Products ON;

    INSERT INTO Products (Id, CategoryId, Name, Description, Price) VALUES
        -- Electronics
        ( 1, 1, 'Laptop Pro 15',           'High-performance 15-inch laptop with 16 GB RAM and SSD',          1299.99),
        ( 2, 1, 'Wireless Headphones',     'Noise-cancelling over-ear Bluetooth headphones with 30h battery',  149.99),
        -- Clothing
        ( 3, 2, 'Men''s Polo Shirt',       'Classic fit polo shirt available in multiple colours',              39.99),
        ( 4, 2, 'Women''s Running Jacket', 'Lightweight wind-resistant running jacket with reflective strips',  89.99),
        -- Books
        ( 5, 3, 'Clean Code',              'A Handbook of Agile Software Craftsmanship - Robert C. Martin',    34.99),
        ( 6, 3, 'Design Patterns',         'Elements of Reusable Object-Oriented Software - GoF',              44.99),
        -- Home & Garden
        ( 7, 4, 'Garden Hose 50ft',        'Heavy-duty expandable garden hose with adjustable spray nozzle',   49.99),
        ( 8, 4, 'LED Desk Lamp',           'Adjustable arm LED desk lamp with USB charging port',              59.99),
        -- Sports & Fitness
        ( 9, 5, 'Running Shoes',           'Lightweight cushioned road running shoes',                        119.99),
        (10, 5, 'Yoga Mat',                'Non-slip 6mm thick premium yoga mat with carry strap',             29.99);

    SET IDENTITY_INSERT Products OFF;
END;


-- ── Customers (IDs 1-2) ───────────────────────────────────────────────────────

IF NOT EXISTS (SELECT 1 FROM Customers)
BEGIN
    SET IDENTITY_INSERT Customers ON;

    INSERT INTO Customers (Id, FullName, DocumentNumber, Email, Phone, Address) VALUES
        (1, 'John Smith',   'DOC-00001', 'john.smith@example.com',   '+1-555-0101', '123 Main St, New York, NY 10001'),
        (2, 'Maria Garcia', 'DOC-00002', 'maria.garcia@example.com', '+1-555-0202', '456 Oak Ave, Los Angeles, CA 90001');

    SET IDENTITY_INSERT Customers OFF;
END;


-- ── Invoices (IDs 1-5) ────────────────────────────────────────────────────────
-- CreatedByUserId = 1 (admin user inserted by DatabaseInitializer on startup)

IF NOT EXISTS (SELECT 1 FROM Invoices)
BEGIN
    SET IDENTITY_INSERT Invoices ON;

    INSERT INTO Invoices (
        Id, InvoiceDate,              Total,   CreatedByUserId,
        CustomerId, CustomerFullName,      CustomerDocumentNumber,
        CustomerEmail,                 CustomerPhone,     CustomerAddress)
    VALUES
        -- Invoice 1: 10 line items → Customer 1 (John Smith)
        (1, '2024-01-15T10:00:00', 1919.90, 1,
         1, 'John Smith',   'DOC-00001', 'john.smith@example.com',   '+1-555-0101', '123 Main St, New York, NY 10001'),

        -- Invoice 2: 8 line items → Customer 2 (Maria Garcia)
        (2, '2024-02-20T14:30:00', 2084.87, 1,
         2, 'Maria Garcia', 'DOC-00002', 'maria.garcia@example.com', '+1-555-0202', '456 Oak Ave, Los Angeles, CA 90001'),

        -- Invoice 3: 5 line items → Customer 1 (John Smith)
        (3, '2024-03-10T09:15:00',  414.91, 1,
         1, 'John Smith',   'DOC-00001', 'john.smith@example.com',   '+1-555-0101', '123 Main St, New York, NY 10001'),

        -- Invoice 4: 3 line items → Customer 2 (Maria Garcia)
        (4, '2024-04-05T16:45:00',  329.94, 1,
         2, 'Maria Garcia', 'DOC-00002', 'maria.garcia@example.com', '+1-555-0202', '456 Oak Ave, Los Angeles, CA 90001'),

        -- Invoice 5: 1 line item  → Customer 1 (John Smith)
        (5, '2024-05-22T11:00:00', 1299.99, 1,
         1, 'John Smith',   'DOC-00001', 'john.smith@example.com',   '+1-555-0101', '123 Main St, New York, NY 10001');

    SET IDENTITY_INSERT Invoices OFF;
END;


-- ── Invoice Details (IDs 1-27) ────────────────────────────────────────────────
-- Totals per invoice:
--   Invoice 1 → 1919.90  (10 products × qty 1)
--   Invoice 2 → 2084.87  ( 8 products, varying qty)
--   Invoice 3 →  414.91  ( 5 products, varying qty)
--   Invoice 4 →  329.94  ( 3 products, varying qty)
--   Invoice 5 → 1299.99  ( 1 product  × qty 1)

IF NOT EXISTS (SELECT 1 FROM InvoiceDetails)
BEGIN
    SET IDENTITY_INSERT InvoiceDetails ON;

    INSERT INTO InvoiceDetails
        (Id, InvoiceId, ProductId, ProductName,             CategoryName,      UnitPrice, Quantity, Subtotal)
    VALUES
        -- ── Invoice 1 (10 line items, all products qty 1) ────────────────────
        ( 1, 1,  1, 'Laptop Pro 15',           'Electronics',      1299.99, 1, 1299.99),
        ( 2, 1,  2, 'Wireless Headphones',     'Electronics',       149.99, 1,  149.99),
        ( 3, 1,  3, 'Men''s Polo Shirt',       'Clothing',           39.99, 1,   39.99),
        ( 4, 1,  4, 'Women''s Running Jacket', 'Clothing',           89.99, 1,   89.99),
        ( 5, 1,  5, 'Clean Code',              'Books',              34.99, 1,   34.99),
        ( 6, 1,  6, 'Design Patterns',         'Books',              44.99, 1,   44.99),
        ( 7, 1,  7, 'Garden Hose 50ft',        'Home & Garden',      49.99, 1,   49.99),
        ( 8, 1,  8, 'LED Desk Lamp',           'Home & Garden',      59.99, 1,   59.99),
        ( 9, 1,  9, 'Running Shoes',           'Sports & Fitness',  119.99, 1,  119.99),
        (10, 1, 10, 'Yoga Mat',                'Sports & Fitness',   29.99, 1,   29.99),

        -- ── Invoice 2 (8 line items, varying quantities) ─────────────────────
        (11, 2,  1, 'Laptop Pro 15',           'Electronics',      1299.99, 1, 1299.99),
        (12, 2,  2, 'Wireless Headphones',     'Electronics',       149.99, 2,  299.98),
        (13, 2,  3, 'Men''s Polo Shirt',       'Clothing',           39.99, 3,  119.97),
        (14, 2,  4, 'Women''s Running Jacket', 'Clothing',           89.99, 1,   89.99),
        (15, 2,  5, 'Clean Code',              'Books',              34.99, 2,   69.98),
        (16, 2,  6, 'Design Patterns',         'Books',              44.99, 1,   44.99),
        (17, 2,  7, 'Garden Hose 50ft',        'Home & Garden',      49.99, 2,   99.98),
        (18, 2,  8, 'LED Desk Lamp',           'Home & Garden',      59.99, 1,   59.99),

        -- ── Invoice 3 (5 line items, varying quantities) ─────────────────────
        (19, 3,  3, 'Men''s Polo Shirt',       'Clothing',           39.99, 2,   79.98),
        (20, 3,  4, 'Women''s Running Jacket', 'Clothing',           89.99, 1,   89.99),
        (21, 3,  5, 'Clean Code',              'Books',              34.99, 3,  104.97),
        (22, 3,  6, 'Design Patterns',         'Books',              44.99, 2,   89.98),
        (23, 3,  7, 'Garden Hose 50ft',        'Home & Garden',      49.99, 1,   49.99),

        -- ── Invoice 4 (3 line items, varying quantities) ─────────────────────
        (24, 4,  8, 'LED Desk Lamp',           'Home & Garden',      59.99, 2,  119.98),
        (25, 4,  9, 'Running Shoes',           'Sports & Fitness',  119.99, 1,  119.99),
        (26, 4, 10, 'Yoga Mat',                'Sports & Fitness',   29.99, 3,   89.97),

        -- ── Invoice 5 (1 line item) ───────────────────────────────────────────
        (27, 5,  1, 'Laptop Pro 15',           'Electronics',      1299.99, 1, 1299.99);

    SET IDENTITY_INSERT InvoiceDetails OFF;
END;
