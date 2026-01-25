IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'bf_identity_db')
    CREATE DATABASE bf_identity_db;
GO

PRINT 'All required databases are checked and created if necessary.';
