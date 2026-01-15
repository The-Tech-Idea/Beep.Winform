# 🔄 CASCADE & COORDINATION DESIGN
## Perfect Master-Detail Coordination for BeepDataBlock

**Date**: December 3, 2025  
**Status**: Design Complete - Ready for Implementation

---

## 🎯 **OVERVIEW**

The Cascade & Coordination System provides **perfect Oracle Forms master-detail behavior** with:
- ✅ **Automatic detail synchronization** when master changes
- ✅ **Cascade delete** with user prompts
- ✅ **Coordinated commit** (master → details)
- ✅ **Coordinated rollback** (details → master)
- ✅ **Query coordination** (master query → detail auto-query)
- ✅ **Unsaved changes coordination** across all blocks
- ✅ **Lock coordination** for multi-user scenarios

---

## 🎨 **COORDINATION SCENARIOS**

### **Scenario 1: Master Record Navigation** ⭐⭐⭐⭐⭐

**Oracle Forms Behavior:**
```
User clicks "Next Record" on CUSTOMERS block
↓
Orders for new customer automatically displayed
↓
Order Items for first order automatically displayed
```

**Implementation:**

```csharp
public partial class BeepDataBlock
{
    public async Task<bool> NextRecord()
    {
        // 1. Check for unsaved changes (this block + all children)
        if (!await CheckAndHandleUnsavedChangesRecursiveAsync())
            return false;
            
        // 2. Navigate to next record
        var success = Data.Units.MoveNext();
        if (!success)
            return false;
            
        // 3. Update system variables
        CurrentRecord++;
        UpdateSystemVariables();
        
        // 4. Fire POST-RECORD-NAVIGATE trigger
        var context = new TriggerContext
        {
            Block = this,
            TriggerType = TriggerType.PostRecordNavigate
        };
        await ExecuteTriggers(TriggerType.PostRecordNavigate, context);
        
        // 5. AUTOMATIC COORDINATION: Query all detail blocks
        await CoordinateDetailBlocks();
        
        return true;
    }
    
    /// <summary>
    /// Oracle Forms-style automatic detail coordination
    /// </summary>
    private async Task CoordinateDetailBlocks()
    {
        if (!IsMasterBlock || ChildBlocks.Count == 0)
            return;
            
        var currentRecord = Data?.Units?.Current;
        
        if (currentRecord == null)
        {
            // Master has no current record → Clear all details
            foreach (var child in ChildBlocks)
            {
                await child.ClearBlockAsync();
            }
            return;
        }
        
        // For each detail block:
        // 1. Set master record
        // 2. Build filter based on relationship
        // 3. Execute query automatically
        foreach (var child in ChildBlocks)
        {
            if (child is BeepDataBlock childBlock)
            {
                // Set master record
                childBlock.SetMasterRecord(currentRecord);
                
                // Build filter from relationship
                var filters = BuildDetailFilters(childBlock, currentRecord);
                
                // Execute query (Oracle Forms automatic behavior)
                await childBlock.ExecuteQueryWithFilters(filters);
                
                // Fire POST-QUERY trigger on detail
                await childBlock.FirePostQuery();
                
                // If detail has children, cascade further
                if (childBlock.ChildBlocks.Count > 0)
                {
                    await childBlock.CoordinateDetailBlocks();
                }
            }
        }
    }
    
    private List<AppFilter> BuildDetailFilters(BeepDataBlock detailBlock, object masterRecord)
    {
        var filters = new List<AppFilter>();
        
        foreach (var relationship in detailBlock.Relationships)
        {
            var masterValue = EntityHelper.GetPropertyValue(
                masterRecord, 
                relationship.RelatedEntityColumnID);
                
            if (masterValue != null)
            {
                filters.Add(new AppFilter
                {
                   FieldName = relationship.EntityColumnID,
                    Operator = "=",
                    FilterValue = masterValue.ToString()
                });
            }
        }
        
        return filters;
    }
}
```

---

### **Scenario 2: Cascade Delete** ⭐⭐⭐⭐⭐

