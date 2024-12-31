using System.ComponentModel;
using TheTechIdea.Beep.Desktop.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class UnitOfWorksConverter : TypeConverter
    {
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context?.Instance is not IBeepDataBlock IBeepDataBlock)
                return new StandardValuesCollection(Array.Empty<object>());

            var dataContext = IBeepDataBlock.DataContext;
            if (dataContext == null)
                return new StandardValuesCollection(Array.Empty<object>());

            // Get properties of the DataContext that are of type IUnitofWork
            var unitOfWorkProperties = dataContext.GetType()
                .GetProperties()
                .Where(p => ProjectHelper.IsUnitOfWorkType(p.PropertyType))
                .Select(p => p.GetValue(dataContext))
                .Where(instance => instance != null)
                .ToList();

            return new StandardValuesCollection(unitOfWorkProperties);
        }




    }
}