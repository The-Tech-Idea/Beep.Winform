using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;

namespace TheTechIdea.Beep.Desktop.Common
{
    public static class AssemblyClassDefinitionManager
    {
        public static IDMEEditor DMEEditor { get; set; }
        public static AssemblyClassDefinition GetAssemblyClassDefinition(string PackageName)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == PackageName).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByClassName(string ClassName)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.className == ClassName).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectType(string ObjectType)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClass(string ObjectType, string BranchClass)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClassAndDataSourceType(string ObjectType, string BranchClass, DatasourceCategory datasourceCategory)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass && x.classProperties.Category == datasourceCategory).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClassAndPointType(string ObjectType, string BranchClass, EnumPointType PointType)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass && x.VisSchema.BranchType == PointType).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByObjectTypeAndBranchClassAndPointTypeAndDataSourceType(string ObjectType, string BranchClass, EnumPointType PointType, DatasourceCategory datasourceCategory)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.classProperties.ObjectType == ObjectType && x.classProperties.ClassType == BranchClass && x.VisSchema.BranchType == PointType && x.classProperties.Category == datasourceCategory).FirstOrDefault();
        }
        public static AssemblyClassDefinition GetAssemblyClassDefinitionByGuid(string Guid)
        {
            return DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.GuidID == Guid).FirstOrDefault();
        }
    }
}
