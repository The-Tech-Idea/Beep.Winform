using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    public enum BreadcrumbStyle
    {
        Classic,
        Modern,
        Pill,
        Flat
    }
   
    /// <summary>
    /// Defines the contract for a breadcrumb display control.
    /// </summary>
    public interface IBreadCrumbDisplay
    {
        #region Properties

        /// <summary>
        /// Gets or sets the background color of the breadcrumb control.
        /// </summary>
        Color BackColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color of the breadcrumb control.
        /// </summary>
        Color ForeColor { get; set; }

        /// <summary>
        /// Gets or sets the font used to display text in the breadcrumb control.
        /// </summary>
        Font TextFont { get; set; }

        /// <summary>
        /// Gets the collection of breadcrumb items.
        /// </summary>
        BindingList<BreadCrumbData> Items { get; }

       
        #endregion

        #region Events

        /// <summary>
        /// Occurs when a breadcrumb item is clicked.
        /// </summary>
        event EventHandler<CrumbClickedEventArgs> CrumbClicked;

        #endregion

        #region Methods

      

        /// <summary>
        /// Applies the current theme to the breadcrumb control.
        /// </summary>
        void ApplyTheme();

        #endregion
    }
    /// <summary>
    /// Provides data for the CrumbClicked event
    /// </summary>
    public class CrumbClickedEventArgs : EventArgs
    {
        public int Index { get; }
        public string Name { get; }
        public string Text { get; }
        public string Crumb => Text ?? Name ?? "";

        public CrumbClickedEventArgs(int index,string name)
        {
            Index = index;
            Name = name;
        }
        public CrumbClickedEventArgs(int index, string name,string text)
        {
            Index = index;
            Name = name;
            Text = text;
        }
    }
    public class BreadCrumbData
    {
        public string Crumb { get; set; }
        public string Path { get; set; }

    }
}
