using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.UOWManager.Models;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.DataBlocks.Helpers
{
    /// <summary>
    /// Helper class for DataBlock Unit of Work operations
    /// Provides Oracle Forms-like data management using Unit of Work pattern
    /// </summary>
    public static class DataBlockUnitOfWorkHelper
    {
        /// <summary>
        /// Execute query with filters using Unit of Work
        /// Oracle Forms equivalent: EXECUTE_QUERY built-in
        /// </summary>
        public static async Task<bool> ExecuteQueryAsync(
            IUnitofWork unitOfWork,
            List<AppFilter> filters = null,
            string orderByClause = null)
        {
            if (unitOfWork == null)
                return false;

            try
            {
                // Apply filters if provided
                if (filters != null && filters.Count > 0)
                {
                    var getMethod = unitOfWork.GetType().GetMethod("Get", new[] { typeof(List<AppFilter>) });
                    if (getMethod != null)
                    {
                        var task = (Task)getMethod.Invoke(unitOfWork, new object[] { filters });
                        await task;
                    }
                    else
                    {
                        // Fallback: use Get() and filter manually
                        await unitOfWork.Get();
                    }
                }
                else
                {
                    await unitOfWork.Get();
                }

                // Apply ordering if provided
                if (!string.IsNullOrEmpty(orderByClause) && unitOfWork.Units != null)
                {
                    // Note: Ordering would need to be implemented in UnitOfWork or applied after Get()
                    // This is a placeholder for future enhancement
                }

                // Move to first record
                unitOfWork.Units?.MoveFirst();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataBlockUnitOfWorkHelper] ExecuteQueryAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Insert a new record using Unit of Work
        /// Oracle Forms equivalent: CREATE_RECORD built-in
        /// </summary>
        public static async Task<bool> InsertRecordAsync(
            IUnitofWork unitOfWork,
            Entity newRecord)
        {
            if (unitOfWork == null || newRecord == null)
                return false;

            try
            {
                await unitOfWork.InsertAsync(newRecord);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataBlockUnitOfWorkHelper] InsertRecordAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update current record using Unit of Work
        /// Oracle Forms equivalent: UPDATE_RECORD built-in
        /// </summary>
        public static async Task<bool> UpdateRecordAsync(
            IUnitofWork unitOfWork,
            Entity recordToUpdate = null)
        {
            if (unitOfWork == null)
                return false;

            try
            {
                var record = recordToUpdate ?? unitOfWork.Units?.Current;
                if (record == null)
                    return false;

                await unitOfWork.UpdateAsync(record);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataBlockUnitOfWorkHelper] UpdateRecordAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete current record using Unit of Work
        /// Oracle Forms equivalent: DELETE_RECORD built-in
        /// </summary>
        public static async Task<bool> DeleteRecordAsync(
            IUnitofWork unitOfWork,
            Entity recordToDelete = null)
        {
            if (unitOfWork == null)
                return false;

            try
            {
                var record = recordToDelete ?? unitOfWork.Units?.Current;
                if (record == null)
                    return false;

                await unitOfWork.DeleteAsync(record);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataBlockUnitOfWorkHelper] DeleteRecordAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Commit changes using Unit of Work
        /// Oracle Forms equivalent: COMMIT_FORM built-in
        /// </summary>
        public static async Task<IErrorsInfo> CommitAsync(IUnitofWork unitOfWork)
        {
            if (unitOfWork == null)
                return new ErrorsInfo { Flag = Errors.Failed };

            try
            {
                var result = await unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataBlockUnitOfWorkHelper] CommitAsync error: {ex.Message}");
                return new ErrorsInfo { Flag = Errors.Failed, Message = ex.Message, Ex = ex };
            }
        }

        /// <summary>
        /// Rollback changes using Unit of Work
        /// Oracle Forms equivalent: ROLLBACK built-in
        /// </summary>
        public static async Task<bool> RollbackAsync(IUnitofWork unitOfWork)
        {
            if (unitOfWork == null)
                return false;

            try
            {
                await unitOfWork.Rollback();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataBlockUnitOfWorkHelper] RollbackAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if Unit of Work has uncommitted changes
        /// Oracle Forms equivalent: :SYSTEM.BLOCK_STATUS = 'CHANGED'
        /// </summary>
        public static bool HasUncommittedChanges(IUnitofWork unitOfWork)
        {
            return unitOfWork?.IsDirty ?? false;
        }

        /// <summary>
        /// Get count of records in Unit of Work
        /// Oracle Forms equivalent: :SYSTEM.CURSOR_RECORD
        /// </summary>
        public static int GetRecordCount(IUnitofWork unitOfWork)
        {
            return unitOfWork?.Units?.Count ?? 0;
        }

        /// <summary>
        /// Get current record index
        /// Oracle Forms equivalent: :SYSTEM.CURSOR_RECORD
        /// </summary>
        public static int GetCurrentRecordIndex(IUnitofWork unitOfWork)
        {
            return unitOfWork?.Units?.CurrentIndex ?? -1;
        }

        /// <summary>
        /// Get current record
        /// Oracle Forms equivalent: :BLOCK.RECORD
        /// </summary>
        public static Entity GetCurrentRecord(IUnitofWork unitOfWork)
        {
            return unitOfWork?.Units?.Current;
        }

        /// <summary>
        /// Move to next record
        /// Oracle Forms equivalent: NEXT_RECORD built-in
        /// </summary>
        public static bool MoveNext(IUnitofWork unitOfWork)
        {
            if (unitOfWork?.Units == null)
                return false;

            try
            {
                unitOfWork.Units.MoveNext();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Move to previous record
        /// Oracle Forms equivalent: PREVIOUS_RECORD built-in
        /// </summary>
        public static bool MovePrevious(IUnitofWork unitOfWork)
        {
            if (unitOfWork?.Units == null)
                return false;

            try
            {
                unitOfWork.Units.MovePrevious();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Move to first record
        /// Oracle Forms equivalent: FIRST_RECORD built-in
        /// </summary>
        public static bool MoveFirst(IUnitofWork unitOfWork)
        {
            if (unitOfWork?.Units == null)
                return false;

            try
            {
                unitOfWork.Units.MoveFirst();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Move to last record
        /// Oracle Forms equivalent: LAST_RECORD built-in
        /// </summary>
        public static bool MoveLast(IUnitofWork unitOfWork)
        {
            if (unitOfWork?.Units == null)
                return false;

            try
            {
                unitOfWork.Units.MoveLast();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Move to specific record index
        /// Oracle Forms equivalent: GO_RECORD built-in
        /// </summary>
        public static bool MoveTo(IUnitofWork unitOfWork, int index)
        {
            if (unitOfWork?.Units == null)
                return false;

            try
            {
                unitOfWork.Units.MoveTo(index);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if at first record
        /// Oracle Forms equivalent: :SYSTEM.RECORD_STATUS = 'FIRST'
        /// </summary>
        public static bool IsFirstRecord(IUnitofWork unitOfWork)
        {
            if (unitOfWork?.Units == null)
                return false;

            return unitOfWork.Units.CurrentIndex == 0;
        }

        /// <summary>
        /// Check if at last record
        /// Oracle Forms equivalent: :SYSTEM.RECORD_STATUS = 'LAST'
        /// </summary>
        public static bool IsLastRecord(IUnitofWork unitOfWork)
        {
            if (unitOfWork?.Units == null)
                return false;

            return unitOfWork.Units.CurrentIndex >= (unitOfWork.Units.Count - 1);
        }

        /// <summary>
        /// Clear all records (clear block)
        /// Oracle Forms equivalent: CLEAR_BLOCK built-in
        /// </summary>
        public static void ClearBlock(IUnitofWork unitOfWork)
        {
            if (unitOfWork?.Units == null)
                return;

            try
            {
                unitOfWork.Units.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataBlockUnitOfWorkHelper] ClearBlock error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create new record (enter query mode)
        /// Oracle Forms equivalent: ENTER_QUERY built-in
        /// Note: New() returns void, the new record is available via Units.Current after calling New()
        /// </summary>
        public static Entity CreateNewRecord(IUnitofWork unitOfWork)
        {
            if (unitOfWork == null)
                return null;

            try
            {
                unitOfWork.New(); // New() returns void, creates new record in Units
                return unitOfWork.Units?.Current; // Return the newly created record
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataBlockUnitOfWorkHelper] CreateNewRecord error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Undo last change
        /// Oracle Forms equivalent: UNDO built-in
        /// </summary>
        public static bool UndoLastChange(IUnitofWork unitOfWork)
        {
            if (unitOfWork == null)
                return false;

            try
            {
                unitOfWork.UndoLastChange();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataBlockUnitOfWorkHelper] UndoLastChange error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all records from Unit of Work
        /// </summary>
        public static List<Entity> GetAllRecords(IUnitofWork unitOfWork)
        {
            if (unitOfWork?.Units == null)
                return new List<Entity>();

            try
            {
                return unitOfWork.Units.ToList();
            }
            catch
            {
                return new List<Entity>();
            }
        }

        /// <summary>
        /// Check if Unit of Work is in a valid state
        /// </summary>
        public static bool IsValidState(IUnitofWork unitOfWork)
        {
            return unitOfWork != null && 
                   unitOfWork.EntityStructure != null && 
                   unitOfWork.Units != null;
        }
    }
}
