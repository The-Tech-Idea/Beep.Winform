
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;

namespace TheTechIdea.Beep.Desktop.Common.Helpers
{
    public static class AssemblyDefinitionsHelper
    {
        #region Assembly Class Definitions
        public static AssemblyClassDefinition GetAssemblyClassDefinition(IDMEEditor DMEEditor, string PackageName)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == PackageName).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByClassName(IDMEEditor DMEEditor, string ClassName)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.className == ClassName).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectType(IDMEEditor DMEEditor, string ObjectType)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClass(IDMEEditor DMEEditor, string ObjectType, string BranchClass)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClassAndDataSourceType(IDMEEditor DMEEditor, string ObjectType, string BranchClass, DatasourceCategory datasourceCategory)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass && x.classProperties.Category == datasourceCategory).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClassAndPointType(IDMEEditor DMEEditor, string ObjectType, string BranchClass, EnumPointType PointType)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass && x.VisSchema.BranchType == PointType).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClassAndPointTypeAndDataSourceType(IDMEEditor DMEEditor, string ObjectType, string BranchClass, EnumPointType PointType, DatasourceCategory datasourceCategory)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass && x.VisSchema.BranchType == PointType && x.classProperties.Category == datasourceCategory).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyBranchsClassDefinitionByGuid(IDMEEditor DMEEditor, string Guid)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.GuidID == Guid).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyGlobalFunctionsClassDefinitionByGuid(IDMEEditor DMEEditor, string Guid)
        {
            return DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.GuidID == Guid).FirstOrDefault();
        }
        #endregion

    }
}
