using System;
using XDMessaging.Native;

namespace XDMessaging
{
    /// <summary>
    /// A class used as a WindowFilterHandler for the WindowsEnum class. This 
    /// filters the results of a windows enumeration based on whether the windows
    /// contain a named property.
    /// </summary>
    internal sealed class WindowEnumFilter
    {
        /// <summary>
        /// The property to search for when filtering the windows.
        /// </summary>
        private string property;
        /// <summary>
        /// The constructor which takes the property name used for filtering
        /// results.
        /// </summary>
        /// <param name="property">The windows property name.</param>
        public WindowEnumFilter(string property)
        {
            this.property = property;
        }
        /// <summary>
        /// The delegate used to filter windows during emuneration. Only windows 
        /// that contain a named property are added to the enum.
        /// </summary>
        /// <param name="hWnd">The window being filtered.</param>
        /// <param name="include">Indicates whether the window should be
        /// inclused in the enumeration output.</param>
        public void WindowFilterHandler(IntPtr hWnd, ref bool include)
        {
            if (Win32.GetProp(hWnd, property) == 0)
                include = false;
        }
    }
}
