# Data Access Patterns

## Overview

This document outlines best practices for data access in Beep applications, focusing on database operations using IDMEEditor, IDataSource, and Unit of Work patterns. It covers patterns for both Windows Forms and ASP.NET Core applications, with and without dependency injection.

## Core Architecture

### Interface Hierarchy

**IDataSource** - Core interface for all data sources:
- Provides CRUD operations (GetEntity, InsertEntity, UpdateEntity, DeleteEntity)
- Supports transactions (BeginTransaction, Commit, EndTransaction)
- Manages entity structures (GetEntityStructure, CreateEntityAs)
- Handles queries (RunQuery, GetEntityAsync, GetScalar)
- Connection management (Openconnection, Closeconnection)

**IDMEEditor** - Data Management Engine Editor:
- Manages multiple data sources (GetDataSource, CreateNewDataSourceConnection)
- Configuration management (ConfigEditor)
- ETL operations (ETL)
- Logging and error handling (Logger, ErrorObject)
- Utility functions (Utilfunction, typesHelper)

### Data Source Categories

**DatasourceCategory** enum defines data source types:
- `RDBMS` - Relational databases (SQL Server, MySQL, PostgreSQL, etc.)
- `INMEMORY` - In-memory databases (DuckDB, SQLite in-memory)
- `FILE` - File-based data sources (CSV, JSON, XML)
- `NOSQL` - NoSQL databases (MongoDB, CouchDB, etc.)
- `CLOUD` - Cloud services (AWS, Azure, GCP)
- `WEBAPI` - Web APIs (REST, GraphQL, OData)
- `VECTORDB` - Vector databases (Pinecone, ChromaDB, etc.)
- `CONNECTOR` - External service connectors (Salesforce, Shopify, etc.)

### Data Source Types

**DataSourceType** enum includes 200+ data source types:
- **RDBMS**: SqlServer, MySQL, PostgreSQL, Oracle, SQLite, DuckDB, etc.
- **NoSQL**: MongoDB, CouchDB, Redis, DynamoDB, etc.
- **File**: CSV, JSON, XML, Parquet, Avro, etc.
- **Cloud**: AWSRedshift, GoogleBigQuery, AzureCloud, etc.
- **VectorDB**: PineCone, ChromaDB, Qdrant, Milvus, etc.
- **Connectors**: Salesforce, Shopify, Stripe, GitHub, etc.

### Driver Configuration

**ConnectionDriversConfig** - Configuration for data source drivers:
- `DriverClass` - Fully qualified class name
- `DbConnectionType` - Type of database connection class
- `AdapterType` - Data adapter type
- `CommandBuilderType` - Command builder type
- `ConnectionString` - Template connection string with placeholders
- `PackageName` - NuGet package name
- `version` - Driver version
- `dllname` - DLL file name
- `DatasourceCategory` - Category of data source
- `DatasourceType` - Specific data source type
- `InMemory` - Whether driver supports in-memory mode
- `CreateLocal` - Whether driver can create local databases

**ConnectionDriversTypes** - Runtime type information:
- `AdapterType` - Type object for adapter
- `CommandBuilderType` - Type object for command builder
- `DbConnectionType` - Type object for connection

### Data Source Implementation Hierarchy

```
IDataSource (interface)
  ├── InMemoryDataSource (base for pure in-memory)
  ├── RDBSource (base for RDBMS)
  │   └── InMemoryRDBSource (RDBMS with in-memory support)
  │       ├── SQLiteDataSource (file-based or in-memory SQLite)
  │       └── DuckDBDataSource (in-memory DuckDB)
  ├── CSVDataSource (file-based CSV)
  ├── JsonDataSource (file-based JSON)
  └── ProxyDataSource (proxy with circuit breaker)
```

## Core Principles

### 1. Use GetEntityAsync with AppFilter (PREFERRED)

**For simple single-table queries** - Clean, maintainable, handles delimiters automatically.

```csharp
// BEST: Use GetEntityAsync with AppFilter
var dataSource = _editor.GetDataSource(connectionName);
var filters = new List<AppFilter> 
{ 
    new AppFilter { FieldName = "ID", Operator = "=", FilterValue = id } 
};
var results = await dataSource.GetEntityAsync("Customers", filters); // Returns IEnumerable<object>
```

### 2. Use GetScalar/GetScalarAsync for Scalar Queries

**For COUNT, SUM, EXISTS checks** - Returns double, use for single-value queries.

```csharp
// Good: Use GetScalar for scalar queries
var paramDelim = dataSource.ParameterDelimiter;
var sql = $"SELECT COUNT(*) FROM Customers WHERE CategoryID = {paramDelim}categoryId";
var count = dataSource.GetScalar(sql); // Returns double
```

### 3. Use RunQuery for Complex Multi-Table Queries

**Only when GetEntityAsync is insufficient** - Returns IEnumerable<object>.

```csharp
// Use RunQuery only for complex multi-table queries or custom SQL
var sql2 = $"SELECT c.*, cat.Name as CategoryName FROM Customers c JOIN Categories cat ON c.CategoryID = cat.ID";
var results2 = dataSource.RunQuery(sql2); // Returns IEnumerable<object>
```

### 4. Use ExecuteSql ONLY for DDL

**CREATE, ALTER, DROP statements** - DDL doesn't return data.

```csharp
// Good: ExecuteSql ONLY for DDL
var createSql = "CREATE TABLE ...";
dataSource.ExecuteSql(createSql); // CORRECT - DDL doesn't return data
```

## Anti-Patterns (What NOT to Do)

```csharp
// Bad: Using ExecuteSql for SELECT queries
var sql = "SELECT * FROM Customers WHERE ID = @id";
dataSource.ExecuteSql(sql); // WRONG - ExecuteSql is for DDL only

// Bad: Using GetDataTable
var dataTable = dataSource.GetDataTable(sql); // WRONG - should use RunQuery

// Bad: Hardcoding parameter delimiters
var sql = "SELECT * FROM Customers WHERE ID = @id"; // WRONG - use ParameterDelimiter
```

## Connection and Data Source Creation

**Required using statements:**
```csharp
using System;
using System.IO;
using System.Reflection; // If using Assembly.GetExecutingAssembly()
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
```

### Data Source Types and Inheritance Hierarchy

Beep provides several data source implementations with a clear inheritance hierarchy:

```
IDataSource (interface)
  └── RDBSource (base class for RDBMS)
      └── InMemoryRDBSource (base for in-memory databases)
          ├── SQLiteDataSource (file-based or in-memory SQLite)
          └── DuckDBDataSource (in-memory DuckDB)
```

**Key Classes:**
- **RDBSource**: Base class for all RDBMS data sources (SQL Server, MySQL, PostgreSQL, etc.)
- **InMemoryRDBSource**: Extends RDBSource with in-memory database capabilities
- **SQLiteDataSource**: Implements SQLite (supports both file-based and in-memory)
- **DuckDBDataSource**: Implements DuckDB in-memory analytics database
- **RDBDataConnection**: Manages database connections and connection strings

