# Setup Guide - PostgreSQL JSONB Demo

This guide will help you set up and run the PostgreSQL JSONB demo application.

## Option 1: Using Docker (Recommended)

### Prerequisites
- Docker and Docker Compose installed
- .NET 8.0 SDK installed

### Steps

1. **Start PostgreSQL with Docker**
   ```bash
   docker-compose up -d
   ```
   This will start:
   - PostgreSQL server on port 5432
   - pgAdmin on port 8080 (optional web interface)

2. **Verify Database Connection**
   - PostgreSQL: `localhost:5432`
   - Username: `postgres`
   - Password: `jsonb_demo_2024`
   - Database: `postgresql_jsonb_demo`

3. **Run the Application**
   ```bash
   # Option 1: Using solution file
   dotnet run --project PostgreSqlJsonbDemo

   # Option 2: From project directory
   cd PostgreSqlJsonbDemo
   dotnet run
   ```

4. **Access the Application**
   - API: https://localhost:7000
   - Swagger UI: https://localhost:7000/swagger (if available)
   - pgAdmin: http://localhost:8080 (admin@jsonbdemo.com / jsonb_demo_2024)

## Option 2: Using Local PostgreSQL

### Prerequisites
- PostgreSQL 13+ installed locally
- .NET 8.0 SDK installed

### Steps

1. **Create Database**
   ```sql
   CREATE DATABASE postgresql_jsonb_demo;
   CREATE DATABASE postgresql_jsonb_demo_dev;
   ```

2. **Update Connection String**
   Edit `appsettings.json` and `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=postgresql_jsonb_demo;Username=postgres;Password=jsonb_demo_2024"
     }
   }
   ```

3. **Run the Application**
   ```bash
   cd PostgreSqlJsonbDemo
   dotnet run
   ```

## Quick Test

1. **Seed Sample Data**
   ```bash
   curl -X POST https://localhost:7000/api/demo/seed-data
   ```

2. **Test JSONB Query**
   ```bash
   curl "https://localhost:7000/api/products/search/specifications?key=cpu&value=Apple%20M2%20Max"
   ```

3. **View API Examples**
   ```bash
   curl https://localhost:7000/api/demo/examples
   ```

## Troubleshooting

### Database Connection Issues
- Check PostgreSQL is running
- Verify connection string credentials
- Ensure database exists
- Check firewall settings

### Entity Framework Issues
- Run `dotnet ef database update` if needed
- Check EF Core logs in development

### Docker Issues
- Run `docker-compose down` and `docker-compose up -d` to restart
- Check logs: `docker-compose logs postgres`

## Development Tips

### Database Management
- Use pgAdmin at http://localhost:8080 for GUI management
- Use psql command line: `docker exec -it postgresql-jsonb-demo psql -U postgres -d postgresql_jsonb_demo`

### Hot Reload
- Use `dotnet watch run` for development with auto-reload

### API Testing
- Use curl, Postman, or the included .http file
- Check API examples at `/api/demo/examples`

## Production Considerations

1. **Security**
   - Change default passwords
   - Use environment variables for secrets
   - Enable SSL/TLS

2. **Performance**
   - Monitor JSONB query performance
   - Add appropriate indexes
   - Consider connection pooling

3. **Backup**
   - Regular database backups
   - Test restore procedures