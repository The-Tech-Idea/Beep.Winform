# BeepGridPro Context Menu Usage Guide

## Overview
BeepGridPro provides a built-in context menu with row data context, allowing developers to access and update row data when context menu items are selected.

## ✨ Automatic Two-Way Data Binding

**Good News!** Changes to `RowDataObject` are **automatically reflected** in the grid if your entity implements `INotifyPropertyChanged`:

```csharp
// Your entity class
public class Customer : INotifyPropertyChanged
{
    private string _name;
    public string Name 
    { 
        get => _name;
        set 
        { 
            _name = value;
            OnPropertyChanged(nameof(Name)); // Grid updates automatically!
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// In your event handler - NO manual sync needed!
if (e.CurrentRow?.RowDataObject is Customer customer)
{
    customer.Name = "Updated Name"; // ✅ Grid cell updates automatically!
}
```

### When Manual Sync Is Needed

Only use `SyncRowDataToGrid()` if your entity **does NOT** implement `INotifyPropertyChanged`:

```csharp
// POCO class without INotifyPropertyChanged
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Manual sync required
if (e.CurrentRow?.RowDataObject is Product product)
{
    product.Name = "Updated";
    product.Price = 99.99m;
    
    // Required for POCO objects
    ((BeepGridPro)sender).SyncRowDataToGrid(e.CurrentRow);
}
```

## Event: GridContextMenuItemSelected

This event is fired when a user selects an item from the context menu, providing full context about the selected row(s) and data.

### Event Arguments: GridContextMenuEventArgs

```csharp
public class GridContextMenuEventArgs : EventArgs
{
    // The selected menu item
    public SimpleItem SelectedItem { get; }
    
    // The action/command from the menu item's Tag property
    public string? Action { get; }
    
    // The current/focused row when context menu was invoked
    public BeepRowConfig? CurrentRow { get; }
    
    // All currently selected rows (for multi-select scenarios)
    public List<BeepRowConfig> SelectedRows { get; }
    
    // The row index of the current row
    public int CurrentRowIndex { get; }
    
    // Set to true to cancel the default action
    public bool Cancel { get; set; }
    
    // Set to false to prevent automatic grid refresh
    public bool RefreshGrid { get; set; } = true;
}
```

## Usage Examples

### Example 1: Access Row Data on Context Menu Action

```csharp
// Subscribe to the event
beepGridPro1.GridContextMenuItemSelected += BeepGridPro1_GridContextMenuItemSelected;

private void BeepGridPro1_GridContextMenuItemSelected(object sender, GridContextMenuEventArgs e)
{
    // Access the current row data
    if (e.CurrentRow != null)
    {
        MessageBox.Show($"Action: {e.Action}\n" +
                       $"Row Index: {e.CurrentRowIndex}\n" +
                       $"Row Data: {e.CurrentRow.RowDataObject}");
    }
}
```

### Example 2: Update Row Data Based on Context Menu Selection

```csharp
private void BeepGridPro1_GridContextMenuItemSelected(object sender, GridContextMenuEventArgs e)
{
    // Handle specific actions
    switch (e.Action)
    {
        case "copy":
            // Access the row's underlying data object
            if (e.CurrentRow?.RowDataObject is Customer customer)
            {
                // Do something with the customer data
                Console.WriteLine($"Copying customer: {customer.Name}");
            }
            break;
            
        case "delete":
            // Confirm deletion with row details
            if (e.CurrentRow != null)
            {
                var result = MessageBox.Show(
                    $"Delete row {e.CurrentRowIndex + 1}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo);
                    
                if (result == DialogResult.No)
                {
                    e.Cancel = true; // Cancel the default delete action
                }
            }
            break;
            
        case "mark_processed":
            // Update the data object - automatic sync if INotifyPropertyChanged
            if (e.CurrentRow?.RowDataObject is Order order)
            {
                order.Status = "Processing";        // ✅ Auto-updates grid
                order.UpdatedDate = DateTime.Now;   // ✅ Auto-updates grid
                // No manual sync needed with INotifyPropertyChanged!
            }
            break;
    }
}
```

### Example 3: Batch Update Multiple Selected Rows

```csharp
private void BeepGridPro1_GridContextMenuItemSelected(object sender, GridContextMenuEventArgs e)
{
    if (e.Action == "bulk_activate" && e.SelectedRows.Count > 0)
    {
        // Confirm action
        var result = MessageBox.Show(
            $"Activate {e.SelectedRows.Count} selected products?",
            "Confirm Batch Update",
            MessageBoxButtons.YesNo);
            
        if (result == DialogResult.No)
        {
            e.Cancel = true;
            return;
        }
        
        // Update each selected row - automatic if INotifyPropertyChanged!
        foreach (var row in e.SelectedRows)
        {
            if (row.RowDataObject is Product product)
            {
                product.IsActive = true;              // ✅ Auto-updates grid
                product.ActivatedDate = DateTime.Now; // ✅ Auto-updates grid
            }
        }
        
        // No manual sync needed with INotifyPropertyChanged!
        e.RefreshGrid = true; // Optional: force full refresh
    }
}
```