### In-Memory vs File-Based Data Sources

**In-Memory Data Sources** (InMemoryRDBSource):
- Data exists only in memory during application runtime
- Faster performance for temporary data
- Data is lost when connection closes (unless explicitly saved)
- Use for: Caching, temporary calculations, testing, staging data

**File-Based Data Sources** (RDBSource):
- Data persists to disk
- Slower than in-memory but persistent
- Use for: Production data, long-term storage, data that must survive restarts

### Creating In-Memory Data Sources

**SQLite In-Memory:**
```csharp
var connectionProps = new ConnectionProperties
{
    ConnectionName = "InMemoryDB",
    DatabaseType = DataSourceType.SqlLite,
    Category = DatasourceCategory.RDBMS,
    DriverName = "SqliteDatasourceCore",
    DriverVersion = "1.0.0",
    IsInMemory = true, // Key property for in-memory
    Database = "InMemoryDB",
    ConnectionString = "Data Source=:memory:;Version=3;"
};

editor.ConfigEditor.AddDataConnection(connectionProps);
var dataSource = editor.GetDataSource("InMemoryDB");
dataSource.Openconnection(); // Opens in-memory database
```

**DuckDB In-Memory:**
```csharp
var connectionProps = new ConnectionProperties
{
    ConnectionName = "DuckDBMemory",
    DatabaseType = DataSourceType.DuckDB,
    Category = DatasourceCategory.INMEMORY,
    DriverName = "DuckDBDataSourceCore",
    DriverVersion = "1.0.0",
    IsInMemory = true,
    Database = "DuckDBMemory",
    ConnectionString = "DataSource=:memory:?cache=shared"
};

editor.ConfigEditor.AddDataConnection(connectionProps);
var dataSource = editor.GetDataSource("DuckDBMemory");
dataSource.Openconnection(); // Opens in-memory DuckDB
```

### In-Memory Data Source Lifecycle

**1. Open In-Memory Database:**
```csharp
// For SQLiteDataSource or DuckDBDataSource
var dataSource = editor.GetDataSource("InMemoryDB");
var result = dataSource.OpenDatabaseInMemory("MyDatabase");
if (result.Flag == Errors.Ok)
{
    // Database is ready
}
```

**2. Load Structure:**
```csharp
// Load entity structures from configuration or source database
CancellationTokenSource token = new CancellationTokenSource();
IProgress<PassedArgs> progress = new Progress<PassedArgs>();
dataSource.LoadStructure(progress, token.Token, false);
```

**3. Create Structure:**
```csharp
// Create tables/entities in the in-memory database
dataSource.CreateStructure(progress, token.Token);
```

**4. Load Data (Optional):**
```csharp
// Copy data from source database to in-memory database
dataSource.LoadData(progress, token.Token);
```

**5. Save Structure (Before Closing):**
```csharp
// Save entity structures to configuration
dataSource.SaveStructure();
```

**6. Close Connection:**
```csharp
dataSource.Closeconnection();
// Note: In-memory data is lost unless saved to file-based source first
```

### Data Source Connection Properties

**Common Properties:**
```csharp
var connectionProps = new ConnectionProperties
{
    // Basic Information
    ConnectionName = "MyDatabase",
    DatabaseType = DataSourceType.SqlLite, // or DuckDB, SqlServer, etc.
    Category = DatasourceCategory.RDBMS, // or INMEMORY, File, Cloud, etc.
    
    // Driver Information
    DriverName = "SqliteDatasourceCore",
    DriverVersion = "1.0.0",
    
    // Connection Details
    ConnectionString = "Data Source=mydatabase.db;Version=3;",
    IsInMemory = false, // true for in-memory databases
    
    // For RDBMS
    Host = "localhost",
    Port = 5432,
    Database = "mydb",
    UserID = "user",
    Password = "password",
    SchemaName = "public",
    
    // For File-based
    FilePath = @"C:\Data",
    FileName = "database.db"
};
```

### Connection Helper Methods

**Get configurations by category:**
```csharp
// Get all RDBMS configurations
var rdbmsConfigs = ConnectionHelper.GetRDBMSConfigs();

// Get all in-memory configurations
var inMemoryConfigs = ConnectionHelper.GetNoSQLConfigs(); // Some in-memory DBs are categorized as NoSQL

// Get file-based configurations
var fileConfigs = ConnectionHelper.GetFileConfigs();
```

**Link connection to drivers:**
```csharp
// Automatically link connection properties to available drivers
var driverConfig = ConnectionHelper.LinkConnection2Drivers(
    connectionProps, 
    editor.ConfigEditor
);
```

**Process connection string:**
```csharp
// Replace placeholders in connection string
var processedConnectionString = ConnectionHelper.ReplaceValueFromConnectionString(
    driverConfig, 
    connectionProps, 
    editor
);
```

**Validate connection string:**
```csharp
bool isValid = ConnectionHelper.IsConnectionStringValid(
    processedConnectionString, 
    DataSourceType.SqlLite
);
```

### Data Source Delimiters

Different data sources use different delimiters:

**SQLiteDataSource:**
- ColumnDelimiter: `"[]"`
- ParameterDelimiter: `"$"`

**DuckDBDataSource:**
- ColumnDelimiter: `"[]"`
- ParameterDelimiter: `"$"`

**SQL Server:**
- ColumnDelimiter: `"[]"` or `""`
- ParameterDelimiter: `"@"`

**PostgreSQL:**
- ColumnDelimiter: `""`
- ParameterDelimiter: `"@"`

**Always use the data source's delimiters:**
```csharp
var dataSource = editor.GetDataSource("MyDatabase");
var colDelim = dataSource.ColumnDelimiter; // Use this for column names
var paramDelim = dataSource.ParameterDelimiter; // Use this for parameters
```

### Pattern 1: With BeepDesktopServices (Recommended for WinForms Apps)

**Full initialization with BeepDesktopServices** - Provides complete service infrastructure.

```csharp
// In Program.cs - Full initialization with BeepDesktopServices
var builder = Host.CreateApplicationBuilder();
BeepDesktopServices.RegisterServices(builder);
var host = builder.Build();
BeepDesktopServices.ConfigureServices(host);
BeepDesktopServices.ConfigureControlsandMenus();

// Start loading assemblies and services
var result = BeepDesktopServices.StartLoading(
    new string[] { "BeepEnterprize", "TheTechIdea", "Beep" }, 
    showWaitForm: true
);

// Access IDMEEditor through AppManager
var editor = BeepDesktopServices.AppManager.DMEEditor;

// Or access through BeepService
var beepService = BeepDesktopServices.BeepService;
var editor = beepService.DMEEditor;

// Create and ensure data source exists
// Option 1: Use AppDomain (most compatible)
var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mydatabase.db");
// Option 2: Use AppContext (requires .NET Core/.NET 5+)
// var dbPath = Path.Combine(AppContext.BaseDirectory, "mydatabase.db");
// Option 3: Use Assembly location
// var dbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mydatabase.db");
var connectionProps = new ConnectionProperties
{
    ConnectionName = "MyDatabase",
    DatabaseType = DataSourceType.SqlLite,
    Category = DatasourceCategory.RDBMS,
    DriverName = "SqliteDatasourceCore",
    DriverVersion = "1.0.0",
    ConnectionString = $"Data Source={dbPath};Version=3;"
};

// Ensure data source exists (helper method pattern)
var dataSource = EnsureDataSource(editor, "MyDatabase", connectionProps);

// Helper method to ensure data source exists
private static IDataSource EnsureDataSource(IDMEEditor editor, string dataSourceName, ConnectionProperties props)
{
    var existing = editor.GetDataSource(dataSourceName);
    if (existing != null)
    {
        return existing;
    }
    editor.ConfigEditor.AddDataConnection(props);
    return editor.GetDataSource(dataSourceName);
}
```

