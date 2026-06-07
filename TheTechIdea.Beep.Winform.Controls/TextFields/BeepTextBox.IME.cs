using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTextBox
    {
        private string _compositionText = string.Empty;
        private int _compositionStart;
        private bool _isComposing;

        private const int WM_IME_STARTCOMPOSITION = 0x010D;
        private const int WM_IME_ENDCOMPOSITION = 0x010E;
        private const int WM_IME_COMPOSITION = 0x010F;
        private const int WM_IME_CHAR = 0x0286;
        private const int WM_IME_SETCONTEXT = 0x0281;

        private const int GCS_COMPSTR = 0x0008;
        private const int GCS_RESULTSTR = 0x0800;
        private const int GCS_COMPATTR = 0x0010;
        private const int GCS_CURSORPOS = 0x0080;

        private const int CFS_POINT = 0x0002;

        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll")]
        private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        private static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);

        [DllImport("imm32.dll")]
        private static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);

        [StructLayout(LayoutKind.Sequential)]
        private struct COMPOSITIONFORM
        {
            public int dwStyle;
            public POINT ptCurrentPos;
            public RECT rcArea;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private bool ProcessImeMessage(ref Message m)
        {
            if (!IsHandleCreated || _readOnly) return false;

            switch (m.Msg)
            {
                case WM_IME_SETCONTEXT:
                    if (m.WParam.ToInt32() != 0)
                    {
                        SetCompositionWindowPosition();
                    }
                    break;

                case WM_IME_STARTCOMPOSITION:
                    _isComposing = true;
                    _compositionText = string.Empty;
                    _compositionStart = _helper?.Caret?.CaretPosition ?? 0;
                    SetCompositionWindowPosition();
                    Invalidate();
                    return true;

                case WM_IME_COMPOSITION:
                    if (!_isComposing) break;
                    var hIMC = ImmGetContext(Handle);
                    if (hIMC != IntPtr.Zero)
                    {
                        try
                        {
                            int flags = m.LParam.ToInt32();

                            if ((flags & GCS_RESULTSTR) != 0)
                            {
                                string result = GetCompositionString(hIMC, GCS_RESULTSTR);
                                if (!string.IsNullOrEmpty(result))
                                {
                                    _helper?.InsertText(result);
                                }
                                _isComposing = false;
                                _compositionText = string.Empty;
                            }
                            else if ((flags & GCS_COMPSTR) != 0)
                            {
                                _compositionText = GetCompositionString(hIMC, GCS_COMPSTR);
                                SetCompositionWindowPosition();
                            }

                            if ((flags & GCS_CURSORPOS) != 0)
                            {
                                SetCompositionWindowPosition();
                            }
                        }
                        finally
                        {
                            ImmReleaseContext(Handle, hIMC);
                        }
                    }
                    Invalidate();
                    return true;

                case WM_IME_ENDCOMPOSITION:
                    _isComposing = false;
                    _compositionText = string.Empty;
                    Invalidate();
                    return true;

                case WM_IME_CHAR:
                    return true;
            }

            return false;
        }

        private string GetCompositionString(IntPtr hIMC, int type)
        {
            int len = ImmGetCompositionStringW(hIMC, type, null, 0);
            if (len <= 0) return string.Empty;

            byte[] buf = new byte[len];
            ImmGetCompositionStringW(hIMC, type, buf, len);
            return System.Text.Encoding.Unicode.GetString(buf);
        }

        private void SetCompositionWindowPosition()
        {
            if (!IsHandleCreated) return;
            var hIMC = ImmGetContext(Handle);
            if (hIMC == IntPtr.Zero) return;

            try
            {
                Point caretScreen;
                try
                {
                    caretScreen = PointToScreen(GetCaretPositionForIme());
                }
                catch
                {
                    caretScreen = PointToScreen(new Point(0, Height / 2));
                }

                var compForm = new COMPOSITIONFORM
                {
                    dwStyle = CFS_POINT,
                    ptCurrentPos = new POINT { x = caretScreen.X, y = caretScreen.Y }
                };
                ImmSetCompositionWindow(hIMC, ref compForm);
            }
            finally
            {
                ImmReleaseContext(Handle, hIMC);
            }
        }

        private Point GetCaretPositionForIme()
        {
            int pos = _helper?.Caret?.CaretPosition ?? 0;
            if (string.IsNullOrEmpty(_text)) return new Point(2, Height / 2);

            string textBefore = _text.Substring(0, Math.Min(pos, _text.Length));
            var size = TextRenderer.MeasureText(textBefore, TextFont);
            return new Point(2 + size.Width, 2);
        }

        internal bool IsComposing => _isComposing;
        internal string CompositionText => _compositionText;
        internal int CompositionStart => _compositionStart;
    }
}
