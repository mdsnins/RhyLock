using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace TreeViewTest
{
    class InterceptMouse
    {
        internal static LowLevelMouseProc m_proc = HookCallback;
        internal static IntPtr m_hookID = IntPtr.Zero;

        public static bool IsMouseOutsideApp
        {
            get;
            set;
        }

        internal static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        internal static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                /*  새로 고친 코드. 그러나 치명적 에러 발생. 
                //check if POint in main window
                Point pt = new Point(hookStruct.pt.x, hookStruct.pt.y);
                IsMouseOutsideApp = false;
                if (App.Current.MainWindow != null)
                {
                    var ptw = App.Current.MainWindow.PointFromScreen(pt);
                    var w = App.Current.MainWindow.Width;
                    var h = App.Current.MainWindow.Height;
                    //if point is outside MainWindow
                    if (ptw.X < 0 || ptw.Y < 0 || ptw.X > w || ptw.Y > h)
                        IsMouseOutsideApp = true;
                    else
                        IsMouseOutsideApp = false;
                }
                */
                /*
                //check if POint in main window  에러 관계로 옛날 코드로 땜빵질 후 수정 필요
                Point pt = new Point(hookStruct.pt.x, hookStruct.pt.y);
                var ptw = App.Current.MainWindow.PointFromScreen(pt);
                var w = App.Current.MainWindow.Width;
                var h = App.Current.MainWindow.Height;
                //if point is outside MainWindow
                if (ptw.X < 0 || ptw.Y < 0 || ptw.X > w || ptw.Y > h)
                    IsMouseOutsideApp = true;
                else
                    IsMouseOutsideApp = false;
                */
                Point pt = new Point(hookStruct.pt.x, hookStruct.pt.y);
                IsMouseOutsideApp = true;
                if (App.Current.MainWindow != null)
                {
                    var ptw = App.Current.MainWindow.PointFromScreen(pt);
                    var w = App.Current.MainWindow.Width;
                    var h = App.Current.MainWindow.Height;
                    //if point is outside MainWindow
                    if (ptw.X < 0 || ptw.Y < 0 || ptw.X > w || ptw.Y > h)
                        IsMouseOutsideApp = true;
                    else
                        IsMouseOutsideApp = false;
                }

            }
            return CallNextHookEx(m_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONUP = 0x0202;
        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
