SELECT 'CREATE DATABASE bf_identity_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'bf_identity_db')\gexec

SELECT 'CREATE DATABASE bf_tenant_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'bf_tenant_db')\gexec

SELECT 'CREATE DATABASE bf_faq_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'bf_faq_db')\gexec