### Pattern 2: Without BeepServices (Standalone/Direct initialization)

**Direct initialization without BeepDesktopServices** - For standalone scenarios or when you don't need the full service infrastructure.

```csharp
// Direct initialization without BeepDesktopServices
var editor = new DMEEditor();

// Option 1: Use ConnectionHelper to create configuration
var sqlServerConfig = ConnectionHelper.CreateSqlServerConfig();
var mongoConfig = ConnectionHelper.CreateMongoDBConfig();
var sqliteConfig = ConnectionHelper.CreateSQLiteConfig();

// Option 2: Create ConnectionProperties directly
// Get base directory (choose one method)
var baseDir = AppDomain.CurrentDomain.BaseDirectory; // Most compatible
// var baseDir = AppContext.BaseDirectory; // .NET Core/.NET 5+ only
// var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); // Alternative

var dbPath = Path.Combine(baseDir, "mydatabase.db");
var connectionProps = new ConnectionProperties
{
    ConnectionName = "MyDatabase",
    DatabaseType = DataSourceType.SqlLite,
    Category = DatasourceCategory.RDBMS,
    DriverName = "SqliteDatasourceCore",
    DriverVersion = "1.0.0",
    ConnectionString = $"Data Source={dbPath};Version=3;"
    // For SQL Server: ServerName, DatabaseName, UserID, Password, etc.
};

// Process connection string with placeholders
var driverConfig = ConnectionHelper.GetBestMatchingDriver(connectionProps, editor.ConfigEditor);
var processedConnectionString = ConnectionHelper.ReplaceValueFromConnectionString(
    driverConfig, 
    connectionProps, 
    editor
);

// Validate connection string
bool isValid = ConnectionHelper.IsConnectionStringValid(
    processedConnectionString, 
    DataSourceType.SqlLite
);

// Link connection to drivers
var linkedDriver = ConnectionHelper.LinkConnection2Drivers(connectionProps, editor.ConfigEditor);

// Add to editor
editor.ConfigEditor.AddDataConnection(connectionProps);

// Get data source for use with UnitOfWork
var dataSource = editor.GetDataSource("MyDatabase");
```

### ConnectionHelper Category Methods

```csharp
// Get configurations by category
var rdbmsConfigs = ConnectionHelper.GetRDBMSConfigs();
var nosqlConfigs = ConnectionHelper.GetNoSQLConfigs();
var vectorDbConfigs = ConnectionHelper.GetVectorDBConfigs();
var fileConfigs = ConnectionHelper.GetFileConfigs();
var cloudConfigs = ConnectionHelper.GetCloudConfigs();
var webApiConfigs = ConnectionHelper.GetWebAPIConfigs();

// Get all configurations
var allConfigs = ConnectionHelper.GetAllConnectionConfigs();
```

## Unit of Work Pattern

### When to Use UnitOfWork vs Direct IDataSource

- **Use UnitOfWork**: Complex operations, transactions, change tracking, multi-entity operations, batch operations
- **Use Direct IDataSource**: Simple queries, read-only operations, one-off queries, when you don't need transaction management

### Basic UnitOfWork Usage

```csharp
// Basic UnitOfWork usage
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

// Load data
var entities = await unitOfWork.Get();

// CRUD operations
unitOfWork.New(); // Create new
unitOfWork.Update(entity); // Update existing
unitOfWork.Delete(entity); // Delete

// Commit transaction
var result = await unitOfWork.Commit();
if (result.Flag != Errors.Ok) {
    await unitOfWork.Rollback();
}
```

### UnitOfWork with Filters

```csharp
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

// Get with filters
var filters = new List<AppFilter>
{
    new AppFilter { FieldName = "CategoryID", Operator = "=", FilterValue = categoryId },
    new AppFilter { FieldName = "Active", Operator = "=", FilterValue = true }
};

var customers = await unitOfWork.Get(filters);
```

### UnitOfWork Change Tracking

```csharp
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

// Check if there are changes
bool isDirty = unitOfWork.IsDirty;

// Get modified entities
var modifiedEntities = unitOfWork.GetModifiedEntities();

// Get added entities
var addedEntities = unitOfWork.GetAddedEntities();

// Get deleted entities
var deletedEntities = unitOfWork.GetDeletedEntities();
```

## MultiDataSourceUnitOfWork

**For distributed transactions** - Coordinating transactions across multiple data sources.

```csharp
using var multiUow = new MultiDataSourceUnitOfWork(editor);

// Add multiple units of work
await multiUow.AddUnitOfWorkAsync<Customer>(ds1, "Customers", "ID");
await multiUow.AddUnitOfWorkAsync<Order>(ds2, "Orders", "ID");

// Define relationships
multiUow.AddRelationship("Customers", "Orders", "CustomerID", "CustomerFK");

// Commit all or rollback all
var result = await multiUow.CommitAllAsync();
if (result.Flag != Errors.Ok) {
    await multiUow.RollbackAllAsync();
}
```

## UnitOfWorkFactory

**Factory pattern for creating UnitOfWork instances** - Dynamic entity type support.

```csharp
var unitOfWork = UnitOfWorkFactory.CreateUnitOfWork(
    typeof(Customer),
    editor,
    "MyDatabase",
    "Customers",
    "ID"
);

// Use with UnitOfWorkWrapper for enhanced error handling
var wrapper = new UnitOfWorkWrapper(unitOfWork);
```

## Transaction Management

### Commit/Rollback Best Practices

```csharp
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

try
{
    // Perform operations
    unitOfWork.New();
    unitOfWork.Update(existingEntity);
    
    // Commit with progress and cancellation
    var result = await unitOfWork.Commit(progress, cancellationToken);
    
    // Always check result
    if (result.Flag != Errors.Ok) {
        // Handle errors
        await unitOfWork.Rollback();
        throw new Exception(result.Message);
    }
}
catch (Exception ex)
{
    // Always rollback on exception
    await unitOfWork.Rollback();
    throw;
}
```

### Transaction Scope Guidelines

- **Keep transactions short** - Don't hold transactions open for long periods
- **One transaction per business operation** - Don't mix multiple unrelated operations
- **Always rollback on errors** - Ensure data consistency
- **Use cancellation tokens** - Allow long-running transactions to be cancelled

