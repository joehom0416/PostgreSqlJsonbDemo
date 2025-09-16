-- Initialize PostgreSQL database for JSONB demo
-- This script is executed when the container starts for the first time

-- Create extension for better JSONB performance
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create development database
CREATE DATABASE postgresql_jsonb_demo_dev;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE postgresql_jsonb_demo TO postgres;
GRANT ALL PRIVILEGES ON DATABASE postgresql_jsonb_demo_dev TO postgres;