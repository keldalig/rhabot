using System;
using System.Threading;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using LavishScriptAPI;

namespace Rhabot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG
            // show an exception when locks are used improperly
            //LavishScript.RequireExplicitFrameLock = true;
#endif
            Application.Run(new ISXLogin());
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // skip if thread abort
            if (e.Exception.Message.ToLower().Contains(Resources.abort))
                return;

            // show the display
            clsError.ShowError(e.Exception, "Global Error");
        }
    }
}