### Example 4: Update Row Data Without Cancelling Default Action

```csharp
private void BeepGridPro1_GridContextMenuItemSelected(object sender, GridContextMenuEventArgs e)
{
    // Add custom logic before/after default actions
    if (e.Action == "paste" && e.CurrentRow != null)
    {
        // Get the grid reference
        var grid = (BeepGridPro)sender;
        
        // Modify the data object before paste
        if (e.CurrentRow.RowDataObject is Invoice invoice)
        {
            // Add audit trail
            invoice.LastModified = DateTime.Now;
            invoice.ModifiedBy = Environment.UserName;
            
            // Sync the audit fields back to grid
            grid.SyncRowDataToGrid(e.CurrentRow);
        }
        
        // Let default paste action continue (e.Cancel remains false)
        // The paste will update other cells, and the audit fields will remain
    }
}
```

### Example 4: Add Custom Context Menu Action

```csharp
private void BeepGridPro1_GridContextMenuItemSelected(object sender, GridContextMenuEventArgs e)
{
    // Add custom logic for specific actions
    if (e.Action == "custom_action")
    {
        // Cancel default behavior
        e.Cancel = true;
        
        var grid = (BeepGridPro)sender;
        
        // Access row data
        if (e.CurrentRow?.RowDataObject is Order order)
        {
            // Custom processing
            order.Status = "Processing";
            order.UpdatedDate = DateTime.Now;
            
            // Sync changes to grid
            grid.SyncRowDataToGrid(e.CurrentRow);
            
            // Optionally update individual cells directly
            var statusCell = e.CurrentRow.Cells
                .FirstOrDefault(c => c.ColumnName == "Status");
            if (statusCell != null)
            {
                statusCell.CellValue = "Processing";
            }
            
            // Refresh grid to show changes
            e.RefreshGrid = true;
        }
    }
}
```

### Example 5: Working with Cell Values Directly

```csharp
private void BeepGridPro1_GridContextMenuItemSelected(object sender, GridContextMenuEventArgs e)
{
    if (e.Action == "update_status" && e.CurrentRow != null)
    {
        var grid = (BeepGridPro)sender;
        
        // Option 1: Update via RowDataObject (recommended for complex changes)
        if (e.CurrentRow.RowDataObject is Product product)
        {
            product.Status = "Active";
            product.LastUpdated = DateTime.Now;
            
            // Sync all changes
            grid.SyncRowDataToGrid(e.CurrentRow);
        }
        
        // Option 2: Update individual cells directly (for simple changes)
        var statusCell = e.CurrentRow.Cells
            .FirstOrDefault(c => c.ColumnName == "Status");
        if (statusCell != null)
        {
            statusCell.CellValue = "Active";
            statusCell.IsDirty = true;
            
            // This will also update the underlying data object
            grid.Data.UpdateCellValue(statusCell, "Active");
        }
        
        e.Cancel = true; // We handled it ourselves
        e.RefreshGrid = true;
    }
}
```

### Example 5: Modify Row Data Before Default Action

```csharp
private void BeepGridPro1_GridContextMenuItemSelected(object sender, GridContextMenuEventArgs e)
{
    // Intercept paste operation
    if (e.Action == "paste" && e.CurrentRow != null)
    {
        var grid = (BeepGridPro)sender;
        
        // Validate or modify data before paste
        if (e.CurrentRow.RowDataObject is Invoice invoice)
        {
            // Add business logic
            if (invoice.Amount > 10000)
            {
                // Require approval for large amounts
                if (!UserHasApprovalRights())
                {
                    MessageBox.Show("Large amount paste requires approval.");
                    e.Cancel = true; // Cancel the paste
                    return;
                }
            }
            
            // Add audit trail
            invoice.LastModified = DateTime.Now;
            invoice.ModifiedBy = Environment.UserName;
            
            // Sync audit fields to grid
            grid.SyncRowDataToGrid(e.CurrentRow);
            
            // Log the operation
            LogOperation($"Pasting data to invoice {invoice.Id}");
        }
        
        // Let default paste action continue (e.Cancel = false)
    }
}
```

## Two-Way Data Binding

### Automatic Binding (Recommended)
When your entity implements `INotifyPropertyChanged`, the grid **automatically updates** when you modify properties:

