using System.Collections.Generic;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Vis.Modules
{
    public enum DataBlockMode
    {
        Query,
        CRUD
    }
    public interface IBeepDataBlock
    {
        string Name { get; set; }
        DataBlockMode BlockMode { get; set; }
        List<IBeepDataBlock> ChildBlocks { get; set; }
        IUnitofWork Data { get; set; }
        IEntityStructure EntityStructure { get; }
        List<EntityField> Fields { get; }
        string ForeignKeyPropertyName { get; set; }
        bool IsInQueryMode { get; }
        string MasterKeyPropertyName { get; set; }
        dynamic MasterRecord { get; }
        IBeepDataBlock ParentBlock { get; set; }
        List<RelationShipKeys> Relationships { get; set; }
        Dictionary<string, IBeepUIComponent> UIComponents { get; set; }

        void Dispose();
        void RemoveChildBlock(IBeepDataBlock childBlock);
        void RemoveParentBlock();
        void SetMasterRecord(dynamic masterRecord);
        void SwitchBlockMode(DataBlockMode newMode);
        void HandleDataChanges();
    }
}