## DefaultsManager Integration

**Automatic default value application** - Handles CreatedDate/CreatedBy, ModifiedDate/ModifiedBy patterns.

```csharp
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

// DefaultsManager automatically applies:
// - CreatedDate = DateTime.UtcNow (on new entities)
// - CreatedBy = currentUserId (on new entities)
// - ModifiedDate = DateTime.UtcNow (on updates)
// - ModifiedBy = currentUserId (on updates)
// - Active = true (if configured)
// - Status = "Active" (if configured)

unitOfWork.New(); // Defaults applied automatically
var result = await unitOfWork.Commit();
```

## Relationship Management Patterns

**Parent-child entity relationships** - Managing related entities across data sources.

```csharp
using var multiUow = new MultiDataSourceUnitOfWork(editor);

// Add parent and child units of work
await multiUow.AddUnitOfWorkAsync<Customer>(ds1, "Customers", "ID");
await multiUow.AddUnitOfWorkAsync<Order>(ds2, "Orders", "ID");

// Define relationship
multiUow.AddRelationship(
    "Customers",      // Parent entity name
    "Orders",         // Child entity name
    "CustomerID",    // Foreign key in child
    "CustomerFK"      // Relationship name
);

// Operations maintain referential integrity
var customer = await multiUow.GetEntityAsync<Customer>("Customers", customerId);
var orders = await multiUow.GetRelatedEntitiesAsync<Order>("Orders", "CustomerID", customerId);

// Commit maintains relationships
var result = await multiUow.CommitAllAsync();
```

## Entity vs DTO Pattern

### Key Principle

- **Entity objects** are passed directly to `IDataSource.InsertEntity(tableName, entity)` and `IDataSource.UpdateEntity(tableName, entity)`
- **Entity objects** are returned directly from `IDataSource.GetEntityAsync(tableName, filters)`
- **NO Dictionary<string, object> conversions** - remove all `Convert*ToDictionary` and `ConvertDictionaryTo*` methods
- **Use DTOs for API input/output**, convert DTOs ↔ Entity for database operations

### Example: DTO to Entity Conversion

```csharp
// DTO for API input
public class CreateCustomerRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string CategoryID { get; set; }
}

// Convert DTO to Entity for database operation
var entity = new Customer
{
    ID = Guid.NewGuid().ToString(),
    Name = request.Name,
    Email = request.Email,
    CategoryID = request.CategoryID,
    Active = true,
    CreatedBy = userId,
    CreatedDate = DateTime.UtcNow
};

// Insert entity using UnitOfWork
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

unitOfWork.Add(entity);
var result = await unitOfWork.Commit();
```

### Example: Entity to DTO Conversion

```csharp
// Get entity from database using UnitOfWork
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

var entity = await unitOfWork.GetByIdAsync(id);

// Convert entity to DTO for API response
var response = new CustomerResponse
{
    Id = entity.ID,
    Name = entity.Name,
    Email = entity.Email,
    CategoryID = entity.CategoryID
};
```

## AppFilter Usage

### Basic Filtering

```csharp
var filters = new List<AppFilter>
{
    new AppFilter 
    { 
        FieldName = "CategoryID", 
        FilterValue = categoryId, 
        Operator = "=" 
    },
    new AppFilter 
    { 
        FieldName = "Active", 
        FilterValue = true, 
        Operator = "=" 
    }
};

var results = await dataSource.GetEntityAsync("Customers", filters);
```

### Complex Filtering

```csharp
var filters = new List<AppFilter>
{
    new AppFilter 
    { 
        FieldName = "CreatedDate", 
        FilterValue = startDate, 
        Operator = ">=" 
    },
    new AppFilter 
    { 
        FieldName = "CreatedDate", 
        FilterValue = endDate, 
        Operator = "<=" 
    },
    new AppFilter 
    { 
        FieldName = "TotalAmount", 
        FilterValue = 0, 
        Operator = ">" 
    }
};
```

## QueryBuilder for Custom Queries

**Use QueryBuilder for parameterized queries** - Provides database-agnostic query construction (only when needed).

```csharp
// Better: Use QueryBuilder for parameterized queries
var sql = QueryBuilder.BuildSelectQuery(
    dataSource, 
    "Customers", 
    null, 
    new Dictionary<string, object> { { "ID", id } }, 
    out var parameters);
var results = dataSource.RunQuery(sql);
```

## Accessing IDataSource

**Access IDataSource via IDMEEditor**: `_editor.GetDataSource(connectionName)`

```csharp
var dataSource = _editor.GetDataSource(connectionName);
var paramDelim = dataSource.ParameterDelimiter;
var colDelim = dataSource.ColumnDelimiter;
```

## Column Delimiters

**Use IDataSource.ColumnDelimiter for column names if needed** - Handled automatically by GetEntityAsync.

```csharp
// Usually not needed with GetEntityAsync
var colDelim = dataSource.ColumnDelimiter; // e.g., "[" for SQL Server, "" for PostgreSQL
```

## Field-Scoped Queries Pattern

**For field-scoped operations** - Always include CategoryID or FieldID filter.

```csharp
// In field-scoped service methods
public async Task<List<object>> GetEntitiesForCategoryAsync(string categoryId, List<AppFilter> additionalFilters = null)
{
    var filters = new List<AppFilter>
    {
        new AppFilter 
        { 
            FieldName = "CategoryID", 
            FilterValue = categoryId, 
            Operator = "=" 
        }
    };
    
    if (additionalFilters != null)
        filters.AddRange(additionalFilters);
        
    // Use UnitOfWork or direct IDataSource
    using var unitOfWork = new UnitofWork<Customer>(
        editor, 
        "MyDatabase", 
        "Customers", 
        "ID"
    );
    
    return await unitOfWork.Get(filters);
}
```

## Error Handling Best Practices

### UnitOfWorkWrapper Error Handling

```csharp
var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

var wrapper = new UnitOfWorkWrapper(unitOfWork);

try
{
    wrapper.Add(entity);
    var result = await wrapper.CommitAsync();
    
    if (!result.Success)
    {
        // Handle validation errors
        foreach (var error in result.Errors)
        {
            Console.WriteLine($"Error: {error.Message}");
        }
        
        await wrapper.RollbackAsync();
    }
}
catch (Exception ex)
{
    await wrapper.RollbackAsync();
    throw;
}
```

### Validation Patterns

```csharp
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

// Validate before commit
if (!ValidateCustomer(entity))
{
    throw new ValidationException("Customer validation failed");
}

var result = await unitOfWork.Commit();
if (result.Flag != Errors.Ok)
{
    await unitOfWork.Rollback();
    throw new Exception(result.Message);
}
```

### Rollback Strategies

- **Always rollback on exception** - Ensure data consistency
- **Check result.Flag** - Don't assume success
- **Log errors before rollback** - For debugging and auditing
- **Use try-finally** - Ensure cleanup happens

## Service Integration Patterns

### Windows Forms with BeepDesktopServices

