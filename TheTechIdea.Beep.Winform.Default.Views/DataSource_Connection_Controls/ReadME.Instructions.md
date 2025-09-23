
# Connection Controls Architecture

## Overview
All connection controls in this directory follow a standardized dialog-based architecture. The base control `uc_DataConnectionBase` provides the main dialog functionality, while specific connection controls inherit from it and add their own tabs.

## Architecture Principles

### 1. Dialog Pattern
- All controls are treated as dialogs that accept a `ConnectionProperties` object as input
- Dialogs return updated or new `ConnectionProperties` objects
- Dialog behavior includes OK/Cancel buttons, validation, and proper dialog result handling

### 2. ViewModel Removal
- No ViewModel usage in any connection controls
- Direct binding to `ConnectionProperties` object
- Simplified data flow without intermediate ViewModel layer

### 3. Parameter Storage (CRITICAL)
- Use `ConnectionProperties.ParameterList` (Dictionary<string,string>) for ALL provider-specific and optional parameters.
- Do NOT use the `ConnectionProperties.Parameters` string field for key/value storage. It’s deprecated for UI binding.
- When a required provider parameter is missing, initialize it in `ParameterList` with a sensible default before binding.
    - Example: `if (!ConnectionProperties.ParameterList.ContainsKey("ENGINE_TYPE")) ConnectionProperties.ParameterList["ENGINE_TYPE"] = "PostgreSQL";`

Common mappings (examples):
- SSL: `SslMode`, `SslCertificate`, `SslKey`, `SslRootCertificate`, `SslCrl`, `TrustServerCertificate`
- Pooling/Advanced: `Pooling`, `MinPoolSize`, `MaxPoolSize`, `ConnectionTimeout`, `CommandTimeout`, `ConnectionLifetime`, `Keepalive`, `ApplicationName`, `SearchPath`
- Cloud-specific (e.g., AWS RDS): `ENGINE_TYPE`, `ACCESS_KEY`, `SECRET_KEY`, `AWS_ENDPOINT`, `ACCOUNT_ID`, `ROLE_ARN`, `PROFILE`, `SSL_ENABLED`, `IGNORE_SSL_ERRORS`, `VALIDATE_CERTIFICATE`, `CERTIFICATE_PATH`, `CERTIFICATE_PASSWORD`, `CONNECTION_POOLING`, `ENCRYPT`

### 3. Base Control Features
`uc_DataConnectionBase` contains all main dialog features:
- Dialog initialization with `ConnectionProperties` input
- OK/Cancel button handling
- Form validation
- Connection testing functionality
- Tab management infrastructure
- Event handling for dialog results

### 4. Inherited Control Responsibilities
 All inherited controls (uc_*Connection.cs):
- Define and create their TabPages and controls in Designer.cs (InitializeComponent)
- Add the TabPages to `beepTabs1` in Designer.cs
- Bind their controls to the `ConnectionProperties` property from the base in code-behind
    - Core, common fields (e.g., Host, Port, Database, UserID, Password) bind to the corresponding top-level properties on `ConnectionProperties`
    - Provider-specific fields MUST bind to entries in `ConnectionProperties.ParameterList`
    - Do not bind to `ConnectionProperties.Parameters` (string)
- Handle connection-type-specific validation if needed

## Implementation Steps

### Step 1: Refactor DataConnectionBase
1. Convert to dialog pattern accepting `ConnectionProperties` parameter
2. Implement dialog result handling (OK/Cancel)
3. Add main dialog features (validation, testing, etc.)
4. Remove any ViewModel dependencies
5. Ensure proper inheritance support for tab addition

### Step 2: Update Inherited Controls
For each specific connection control:
1. Remove ViewModel usage
2. Create connection-specific TabPages in Designer.cs and add them to `beepTabs1`
3. Instantiate and configure the required controls in Designer.cs
4. Bind all controls to `base.ConnectionProperties` in code-behind
     - Initialize missing keys in `ParameterList` first, then bind
     - Example (binding pattern):
         - Defaults
             - `if (!ConnectionProperties.ParameterList.ContainsKey("CONNECTION_TIMEOUT")) ConnectionProperties.ParameterList["CONNECTION_TIMEOUT"] = "30";`
         - Binding
             - `connectionTimeoutTextBox.DataBindings.Add("Text", ConnectionProperties.ParameterList, "CONNECTION_TIMEOUT", true, DataSourceUpdateMode.OnPropertyChanged);`
5. Ensure proper dialog behavior inheritance

### Step 3: Testing
- Test each control as a dialog
- Verify `ConnectionProperties` input/output
- Ensure tab-specific functionality works
- Validate connection testing features

## Key Properties and Methods

### DataConnectionBase Properties
- `ConnectionProperties ConnectionProperties { get; set; }` - The main data object
- `DialogResult DialogResult { get; set; }` - Dialog result handling
  
### ConnectionProperties Fields (for binding)
- Top-level common fields: `Host`, `Port`, `Database`, `UserID`, `Password`, etc.
- Provider parameters: `ParameterList` (Dictionary<string,string>) — use this for all key/value pairs
- Deprecated for binding: `Parameters` (string)

### DataConnectionBase Methods
- `void InitializeDialog(ConnectionProperties props)` - Initialize with connection data
- `ConnectionProperties GetUpdatedProperties()` - Get updated connection properties
- `void AddTab(TabPage tab)` - Add connection-specific tabs
- `bool ValidateConnection()` - Validate connection parameters

## Usage Example

```csharp
// Create and show connection dialog
var connectionDialog = new uc_SqlServerConnection();
connectionDialog.InitializeDialog(existingConnectionProperties);

if (connectionDialog.ShowDialog() == DialogResult.OK)
{
    var updatedProperties = connectionDialog.GetUpdatedProperties();
    // Use updated properties
}
```

## File Structure
- `uc_DataConnectionBase.cs` - Base dialog control
- `uc_*Connection.cs` - Specific connection type controls
- `plan.instructions.md` - Detailed refactoring plan with all controls listed
- `ReadME.Instructions.md` - This file with architecture overview

## Notes
- All controls inherit from `uc_DataConnectionBase`
- ConnectionProperties class from DataManagementModels is used as the data model
- No modifications needed to DataManagementEngine or DataManagementModels projects
- Focus on UI binding and dialog behavior only
- Use `ParameterList` everywhere for non-core parameters; never bind to the `Parameters` string