using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace AgentFire.Gwent.DuelSimulator.Components
{
    internal static class KeyboardHook
    {
        public delegate void RawKeyEventHandler(Key key, bool wasPressed);
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static class InterceptKeys
        {
            public static int WH_KEYBOARD_LL = 13;
            public static int WM_KEYDOWN = 0x0100;
            public static int WM_KEYUP = 0x0101;

            public static IntPtr SetHook(LowLevelKeyboardProc proc)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                return HookCallbackInner(nCode, wParam, lParam);
            }
            catch
            {
                Debugger.Break();
            }

            return InterceptKeys.CallNextHookEx(hookId, nCode, wParam, lParam);
        }
        private static IntPtr HookCallbackInner(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)InterceptKeys.WM_KEYDOWN || wParam == (IntPtr)InterceptKeys.WM_KEYUP)
                {
                    KeyEvent?.Invoke(KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(lParam)), wParam == (IntPtr)InterceptKeys.WM_KEYDOWN);
                }
            }

            return InterceptKeys.CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public static event RawKeyEventHandler KeyEvent;

        private static readonly LowLevelKeyboardProc _handler = HookCallback;
        private static IntPtr hookId = IntPtr.Zero;

        static KeyboardHook()
        {
            IntPtr hookId = InterceptKeys.SetHook(_handler);
            AppDomain.CurrentDomain.ProcessExit += (s, e) => InterceptKeys.UnhookWindowsHookEx(hookId);
        }
    }
}
