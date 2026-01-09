using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.UOWManager.Models;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.Integrated.DataBlocks.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Enhanced Unit of Work Operations
    /// Provides Oracle Forms-like data management using Unit of Work pattern
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Enhanced Query Operations

        /// <summary>
        /// Execute query with enhanced Unit of Work support
        /// Oracle Forms equivalent: EXECUTE_QUERY built-in
        /// </summary>
        public async Task<bool> ExecuteQueryWithUnitOfWorkAsync(
            List<AppFilter> filters = null,
            string orderByClause = null)
        {
            if (Data == null)
            {
                Status = "No Unit of Work assigned to DataBlock";
                return false;
            }

            if (!DataBlockUnitOfWorkHelper.IsValidState(Data))
            {
                Status = "Unit of Work is in invalid state";
                return false;
            }

            try
            {
                // Fire pre-query trigger
                var preQueryArgs = new UnitofWorkParams();
                OnPreQuery?.Invoke(this, preQueryArgs);
                if (preQueryArgs.Cancel)
                {
                    Status = "Query cancelled by trigger";
                    return false;
                }

                // Execute query using helper
                var success = await DataBlockUnitOfWorkHelper.ExecuteQueryAsync(
                    Data,
                    filters,
                    orderByClause);

                if (success)
                {
                    // Fire post-query trigger
                    var postQueryArgs = new UnitofWorkParams();
                    OnPostQuery?.Invoke(this, postQueryArgs);

                    // Update child blocks
                    foreach (var childBlock in ChildBlocks)
                    {
                        childBlock.SetMasterRecord(Data.Units?.Current);
                    }

                    Status = $"Query executed successfully. {GetRecordCount()} records found.";
                }
                else
                {
                    Status = "Query execution failed";
                }

                return success;
            }
            catch (Exception ex)
            {
                Status = $"Query error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[BeepDataBlock] ExecuteQueryWithUnitOfWorkAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Execute query-by-example from UI controls
        /// Oracle Forms equivalent: EXECUTE_QUERY built-in (in query mode)
        /// </summary>
        public async Task<bool> ExecuteQueryByExampleAsync()
        {
            if (!IsInQueryMode)
            {
                Status = "Not in query mode";
                return false;
            }

            // Build filters from UI controls
            var fieldValues = new Dictionary<string, object>();
            var operators = new Dictionary<string, TheTechIdea.Beep.Winform.Controls.QueryOperator>();

            foreach (var component in UIComponents.Values)
            {
                if (!string.IsNullOrEmpty(component.BoundProperty))
                {
                    var value = component.GetValue();
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        fieldValues[component.BoundProperty] = value;
                        // Get operator if set (from QueryBuilder)
                        if (_queryOperators.ContainsKey(component.BoundProperty))
                        {
                            operators[component.BoundProperty] = _queryOperators[component.BoundProperty];
                        }
                    }
                }
            }

            // Build filters using helper
            var filters = DataBlockQueryHelper.BuildQueryFilters(fieldValues, operators);

            // Validate filters
            if (!DataBlockQueryHelper.ValidateQueryFilters(filters, out string errorMessage))
            {
                Status = errorMessage;
                return false;
            }

            // Execute query using CoordinatedQuery if available, otherwise use Unit of Work helper
            bool success;
            if (IsCoordinated && FormManager != null)
            {
                success = await CoordinatedQuery(filters);
            }
            else
            {
                success = await ExecuteQueryWithUnitOfWorkAsync(filters, null);
            }

            if (success)
            {
                IsInQueryMode = false;
                await SwitchBlockModeAsync(DataBlockMode.CRUD);
            }

            return success;
        }

        #endregion

        #region Enhanced CRUD Operations

        /// <summary>
        /// Insert record with enhanced Unit of Work support
        /// Oracle Forms equivalent: CREATE_RECORD built-in
        /// </summary>
        public async Task<bool> InsertRecordWithUnitOfWorkAsync(Entity newRecord)
        {
            if (Data == null)
            {
                Status = "No Unit of Work assigned to DataBlock";
                return false;
            }

            if (newRecord == null)
            {
                Status = "Record is null";
                return false;
            }

            try
            {
                // Fire pre-insert trigger
                var preInsertArgs = new UnitofWorkParams();
                OnPreInsert?.Invoke(this, preInsertArgs);
                if (preInsertArgs.Cancel)
                {
                    Status = "Insert cancelled by trigger";
                    return false;
                }

                // Insert using helper
                var success = await DataBlockUnitOfWorkHelper.InsertRecordAsync(Data, newRecord);

                if (success)
                {
                    // Fire post-insert trigger
                    var postInsertArgs = new UnitofWorkParams();
                    OnPostInsert?.Invoke(this, postInsertArgs);

                    // Update child blocks
                    foreach (var childBlock in ChildBlocks)
                    {
                        childBlock.SetMasterRecord(Data.Units?.Current);
                    }

                    Status = "Record inserted successfully";
                }
                else
                {
                    Status = "Insert failed";
                }

                return success;
            }
            catch (Exception ex)
            {
                Status = $"Insert error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[BeepDataBlock] InsertRecordWithUnitOfWorkAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update current record with enhanced Unit of Work support
        /// Oracle Forms equivalent: UPDATE_RECORD built-in
        /// </summary>
        public async Task<bool> UpdateCurrentRecordWithUnitOfWorkAsync()
        {
            if (Data == null)
            {
                Status = "No Unit of Work assigned to DataBlock";
                return false;
            }

            var currentRecord = DataBlockUnitOfWorkHelper.GetCurrentRecord(Data);
            if (currentRecord == null)
            {
                Status = "No current record to update";
                return false;
            }

            try
            {
                // Fire pre-update trigger
                var preUpdateArgs = new UnitofWorkParams();
                OnPreUpdate?.Invoke(this, preUpdateArgs);
                if (preUpdateArgs.Cancel)
                {
                    Status = "Update cancelled by trigger";
                    return false;
                }

                // Update using helper
                var success = await DataBlockUnitOfWorkHelper.UpdateRecordAsync(Data, currentRecord);

                if (success)
                {
                    // Fire post-update trigger
                    var postUpdateArgs = new UnitofWorkParams();
                    OnPostUpdate?.Invoke(this, postUpdateArgs);

                    Status = "Record updated successfully";
                }
                else
                {
                    Status = "Update failed";
                }

                return success;
            }
            catch (Exception ex)
            {
                Status = $"Update error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[BeepDataBlock] UpdateCurrentRecordWithUnitOfWorkAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete current record with enhanced Unit of Work support
        /// Oracle Forms equivalent: DELETE_RECORD built-in
        /// </summary>
        public async Task<bool> DeleteCurrentRecordWithUnitOfWorkAsync()
        {
            if (Data == null)
            {
                Status = "No Unit of Work assigned to DataBlock";
                return false;
            }

            var currentRecord = DataBlockUnitOfWorkHelper.GetCurrentRecord(Data);
            if (currentRecord == null)
            {
                Status = "No current record to delete";
                return false;
            }

            try
            {
                // Fire pre-delete trigger
                var preDeleteArgs = new UnitofWorkParams();
                OnPreDelete?.Invoke(this, preDeleteArgs);
                if (preDeleteArgs.Cancel)
                {
                    Status = "Delete cancelled by trigger";
                    return false;
                }

                // Delete using helper
                var success = await DataBlockUnitOfWorkHelper.DeleteRecordAsync(Data, currentRecord);

                if (success)
                {
                    // Fire post-delete trigger
                    var postDeleteArgs = new UnitofWorkParams();
                    OnPostDelete?.Invoke(this, postDeleteArgs);

                    // Update child blocks
                    foreach (var childBlock in ChildBlocks)
                    {
                        childBlock.SetMasterRecord(Data.Units?.Current);
                    }

                    Status = "Record deleted successfully";
                }
                else
                {
                    Status = "Delete failed";
                }

                return success;
            }
            catch (Exception ex)
            {
                Status = $"Delete error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[BeepDataBlock] DeleteCurrentRecordWithUnitOfWorkAsync error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Enhanced Transaction Operations

        /// <summary>
        /// Commit changes with enhanced Unit of Work support
        /// Oracle Forms equivalent: COMMIT_FORM built-in
        /// </summary>
        public async Task<bool> CommitWithUnitOfWorkAsync()
        {
            if (Data == null)
            {
                Status = "No Unit of Work assigned to DataBlock";
                return false;
            }

            // Check for unsaved changes in child blocks
            if (ChildBlocks.Any(cb => DataBlockUnitOfWorkHelper.HasUncommittedChanges(cb.Data)))
            {
                Status = "Child blocks have uncommitted changes";
                return false;
            }

            try
            {
                // Note: PreCommit and PostCommit are handled by UnitOfWork events, not DataBlock events
                // The UnitOfWork's PreCommit and PostCommit events are already subscribed in SubscribeEvents()

                // Commit using helper
                var result = await DataBlockUnitOfWorkHelper.CommitAsync(Data);

                if (result.Flag == Errors.Ok)
                {
                    // PostCommit event is already handled by HandleDataChanges via UnitOfWork.PostCommit

                    // Commit child blocks
                    foreach (var childBlock in ChildBlocks)
                    {
                        if (childBlock.Data != null)
                        {
                            await DataBlockUnitOfWorkHelper.CommitAsync(childBlock.Data);
                        }
                    }

                    Status = "Commit successful";
                    return true;
                }
                else
                {
                    Status = $"Commit failed: {result}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Status = $"Commit error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[BeepDataBlock] CommitWithUnitOfWorkAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Rollback changes with enhanced Unit of Work support
        /// Oracle Forms equivalent: ROLLBACK built-in
        /// </summary>
        public async Task<bool> RollbackWithUnitOfWorkAsync()
        {
            if (Data == null)
            {
                Status = "No Unit of Work assigned to DataBlock";
                return false;
            }

            try
            {
                // Rollback using helper
                var success = await DataBlockUnitOfWorkHelper.RollbackAsync(Data);

                if (success)
                {
                    // Rollback child blocks
                    foreach (var childBlock in ChildBlocks)
                    {
                        if (childBlock.Data != null)
                        {
                            await DataBlockUnitOfWorkHelper.RollbackAsync(childBlock.Data);
                        }
                    }

                    Status = "Rollback successful";
                }
                else
                {
                    Status = "Rollback failed";
                }

                return success;
            }
            catch (Exception ex)
            {
                Status = $"Rollback error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[BeepDataBlock] RollbackWithUnitOfWorkAsync error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Enhanced Navigation Operations

        /// <summary>
        /// Move to next record with enhanced Unit of Work support
        /// Oracle Forms equivalent: NEXT_RECORD built-in
        /// </summary>
        public async Task<bool> MoveNextWithUnitOfWorkAsync()
        {
            if (Data == null)
            {
                return false;
            }

            // Check for unsaved changes
            if (!await CheckAndHandleUnsavedChangesRecursiveAsync())
            {
                return false;
            }

            var success = DataBlockUnitOfWorkHelper.MoveNext(Data);

            if (success)
            {
                // Update child blocks
                foreach (var childBlock in ChildBlocks)
                {
                    childBlock.SetMasterRecord(Data.Units?.Current);
                }
            }

            return success;
        }

        /// <summary>
        /// Move to previous record with enhanced Unit of Work support
        /// Oracle Forms equivalent: PREVIOUS_RECORD built-in
        /// </summary>
        public async Task<bool> MovePreviousWithUnitOfWorkAsync()
        {
            if (Data == null)
            {
                return false;
            }

            // Check for unsaved changes
            if (!await CheckAndHandleUnsavedChangesRecursiveAsync())
            {
                return false;
            }

            var success = DataBlockUnitOfWorkHelper.MovePrevious(Data);

            if (success)
            {
                // Update child blocks
                foreach (var childBlock in ChildBlocks)
                {
                    childBlock.SetMasterRecord(Data.Units?.Current);
                }
            }

            return success;
        }

        /// <summary>
        /// Move to first record with enhanced Unit of Work support
        /// Oracle Forms equivalent: FIRST_RECORD built-in
        /// </summary>
        public async Task<bool> MoveFirstWithUnitOfWorkAsync()
        {
            if (Data == null)
            {
                return false;
            }

            // Check for unsaved changes
            if (!await CheckAndHandleUnsavedChangesRecursiveAsync())
            {
                return false;
            }

            var success = DataBlockUnitOfWorkHelper.MoveFirst(Data);

            if (success)
            {
                // Update child blocks
                foreach (var childBlock in ChildBlocks)
                {
                    childBlock.SetMasterRecord(Data.Units?.Current);
                }
            }

            return success;
        }

        /// <summary>
        /// Move to last record with enhanced Unit of Work support
        /// Oracle Forms equivalent: LAST_RECORD built-in
        /// </summary>
        public async Task<bool> MoveLastWithUnitOfWorkAsync()
        {
            if (Data == null)
            {
                return false;
            }

            // Check for unsaved changes
            if (!await CheckAndHandleUnsavedChangesRecursiveAsync())
            {
                return false;
            }

            var success = DataBlockUnitOfWorkHelper.MoveLast(Data);

            if (success)
            {
                // Update child blocks
                foreach (var childBlock in ChildBlocks)
                {
                    childBlock.SetMasterRecord(Data.Units?.Current);
                }
            }

            return success;
        }

        #endregion

        #region Helper Properties

        /// <summary>
        /// Get record count using Unit of Work helper
        /// Oracle Forms equivalent: :SYSTEM.CURSOR_RECORD
        /// </summary>
        public int GetRecordCount()
        {
            return DataBlockUnitOfWorkHelper.GetRecordCount(Data);
        }

        /// <summary>
        /// Get current record index using Unit of Work helper
        /// Oracle Forms equivalent: :SYSTEM.CURSOR_RECORD
        /// </summary>
        public int GetCurrentRecordIndex()
        {
            return DataBlockUnitOfWorkHelper.GetCurrentRecordIndex(Data);
        }

        /// <summary>
        /// Check if has uncommitted changes using Unit of Work helper
        /// Oracle Forms equivalent: :SYSTEM.BLOCK_STATUS = 'CHANGED'
        /// </summary>
        public bool HasUncommittedChanges()
        {
            return DataBlockUnitOfWorkHelper.HasUncommittedChanges(Data);
        }

        /// <summary>
        /// Check if at first record
        /// Oracle Forms equivalent: :SYSTEM.RECORD_STATUS = 'FIRST'
        /// </summary>
        public bool IsFirstRecord()
        {
            return DataBlockUnitOfWorkHelper.IsFirstRecord(Data);
        }

        /// <summary>
        /// Check if at last record
        /// Oracle Forms equivalent: :SYSTEM.RECORD_STATUS = 'LAST'
        /// </summary>
        public bool IsLastRecord()
        {
            return DataBlockUnitOfWorkHelper.IsLastRecord(Data);
        }

        #endregion
    }
}
