# PostgreSQL JSONB Demo with .NET Entity Framework Core

A comprehensive demonstration of PostgreSQL's JSONB capabilities using .NET and Entity Framework Core. This project showcases how to effectively store, query, and manipulate JSON data in PostgreSQL using modern .NET development practices.

## Features

### 🚀 Core JSONB Capabilities
- **JSONB Storage**: Store complex JSON documents efficiently
- **JSONB Querying**: Advanced querying with containment operators
- **JSONB Indexing**: GIN indexes for fast JSON queries
- **JSONB Path Operations**: Extract and manipulate JSON paths
- **JSONB Arrays**: Work with JSON arrays and nested objects

### 📊 Demo Entities
- **Products**: Store specifications, metadata, and tags as JSONB
- **Users**: Store profiles, preferences, and addresses as JSONB
- **Orders**: Store items, shipping info, and order history as JSONB
- **Logs**: Store structured log data with context as JSONB

## Prerequisites

- .NET 10RC or later
- PostgreSQL 13+ (with JSONB support)
- Visual Studio 2026 Insider or VS Code (if you open vs2022, kindly recreate the sln file)

## Project Structure

```
postgresql-jsonb-demo/
├── PostgreSqlJsonbDemo.sln          # Solution file
├── PostgreSqlJsonbDemo/             # Main API project
│   ├── Controllers/                 # API controllers
│   ├── Data/                       # Entity Framework context
│   ├── Models/                     # Entity models
│   └── Program.cs                  # Application entry point
├── docker-compose.yml              # PostgreSQL + pgAdmin setup
├── init.sql                        # Database initialization
├── README.md                       # This file
└── SETUP.md                        # Detailed setup guide
```

## Quick Start

### 1. Clone and Setup
```bash
# Open solution in Visual Studio
# Or restore packages via CLI:
dotnet restore PostgreSqlJsonbDemo.sln
```

### 2. Configure Database
The connection string is already configured in `appsettings.json` to match Docker Compose:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=postgresql_jsonb_demo;Username=postgres;Password=jsonb_demo_2024"
  }
}
```

### 3. Run the Application
```bash
dotnet run
```

### 4. Seed Demo Data
```bash
curl -X POST https://localhost:7000/api/demo/seed-data
```

## API Endpoints

### Products API

#### Basic CRUD
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

#### JSONB Queries
- `GET /api/products/search/specifications?key=cpu&value=Apple M2 Max` - Search by specification
- `GET /api/products/search/tags?tag=laptop` - Search by tag
- `GET /api/products/search/price-range?minPrice=1000&maxPrice=3000` - Search by price in specs

#### JSONB Operations
- `PATCH /api/products/{id}/specifications` - Update specifications
- `POST /api/products/{id}/tags` - Add tag
- `DELETE /api/products/{id}/tags/{tag}` - Remove tag

### Users API

#### Basic CRUD
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

#### JSONB Queries
- `GET /api/users/search/profile?key=occupation&value=Software Engineer` - Search by profile
- `GET /api/users/search/city?city=San Francisco` - Search by city
- `GET /api/users/search/age-range?minAge=25&maxAge=35` - Search by age range

#### JSONB Operations
- `PATCH /api/users/{id}/profile` - Update profile
- `PATCH /api/users/{id}/preferences` - Update preferences
- `PATCH /api/users/{id}/address` - Update address

### Orders API

#### Basic CRUD
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}` - Update order
- `DELETE /api/orders/{id}` - Delete order

#### JSONB Queries
- `GET /api/orders/search/status?status=shipped` - Get orders by status
- `GET /api/orders/search/items?productName=MacBook Pro` - Search by product in items
- `GET /api/orders/search/total-range?minTotal=1000&maxTotal=5000` - Search by total range
- `GET /api/orders/search/shipping-city?city=San Francisco` - Search by shipping city

#### JSONB Operations
- `PATCH /api/orders/{id}/status` - Update status (adds to history)
- `PATCH /api/orders/{id}/items` - Update order items
- `GET /api/orders/{id}/history` - Get order history

### Logs API

#### Basic Operations
- `GET /api/logs` - Get logs (paginated)
- `GET /api/logs/{id}` - Get log by ID
- `POST /api/logs` - Create new log entry

#### JSONB Queries
- `GET /api/logs/search/level/error` - Get logs by level
- `GET /api/logs/search/message?query=payment` - Search in messages
- `GET /api/logs/search/data?key=errorCode&value=CARD_DECLINED` - Search by data field
- `GET /api/logs/search/context?key=source&value=payment-service` - Search by context
- `GET /api/logs/search/daterange?from=2024-01-01&to=2024-01-31` - Search by date range

#### Analytics
- `GET /api/logs/analytics/errors` - Get error analytics
- `DELETE /api/logs/cleanup?daysOld=30` - Cleanup old logs

### Demo API
- `GET /api/demo/examples` - Get API examples and documentation
- `POST /api/demo/seed-data` - Seed database with sample data
- `DELETE /api/demo/clear-data` - Clear all data

## JSONB Query Examples

