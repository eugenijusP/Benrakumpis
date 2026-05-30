CREATE TABLE pictures (
    id          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    blob_url    NVARCHAR(500)    NOT NULL,
    [order]     INT              NOT NULL DEFAULT 0,
    uploaded_at DATETIME2        NOT NULL DEFAULT GETUTCDATE()
);
