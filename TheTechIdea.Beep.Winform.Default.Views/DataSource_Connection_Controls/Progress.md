# Data Source Connection Controls - Progress

Updated once per change to avoid repetition.

## Completed
- Oracle: `uc_OracleConnection` (base wired, metadata, constructors)
- SQL Server: `uc_SqlServerConnection` (base wired, metadata, constructors)
- MySQL: `uc_MySqlConnection` (base wired, metadata, constructors)
- PostgreSQL: `uc_PostgreConnection` (base wired, metadata, constructors)
- SQLite: `uc_SqliteConnection` (base wired, metadata, constructors)

## In Progress / Next
- Add provider-specific tabs and field bindings where needed (e.g., SQLite Files tab, Postgre SearchPath, MySQL SSLMode)
- Implement file-based controls (CSV, JSON, XML, Excel)
- Implement Web API controls (REST, GraphQL, OData)
- Implement NoSQL controls (MongoDB, Redis)

## Notes
- All new controls inherit `uc_DataConnectionBase`. Base handles drivers, versions, bindings.
- Use `Details.AddinName = "{Provider} Connection"` consistently.
- Keep `OnNavigatedTo(parameters)` override minimal and call base.
