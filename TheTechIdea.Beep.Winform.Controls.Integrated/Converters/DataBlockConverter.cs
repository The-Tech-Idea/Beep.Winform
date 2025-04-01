using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class DataBlockConverter : TypeConverter
    {
        private Dictionary<string, BeepDataBlock> _blockMap = new();
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(BeepDataBlock))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context?.Container == null || context.Instance is not BeepDataBlock currentBlock)
                return new StandardValuesCollection(Array.Empty<string>());

            // Get all components in the container
            var designerHost = context.Container as IDesignerHost;
            var allBlocks = designerHost.Container.Components
                .OfType<BeepDataBlock>()
                .Where(block => block != currentBlock && !IsChildBlock(currentBlock, block))
                .ToList();

            // Map block names to objects for conversion
            _blockMap = allBlocks
                .Where(block => block.Site != null)
                .ToDictionary(block => block.Name, block => block);

            // Return the names of the blocks
            return new StandardValuesCollection(_blockMap.Keys.ToList());
        }
        public override object ConvertFrom(
     ITypeDescriptorContext context,
     CultureInfo culture,
     object value)
        {
            // If the incoming value is a block, just return it.
            if (value is BeepDataBlock blockValue)
                return blockValue;

            // If it's a string, try to look it up in _blockMap.
            if (value is string blockName)
            {
                if (_blockMap != null && _blockMap.TryGetValue(blockName, out var block))
                {
                    return block;
                }
                else
                {
                    // Decide how you want to handle an unknown block name:
                    // Option A: return null (if your property can be null)
                    // return null;

                    // Option B: throw a descriptive exception
                    throw new ArgumentException($"Block named '{blockName}' was not found.");
                }
            }
            // Otherwise, fall back to the base behavior
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is BeepDataBlock block)
            {
                return block.Name ?? block.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        private bool IsChildBlock(IBeepDataBlock parentBlock, IBeepDataBlock block)
        {
            if (parentBlock.ChildBlocks == null || !parentBlock.ChildBlocks.Any())
                return false;

            foreach (var child in parentBlock.ChildBlocks)
            {
                if (child == block || IsChildBlock(child, block))
                    return true;
            }

            return false;
        }
    }
}
