using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Desktop.Common
{
    public static class MiscFunctions
    {
        public static string GetRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetRandomString(int length, string chars)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static BeepMouseEventArgs GetMouseEventArgs(string eventname, MouseEventArgs e)

        {
            BeepMouseEventArgs args = new BeepMouseEventArgs();
            args.EventName = eventname;
            args.Button = (BeepMouseEventArgs.BeepMouseButtons)e.Button;
            args.Clicks = e.Clicks;
            args.X = e.X;
            args.Y = e.Y;
            args.Delta = e.Delta;
            args.Handled = false;
            args.Data = e;
            return args;

        }
    }
}
