-- Initialize databases for each microservice

-- Create separate databases for each service
CREATE DATABASE orderservice;
CREATE DATABASE inventoryservice;
CREATE DATABASE notificationservice;

-- Grant privileges to postgres user (for demo purposes)
GRANT ALL PRIVILEGES ON DATABASE orderservice TO postgres;
GRANT ALL PRIVILEGES ON DATABASE inventoryservice TO postgres;
GRANT ALL PRIVILEGES ON DATABASE notificationservice TO postgres;
