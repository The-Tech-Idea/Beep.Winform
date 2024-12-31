using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{

        public class ExampleUnitOfWorkHolder
        {
            // Public property of type IUnitofWork
            [Browsable(true)]
            [Category("Data")]
            [Description("An example implementation of IUnitofWork for data operations.")]
            public IUnitofWork MyUnitOfWork { get; set; }

            // Constructor to initialize MyUnitOfWork with a sample implementation
            public ExampleUnitOfWorkHolder()
            {
                
            }
        }

      
 

}
