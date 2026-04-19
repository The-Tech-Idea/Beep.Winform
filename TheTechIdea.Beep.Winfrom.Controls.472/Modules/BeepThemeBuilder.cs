using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class BeepThemeBuilder
    {
        private BeepThemeBase _theme = new BeepThemeBase();

        public BeepThemeBuilder WithPrimaryColor(Color color)
        {
            _theme.Palette.PrimaryColor = color;
            return this;
        }

        public BeepThemeBuilder WithTitleBar(Action<TitleBarThemeBuilder> configure)
        {
            var titleBarBuilder = new TitleBarThemeBuilder(_theme.TitleBar);
            configure(titleBarBuilder);
            return this;
        }

        // Add methods for other theme aspects

        public BeepThemeBase Build()
        {
            return _theme;
        }
    }

    public class TitleBarThemeBuilder
    {
        private TitleBarTheme _titleBar;

        public TitleBarThemeBuilder(TitleBarTheme titleBar)
        {
            _titleBar = titleBar;
        }

        public TitleBarThemeBuilder WithBackColor(Color color)
        {
            _titleBar.BackColor = color;
            return this;
        }

        public TitleBarThemeBuilder WithForeColor(Color color)
        {
            _titleBar.ForeColor = color;
            return this;
        }

        // Add methods for other TitleBar properties
    }

}
