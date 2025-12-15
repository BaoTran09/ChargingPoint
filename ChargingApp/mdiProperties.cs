using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChargingApp
{
    public static class mdiProperties
    {
        // --- Win32 API ---
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy,
            uint uFlags);

        // --- Constants ---
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_CLIENTEDGE = 0x200;

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private const uint SWP_NOOWNERZORDER = 0x0200;

        // --- Extension Method ---
        public static bool SetBevel(this Form form, bool show)
        {
            foreach (Control c in form.Controls)
            {
                if (c is MdiClient client)
                {
                    int style = GetWindowLong(client.Handle, GWL_EXSTYLE);

                    if (show)
                    {
                        style |= WS_EX_CLIENTEDGE;     // Thêm viền
                    }
                    else
                    {
                        style &= ~WS_EX_CLIENTEDGE;    // Xóa viền (fix lỗi ở code cũ)
                    }

                    SetWindowLong(client.Handle, GWL_EXSTYLE, style);

                    // Áp dụng thay đổi giao diện
                    SetWindowPos(
                        client.Handle,
                        IntPtr.Zero,
                        0, 0, 0, 0,
                        SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE |
                        SWP_NOZORDER | SWP_NOOWNERZORDER | SWP_FRAMECHANGED
                    );

                    return true;
                }
            }

            return false;
        }






















    }
}
