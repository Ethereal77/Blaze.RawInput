// Copyright © 2020 Infinisis

using System;
using System.Runtime.InteropServices;

namespace Blaze.Interop.Win32
{
    /// <summary>
    ///   Internal class to interact with native Win32 Messages.
    /// </summary>
    internal class Win32Native
    {
        public static IntPtr GetWindowLong(IntPtr hWnd, WindowLongType windowLongType) =>
            IntPtr.Size == 4
                ? GetWindowLong32(hWnd, windowLongType)
                : GetWindowLong64(hWnd, windowLongType);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetWindowLong32(IntPtr hwnd, WindowLongType windowLongType);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetWindowLong64(IntPtr hwnd, WindowLongType windowLongType);

        public static IntPtr SetWindowLong(IntPtr hwnd, WindowLongType windowLongType, IntPtr wndProcPtr) =>
            IntPtr.Size == 4
                ? SetWindowLong32(hwnd, windowLongType, wndProcPtr)
                : SetWindowLongPtr64(hwnd, windowLongType, wndProcPtr);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Unicode)]
        private static extern IntPtr SetWindowLong32(IntPtr hwnd, WindowLongType windowLongType, IntPtr wndProc);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Unicode)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hwnd, WindowLongType windowLongType, IntPtr wndProc);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CallWindowProc(IntPtr wndProc, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        public enum WindowLongType
        {
            UserData = -21,         // 0xFFFFFFEB
            ExtendedStyle = -20,    // 0xFFFFFFEC
            Style = -16,            // 0xFFFFFFF0
            Id = -12,               // 0xFFFFFFF4
            HwndParent = -8,        // 0xFFFFFFF8
            HInstance = -6,         // 0xFFFFFFFA
            WndProc = -4,           // 0xFFFFFFFC
        }

        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