```csharp
// In Program.cs
var builder = Host.CreateApplicationBuilder();
BeepDesktopServices.RegisterServices(builder);
var host = builder.Build();
BeepDesktopServices.ConfigureServices(host);

// Access editor
var editor = BeepDesktopServices.AppManager.DMEEditor;

// In forms with dependency injection
public partial class MainForm : Form
{
    private readonly IDMEEditor _editor;
    
    public MainForm(IDMEEditor editor)
    {
        _editor = editor;
        InitializeComponent();
    }
}
```

### Windows Forms without Services

```csharp
// Direct instantiation
var editor = new DMEEditor();
editor.ConfigEditor.LoadConfig(...);

// Or access via static properties if BeepDesktopServices was initialized
var editor = BeepDesktopServices.AppManager.DMEEditor;
```

### ASP.NET Core with Dependency Injection

```csharp
// In Program.cs or Startup.cs
// Get base directory (choose appropriate method for your framework)
var baseDirectory = AppDomain.CurrentDomain.BaseDirectory; // Most compatible
// var baseDirectory = AppContext.BaseDirectory; // .NET Core/.NET 5+ only

services.AddBeepServices(options =>
{
    options.DirectoryPath = baseDirectory;
    options.ContainerName = "MyApp";
    options.ConfigType = BeepConfigType.Application;
});

// In controllers/services
public class CustomerController : ControllerBase
{
    private readonly IDMEEditor _editor;
    
    public CustomerController(IDMEEditor editor)
    {
        _editor = editor;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var dataSource = _editor.GetDataSource("MyDatabase");
        var filters = new List<AppFilter>();
        var results = await dataSource.GetEntityAsync("Customers", filters);
        return Ok(results);
    }
}
```

## Decision Matrix

### When to Use Each Pattern

| Pattern | Use When | Example |
|---------|----------|---------|
| **GetEntityAsync + AppFilter** | Simple single-table queries, read operations | Get all active customers |
| **UnitOfWork** | Complex operations, transactions, change tracking | Create order with line items |
| **MultiDataSourceUnitOfWork** | Cross-datasource transactions, distributed operations | Update customer in DB1 and order in DB2 |
| **Direct IDataSource** | Simple queries, read-only, one-off queries | Count records, check existence |
| **UnitOfWorkWrapper** | When you need enhanced error handling and validation | Complex business logic with validation |
| **RunQuery** | Complex multi-table queries, custom SQL | Reports with joins across multiple tables |
| **ExecuteSql** | DDL operations only | CREATE TABLE, ALTER TABLE |

## Key Principles Summary

1. **Use GetEntityAsync with AppFilter** for simple single-table queries - PREFERRED
2. **Use GetScalar/GetScalarAsync** for scalar queries - Returns double
3. **Use RunQuery** for complex multi-table queries - Only when GetEntityAsync is insufficient
4. **Use ExecuteSql ONLY for DDL** - CREATE, ALTER, DROP statements
5. **Never hardcode @ in SQL queries** - Always use IDataSource.ParameterDelimiter (not needed with GetEntityAsync + AppFilter)
6. **Use UnitOfWork for complex operations** - Provides transaction management, change tracking, and batch operations
7. **Use QueryBuilder for custom queries** - Provides database-agnostic query construction (only when needed)
8. **Access IDataSource via IDMEEditor**: `_editor.GetDataSource(connectionName)`
9. **Column delimiters**: Use IDataSource.ColumnDelimiter for column names if needed (handled automatically by GetEntityAsync)
10. **Entity objects** are passed directly - NO Dictionary conversions
11. **Use DTOs for API input/output** - Convert DTOs ↔ Entity for database operations
12. **Always rollback on errors** - Ensure data consistency
13. **Keep transactions short** - Don't hold transactions open for long periods
14. **Use MultiDataSourceUnitOfWork** for distributed transactions across multiple data sources

## Caching Patterns

### CacheManager Overview

**CacheManager** provides a high-performance caching solution with multiple providers, retry logic, and monitoring. It supports in-memory, distributed, and hybrid caching strategies.

### Cache Providers

**Available Providers:**
- **SimpleCacheProvider** (Default) - High-performance in-memory cache with LRU eviction
- **MemoryCacheProvider** - Enhanced memory cache with size management
- **RedisCacheProvider** - Distributed caching support
- **HybridCacheProvider** - L1/L2 cache combination (fast local + distributed)

### Basic Cache Usage

```csharp
// Initialize with default settings
CacheManager.Initialize();

// Store data with expiration
await CacheManager.SetAsync("user:123", userData, TimeSpan.FromMinutes(30));

// Retrieve data
var user = await CacheManager.GetAsync<UserData>("user:123");

// GetOrCreate pattern (recommended)
var expensiveData = await CacheManager.GetOrCreateAsync("expensive:key", async () =>
{
    return await ComputeExpensiveOperation();
}, TimeSpan.FromHours(1));
```

### Advanced Cache Configuration

```csharp
var config = new CacheConfiguration
{
    DefaultExpiry = TimeSpan.FromMinutes(30),
    MaxItems = 10000,
    EnableStatistics = true,
    KeyPrefix = "myapp:",
    EnableCompression = true,
    CompressionThreshold = 1024,
    MemoryCache = new MemoryCacheConfiguration
    {
        SizeLimit = 100 * 1024 * 1024, // 100MB
        CompactionPercentage = 0.05, // 5%
        ExpirationScanFrequency = TimeSpan.FromMinutes(1)
    }
};

// Use Redis as primary, InMemory as fallback
CacheManager.Initialize(config, CacheProviderType.Redis, CacheProviderType.InMemory);
```

### Batch Operations

```csharp
// Set multiple items at once
var items = new Dictionary<string, object>
{
    ["key1"] = value1,
    ["key2"] = value2
};
await CacheManager.SetManyAsync(items, TimeSpan.FromMinutes(15));

// Get multiple items
var results = await CacheManager.GetManyAsync<object>(items.Keys);
```

### Cache Statistics and Monitoring

```csharp
// Get cache statistics
var stats = CacheManager.GetStatistics();
Console.WriteLine($"Hit Ratio: {stats.CombinedHitRatio:F2}%");
Console.WriteLine($"Memory Usage: {stats.TotalMemoryUsage / 1024 / 1024} MB");
Console.WriteLine($"Total Items: {stats.TotalItemCount}");

// Health checks
var health = await CacheManager.CheckHealthAsync();
foreach (var provider in new[] { health.PrimaryProviderHealth, health.FallbackProviderHealth })
{
    Console.WriteLine($"{provider?.ProviderName}: {provider?.IsHealthy}");
}
```

### CachedMemoryDataSource

**CachedMemoryDataSource** - A data source implementation that uses MemoryCache as storage backend:

```csharp
// Create cached memory data source
var cachedDataSource = new CachedMemoryDataSource(
    "CachedDB",
    logger,
    editor,
    DataSourceType.CachedMemory,
    errorObject
);

// Use like any other IDataSource
cachedDataSource.Openconnection();
var results = await cachedDataSource.GetEntityAsync("Customers", filters);
```

