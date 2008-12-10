using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ISXBotHelper.Properties;
using Rhabot;
using rs;
using System.Collections.Generic;

namespace ISXBotHelper.Uploads
{
    /// <summary>
    /// Uploads logs to the rhabot server
    /// </summary>
    internal static class clsUploadLog
    {
        public static void UploadLogs()
        {
            frmUploadLog frmProgress = null;

            try
            {
                // create the progress window
                frmProgress = new frmUploadLog();

                // set the form's text
                frmProgress.SetText(clsGlobals.SetFormText(Resources.UploadRhabotLogs));

                // show the form
                frmProgress.Show();

                // upload files
                DoUpload(frmProgress);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.UploadLog);
            }            

            finally
            {
                // close the form if it is still open
                if ((frmProgress != null) && (frmProgress.Visible))
                    frmProgress.Close();
            }
        }

        private static void DoUpload(frmUploadLog frmProgress)
        {
            List<string> LogFiles;
            int counter = 5, j = 1;
            clsRS crs = new clsRS();

            try
            {
                // get the last five logs
                LogFiles = new List<string>(Directory.GetFiles(Path.GetDirectoryName(clsSettings.LogFilePath)));

                // if no files, then exit
                if ((LogFiles == null) || (LogFiles.Count == 0))
                {
                    clsError.ShowError(new Exception(Resources.NoLogFilesFound), Resources.UploadLog, string.Empty, true, new StackFrame(0, true), false);
                    return;
                }

                // sort the log files, put newest first
                LogFiles.Sort(LogFileSort);
                LogFiles.Reverse();

                // change log counter if we have less than 5 logs
                if (LogFiles.Count < 5)
                    counter = LogFiles.Count;
                counter = LogFiles.Count - counter + 1;

                // set max value
                frmProgress.SetMaxValue(counter + 1);

                // upload the five newest logs
                for (int i = LogFiles.Count - 1; i > counter; i--)
                {
                    // update the label
                    frmProgress.SetLabelText(string.Format(Resources.UploadingLogX, Path.GetFileName(LogFiles[i])));

                    // update the counter
                    frmProgress.SetValue(j++);
                    Application.DoEvents();

                    // read the file
                    string LogText = File.ReadAllText(LogFiles[i]);

                    // upload it
                    if (!crs.UploadLog(clsSettings.LoginInfo.UserID, Path.GetFileName(LogFiles[i]), LogText, clsSettings.UpdateText, clsSettings.IsDCd))
                        clsError.ShowError(new Exception(string.Format(Resources.UploadingLogError, LogFiles[i])), Resources.UploadLog, string.Empty, true, new StackFrame(0, true), false);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.UploadLog);
            }
        }

        /// <summary>
        /// Compares the date/time for each file
        /// </summary>
        private static int LogFileSort(string File1, string File2)
        {
            return File.GetCreationTime(File1).CompareTo(File.GetCreationTime(File2));
        }
    }
}