**Oracle Forms Behavior:**
```
User deletes CUSTOMER record
↓
System checks for related ORDERS
↓
Prompt: "Delete related orders?"
↓
If YES: Delete orders → Delete order items (cascade)
If NO: Cancel customer delete
```

**Implementation:**

```csharp
public partial class BeepDataBlock
{
    public async Task<bool> CascadeDelete()
    {
        // 1. Check if this is a master block with details
        if (!IsMasterBlock || ChildBlocks.Count == 0)
        {
            // Simple delete
            await DeleteCurrentRecordAsync();
            return true;
        }
        
        // 2. Check if any detail blocks have records
        var detailRecordCounts = new Dictionary<string, int>();
        var totalDetailRecords = 0;
        
        foreach (var child in ChildBlocks)
        {
            var count = child.Data?.Units?.Count ?? 0;
            if (count > 0)
            {
                detailRecordCounts[child.Name] = count;
                totalDetailRecords += count;
            }
        }
        
        if (totalDetailRecords == 0)
        {
            // No detail records → Simple delete
            await DeleteCurrentRecordAsync();
            return true;
        }
        
        // 3. Build detail summary for prompt
        var detailSummary = string.Join("\n", 
            detailRecordCounts.Select(kvp => $"  • {kvp.Key}: {kvp.Value} record(s)"));
            
        // 4. Prompt user
        var message = $"This record has {totalDetailRecords} related detail record(s):\n\n" +
                     $"{detailSummary}\n\n" +
                     $"Delete all related records?";
                     
        var result = MessageBox.Show(
            message,
            "Cascade Delete Confirmation",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning);
            
        if (result == DialogResult.Cancel)
            return false;
            
        if (result == DialogResult.No)
        {
            MessageBox.Show(
                "Cannot delete master record with existing detail records.",
                "Delete Cancelled",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return false;
        }
        
        // 5. User chose YES → Cascade delete
        try
        {
            // Fire PRE-DELETE trigger (can still cancel)
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PreDelete,
                Parameters = new Dictionary<string, object>
                {
                    ["IsCascade"] = true,
                    ["DetailRecordCount"] = totalDetailRecords
                }
            };
            
            if (!await ExecuteTriggers(TriggerType.PreDelete, context))
                return false;
                
            // 6. Delete details first (bottom-up)
            await CascadeDeleteChildren();
            
            // 7. Delete master record
            await DeleteCurrentRecordAsync();
            
            // 8. Fire POST-DELETE trigger
            await ExecuteTriggers(TriggerType.PostDelete, context);
            
            MessageBox.Show(
                $"Master record and {totalDetailRecords} related record(s) deleted successfully.",
                "Delete Complete",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error during cascade delete: {ex.Message}",
                "Delete Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return false;
        }
    }
    
    private async Task CascadeDeleteChildren()
    {
        // Delete in reverse order (deepest first)
        var orderedChildren = ChildBlocks.OrderByDescending(c => GetBlockDepth(c)).ToList();
        
        foreach (var child in orderedChildren)
        {
            if (child is BeepDataBlock childBlock)
            {
                // If child has children, cascade further
                if (childBlock.ChildBlocks.Count > 0)
                {
                    await childBlock.CascadeDeleteChildren();
                }
                
                // Delete all records in this child block
                await childBlock.DeleteAllRecordsAsync();
            }
        }
    }
    
    private int GetBlockDepth(IBeepDataBlock block)
    {
        int depth = 0;
        var current = block;
        while (current.ParentBlock != null)
        {
            depth++;
            current = current.ParentBlock;
        }
        return depth;
    }
    
    private async Task DeleteAllRecordsAsync()
    {
        if (Data?.Units == null)
            return;
            
        var recordsToDelete = Data.Units.ToList();
        
        foreach (var record in recordsToDelete)
        {
            await Data.DeleteAsync(record);
        }
    }
}
```

---

### **Scenario 3: Coordinated Commit** ⭐⭐⭐⭐⭐

**Oracle Forms Behavior:**
```
User clicks "Save" (COMMIT_FORM)
↓
Save master first (to get generated keys)
↓
Save details (using master keys)
↓
Save detail-details (using detail keys)
↓
All or nothing (transaction)
```