**Features:**
- Uses MemoryCacheProvider for structured caching
- Automatic expiration and size limits
- Thread-safe operations
- Statistics and monitoring support

### Caching Best Practices

**1. Choose the Right Provider:**
- **SimpleCacheProvider**: Development, small applications
- **MemoryCacheProvider**: Memory-constrained environments
- **RedisCacheProvider**: Production, distributed scenarios
- **HybridCacheProvider**: High-performance, multi-tier applications

**2. Use GetOrCreate Pattern:**
```csharp
// Always use GetOrCreate for expensive operations
var data = await CacheManager.GetOrCreateAsync("key", async () =>
{
    return await ExpensiveDatabaseQuery();
}, TimeSpan.FromMinutes(10));
```

**3. Set Appropriate Expiration:**
- Short-lived data: Minutes to hours
- Configuration data: Hours to days
- Reference data: Days to weeks
- Use sliding expiration for frequently accessed data

**4. Monitor Cache Performance:**
```csharp
// Regularly check statistics
var stats = CacheManager.GetStatistics();
if (stats.CombinedHitRatio < 0.7) // Less than 70% hit ratio
{
    // Consider adjusting expiration times or cache warming
}
```

**5. Handle Cache Failures Gracefully:**
```csharp
try
{
    var data = await CacheManager.GetAsync<Data>("key");
    if (data == null)
    {
        // Fallback to database
        data = await LoadFromDatabase();
    }
}
catch (Exception ex)
{
    // Log and fallback to database
    Logger.LogError(ex, "Cache operation failed");
    data = await LoadFromDatabase();
}
```

**6. Use Key Prefixes:**
```csharp
// Configure key prefix to avoid collisions
var config = new CacheConfiguration
{
    KeyPrefix = "myapp:users:" // All keys will be prefixed
};
```

**7. Cache Warming:**
```csharp
// Pre-populate cache with frequently accessed data
var warmupData = new Dictionary<string, Func<Task<object>>>
{
    ["popular:items"] = () => LoadPopularItems(),
    ["config:settings"] = () => LoadConfiguration()
};

await CacheManager.WarmCacheAsync(warmupData);
```

### Integration with Data Sources

**Cache query results:**
```csharp
public async Task<IEnumerable<Customer>> GetCustomersAsync(string categoryId)
{
    var cacheKey = $"customers:category:{categoryId}";
    
    return await CacheManager.GetOrCreateAsync(cacheKey, async () =>
    {
        var dataSource = _editor.GetDataSource("MyDatabase");
        var filters = new List<AppFilter>
        {
            new AppFilter { FieldName = "CategoryID", Operator = "=", FilterValue = categoryId }
        };
        
        var results = await dataSource.GetEntityAsync("Customers", filters);
        return results.Cast<Customer>().ToList();
    }, TimeSpan.FromMinutes(15));
}
```

**Cache entity structures:**
```csharp
public async Task<EntityStructure> GetEntityStructureAsync(string entityName)
{
    var cacheKey = $"entity:structure:{entityName}";
    
    return await CacheManager.GetOrCreateAsync(cacheKey, async () =>
    {
        var dataSource = _editor.GetDataSource("MyDatabase");
        return dataSource.GetEntityStructure(entityName, refresh: false);
    }, TimeSpan.FromHours(24)); // Entity structures change infrequently
}
```

## ETL Patterns

### ETLEditor Overview

**ETLEditor** orchestrates Extract, Transform, and Load operations: script generation, entity creation, data copying, and imports. It provides a comprehensive workflow for migrating data between data sources.

### Basic ETL Workflow

```csharp
var etl = new ETLEditor(dmeEditor);

// 1) Build script header from source
etl.CreateScriptHeader(
    sourceDs, 
    new Progress<PassedArgs>(p => Console.WriteLine(p.Messege)), 
    token);

// 2) Optionally filter/modify scripts
// etl.Script.ScriptDetails = etl.Script.ScriptDetails.Where(...).ToList();

// 3) Execute (create entities, then optionally copy data)
var result = await etl.RunCreateScript(
    progress, 
    token, 
    copydata: true, 
    useEntityStructure: true);
```

### ETL Import Workflow

```csharp
// Prepare mapping
var mapTuple = MappingManager.CreateEntityMap(
    dmeEditor, 
    "LegacyCustomers", 
    "LegacyDB", 
    "Customers", 
    "MainDB");
var entityMap = mapTuple.Item2;
var selected = entityMap.MappedEntities.First();

// Create and run import script
etl.CreateImportScript(entityMap, selected);
var importResult = await etl.RunImportScript(progress, token);
```

### ETL Notes

- Entity creation uses destination datasource's `CreateEntityAs`
- CopyData steps call internal `RunCopyEntityScript` which fetches, maps, applies defaults, and inserts
- Use `StopErrorCount` to guard execution
- Check `LoadDataLogs` for detailed run logs

### ETLDataCopier Pattern

**ETLDataCopier** provides high-throughput data copying with batching, optional parallelism, and retry capabilities.

```csharp
var copier = new ETLDataCopier(dmeEditor);

var result = await copier.CopyEntityDataAsync(
    sourceDs: dmeEditor.GetDataSource("Legacy"),
    destDs: dmeEditor.GetDataSource("Modern"),
    srcEntity: "Customers",
    destEntity: "Customers",
    progress: new Progress<PassedArgs>(p => Console.WriteLine(p.Messege)),
    token: CancellationToken.None,
    map_DTL: selectedMapping,     // optional EntityDataMap_DTL
    customTransformation: r => r,  // optional transformation delegate
    batchSize: 200,
    enableParallel: true,
    maxRetries: 2);
```

### ETLDataCopier Features

- **Defaults Application**: Defaults are applied per transformed record using `MappingDefaultsHelper.ApplyDefaultsToObject`
- **Mapping Preservation**: Only null/default destination fields are filled; mapped values are preserved
- **Defaults Resolution**: Goes through `DefaultsManager` (static values and rule-based values)
- **Performance**: Use `batchSize` and `enableParallel` according to dataset size and destination capabilities
- **Transformation**: Provide a `customTransformation` for last-mile enrichment/cleansing after mapping/defaults

### ETLEntityProcessor Pattern

**ETLEntityProcessor** provides record validation, transformation, and batch processing capabilities.

```csharp
var processor = new ETLEntityProcessor();

// Validate records
var (validRecords, invalidRecords) = processor.ValidateRecords(
    records,
    record => record != null && !string.IsNullOrEmpty(record.Name));

// Transform records
var transformedRecords = processor.TransformRecords(
    records,
    record => 
    {
        // Custom transformation logic
        record.ProcessedDate = DateTime.Now;
        return record;
    });

// Process records with optional parallelism
await processor.ProcessRecordsAsync(
    records,
    async record => 
    {
        await ProcessRecordAsync(record);
    },
    parallel: true);
```

## Data Import Patterns

### DataImportManager Overview

**DataImportManager** provides enhanced data import functionality with helper-based architecture, DefaultsManager integration, and comprehensive validation and transformation capabilities.

