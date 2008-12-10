using System;
using System.Runtime.InteropServices;

namespace XDMessaging.Native
{
    /// <summary>
    /// The native Win32 APIs used by the library.
    /// </summary>
    internal static class Win32
    {
        /// <summary>
        /// The WM_COPYDATA constant.
        /// </summary>
        public const int WM_COPYDATA = 0x4A;
        /// <summary>
        /// The WM_CHILD window style constant.
        /// </summary>
        public const uint WS_CHILD = 0x40000000;
        /// <summary>
        /// The struct used to marshal data between applications using
        /// the windows messaging API.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }
        /// <summary>
        /// Returns a pointer to the Desktop window.
        /// </summary>
        /// <returns>Pointer to the desktop window.</returns>
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();
        /// <summary>
        /// Sends a native windows message to a specified window.
        /// </summary>
        /// <param name="hwnd">The window to which the message should be sent.</param>
        /// <param name="wMsg">The native windows message type.</param>
        /// <param name="wParam">A pointer to the wPAram data.</param>
        /// <param name="lParam">The struct containing lParam data</param>
        /// <returns></returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int SendMessage(IntPtr hwnd, int wMsg, int wParam, ref COPYDATASTRUCT lParam);
        /// <summary>
        /// A delegate used by the EnumChildWindows windows API to enumerate windows.
        /// </summary>
        /// <param name="hwnd">A pointer to a window that was found.</param>
        /// <param name="lParam">The lParam passed by the EnumChildWindows API.</param>
        /// <returns></returns>
        public delegate int EnumWindowsProc(IntPtr hwnd, int lParam);
        /// <summary>
        /// The API used to enumerate child windows of a given parent window.
        /// </summary>
        /// <param name="hwndParent">The parent window.</param>
        /// <param name="lpEnumFunc">The delegate called when a window is located.</param>
        /// <param name="lParam">The lParam passed to the deleage.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);
        /// <summary>
        /// Gets a named window property for a given window address. 
        /// This returns zero if not found.
        /// </summary>
        /// <param name="hwnd">The window containing the property.</param>
        /// <param name="lpString">The property name to lookup.</param>
        /// <returns>The property data, or 0 if not found.</returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int GetProp(IntPtr hwnd, string lpString);
        /// <summary>
        /// Sets a window proerty value.
        /// </summary>
        /// <param name="hwnd">The window on which to attach the property.</param>
        /// <param name="lpString">The property name.</param>
        /// <param name="hData">The property value.</param>
        /// <returns>A value indicating whether the function succeeded.</returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int SetProp(IntPtr hwnd, string lpString, int hData);
        /// <summary>
        /// Removes a named property from a given window.
        /// </summary>
        /// <param name="hwnd">The window whose property should be removed.</param>
        /// <param name="lpString">The property name.</param>
        /// <returns>A value indicating whether the function succeeded.</returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int RemoveProp(IntPtr hwnd, string lpString);
    }
}