**Implementation:**

```csharp
public partial class BeepDataBlock
{
    public async Task<bool> CoordinatedCommit()
    {
        try
        {
            // 1. Fire PRE-COMMIT trigger
            var preContext = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PreBlockCommit
            };
            
            if (!await ExecuteTriggers(TriggerType.PreBlockCommit, preContext))
                return false;
                
            // 2. Validate all blocks recursively
            if (!await ValidateRecursive())
            {
                MessageBox.Show("Validation failed. Cannot commit.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            // 3. Commit in correct order: Master → Details → Detail-Details
            var commitOrder = GetCommitOrder();
            
            foreach (var block in commitOrder)
            {
                if (block.Data?.IsDirty == true)
                {
                    // Fire PRE-COMMIT for this block
                    var blockContext = new TriggerContext
                    {
                        Block = block,
                        TriggerType = TriggerType.PreBlockCommit
                    };
                    
                    if (!await block.ExecuteTriggers(TriggerType.PreBlockCommit, blockContext))
                    {
                        // Rollback all previous commits
                        await RollbackCommittedBlocks(commitOrder, block);
                        return false;
                    }
                    
                    // Commit this block
                    await block.Data.Commit();
                    
                    // Fire POST-COMMIT for this block
                    await block.ExecuteTriggers(TriggerType.PostBlockCommit, blockContext);
                }
            }
            
            // 4. Fire POST-COMMIT trigger on master
            var postContext = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PostBlockCommit
            };
            await ExecuteTriggers(TriggerType.PostBlockCommit, postContext);
            
            MessageBox.Show("All changes saved successfully.", "Commit Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            return true;
        }
        catch (Exception ex)
        {
            // Rollback everything
            await CoordinatedRollback();
            
            MessageBox.Show($"Error during commit: {ex.Message}\n\nAll changes rolled back.",
                "Commit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            return false;
        }
    }
    
    private List<BeepDataBlock> GetCommitOrder()
    {
        var ordered = new List<BeepDataBlock>();
        
        // Add self first (master)
        ordered.Add(this);
        
        // Add children in depth order
        foreach (var child in ChildBlocks)
        {
            if (child is BeepDataBlock childBlock)
            {
                ordered.AddRange(childBlock.GetCommitOrder());
            }
        }
        
        return ordered;
    }
    
    private async Task RollbackCommittedBlocks(List<BeepDataBlock> blocks, BeepDataBlock failedBlock)
    {
        // Rollback all blocks that were committed before the failure
        var index = blocks.IndexOf(failedBlock);
        
        for (int i = 0; i < index; i++)
        {
            if (blocks[i].Data != null)
            {
                await blocks[i].Data.Rollback();
            }
        }
    }
    
    private async Task<bool> ValidateRecursive()
    {
        // Validate this block
        if (!await ValidateRecord())
            return false;
            
        // Validate all child blocks
        foreach (var child in ChildBlocks)
        {
            if (child is BeepDataBlock childBlock)
            {
                if (!await childBlock.ValidateRecursive())
                    return false;
            }
        }
        
        return true;
    }
}
```

---

### **Scenario 4: Coordinated Rollback** ⭐⭐⭐⭐

**Oracle Forms Behavior:**
```
User clicks "Cancel" (ROLLBACK_FORM)
↓
Rollback details first (deepest first)
↓
Rollback master last
↓
Restore original values
```

**Implementation:**

