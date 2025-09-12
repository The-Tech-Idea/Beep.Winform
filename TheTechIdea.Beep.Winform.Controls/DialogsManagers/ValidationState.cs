using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public sealed class ValidationState
    {
        public bool IsValid { get; set; } = true;
        public List<string> Errors { get; set; } = new();
        public static ValidationState Ok() => new ValidationState { IsValid = true };
        public static ValidationState Fail(params string[] errors) => new ValidationState { IsValid = false, Errors = errors.ToList() };
    }
}
