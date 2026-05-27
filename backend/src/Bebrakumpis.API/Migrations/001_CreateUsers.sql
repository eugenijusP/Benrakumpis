CREATE TABLE users (
    id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    username    NVARCHAR(100)    NOT NULL UNIQUE,
    password_hash NVARCHAR(256)  NOT NULL,
    role        NVARCHAR(50)     NOT NULL DEFAULT 'User',
    created_at  DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Seed default admin user (password: Admin@123)
INSERT INTO users (id, username, password_hash, role)
VALUES (
    NEWID(),
    'admin',
    '$2a$11$mupcGyKyy3rVDrZltZccKu4VJSgx/S05d59hPdHTpKo5QopKqTuC.',
    'Admin'
);