```csharp
public partial class BeepDataBlock
{
    public async Task<bool> CoordinatedRollback()
    {
        try
        {
            // 1. Fire PRE-ROLLBACK trigger
            var preContext = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PreBlockRollback
            };
            
            await ExecuteTriggers(TriggerType.PreBlockRollback, preContext);
            
            // 2. Rollback in reverse order: Detail-Details → Details → Master
            var rollbackOrder = GetRollbackOrder();
            
            foreach (var block in rollbackOrder)
            {
                if (block.Data?.IsDirty == true)
                {
                    await block.Data.Rollback();
                    
                    // Fire POST-ROLLBACK for this block
                    var blockContext = new TriggerContext
                    {
                        Block = block,
                        TriggerType = TriggerType.PostBlockRollback
                    };
                    await block.ExecuteTriggers(TriggerType.PostBlockRollback, blockContext);
                }
            }
            
            // 3. Fire POST-ROLLBACK trigger on master
            var postContext = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PostBlockRollback
            };
            await ExecuteTriggers(TriggerType.PostBlockRollback, postContext);
            
            // 4. Refresh display
            await RefreshAllBlocks();
            
            MessageBox.Show("All changes cancelled.", "Rollback Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during rollback: {ex.Message}",
                "Rollback Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
    
    private List<BeepDataBlock> GetRollbackOrder()
    {
        var ordered = new List<BeepDataBlock>();
        
        // Add children first (depth-first, reverse)
        foreach (var child in ChildBlocks.Reverse())
        {
            if (child is BeepDataBlock childBlock)
            {
                ordered.AddRange(childBlock.GetRollbackOrder());
            }
        }
        
        // Add self last (master)
        ordered.Add(this);
        
        return ordered;
    }
    
    private async Task RefreshAllBlocks()
    {
        // Refresh this block
        Data?.Units?.ResetBindings();
        
        // Refresh all children
        foreach (var child in ChildBlocks)
        {
            if (child is BeepDataBlock childBlock)
            {
                await childBlock.RefreshAllBlocks();
            }
        }
    }
}
```

---

### **Scenario 5: Query Coordination** ⭐⭐⭐⭐⭐

**Oracle Forms Behavior:**
```
User enters query criteria in CUSTOMERS and ORDERS blocks
↓
User clicks "Execute Query"
↓
Query CUSTOMERS with criteria
↓
For each customer: Query ORDERS with criteria + CustomerID filter
↓
For each order: Query ORDER_ITEMS
```

**Implementation:**

```csharp
public partial class BeepDataBlock
{
    public async Task EnterQueryModeCoordinated()
    {
        // 1. Enter query mode for this block
        await SwitchBlockModeAsync(DataBlockMode.Query);
        
        // 2. Clear all fields
        foreach (var component in UIComponents.Values)
        {
            component.ClearValue();
        }
        
        // 3. Enter query mode for all child blocks
        foreach (var child in ChildBlocks)
        {
            if (child is BeepDataBlock childBlock)
            {
                await childBlock.EnterQueryModeCoordinated();
            }
        }
        
        // 4. Fire WHEN-CLEAR-BLOCK trigger
        await FireWhenClearBlock();
    }
    
    public async Task ExecuteQueryCoordinated()
    {
        // 1. Collect query criteria from this block
        var masterFilters = GetQueryFiltersFromControls();
        
        // 2. Fire PRE-QUERY trigger
        var preContext = new TriggerContext
        {
            Block = this,
            TriggerType = TriggerType.PreQuery,
            Parameters = new Dictionary<string, object> { ["Filters"] = masterFilters }
        };
        
        if (!await ExecuteTriggers(TriggerType.PreQuery, preContext))
            return;
            
        // 3. Execute query on master
        await ExecuteQueryWithFilters(masterFilters);
        
        // 4. Fire POST-QUERY trigger on master
        await FirePostQuery();
        
        // 5. Update query hits
        QueryHits = Data?.Units?.Count ?? 0;
        
        // 6. If master has results, query details
        if (QueryHits > 0)
        {
            // Move to first record
            Data.Units.MoveFirst();
            CurrentRecord = 1;
            
            // Query details with coordination
            await QueryDetailsCoordinated(masterFilters);
        }
        else
        {
            // No master results → Clear all details
            foreach (var child in ChildBlocks)
            {
                await child.ClearBlockAsync();
            }
        }
        
        // 7. Switch to CRUD mode
        await SwitchBlockModeAsync(DataBlockMode.CRUD);
        
        // 8. Show query results
        MessageBox.Show(
            $"Query returned {QueryHits} record(s).",
            "Query Complete",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
    
    private async Task QueryDetailsCoordinated(List<AppFilter> masterFilters)
    {
        var currentMasterRecord = Data?.Units?.Current;
        if (currentMasterRecord == null)
            return;
            
        foreach (var child in ChildBlocks)
        {
            if (child is BeepDataBlock childBlock)
            {
                // 1. Collect query criteria from detail block
                var detailFilters = childBlock.GetQueryFiltersFromControls();
                
                // 2. Add master-detail relationship filters
                var relationshipFilters = BuildDetailFilters(childBlock, currentMasterRecord);
                detailFilters.AddRange(relationshipFilters);
                
                // 3. Execute query on detail
                await childBlock.ExecuteQueryWithFilters(detailFilters);
                
                // 4. Fire POST-QUERY on detail
                await childBlock.FirePostQuery();
                
                // 5. If detail has results and has children, cascade further
                if (childBlock.QueryHits > 0 && childBlock.ChildBlocks.Count > 0)
                {
                    childBlock.Data.Units.MoveFirst();
                    await childBlock.QueryDetailsCoordinated(detailFilters);
                }
            }
        }
    }
    
    private async Task ExecuteQueryWithFilters(List<AppFilter> filters)
    {
        if (Data == null)
            return;
            
        // Use UnitofWork to execute query
        var getMethod = Data.GetType().GetMethod("Get", new[] { typeof(List<AppFilter>) });
        if (getMethod != null)
        {
            var task = (Task)getMethod.Invoke(Data, new object[] { filters });
            await task;
        }
    }
}
```