### 1. Containment Queries
```sql
-- Find products with specific CPU
SELECT * FROM "Products"
WHERE "Specifications" @> '{"cpu": "Apple M2 Max"}';

-- Find users with dark theme preference
SELECT * FROM "Users"
WHERE "Preferences" @> '{"theme": "dark"}';
```

### 2. Path Extraction
```sql
-- Get user ages
SELECT "Name", "Profile"->>'age' as age
FROM "Users";

-- Get product colors
SELECT "Name", "Specifications"->>'color' as color
FROM "Products";
```

### 3. Array Operations
```sql
-- Find products with specific tag
SELECT * FROM "Products"
WHERE "Tags" @> '["laptop"]';

-- Get all tags from products
SELECT DISTINCT jsonb_array_elements_text("Tags") as tag
FROM "Products";
```

### 4. Nested Object Queries
```sql
-- Find users with email notifications enabled
SELECT * FROM "Users"
WHERE "Preferences"->'notifications'->>'email' = 'true';

-- Find orders with credit card payments
SELECT * FROM "Orders"
WHERE "PaymentInfo"->>'method' = 'credit_card';
```

## Database Schema

### JSONB Columns and Indexes

#### Products Table
```sql
CREATE TABLE "Products" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Price" DECIMAL(18,2),
    "Specifications" JSONB,
    "Metadata" JSONB,
    "Tags" JSONB,
    "CreatedAt" TIMESTAMP,
    "UpdatedAt" TIMESTAMP
);

-- GIN indexes for fast JSONB queries
CREATE INDEX idx_products_specifications ON "Products" USING GIN ("Specifications");
CREATE INDEX idx_products_tags ON "Products" USING GIN ("Tags");
```

#### Users Table
```sql
CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Email" VARCHAR(255) UNIQUE NOT NULL,
    "Name" VARCHAR(200) NOT NULL,
    "Profile" JSONB,
    "Preferences" JSONB,
    "Address" JSONB,
    "CreatedAt" TIMESTAMP,
    "UpdatedAt" TIMESTAMP
);

CREATE INDEX idx_users_profile ON "Users" USING GIN ("Profile");
```

## Entity Framework Configuration

### JSONB Property Configuration
```csharp
modelBuilder.Entity<Product>(entity =>
{
    entity.Property(e => e.Specifications)
        .HasColumnType("jsonb");

    entity.HasIndex(e => e.Specifications)
        .HasMethod("gin");
});
```

### JSONB Queries in LINQ
```csharp
// Containment query
var products = await context.Products
    .Where(p => EF.Functions.JsonContains(p.Specifications,
        JsonDocument.Parse("{\"cpu\":\"Apple M2 Max\"}")))
    .ToListAsync();

// Path extraction
var users = await context.Users
    .Where(u => EF.Functions.JsonExtractPathText(u.Address, "city") == "San Francisco")
    .ToListAsync();

// Array contains
var products = await context.Products
    .Where(p => p.Tags.Contains("laptop"))
    .ToListAsync();
```

## Performance Considerations

### 1. Use GIN Indexes
```sql
CREATE INDEX idx_jsonb_field ON table_name USING GIN (jsonb_field);
```

### 2. Optimize Queries
- Use containment operators (`@>`, `<@`) for exact matches
- Use path operators (`->`, `->>`) for key extraction
- Combine JSONB queries with traditional SQL WHERE clauses

### 3. JSONB vs JSON
- Use JSONB for better performance and indexing
- JSONB removes duplicate keys and maintains order
- JSONB supports more operators and functions

## Best Practices

### 1. Model Design
- Keep JSONB documents reasonably sized
- Use consistent schema within JSONB documents
- Consider normalization vs. denormalization trade-offs

### 2. Querying
- Create appropriate indexes for your query patterns
- Use EXPLAIN ANALYZE to optimize query performance
- Consider partial indexes for filtered JSONB queries

### 3. Application Code
- Validate JSONB structure in application code
- Use strongly-typed models when possible
- Handle JSON serialization/deserialization carefully

## Sample Data Structure

### Product Specifications
```json
{
  "cpu": "Apple M2 Max",
  "ram": "32GB",
  "storage": "1TB SSD",
  "display": "16.2-inch Liquid Retina XDR",
  "ports": ["3x Thunderbolt 4", "HDMI", "SD card", "MagSafe 3"]
}
```

### User Profile
```json
{
  "age": 30,
  "gender": "male",
  "occupation": "Software Engineer",
  "interests": ["technology", "gaming", "reading"]
}
```

### Order Items
```json
[
  {
    "productId": 1,
    "name": "MacBook Pro 16\"",
    "price": 2499.99,
    "quantity": 1
  }
]
```

### Log Context
```json
{
  "sessionId": "sess_abc123",
  "requestId": "req_xyz789",
  "source": "authentication-service"
}
```

## Testing

### Run the Application
```bash
dotnet run
```

### Test with Sample Data
1. Seed data: `POST /api/demo/seed-data`
2. Try JSONB queries: `GET /api/products/search/specifications?key=cpu&value=Apple M2 Max`
3. Update JSONB fields: `PATCH /api/users/1/profile`
4. View examples: `GET /api/demo/examples`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Submit a pull request

## License

This project is provided for educational and demonstration purposes.
