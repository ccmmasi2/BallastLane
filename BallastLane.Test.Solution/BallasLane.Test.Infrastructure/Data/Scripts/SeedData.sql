IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@test.com')
BEGIN

INSERT INTO Users
(
    Username,
    Email,
    PasswordHash
)
VALUES
(
    'admin',
    'admin@test.com',
    '$2a$11$examplehash'
)

END