---

### **Scenario 6: Unsaved Changes Coordination** ⭐⭐⭐⭐⭐

**Oracle Forms Behavior:**
```
User has unsaved changes in CUSTOMERS and ORDERS
↓
User clicks "Next Record"
↓
Prompt: "You have unsaved changes. Save/Discard/Cancel?"
↓
If SAVE: Save all blocks (master + details)
If DISCARD: Rollback all blocks
If CANCEL: Stay on current record
```

**Implementation:**

```csharp
public partial class BeepDataBlock
{
    public async Task<bool> CheckAndHandleUnsavedChangesRecursiveAsync()
    {
        // 1. Collect all dirty blocks (self + children recursively)
        var dirtyBlocks = new List<BeepDataBlock>();
        CollectDirtyBlocks(this, dirtyBlocks);
        
        if (dirtyBlocks.Count == 0)
            return true;  // No unsaved changes
            
        // 2. Build summary of changes
        var changesSummary = BuildChangesSummary(dirtyBlocks);
        
        // 3. Prompt user
        var message = $"You have unsaved changes:\n\n{changesSummary}\n\n" +
                     $"What would you like to do?";
                     
        var result = MessageBox.Show(
            message,
            "Unsaved Changes",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button1);
            
        // 4. Handle user choice
        switch (result)
        {
            case DialogResult.Yes:  // Save
                return await CoordinatedCommit();
                
            case DialogResult.No:   // Discard
                await CoordinatedRollback();
                return true;
                
            case DialogResult.Cancel:  // Cancel operation
                return false;
                
            default:
                return false;
        }
    }
    
    private void CollectDirtyBlocks(BeepDataBlock block, List<BeepDataBlock> dirtyBlocks)
    {
        if (block.Data?.IsDirty == true)
            dirtyBlocks.Add(block);
            
        foreach (var child in block.ChildBlocks)
        {
            if (child is BeepDataBlock childBlock)
                CollectDirtyBlocks(childBlock, dirtyBlocks);
        }
    }
    
    private string BuildChangesSummary(List<BeepDataBlock> dirtyBlocks)
    {
        var summary = new List<string>();
        
        foreach (var block in dirtyBlocks)
        {
            var changes = block.Data.Units.GetPendingChanges();
            
            var blockSummary = $"• {block.Name}:";
            if (changes.Added.Count > 0)
                blockSummary += $" {changes.Added.Count} new";
            if (changes.Modified.Count > 0)
                blockSummary += $" {changes.Modified.Count} modified";
            if (changes.Deleted.Count > 0)
                blockSummary += $" {changes.Deleted.Count} deleted";
                
            summary.Add(blockSummary);
        }
        
        return string.Join("\n", summary);
    }
}
```

---

### **Scenario 7: Lock Coordination** ⭐⭐⭐

