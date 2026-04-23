# Phase 11: Export Engine

**Priority:** P2 | **Track:** Feature Additions | **Status:** Pending

## Objective

Add export capabilities to BeepGridPro supporting Excel (xlsx), CSV, PDF, and HTML formats.

## Implementation Steps

### Step 1: Create Interface

```csharp
// Export/IGridExporter.cs
public interface IGridExporter
{
    string FormatName { get; }
    void Export(BeepGridPro grid, Stream output, ExportOptions options);
    Task ExportAsync(BeepGridPro grid, Stream output, ExportOptions options);
}
```

### Step 2: Create ExportOptions Model

```csharp
// Models/ExportOptions.cs
public class ExportOptions
{
    public bool IncludeHeaders { get; set; } = true;
    public bool IncludeHiddenColumns { get; set; } = false;
    public bool IncludeHiddenRows { get; set; } = false;
    public string? Title { get; set; }
    public string? FileName { get; set; }
    public Encoding Encoding { get; set; } = Encoding.UTF8;
}
```

### Step 3: Create Export Engine

```csharp
// Export/GridExportEngine.cs
public class GridExportEngine
{
    private readonly Dictionary<string, IGridExporter> _exporters = new();

    public void RegisterExporter(IGridExporter exporter);
    public void Export(BeepGridPro grid, string format, Stream output, ExportOptions options);
    public IEnumerable<string> GetSupportedFormats();
}
```

### Step 4: Implement Exporters

- `GridCsvExporter.cs` — uses built-in `StringBuilder`
- `GridExcelExporter.cs` — uses OpenXML SDK or ClosedXML (optional NuGet)
- `GridHtmlExporter.cs` — uses built-in HTML generation
- `GridPdfExporter.cs` — uses iTextSharp or QuestPDF (optional NuGet)

### Step 5: Add Public Methods to BeepGridPro

```csharp
public void ExportToCsv(Stream output, ExportOptions? options = null);
public void ExportToExcel(Stream output, ExportOptions? options = null);
public void ExportToHtml(Stream output, ExportOptions? options = null);
public void ExportToPdf(Stream output, ExportOptions? options = null);
public void ExportToFile(string filePath, ExportOptions? options = null);
```

### Step 6: Respect Grid State

- Only export visible rows (honor `IsVisible`) unless `IncludeHiddenRows = true`
- Respect column display order and visibility
- Apply current sort order to exported data

## Acceptance Criteria

- [ ] CSV export produces valid CSV
- [ ] Excel export produces valid xlsx
- [ ] HTML export produces valid HTML table
- [ ] PDF export produces valid PDF
- [ ] Hidden rows excluded by default
- [ ] Hidden columns excluded by default
- [ ] Column order matches display order
- [ ] Sort order preserved in export

## Files to Create

- `Export/IGridExporter.cs`
- `Export/GridExportEngine.cs`
- `Export/GridCsvExporter.cs`
- `Export/GridExcelExporter.cs`
- `Export/GridHtmlExporter.cs`
- `Export/GridPdfExporter.cs`
- `Models/ExportOptions.cs`

## Files to Modify

- `BeepGridPro.cs` (add public export methods)
