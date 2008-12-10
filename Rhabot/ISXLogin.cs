using System;
using System.IO;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXBotHelper.Settings;
using System.Diagnostics;

namespace Rhabot
{
    public partial class ISXLogin : Form
    {
        public ISXLogin()
        {
            InitializeComponent();
        }

        private void ISXLogin_Load(object sender, EventArgs e)
        {
            try
            {
                // get log folder
                string logFolder = Path.Combine(clsSettings.GetAppPath, "Logs");
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                // set the log path
                clsSettings.LogFilePath = Path.Combine(logFolder,
                    string.Format("{6} {0}_{1}_{2}-{3}_{4}_{5}.log",
                        DateTime.Now.Month.ToString().Trim(),
                        DateTime.Now.Day.ToString().Trim(),
                        DateTime.Now.Year.ToString().Trim(),
                        DateTime.Now.Hour.ToString().Trim(),
                        DateTime.Now.Minute.ToString().Trim(),
                        DateTime.Now.Second.ToString().Trim(),
                        Resources.Rhabot));

                // load the global settings
                clsSettings.LoadRhabotSettings();

                // set the login name if we have it
                if (!string.IsNullOrEmpty(clsSettings.LastLoginName))
                {
                    this.txtUsername.Text = clsSettings.LastLoginName.Trim();
                    this.txtUsername.TabIndex = 1;
                    this.txtPassword.TabIndex = 0;
                }

                // start the timer
                this.timer1.Enabled = true;
                this.timer1.Start();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Login);
            }            
        }

        private void ISXLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // if not shutting down, then force a shutdown
                if (!clsSettings.IsShuttingDown)
                    clsSettings.Shutdown();
            }

            catch (Exception excep)
            {
#if DEBUG
                clsError.ShowError(excep, Resources.Login);
#endif
            }
        }

        private void cmdLogin_Click(object sender, EventArgs e)
        {
            DoLogin(this.txtUsername.Text, this.txtPassword.Text);
        }

        private void cmdFree_Click(object sender, EventArgs e)
        {
            // nologin, foo
            DoLogin(
                clsGlobals.ConvertToString(new byte[32] { 80, 171, 201, 39, 235, 192, 244, 146, 184, 5, 101, 19, 6, 201, 222, 62, 147, 35, 67, 122, 238, 159, 42, 88, 112, 224, 88, 247, 3, 208, 58, 22 }),
                clsGlobals.ConvertToString(new byte[32] { 108, 164, 212, 208, 85, 49, 202, 62, 135, 119, 138, 80, 74, 142, 133, 73, 147, 35, 67, 122, 238, 159, 42, 88, 112, 224, 88, 247, 3, 208, 58, 22 }));
        }

        /// <summary>
        /// Performs the login. Opens main form if successful
        /// </summary>
        private void DoLogin(string username, string password)
        {
            try
            {
                // Rhabot_ALogin
                this.Text = clsGlobals.SetFormText(Resources.RhabotAttemptingLogin);

                // clear the fields
                this.txtPassword.Text = string.Empty;
                this.txtUsername.Text = string.Empty;
                Application.DoEvents();

                // do the login
                string MyGUID = clsLogin.Login(username, password);

                // display server message
                if (!string.IsNullOrEmpty(clsLogin.ServerMessage))
                    MessageBox.Show(clsLogin.ServerMessage);

                // display download dialog
                if (!string.IsNullOrEmpty(clsLogin.DownloadLink))
                {
                    // ask if they want to download new version
                    if (MessageBox.Show(Resources.AnewversionofRhabotisavailable, Resources.NewRhabotVersionAvailable, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        clsGlobals.ExecuteFile(clsLogin.DownloadLink);

                        MessageBox.Show(Resources.youmustinstallthenewversion);
                    }

                    // fail login
                    this.Text = clsGlobals.SetFormText(Resources.UnabletoLogin);
                    this.txtUsername.Focus();
                    MyGUID = string.Empty;
                    return;
                }

                // if login failed, tell the user and exit
                if (string.IsNullOrEmpty(MyGUID))
                {
                    MessageBox.Show(Resources.Logininformationisinvalid);
                    this.txtUsername.Focus();
                    this.Text = clsGlobals.SetFormText(Resources.RhabotLogin);
                    return;
                }

                this.Text = clsGlobals.SetFormText(Resources.RhabotLoginSuccessful);
                Application.DoEvents();

                // set the last login name
                if (username != Resources.nologin)
                    clsSettings.LastLoginName = username.Trim();

                // set the guid
                clsSettings.MyGuid = MyGUID;
                this.Text = clsGlobals.SetFormText(Resources.RhabotLoading);
                Application.DoEvents();

                // open the main window
                clsGlobals.frmMain = new ISXMain();
                this.Hide();
                clsGlobals.frmMain.Show();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Login);
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
#if DEBUG
            // exit if incorrect info
            if ((this.txtUsername.Text != "bob") && (this.txtUsername.Text != "jones"))
                return;

            this.txtUsername.Text = string.Empty;
            this.txtPassword.Text = string.Empty;
            this.txtUsername.Focus();

            // open the create user window
            ISXCreateUser frmCreate = new ISXCreateUser();
            frmCreate.Show();
#endif
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // stop the timer
            this.timer1.Stop();
            this.timer1.Enabled = false;

#if DEBUG
            // CREATE New User

            //// John Warwick - for finding two bugs
            //string username = "Jarua";
            //string pswd = "p1o2i3u4";
            //string email = "shaolyen@gmail.com";

            //// create the user
            //if (clsLogin.CreateUser(username, pswd, email, "", false, true, ""))
            //    MessageBox.Show("User Created");
            //else
            //    MessageBox.Show("User NOT Created");
#endif


            // TESTING
#if DEBUG
#endif
        }
    }
}