**Oracle Forms Behavior:**
```
User edits record
↓
Record locked automatically
↓
Detail records locked
↓
Other users see "Record locked by User1"
```

**Implementation:**

```csharp
public partial class BeepDataBlock
{
    private Dictionary<object, RecordLock> _recordLocks = new();
    
    public async Task<bool> LockCurrentRecord()
    {
        var currentRecord = Data?.Units?.Current;
        if (currentRecord == null)
            return false;
            
        // 1. Check if already locked
        if (_recordLocks.ContainsKey(currentRecord))
            return true;  // Already locked by us
            
        // 2. Try to acquire lock from database
        var lockResult = await AcquireDatabaseLock(currentRecord);
        
        if (!lockResult.Success)
        {
            MessageBox.Show(
                $"Record is locked by {lockResult.LockedBy} since {lockResult.LockTime}.",
                "Record Locked",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }
        
        // 3. Store lock information
        _recordLocks[currentRecord] = new RecordLock
        {
            Record = currentRecord,
            LockedBy = Environment.UserName,
            LockTime = DateTime.Now
        };
        
        // 4. Fire ON-LOCK trigger
        var context = new TriggerContext
        {
            Block = this,
            TriggerType = TriggerType.OnLock
        };
        await ExecuteTriggers(TriggerType.OnLock, context);
        
        // 5. Lock related detail records
        await LockDetailRecords();
        
        return true;
    }
    
    private async Task LockDetailRecords()
    {
        foreach (var child in ChildBlocks)
        {
            if (child is BeepDataBlock childBlock)
            {
                // Lock all detail records for current master
                foreach (var detailRecord in childBlock.Data.Units)
                {
                    await childBlock.LockRecord(detailRecord);
                }
            }
        }
    }
    
    public async Task UnlockCurrentRecord()
    {
        var currentRecord = Data?.Units?.Current;
        if (currentRecord == null || !_recordLocks.ContainsKey(currentRecord))
            return;
            
        // Release database lock
        await ReleaseDatabaseLock(currentRecord);
        
        // Remove from local locks
        _recordLocks.Remove(currentRecord);
        
        // Unlock detail records
        await UnlockDetailRecords();
    }
    
    private async Task<LockResult> AcquireDatabaseLock(object record)
    {
        // Implementation depends on database system
        // For SQL Server: Use UPDATE with ROWLOCK hint
        // For Oracle: Use SELECT FOR UPDATE
        // For others: Use optimistic locking with version field
        
        return new LockResult { Success = true };
    }
}

public class RecordLock
{
    public object Record { get; set; }
    public string LockedBy { get; set; }
    public DateTime LockTime { get; set; }
}

public class LockResult
{
    public bool Success { get; set; }
    public string LockedBy { get; set; }
    public DateTime? LockTime { get; set; }
}
```

---

## 🎨 **COORDINATION HELPERS**

### **CoordinationHelper.cs**

```csharp
public static class CoordinationHelper
{
    /// <summary>
    /// Get all blocks in hierarchy (master + all descendants)
    /// </summary>
    public static List<BeepDataBlock> GetAllBlocksInHierarchy(BeepDataBlock masterBlock)
    {
        var blocks = new List<BeepDataBlock> { masterBlock };
        
        foreach (var child in masterBlock.ChildBlocks)
        {
            if (child is BeepDataBlock childBlock)
            {
                blocks.AddRange(GetAllBlocksInHierarchy(childBlock));
            }
        }
        
        return blocks;
    }
    
    /// <summary>
    /// Get block depth in hierarchy (0 = master, 1 = detail, 2 = detail-detail, etc.)
    /// </summary>
    public static int GetBlockDepth(BeepDataBlock block)
    {
        int depth = 0;
        var current = block;
        
        while (current.ParentBlock != null)
        {
            depth++;
            current = current.ParentBlock as BeepDataBlock;
        }
        
        return depth;
    }
    
    /// <summary>
    /// Get master block (root of hierarchy)
    /// </summary>
    public static BeepDataBlock GetMasterBlock(BeepDataBlock block)
    {
        var current = block;
        
        while (current.ParentBlock != null)
        {
            current = current.ParentBlock as BeepDataBlock;
        }
        
        return current;
    }
    
    /// <summary>
    /// Check if block is ancestor of another block
    /// </summary>
    public static bool IsAncestorOf(BeepDataBlock ancestor, BeepDataBlock descendant)
    {
        var current = descendant.ParentBlock as BeepDataBlock;
        
        while (current != null)
        {
            if (current == ancestor)
                return true;
            current = current.ParentBlock as BeepDataBlock;
        }
        
        return false;
    }
}
```

