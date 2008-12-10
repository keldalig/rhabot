using System;
using System.Collections.Generic;
using System.Text;
using XDMessaging.Utils;
using XDMessaging.Native;

namespace XDMessaging
{
    /// <summary>
    /// Class used to broadcast messages to other applications listening
    /// on a particular channel.
    /// </summary>
    internal static class XDBroadcast
    {
        /// <summary>
        /// The API used to broadcast messages to a channel, and other applications that
        /// may be listening.
        /// </summary>
        /// <param name="channel">The channel name to broadcast on.</param>
        /// <param name="message">The string message data.</param>
        public static void SendToChannel(string channel, string message)
        {
            // create a DataGram instance
            DataGram dataGram = new DataGram(channel, message);
            // Allocate the DataGram to a memory address contained in COPYDATASTRUCT
            Win32.COPYDATASTRUCT dataStruct = dataGram.ToStruct();
            // Use a filter with the EnumWindows class to get a list of windows containing
            // a property name that matches the destination channel. These are the listening
            // applications.
            WindowEnumFilter filter = new WindowEnumFilter(XDListener.GetChannelKey(channel));
            WindowsEnum winEnum = new WindowsEnum(filter.WindowFilterHandler);
            foreach (IntPtr hWnd in winEnum.Enumerate(Win32.GetDesktopWindow()))
            {
                // For each listening window, send the message data.
                Win32.SendMessage(hWnd, Win32.WM_COPYDATA, (int)IntPtr.Zero, ref dataStruct);
            }
            // free the memory
            dataGram.Dispose();
        }
    }
}
