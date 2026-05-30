DECLARE @constraint NVARCHAR(256) =
    (SELECT name FROM sys.default_constraints
     WHERE parent_object_id = OBJECT_ID('houses')
       AND COL_NAME(parent_object_id, parent_column_id) = 'reserved_color');
IF @constraint IS NOT NULL
    EXEC('ALTER TABLE houses DROP CONSTRAINT [' + @constraint + ']');
ALTER TABLE houses DROP COLUMN reserved_color;