### Basic Import (Backward Compatible)

```csharp
using var importManager = new DataImportManager(dmeEditor);

// Configure using familiar properties
importManager.SourceEntityName = "SourceCustomers";
importManager.SourceDataSourceName = "ExternalCRM";
importManager.DestEntityName = "Customers";
importManager.DestDataSourceName = "MainDatabase";

// Load destination structure (auto-loads defaults)
var loadResult = importManager.LoadDestEntityStructure("Customers", "MainDatabase");

// Execute import
var progress = new Progress<IPassedArgs>(args => Console.WriteLine(args.Messege));
using var cts = new CancellationTokenSource();
var result = await importManager.RunImportAsync(progress, cts.Token, null, 100);
```

### Enhanced Configuration Import

```csharp
using var importManager = new DataImportManager(dmeEditor);

// Create enhanced configuration
var config = importManager.CreateImportConfiguration(
    "ProductsExport", 
    "ExternalSystem",
    "Products", 
    "MainDatabase");

// Configure advanced options
config.SourceFilters.Add(new AppFilter 
{ 
    FieldName = "ModifiedDate", 
    Operator = ">=", 
    FilterValue = DateTime.Today.AddDays(-7).ToString() 
});

config.SelectedFields = new List<string> { "ProductCode", "ProductName", "Price" };
config.BatchSize = 200;
config.ApplyDefaults = true; // Uses DefaultsManager automatically

// Custom transformation
config.CustomTransformation = (record) => 
{
    // Apply business logic
    if (record is Dictionary<string, object> dict)
    {
        dict["ImportedDate"] = DateTime.Now;
        dict["ImportedBy"] = Environment.UserName;
    }
    return record;
};

// Execute with enhanced features
var result = await importManager.RunImportAsync(config, progress, cancellationToken);
```

### DataImportManager Helpers

```csharp
using var importManager = new DataImportManager(dmeEditor);

// Use helpers directly for fine-grained control
var validation = importManager.ValidationHelper.ValidateImportConfiguration(config);
var optimalBatchSize = importManager.BatchHelper.CalculateOptimalBatchSize(50000, 2048);
var metrics = importManager.ProgressHelper.CalculatePerformanceMetrics(startTime, processed, total);

// Access detailed logging
var errors = importManager.ProgressHelper.GetLogEntriesByLevel(ImportLogLevel.Error);
var logText = importManager.ProgressHelper.ExportLogToText();
```

### DataImportManager Features

- **Helper-Based Architecture**: ValidationHelper, TransformationHelper, BatchHelper, ProgressHelper
- **DefaultsManager Integration**: Automatic loading and application of default values
- **Performance Optimization**: Intelligent batch size calculation based on data characteristics
- **Advanced Monitoring**: Real-time progress reporting with performance metrics
- **Comprehensive Logging**: Different log levels (Info, Warning, Error, Success, Debug)

### Import Control Operations

```csharp
// Pause/Resume Import
importManager.PauseImport();  // Pauses current operation
importManager.ResumeImport(); // Resumes paused operation

// Cancel Import
importManager.CancelImport(); // Requests cancellation

// Check Status
var status = importManager.GetImportStatus();
Console.WriteLine($"Running: {status.IsRunning}, Paused: {status.IsPaused}");
```

## Data Synchronization Patterns

### BeepSyncManager Overview

**BeepSyncManager** provides modern, clean synchronization functionality with helper-based architecture, following Single Responsibility Principle. It consolidates functionality from `DataSyncManager` and `DataSyncService`.

### Basic Synchronization

```csharp
// Initialize the manager
var syncManager = new BeepSyncManager(dmeEditor);

// Create a sync schema
var schema = new DataSyncSchema
{
    Id = Guid.NewGuid().ToString(),
    SourceDataSourceName = "SourceDB",
    DestinationDataSourceName = "DestinationDB",
    SourceEntityName = "Customers",
    DestinationEntityName = "Customers",
    SourceSyncDataField = "CustomerId",
    DestinationSyncDataField = "CustomerId",
    SyncType = "Full",
    SyncDirection = "SourceToDestination"
};

// Add field mappings
schema.MappedFields.Add(new FieldSyncData
{
    SourceField = "Name",
    DestinationField = "CustomerName"
});

// Add schema and sync
syncManager.AddSyncSchema(schema);
var result = await syncManager.SyncDataAsync(schema, progress: progressReporter);

// Save schemas
await syncManager.SaveSchemasAsync();
```

### BeepSyncManager Helpers

- **DataSourceHelper**: Manages all `IDataSource` operations, handles connection validation
- **FieldMappingHelper**: Handles field mapping between source and destination, creates destination entities
- **SyncValidationHelper**: Validates sync schemas, data source accessibility, entity existence
- **SyncProgressHelper**: Progress reporting, comprehensive logging, sync run tracking
- **SchemaPersistenceHelper**: Async schema persistence to JSON files, schema backup functionality

### Sync Features

- **Clean Architecture**: Helper-based design vs. monolithic classes
- **Async Operations**: Full async support throughout
- **Better Error Handling**: Detailed error information and logging
- **Progress Reporting**: Rich progress information for UI integration
- **Testability**: Clean interfaces allow easy mocking for unit tests
- **Resource Management**: Proper disposal and resource cleanup
- **Validation**: Comprehensive pre-sync validation
- **Persistence**: Robust schema persistence with backup support

## Migration Patterns

### MigrationManager Overview

**MigrationManager** provides schema migration capabilities, allowing you to ensure entities exist, add missing columns, and manage schema evolution.

### Basic Migration Operations

```csharp
var migrationManager = new MigrationManager(dmeEditor);
migrationManager.MigrateDataSource = dmeEditor.GetDataSource("TargetDatabase");

// Ensure entity exists (create if missing, add missing columns)
var result = migrationManager.EnsureEntity(
    entityStructure, 
    createIfMissing: true, 
    addMissingColumns: true);

// Ensure entity from POCO type
var result = migrationManager.EnsureEntity(
    typeof(Customer), 
    createIfMissing: true, 
    addMissingColumns: true, 
    detectRelationships: true);

// Get missing columns
var missingColumns = migrationManager.GetMissingColumns(currentEntity, desiredEntity);
```

### Migration Operations

```csharp
// Create entity
var result = migrationManager.CreateEntity(entityStructure);

// Drop entity
var result = migrationManager.DropEntity("OldTableName");

// Truncate entity
var result = migrationManager.TruncateEntity("TableName");

// Rename entity
var result = migrationManager.RenameEntity("OldName", "NewName");

// Alter column
var result = migrationManager.AlterColumn("TableName", "ColumnName", newColumnField);

// Drop column
var result = migrationManager.DropColumn("TableName", "ColumnName");

// Rename column
var result = migrationManager.RenameColumn("TableName", "OldColumnName", "NewColumnName");

// Create index
var result = migrationManager.CreateIndex(
    "TableName", 
    "IndexName", 
    new[] { "Column1", "Column2" },
    options: new Dictionary<string, object> { { "Unique", true } });
```

