using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices; // used for mouse API
using System.Windows.Forms;
using ISXWoW;
using LavishVMAPI;
using LavishScriptAPI;

namespace Tutorials
{
    public class clsLogin
    {
        #region Login Functions

        /// <summary>
        /// Login and select a character to load. Returns false is disconnected or login error
        /// </summary>
        /// <param name="username">wow username</param>
        /// <param name="password">wow password</param>
        /// <param name="CharacterName">character to load</param>
        public bool Login(string username, string password, string CharacterName)
        {
            string tempStr = "";

            using (new clsFrameLock.LockBuffer())
            {
                // make sure we have a login window visible and the glue dialog (message box) is not visible
                if ((!ISXWoW.UI.AccountLoginFrame.IsVisible) || (ISXWoW.UI.GlueDialogFrame.IsVisible))
                    return false; // glue is showing, or login is not visible

                // try to login
                ISXWoW.UI.AccountLoginFrame.TryLogin(username, password);
            }

            // while the glue dialog is up, make sure it doesn't show an error message
            while (GlueVisible())
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // get the text of the glue dialog
                    tempStr = ISXWoW.UI.GlueDialogFrame.Text;
                }

                // if disconnected or login error, return false
                if ((tempStr.Contains("Disconnect")) || (tempStr.Contains("Please check the spelling")))
                    return false;
            }

            // sleep a few seconds to allow character select screen to load
            System.Threading.Thread.Sleep(5000);

            // if the character select screen is not shown, we probably have a problem, so exit
            using (new clsFrameLock.LockBuffer())
            {
                if (!ISXWoW.UI.CharacterSelectFrame.IsVisible)
                    return false;
            }

            // we have a character select screen, let's find our character
            if (ISXWoW.UI.CharacterSelectFrame.SelectCharacter(CharacterName))
                LavishScript.ExecuteCommand("press enter");

            // wait a few seconds to make sure the click worked
            System.Threading.Thread.Sleep(5000);

            // if the character screen is still showing, we have a problem
            using (new clsFrameLock.LockBuffer())
            {
                if (!ISXWoW.UI.CharacterSelectFrame.IsVisible)
                    return false;
            }

            // since we made it here, everything worked ok
            return true;
        }

        /// <summary>
        /// Returns the visibility of the glue window
        /// </summary>
        private bool GlueVisible()
        {
            // get the visibility of the glue dialog
            using (new clsFrameLock.LockBuffer())
            {
                return ISXWoW.UI.GlueDialogFrame.IsVisible;
            }
        }

        // Login Functions
        #endregion
    }
}