```csharp
public class Customer : INotifyPropertyChanged
{
    private string _name;
    private string _status;
    
    public string Name 
    { 
        get => _name;
        set { _name = value; OnPropertyChanged(nameof(Name)); }
    }
    
    public string Status 
    { 
        get => _status;
        set { _status = value; OnPropertyChanged(nameof(Status)); }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

// Usage - changes automatically reflect in grid!
if (e.CurrentRow?.RowDataObject is Customer customer)
{
    customer.Name = "New Name";   // ✅ Grid updates automatically!
    customer.Status = "Active";   // ✅ Grid updates automatically!
}
```

### Grid → Data Object (Always Automatic)
When you edit cells in the grid, changes automatically flow to the underlying data object:

```csharp
// User edits "Name" cell in grid
// → cell.CellValue changes
// → GridDataHelper.UpdateCellValue() is called  
// → Automatically updates customer.Name property
```

### Data Object → Grid
- **With INotifyPropertyChanged**: ✅ **Automatic** - grid listens for PropertyChanged events
- **Without INotifyPropertyChanged**: ⚠️ **Manual sync required** using `SyncRowDataToGrid()`

### Manual Sync (For POCO Objects)

Only needed for Plain Old CLR Objects (POCOs) without `INotifyPropertyChanged`:

```csharp
// POCO without INotifyPropertyChanged
public class Product 
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Manual sync required
var grid = (BeepGridPro)sender;
if (e.CurrentRow?.RowDataObject is Product product)
{
    product.Name = "New Name";
    product.Price = 99.99m;
    
    // Required for POCO objects
    grid.SyncRowDataToGrid(e.CurrentRow);
}

// Batch sync for multiple POCO objects
foreach (var row in e.SelectedRows)
{
    if (row.RowDataObject is Product p)
    {
        p.Price *= 1.1m;
    }
}
grid.SyncRowDataToGrid(e.SelectedRows);
```

## Built-in Context Menu Actions

The following actions are available by default:

- `copy` - Copy selected cells to clipboard
- `copy_headers` - Copy with column headers
- `cut` - Cut selected cells (if not ReadOnly)
- `paste` - Paste from clipboard (if not ReadOnly)
- `select_all` - Select all rows
- `clear_selection` - Clear row selection
- `insert` - Insert new row (if not ReadOnly)
- `delete` - Delete selected rows (if not ReadOnly)
- `autosize` - Auto-size all columns
- `reset_order` - Reset column display order
- `export_excel` - Export to Excel (placeholder)
- `export_csv` - Export to CSV (placeholder)

## Access Row Data Object

Each `BeepRowConfig` has a `RowDataObject` property that contains the underlying data entity:

```csharp
// Cast to your entity type
if (e.CurrentRow?.RowDataObject is Customer customer)
{
    // Access all customer properties
    var name = customer.Name;
    var email = customer.Email;
    
    // Modify the entity
    customer.LastModified = DateTime.Now;
}
```

## Access Cell Values

```csharp
// Get specific cell by column name
var emailCell = e.CurrentRow.Cells
    .FirstOrDefault(c => c.ColumnName == "Email");
    
if (emailCell != null)
{
    var email = emailCell.CellValue?.ToString();
    
    // Update cell value
    emailCell.CellValue = "newemail@example.com";
}
```

## Tips

1. **Cancel Default Actions**: Set `e.Cancel = true` to prevent the built-in action from executing
2. **Control Refresh**: Set `e.RefreshGrid = false` if you're handling the refresh yourself
3. **Multi-Select**: Check `e.SelectedRows.Count` to see if multiple rows are selected
4. **Null Checks**: Always check if `CurrentRow` is null before accessing it
5. **Type Safety**: Use pattern matching or `is` operator to safely cast `RowDataObject`

## Complete Example

```csharp
public class MyForm : Form
{
    private BeepGridPro beepGridPro1;
    
    private void InitializeGrid()
    {
        beepGridPro1 = new BeepGridPro();
        beepGridPro1.GridContextMenuItemSelected += OnGridContextMenu;
        beepGridPro1.DataSource = GetCustomers();
    }
    
    private void OnGridContextMenu(object sender, GridContextMenuEventArgs e)
    {
        // Log the action
        Console.WriteLine($"Context menu action: {e.Action}, Row: {e.CurrentRowIndex}");
        
        // Handle specific actions
        switch (e.Action)
        {
            case "delete":
                if (!ConfirmDelete(e.SelectedRows.Count))
                {
                    e.Cancel = true;
                }
                break;
                
            case "copy":
                // Add custom copy logic
                if (e.CurrentRow?.RowDataObject is Customer customer)
                {
                    Clipboard.SetText($"{customer.Name} - {customer.Email}");
                    e.Cancel = true; // Use custom copy instead of default
                }
                break;
        }
    }
    
    private bool ConfirmDelete(int count)
    {
        var message = count > 1 
            ? $"Delete {count} rows?" 
            : "Delete this row?";
        return MessageBox.Show(message, "Confirm", 
            MessageBoxButtons.YesNo) == DialogResult.Yes;
    }
}
```