### Migration Best Practices

- **Use EnsureEntity**: For schema evolution scenarios where you want to add columns without dropping data
- **Test First**: Always test migrations on a copy of production data
- **Version Control**: Track schema changes in version control
- **Backup**: Always backup before running destructive operations (Drop, Truncate)
- **Incremental**: Use incremental migrations rather than full schema replacements

## Mapping Patterns

### MappingManager Overview

**MappingManager** provides utility methods to create and manage entity mappings between source and destination entities, enabling data transformation and migration.

### Creating Entity Mappings

```csharp
// Create mapping for migration between two entities
var mapTuple = MappingManager.CreateEntityMap(
    dmeEditor,
    "LegacyCustomers",      // Source entity name
    "LegacyDB",             // Source data source name
    "Customers",            // Destination entity name
    "MainDB");              // Destination data source name

var entityMap = mapTuple.Item2;
if (mapTuple.Item1.Flag == Errors.Ok)
{
    // Mapping created successfully
}

// Create mapping for destination entity only
var mapTuple = MappingManager.CreateEntityMap(
    dmeEditor,
    "Customers",            // Destination entity name
    "MainDB");              // Destination data source name

// Add source entity to existing mapping
var detail = MappingManager.AddEntityToMappedEntities(
    dmeEditor,
    "SourceDB",             // Source data source name
    "SourceCustomers",      // Source entity name
    destinationEntity);     // Destination entity structure
```

### Mapping Usage Patterns

```csharp
// Load existing mapping
var mapping = MappingManager.LoadMapping(dmeEditor, "Customers", "MainDB");

// Save mapping
MappingManager.SaveMapping(dmeEditor, "Customers", "MainDB", mapping);

// Get field mappings
var fieldMappings = mapping.MappedEntities.First().FieldMappings;

// Use mapping in ETL operations
var etl = new ETLEditor(dmeEditor);
etl.CreateImportScript(mapping, mapping.MappedEntities.First());
```

### Mapping Best Practices

- **Field Mapping**: Explicitly map fields that have different names or types
- **Type Conversion**: MappingManager handles type conversion automatically
- **Default Values**: Use MappingDefaultsHelper to apply defaults during transformation
- **Validation**: Validate mappings before running large data operations
- **Reusability**: Save mappings for reuse across multiple operations

## UnitOfWork Advanced Patterns

### UnitOfWork Helpers

**UnitOfWork** uses a helper-based architecture for enhanced functionality:

- **IUnitofWorkDefaults<T>**: Default value operations and DefaultsManager integration
- **IUnitofWorkValidation<T>**: Validation operations and business rule enforcement
- **IUnitofWorkDataHelper<T>**: Data operations and query building
- **IUnitofWorkStateHelper<T>**: State management and change tracking

### UnitOfWork with DefaultsManager

```csharp
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

// DefaultsManager automatically applies:
// - CreatedDate = DateTime.UtcNow (on new entities)
// - CreatedBy = currentUserId (on new entities)
// - ModifiedDate = DateTime.UtcNow (on updates)
// - ModifiedBy = currentUserId (on updates)
// - Active = true (if configured)
// - Status = "Active" (if configured)

unitOfWork.New(); // Defaults applied automatically
var result = await unitOfWork.Commit();
```

### UnitOfWork Change Tracking

```csharp
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

// Check if there are changes
bool isDirty = unitOfWork.IsDirty;

// Get modified entities
var modifiedEntities = unitOfWork.GetModifiedEntities();

// Get added entities
var addedEntities = unitOfWork.GetAddedEntities();

// Get deleted entities
var deletedEntities = unitOfWork.GetDeletedEntities();

// Get entity state
var state = unitOfWork.GetEntityState(entity);
```

### UnitOfWork Validation

```csharp
using var unitOfWork = new UnitofWork<Customer>(
    editor, 
    "MyDatabase", 
    "Customers", 
    "ID"
);

// Validate before commit
var validationResult = unitOfWork.Validate();
if (!validationResult.IsValid)
{
    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"Validation Error: {error.Message}");
    }
    return;
}

var result = await unitOfWork.Commit();
```

## ClassCreator and Code Generation Patterns

### ClassCreator Overview

**ClassCreator** provides comprehensive code generation capabilities for creating classes from database schemas, POCOs, and entity structures.

### Generate Classes from Entity Structure

```csharp
var classCreator = new ClassCreator(dmeEditor);

// Generate class from entity structure
var entityStructure = dataSource.GetEntityStructure("Customers");
var classCode = classCreator.CreateClassFromEntityStructure(
    entityStructure,
    namespaceName: "MyApp.Models",
    className: "Customer");

// Generate POCO from entity structure
var pocoCode = classCreator.CreatePocoFromEntityStructure(
    entityStructure,
    namespaceName: "MyApp.Models",
    className: "Customer");
```

### Generate Entity Structure from POCO

```csharp
var classCreator = new ClassCreator(dmeEditor);

// Create entity structure from POCO type
var entityStructure = classCreator.CreateEntityStructureFromPoco(typeof(Customer));

// Use entity structure for migration or ETL
var migrationManager = new MigrationManager(dmeEditor);
migrationManager.EnsureEntity(typeof(Customer), createIfMissing: true);
```

### Generate Web API Controllers

```csharp
var webApiGenerator = new WebApiGenerator(dmeEditor);

// Generate controller from entity
var controllerCode = webApiGenerator.GenerateController(
    entityStructure,
    namespaceName: "MyApp.Controllers",
    controllerName: "CustomersController",
    baseRoute: "api/customers");
```

### ClassCreator Helpers

- **PocoClassGeneratorHelper**: Generate POCO classes
- **DatabaseClassGeneratorHelper**: Generate classes from database schemas
- **WebApiGeneratorHelper**: Generate Web API controllers
- **UiComponentGeneratorHelper**: Generate UI components
- **ValidationAndTestingGeneratorHelper**: Generate validation and test code

## References

- See `ARCHITECTURE.md` for system architecture
- See BeepDM documentation for UnitOfWork patterns
- See `.github/instructions/cursor-instructions.md` for IDE-specific guidance
- See BeepDM/Docs/UnitOfWork.rst for detailed UnitOfWork documentation
- See BeepDM/Docs/MultiDataSourceUnitOfWork.rst for distributed transaction patterns
- See BeepDM/DataManagementEngineStandard/Caching/README.md for comprehensive caching documentation
- See BeepDM/DataManagementEngineStandard/Editor/ETL/ for ETL documentation
- See BeepDM/DataManagementEngineStandard/Editor/Importing/README_DataImportManager.md for DataImportManager documentation
- See BeepDM/DataManagementEngineStandard/Editor/BeepSync/README.md for BeepSyncManager documentation
- See BeepDM/DataManagementEngineStandard/Editor/Migration/ for MigrationManager documentation
- See BeepDM/DataManagementEngineStandard/Editor/Mapping/ for MappingManager documentation
- See BeepDM/DataManagementEngineStandard/Tools/ for ClassCreator documentation
