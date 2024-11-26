using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Design.UIEditor
{
    /// <summary>
    /// Provides data for the <see cref="BeepImage.ImageLoadError"/> event.
    /// </summary>
    public class ImageLoadErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the exception that occurred during image loading.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLoadErrorEventArgs"/> class with the specified exception.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        public ImageLoadErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
