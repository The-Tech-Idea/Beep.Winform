# Connection Controls Refactoring Plan

## Overview
This plan outlines the refactoring of all connection controls in the DataSource_Connection_Controls directory. The goal is to standardize the architecture by:

1. Treating DataConnectionBase as a dialog that accepts a ConnectionProperties object and returns updated/new ConnectionProperties
2. Removing ViewModel usage from all controls
3. Centralizing main features in DataConnectionBase
4. Having inherited controls add their specific tabs and controls in Designer.cs and bind them to the ConnectionProperties property
5. Using beepTabs1 as the tab host with all required TabPages created and added in InitializeComponent

## Connection Controls List

### Base Control
- `uc_DataConnectionBase.cs` - Base dialog control with main features

### Database Connections (15 controls)
- `uc_ADOConnection.cs`
- `uc_DB2Connection.cs`
- `uc_DuckDBConnection.cs`
- `uc_FirebirdConnection.cs`
- `uc_HanaConnection.cs`
- `uc_MySqlConnection.cs`
- `uc_OracleConnection.cs`
- `uc_PostgreConnection.cs`
- `uc_SqlCompactConnection.cs`
- `uc_SqliteConnection.cs`
- `uc_SqlServerConnection.cs`
- `uc_TeradataConnection.cs`
- `uc_TimeScaleConnection.cs`
- `uc_VerticaConnection.cs`
- `uc_VistaDBConnection.cs`

### NoSQL Databases (17 controls)
- `uc_ArangoDBConnection.cs`
- `uc_CassandraConnection.cs`
- `uc_ChromaDBConnection.cs`
- `uc_CockroachDBConnection.cs`
- `uc_CouchbaseConnection.cs`
- `uc_CouchDBConnection.cs`
- `uc_DynamoDBConnection.cs`
- `uc_ElasticSearchConnection.cs`
- `uc_JanusGraphConnection.cs`
- `uc_LiteDBConnection.cs`
- `uc_MilvusConnection.cs`
- `uc_MongoDBConnection.cs`
- `uc_Neo4jConnection.cs`
- `uc_OrientDBConnection.cs`
- `uc_PineConeConnection.cs`
- `uc_QdrantConnection.cs`
- `uc_RavenDBConnection.cs`
- `uc_RedisConnection.cs`
- `uc_RedisVectorConnection.cs`
- `uc_SolrConnection.cs`
- `uc_WeaviateConnection.cs`
- `uc_ZillizConnection.cs`

### Cloud Services (7 controls)
- `uc_AWSRDSConnection.cs`
- `uc_AWSRedshiftConnection.cs`
- `uc_AzureSQLConnection.cs`
- `uc_GoogleBigQueryConnection.cs`
- `uc_SnowflakeConnection.cs`
- `uc_SpannerConnection.cs`
- `uc_SupabaseConnection.cs`

### Message Queues & Streaming (12 controls)
- `uc_ActiveMQConnection.cs`
- `uc_AWSKinesisConnection.cs`
- `uc_AWSSNSConnection.cs`
- `uc_AWSSQSConnection.cs`
- `uc_AzureServiceBusConnection.cs`
- `uc_KafkaConnection.cs`
- `uc_KafkaStreamsConnection.cs`
- `uc_NatsConnection.cs`
- `uc_NATSConnection.cs`
- `uc_PulsarConnection.cs`
- `uc_PulsarFunctionsConnection.cs`
- `uc_RabbitMQConnection.cs`
- `uc_ZeroMQConnection.cs`

### Workflow & Orchestration (17 controls)
- `uc_ActivitiConnection.cs`
- `uc_ApacheAirflowConnection.cs`
- `uc_AWSGlueConnection.cs`
- `uc_AWSStepFunctionsConnection.cs`
- `uc_AWSSWFConnection.cs`
- `uc_BonitaConnection.cs`
- `uc_CamundaConnection.cs`
- `uc_ConductorConnection.cs`
- `uc_FlinkConnection.cs`
- `uc_FlowableConnection.cs`
- `uc_GoogleCloudWorkflowsConnection.cs`
- `uc_jBPMConnection.cs`
- `uc_NifiConnection.cs`
- `uc_PrefectConnection.cs`
- `uc_StreamSetsConnection.cs`
- `uc_TemporalConnection.cs`
- `uc_ZeebeConnection.cs`

