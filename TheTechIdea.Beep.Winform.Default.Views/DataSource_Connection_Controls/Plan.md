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


Summary plan to implement connection user controls for all DataSourceType values
1.	Standardize base pattern
•	Class naming: uc_{Provider}Connection : uc_DataConnectionBase
•	File path: ...Default.Views\DataSource_Connection_Controls\uc_{Provider}Connection.cs
•	Addin metadata: set DatasourceType = DataSourceType.{Provider}, Category = DatasourceCategory.{Category}, addinType = AddinType.ConnectionProperties
•	Constructors: default and IServiceProvider versions; set Details.AddinName = "{Provider} Connection"
•	Override OnNavigatedTo(parameters) and call base.OnNavigatedTo(parameters) (base handles drivers, versions, bindings)
•	Use BeepTabs from base; add provider tabs only when needed
2.	Fields mapping to ConnectionProperties (reuse everywhere)
•	Standard fields:
•	DriverName, DriverVersion (base controls map these)
•	UserID, Password, ConnectionString (base binds these)
•	Host/Server, Port, Database (ServiceName/SID for Oracle), InstanceName (if applicable)
•	FilePath, FileName, Extension (file-based)
•	IsLocal, IsInMemory
•	Keep Connection.ConnectionString updated (builder or manual override)
3.	Standard tabs (apply as needed per provider)
•	Connection: driver/version, UserID, Password, ConnectionString
•	Server: Host, Port, Instance, Database/Service/SID
•	Authentication: mode (Basic/Integrated/API/OAuth), SSL/TLS settings (Enable, Cert paths, Verify server)
•	Files: FilePath, FileName, Extension, format options (CSV delimiter/encoding, Excel sheet)
•	Cloud/API: base URL, region, project/account/workspace, API keys/secret, OAuth client/secret/token endpoint
•	Options: Pooling, Timeouts, Retries, Schema/SearchPath, extra options (key/value)
•	Test/Diagnostics: Test connection button, Build/Preview connection string, log area
4.	Provider templates by category (use this as a checklist)
A) RDBMS (DatasourceCategory.RDBMS)
•	Types: Oracle, SqlServer, Mysql, MariaDB, Postgre, DB2, FireBird, Hana, SqlLite, SqlCompact, AzureSQL, AWSRDS, Cockroach, Spanner, TerraData, Vertica
•	Fields:
•	Server: Host, Port (defaults), Instance (if used), Database (or ServiceName/SID)
•	Auth: UserID, Password, Integrated (SqlServer/AzureSQL)
•	TLS/SSL: Enable, Encrypt, TrustServerCertificate, SSLMode (MySQL/Postgre)
•	Options: Pooling, Timeouts, Schema/SearchPath
•	Defaults and notes:
•	Oracle: port 1521; ServiceName or SID
•	SqlServer/AzureSQL: 1433; optional InstanceName; Integrated Security; Encrypt/TrustServerCertificate
•	MySQL/MariaDB: 3306; SslMode
•	Postgre: 5432; SSLMode; SearchPath
•	SQLite/SqlCompact: use Files tab (FilePath/FileName/Extension), no Host/Port
•	SAP Hana: default 30015
B) NOSQL (DatasourceCategory.NOSQL)
•	Types: MongoDB, CouchDB, RavenDB, Couchbase, Redis, DynamoDB, Firebase, LiteDB, ArangoDB, Cassandra, OrientDB, ElasticSearch, ClickHouse, InfluxDB, VistaDB
•	Fields:
•	Hosts list (clusters), Port(s), Database/Keyspace/Bucket/Index
•	Auth: UserID/Password or API Key/Token
•	TLS/SSL: Enable, CA, certs
•	Notes:
•	MongoDB: SRV or host list; Database; ReplicaSet; TLS (27017)
•	Cassandra: hosts, port 9042, Keyspace, consistency
•	Redis: Host, Port 6379, Password, DB index
•	ElasticSearch: base URL, Index, TLS
•	Couchbase: Cluster/Host, Bucket, User/Pass
•	InfluxDB: URL, Org/Bucket/Token
•	LiteDB/VistaDB: file-based → Files tab
C) GraphDB (DatasourceCategory.GraphDB)
•	Types: Neo4j, TigerGraph, JanusGraph
•	Fields:
•	Neo4j: bolt URI (host:port[/db]), User/Pass, TLS
•	TigerGraph: host/port, Token
•	JanusGraph: backend endpoints (maps to Cassandra/HBase configs)
D) Columnar/BigData/TimeSeries (DatasourceCategory.ColumnarDB/BigData/TimeSeriesDB)
•	Types: ClickHouse (also NoSQL), Hadoop, Kudu, Druid, Pinot, Parquet, Avro, ORC, Feather, TimeScale
•	Fields:
•	Server-based: Host, Port, Database, TLS, User/Pass
•	File-based: FilePath, FileName/pattern, compression/format flags
•	Options: schema inference
•	Notes:
•	Parquet/Avro/ORC/Feather: focus on Files tab
•	TimeScale: Postgre template (5432 + SearchPath)
E) In-Memory (DatasourceCategory.INMEMORY)
•	Types: RealIM, Petastorm, RocketSet
•	Fields: Name, optional persistence path, TTL/memory sizing
•	Use ViewModel CreateInMemoryConnection path
F) Cloud/DataWarehouse/DataLake (DatasourceCategory.CLOUD/DataWarehouse/DataLake)
•	Types: AWSRedshift, GoogleBigQuery, AWSGlue, AWSAthena, AzureCloud, SnowFlake, DataBricks, Firebolt, Hologres, Supabase
•	Fields:
•	Region, Account/Project/Workspace
•	Auth: AccessKey/SecretKey, OAuth client/secret/token, Role
•	Warehouse/Database/Schema
•	Storage paths (S3/Blob) for staging (Athena/Glue)
•	Notes:
•	BigQuery: Project, Dataset, key JSON file
•	Snowflake: Account, Warehouse, Role, Database, Schema, auth mode
G) Streaming/Messaging (DatasourceCategory.Stream/MessageQueue)
•	Types: Kafka, RabbitMQ, ActiveMQ, Pulsar, Kinesis, SQS, SNS, AzureServiceBus, Nats, ZeroMQ, MassTransit
•	Fields:
•	Bootstrap servers/Endpoints
•	Topic/Queue/Exchange
•	GroupId/ClientId (Kafka), SSL/SASL
•	Credentials (user/password/keys)
H) MLModel (DatasourceCategory.MLModel)
•	Types: TFRecord, ONNX, PyTorchData, ScikitLearnData, Onnx
•	Fields: Files tab (model/data paths), pre/post options, schema inference
I) FILE (DatasourceCategory.FILE)
•	Types: FlatFile, CSV, TSV, Text, YAML, Json, XML, Xls, Doc, Docx, PPT, PPTX, PDF, Markdown, Log, INI, HTML, SQL, Parquet, Avro, ORC, Feather, RecordIO, LibSVM, GraphML, DICOM, LAS
•	Fields:
•	FilePath, FileName/pattern, Extension
•	CSV/TSV/Text: delimiter, quote, encoding, header row
•	Excel: sheet, header row
•	XML/Json: root/array paths
•	Binary: extraction flags
J) WEBAPI (DatasourceCategory.WEBAPI)
•	Types: WebApi, RestApi, OData, GraphQL
•	Fields:
•	Base URL, Endpoint path
•	Auth: None/Basic/Bearer/API Key/OAuth
•	Headers (key/value), Query params, Pagination mode
•	Test: sample call and show response
K) Misc/Bridges (DatasourceCategory.NONE or appropriate)
•	Types: ODBC, OLEDB, ADO
•	Fields: Full connection string, DSN, Provider (minimal UI)
L) Search/Workflow/IoT (various categories)
•	Types: Solr (SearchEngine), OPC (Industrial), AWSSWF, AWSStepFunctions (Workflow), AWSIoT, AWSIoTCore, AWSIoTAnalytics (IoT)
•	Fields:
•	Solr: base URL, Collection
•	OPC: endpoint URL, security mode, certs
•	SWF/StepFunctions: region, credentials, workflow id
•	IoT: endpoint, registry ids, certs/keys
M) VectorDB (DatasourceCategory.VectorDB)
•	Types: ChromaDB, PineCone, Qdrant, Weaviate, Milvus, RedisVector, Zilliz, Vespa, ShapVector
•	Fields:
•	Endpoint/host/port or cloud API URL
•	API Key/Auth
•	Collection/Index/Namespace
•	Dimension, distance metric, replicas
•	RedisVector: reuse Redis host/port + vector options
N) Connectors (DatasourceCategory.Connector)
•	Large set (CRM, Marketing, E‑commerce, PM, Comms, Storage, Payments, Social, Workflow, DevTools, Support, Analytics)
•	Fields: Base URL, Auth (API Key/OAuth), resource identifier (account/workspace/project), pagination, rate limits
5.	Implementation checklist for every provider
•	Create class uc_{Provider}Connection inheriting uc_DataConnectionBase
•	Add [AddinAttribute(Caption="{Provider} Connection", DatasourceType=DataSourceType.{Provider}, Category=DatasourceCategory.{Category}, Name="uc_{Provider}Connection", addinType=AddinType.ConnectionProperties, displayType=DisplayType.Popup, misc="Config", menu="Configuration")]
•	Constructors: call InitializeComponent() and set Details.AddinName
•	Optionally add provider-specific tabs; bind fields to ConnectionProperties (Host, Port, Database, FilePath, etc.)
•	Add a “Build” button (optional): construct provider connection string and assign Connection.ConnectionString
•	“Test” button: try Editor.OpenDataSource(Connection.ConnectionName) and show result
6.	Base control status (already in place)
•	Binds UserID, Password, ConnectionString to DataConnectionViewModel.Connection
•	Populates drivers and versions from Config_editor.DataDriversClasses filtered by category/type
•	Updates Connection.DriverName and Connection.DriverVersion on selection
7.	Recommended defaults registry (optional optimization)
•	Maintain a simple registry per DataSourceType:
•	Default port
•	Connection string template placeholders
•	Required fields list
•	Use it to auto-fill fields and build strings consistently

		Add provider-specific optional tabs/field bindings where needed:
•	example : SQLite: Files tab only (FilePath/FileName/Extension).
•				PostgreSQL: SearchPath, SSLMode in Options/Authentication.
•				MySQL: SSLMode in Authentication/Options.
•				SQL Server: Integrated/SQL Auth, Encrypt/TrustServerCertificate.