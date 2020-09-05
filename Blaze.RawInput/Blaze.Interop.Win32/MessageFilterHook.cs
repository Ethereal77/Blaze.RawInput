// Copyright © 2020 Infinisis

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Blaze.Collections;

namespace Blaze.Interop.Win32
{
    /// <summary>
    ///   Provides a way to hook to the <c>WndProc</c> of an existing window handle using <see cref="IMessageFilter"/>.
    /// </summary>
    public class MessageFilterHook
    {
        private static readonly Dictionary<IntPtr, MessageFilterHook> g_RegisteredHooksByHwnd = new Dictionary<IntPtr, MessageFilterHook>(EqualityComparer.DefaultIntPtr);

        private readonly IntPtr windowHandle;
        private readonly IntPtr defaultWndProc;
        private readonly IntPtr newWndProcPtr;
        private readonly Win32Native.WndProc newWndProc;

        private List<IMessageFilter> currentFilters;

        private bool isDisposed;


        /// <summary>
        ///   Initializes a new instance of the <see cref="MessageFilterHook" /> class.
        /// </summary>
        /// <param name="windowHandle">The handle of the window whose <c>WndProc</c> function has to be hooked.</param>
        private MessageFilterHook(IntPtr windowHandle)
        {
            this.windowHandle = windowHandle;

            defaultWndProc = Win32Native.GetWindowLong(windowHandle, Win32Native.WindowLongType.WndProc);
            newWndProc = new Win32Native.WndProc(WndProc);
            newWndProcPtr = Marshal.GetFunctionPointerForDelegate((Delegate)this.newWndProc);

            currentFilters = new List<IMessageFilter>();

            Win32Native.SetWindowLong(windowHandle, Win32Native.WindowLongType.WndProc, this.newWndProcPtr);
        }


        /// <summary>
        ///   Adds a message filter to a window.
        /// </summary>
        /// <param name="windowHandle">The handle of the window.</param>
        /// <param name="messageFilter">The message filter.</param>
        public static void AddMessageFilter(IntPtr windowHandle, IMessageFilter messageFilter)
        {
            lock (g_RegisteredHooksByHwnd)
            {
                windowHandle = GetSafeWindowHandle(windowHandle);

                if (!g_RegisteredHooksByHwnd.TryGetValue(windowHandle, out MessageFilterHook messageFilterHook))
                {
                    messageFilterHook = new MessageFilterHook(windowHandle);
                    g_RegisteredHooksByHwnd.Add(windowHandle, messageFilterHook);
                }

                messageFilterHook.AddMessageMilter(messageFilter);
            }
        }

        /// <summary>
        ///   Removes a message filter associated with a window.
        /// </summary>
        /// <param name="windowHandle">The handle of the window.</param>
        /// <param name="messageFilter">The message filter.</param>
        public static void RemoveMessageFilter(IntPtr windowHandle, IMessageFilter messageFilter)
        {
            lock (g_RegisteredHooksByHwnd)
            {
                windowHandle = GetSafeWindowHandle(windowHandle);

                if (!g_RegisteredHooksByHwnd.TryGetValue(windowHandle, out MessageFilterHook messageFilterHook))
                    return;

                messageFilterHook.RemoveMessageFilter(messageFilter);
                if (!messageFilterHook.isDisposed)
                    return;

                g_RegisteredHooksByHwnd.Remove(windowHandle);
                messageFilterHook.RestoreWndProc();
            }
        }

        private void AddMessageMilter(IMessageFilter filter)
        {
            var messageFilters = new List<IMessageFilter>(currentFilters);
            if (!messageFilters.Contains(filter))
                messageFilters.Add(filter);

            currentFilters = messageFilters;
        }

        private void RemoveMessageFilter(IMessageFilter filter)
        {
            var messageFilters = new List<IMessageFilter>(currentFilters);
            messageFilters.Remove(filter);

            if (messageFilters.Count == 0)
            {
                isDisposed = true;
                RestoreWndProc();
            }

            currentFilters = messageFilters;
        }

        private void RestoreWndProc()
        {
            if (!(Win32Native.GetWindowLong(windowHandle, Win32Native.WindowLongType.WndProc) == newWndProcPtr))
                return;

            Win32Native.SetWindowLong(windowHandle, Win32Native.WindowLongType.WndProc, defaultWndProc);
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (isDisposed)
            {
                RestoreWndProc();
            }
            else
            {
                var message = new Message()
                {
                    HWnd = windowHandle,
                    LParam = lParam,
                    Msg = msg,
                    WParam = wParam
                };

                foreach (IMessageFilter messageFilter in currentFilters)
                {
                    if (messageFilter.PreFilterMessage(ref message))
                        return message.Result;
                }
            }

            return Win32Native.CallWindowProc(defaultWndProc, hWnd, msg, wParam, lParam);
        }

        private static IntPtr GetSafeWindowHandle(IntPtr windowHandle)
        {
            return (windowHandle != IntPtr.Zero)
                ? windowHandle
                : Process.GetCurrentProcess().MainWindowHandle;
        }
    }
}