### File Formats (9 controls)
- `uc_AvroConnection.cs`
- `uc_CSVConnection.cs`
- `uc_ExcelConnection.cs`
- `uc_FeatherConnection.cs`
- `uc_JSONConnection.cs`
- `uc_ORCConnection.cs`
- `uc_ParquetConnection.cs`
- `uc_PetastormConnection.cs`
- `uc_XMLConnection.cs`

### APIs & Web Services (4 controls)
- `uc_GraphQLConnection.cs`
- `uc_ODataConnection.cs`
- `uc_RESTConnection.cs`
- `uc_WebAPIConnection.cs`

### Other Connections (35 controls)
- `uc_AWSAthenaConnection.cs`
- `uc_AzureCloudConnection.cs`
- `uc_AzureLogicAppsConnection.cs`
- `uc_ClickHouseConnection.cs`
- `uc_DataBricksConnection.cs`
- `uc_DataFlowConnection.cs`
- `uc_DruidConnection.cs`
- `uc_FirebaseConnection.cs`
- `uc_FireboltConnection.cs`
- `uc_GoogleSheetsConnection.cs`
- `uc_HadoopConnection.cs`
- `uc_HologresConnection.cs`
- `uc_InfluxDBConnection.cs`
- `uc_KuduConnection.cs`
- `uc_MassTransitConnection.cs`
- `uc_MiModelConnection.cs`
- `uc_ODBCConnection.cs`
- `uc_OLEDBConnection.cs`
- `uc_OPCConnection.cs`
- `uc_PinotConnection.cs`
- `uc_PrestoConnection.cs`
- `uc_RealIMConnection.cs`
- `uc_RocketSetConnection.cs`
- `uc_ShapVectorConnection.cs`
- `uc_SparkConnection.cs`
- `uc_TeradataConnection_Backup.cs`
- `uc_TigerGraphConnection.cs`
- `uc_TrinoConnection.cs`
- `uc_VespaConnection.cs`

**Total: 117 connection controls** (excluding base class)

## Skill-Aware Guidance

- Refer to the `beep-winform-ui` skill (SKILL.md + references/controls-overview.md) for how the WinForms controls share `BaseControl`, `Styling`, `ThemeManagement`, and `FontManagement`. Treat this folder as part of that ecosystem and keep theming consistent with `BeepThemesManager`, `BeepControl.ApplyTheme`, and `BeepGlobalThemeManager`.
- When refactoring dialogues, look at how `BaseControl` defines the shared properties/methods and flow. This matches the same pattern the skill documents for `BeepPanel`/`BeepButton` combos: always apply theme updates through the theme manager before returning to the caller.
- Use the `Styling` and `FontManagement` helpers when you customize tabs (especially for icons or themed fonts) so the controls stay consistent with the rest of the library; the skill highlights the importance of calling `ApplyThemeOnImage` on `BeepImage` instances.


## Refactoring Steps

1. **Refactor DataConnectionBase**
   - Convert to dialog pattern
   - Accept ConnectionProperties input
   - Return updated/new ConnectionProperties
   - Remove ViewModel dependencies
   - Implement main dialog features (OK/Cancel, validation, etc.)

2. **Update All Inherited Controls**
   - Remove ViewModel usage
   - Create required TabPages for the connection type in Designer.cs and add to beepTabs1
   - Instantiate and configure all controls in Designer.cs (InitializeComponent)
   - Bind controls to ConnectionProperties property from base in code-behind (constructor/on-load)
   - Ensure they inherit dialog behavior from base

3. **Testing & Validation**
   - Test each control individually
   - Verify dialog behavior
   - Ensure ConnectionProperties are properly updated

## Progress Tracking
- [x] DataConnectionBase refactored
- [x] Database connections: 2/15 completed (SQL Server, MySQL)
- [ ] NoSQL connections: 0/22 completed
- [ ] Cloud services: 4/7 completed (AWS RDS, AWS Redshift, AWS Kinesis, AWS Glue)
- [ ] Message queues: 0/13 completed
- [ ] Workflow connections: 0/17 completed
- [ ] File format connections: 0/9 completed
- [ ] API connections: 0/4 completed
- [ ] Other connections: 0/35 completed
- [ ] All controls tested

**Total Progress: 2/117 controls completed (1.71%)**
