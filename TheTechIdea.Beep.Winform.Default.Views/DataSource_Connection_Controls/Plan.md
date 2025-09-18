# Data Source Connection Controls Implementation Plan

## Overview
This plan outlines the implementation of **individual data source connection controls** for the Beep Framework. Instead of generic category-based controls, we'll create specific controls for each data source type to handle their unique connection requirements.

## Current Status
- ✅ ReadMEInstructions.md analyzed
- ✅ Existing uc_RDBMSConnections pattern studied
- ✅ DataConnectionViewModel pattern understood
- ✅ Available data source types identified from DataSourceType enum
- 🔄 **APPROACH UPDATED**: Moving from category-based to individual data source controls

## Individual Data Source Controls to Implement

### Relational Databases (RDBMS)
1. **Oracle** - TNS names, service names, connection pooling
2. **SQL Server** - Instance names, authentication methods, connection pooling
3. **MySQL** - Connection pooling, SSL options
4. **PostgreSQL** - Schema specifications, SSL modes
5. **SQLite** - File path, connection modes
6. **Firebird** - Database path, user roles

### NoSQL Databases
7. **MongoDB** - Connection string, database name, authentication
8. **CouchDB** - URL, database name, authentication
9. **Redis** - Host, port, password, database number
10. **RavenDB** - URLs, database name, certificates

### File Formats
11. **CSV** - File path, delimiter, encoding, headers
12. **JSON** - File path, schema validation
13. **XML** - File path, schema validation, XPath
14. **Excel** - File path, sheet selection, range

### APIs & Web Services
15. **REST API** - Base URL, authentication, headers
16. **GraphQL** - Endpoint URL, authentication, schema
17. **OData** - Service URL, authentication

### Vector Databases
18. **ChromaDB** - Host, port, collection settings
19. **Pinecone** - API key, environment, index settings
20. **Weaviate** - URL, authentication, class settings

## Implementation Pattern for Each Individual Control

### 1. ViewModel Creation (in TheTechIdea.Beep.MVVM)
- Create specific ViewModel inheriting from BaseViewModel
- Use UnitofWork<ConnectionProperties> pattern
- Filter by specific DataSourceType (not category)
- Include data source-specific properties and validation
- Implement Save(), UpdateConnection(), and other common methods

### 2. User Control Creation (in TheTechIdea.Beep.Winform.Default.Views)
- Create specific UserControl inheriting from TemplateUserControl and IAddinVisSchema
- Add data source-specific AddinAttribute and AddinVisSchema attributes
- Implement grid-based interface using BeepSimpleGrid
- Handle cell value changes and save events
- Configure columns specific to the data source

### 3. Data Source-Specific Properties
Each control will have unique properties based on the data source requirements:

**Oracle:**
- TNS Name/Network Alias
- Service Name
- Connection Pooling settings
- Oracle-specific authentication

**SQL Server:**
- Server Instance Name
- Authentication Mode (Windows/SQL)
- Connection Pooling
- Application Intent

**MongoDB:**
- Connection String
- Database Name
- Authentication Database
- Replica Set settings

## Priority Implementation Order
1. **Oracle** - Enterprise RDBMS, complex connection requirements
2. **SQL Server** - Widely used, multiple authentication methods
3. **MySQL** - Popular open-source database
4. **PostgreSQL** - Advanced open-source database
5. **MongoDB** - Leading NoSQL database
6. **Redis** - High-performance key-value store
7. **CSV** - Most common file format
8. **REST API** - Standard web service pattern

## File Structure to Create
```
DataSource_Connection_Controls/
├── Plan.md (this updated file)
├── Progress.md
├── Individual_Controls/
│   ├── RDBMS/
│   │   ├── uc_OracleConnections.cs
│   │   ├── uc_SQLServerConnections.cs
│   │   ├── uc_MySQLConnections.cs
│   │   └── ...
│   ├── NoSQL/
│   │   ├── uc_MongoDBConnections.cs
│   │   ├── uc_RedisConnections.cs
│   │   └── ...
│   ├── Files/
│   │   ├── uc_CSVConnections.cs
│   │   ├── uc_JSONConnections.cs
│   │   └── ...
│   └── APIs/
│       ├── uc_RESTAPIConnections.cs
│       └── ...
```

## Benefits of Individual Controls
- **Specific Configuration**: Each data source gets tailored connection options
- **Better Validation**: Data source-specific validation rules
- **Enhanced UX**: Users see only relevant connection parameters
- **Maintainability**: Easier to modify for specific data source requirements
- **Extensibility**: New data sources can be added without affecting others

## Next Steps
1. Update Progress.md to reflect individual control approach
2. Start with Oracle connections (most complex RDBMS)
3. Create OracleConnectionViewModel with Oracle-specific properties
4. Create uc_OracleConnections control
5. Test and refine the implementation
6. Continue with SQL Server, then other high-priority data sources

### 3. DTO/Model Creation (if needed)
- Create category-specific DTOs if existing ConnectionProperties is insufficient
- Place in TheTechIdea.Beep.MVVM project

## Priority Implementation Order
1. **NOSQL** - High priority, commonly used
2. **FILE** - High priority, file-based data sources
3. **WEBAPI** - Medium priority, API connections
4. **VECTORDATABASE** - Medium priority, AI/ML focus
5. **CLOUD** - Medium priority, cloud services

## File Structure to Create
```
DataSource_Connection_Controls/
├── Plan.md (this file)
├── Progress.md
├── NOSQL/
│   ├── uc_NOSQLConnections.cs
│   ├── uc_NOSQLConnections.Designer.cs
│   └── uc_NOSQLConnections.resx
├── FILE/
│   ├── uc_FileConnections.cs
│   ├── uc_FileConnections.Designer.cs
│   └── uc_FileConnections.resx
└── ...
```

## Next Steps
1. Create Progress.md file to track implementation status
2. Start with NOSQL category implementation
3. Create uc_NOSQLConnections control following the RDBMS pattern
4. Create NOSQLConnectionViewModel in MVVM project
5. Test and refine the implementation

## Dependencies
- TheTechIdea.Beep.MVVM project for ViewModels
- TheTechIdea.Beep.Winform.Default.Views for UI controls
- DataManagementEngine for data source definitions
- DataManagementModels for data structures</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Default.Views\Configuration\DataSource_Connection_Controls\Plan.md