---

## 🎯 **COORDINATION RULES**

### **Rule 1: Navigation Coordination**
When master navigates → Details auto-query

### **Rule 2: Commit Coordination**
Commit order: Master → Details → Detail-Details (top-down)

### **Rule 3: Rollback Coordination**
Rollback order: Detail-Details → Details → Master (bottom-up)

### **Rule 4: Delete Coordination**
Delete order: Detail-Details → Details → Master (bottom-up)

### **Rule 5: Query Coordination**
Query order: Master first, then details based on master results

### **Rule 6: Lock Coordination**
Lock order: Master → Details → Detail-Details (top-down)

### **Rule 7: Unsaved Changes Coordination**
Check ALL blocks (master + all descendants) before any navigation

---

## 🏆 **BENEFITS**

### **For Developers:**
- ✅ **Automatic coordination** - No manual detail refresh!
- ✅ **Data integrity** - Cascade operations maintain integrity
- ✅ **Less code** - Coordination is built-in
- ✅ **Oracle Forms compatible** - Same behavior

### **For Users:**
- ✅ **Intuitive** - Works like Oracle Forms
- ✅ **Safe** - Prompts for unsaved changes
- ✅ **Consistent** - Predictable behavior
- ✅ **Informative** - Clear messages

### **For Applications:**
- ✅ **Reliable** - Comprehensive error handling
- ✅ **Maintainable** - Clear coordination rules
- ✅ **Scalable** - Works with any depth hierarchy
- ✅ **Performant** - Optimized queries

---

## 📊 **IMPLEMENTATION CHECKLIST**

### **Files to Create:**
- [ ] `BeepDataBlock.Coordination.cs` - Coordination methods
- [ ] `Helpers/CoordinationHelper.cs` - Coordination utilities
- [ ] `Models/RecordLock.cs` - Lock management models
- [ ] `Models/CoordinationRule.cs` - Coordination rules

### **Methods to Implement:**
- [ ] `CoordinateDetailBlocks()` - Auto-query details
- [ ] `CascadeDelete()` - Cascade delete with prompts
- [ ] `CoordinatedCommit()` - Top-down commit
- [ ] `CoordinatedRollback()` - Bottom-up rollback
- [ ] `EnterQueryModeCoordinated()` - Coordinated query mode
- [ ] `ExecuteQueryCoordinated()` - Coordinated query execution
- [ ] `CheckAndHandleUnsavedChangesRecursiveAsync()` - Recursive dirty check
- [ ] `LockCurrentRecord()` - Lock coordination
- [ ] `UnlockCurrentRecord()` - Unlock coordination

### **Integration:**
- [ ] Integrate with existing navigation methods
- [ ] Integrate with UnitofWork operations
- [ ] Integrate with FormsManager
- [ ] Integrate with trigger system

---

## 🚀 **ESTIMATED EFFORT**

**Total**: 3 days

**Day 1**: Coordination models and helpers  
**Day 2**: Coordination methods (commit, rollback, delete)  
**Day 3**: Query coordination and lock coordination  

---

## 🏆 **SUCCESS METRICS**

- ✅ Automatic detail synchronization on master navigation
- ✅ Cascade delete with user prompts
- ✅ Coordinated commit (master → details)
- ✅ Coordinated rollback (details → master)
- ✅ Coordinated query with criteria
- ✅ Recursive unsaved changes detection
- ✅ Lock coordination for multi-user

**Perfect Oracle Forms master-detail coordination!** 🔄

---

**This is the HEART of Oracle Forms compatibility!** 🏛️

