using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
   

}
public class DockItemState
{
    public SimpleItem Item { get; set; }
    public float CurrentScale { get; set; } = 1.0f;
    public float TargetScale { get; set; } = 1.0f;
    public bool IsHovered { get; set; }
    public bool IsSelected { get; set; }
}