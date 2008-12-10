using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MerlinEncrypt;

namespace Rhabot
{
    public partial class ISXCreateUser : Form
    {
        public ISXCreateUser()
        {
            InitializeComponent();
        }

        private void cmdCreate_Click(object sender, EventArgs e)
        {
#if DEBUG
            try
            {
                if (! ISXBotHelper.Settings.clsLogin.CreateUser(
                    this.txtUsername.Text,
                    this.txtPassword.Text,
                    this.txtEmail.Text,
                    this.txtIsxName.Text,
                    this.chkBeta.Checked,
                    this.chkPaid.Checked,
                    this.txtKey.Text))
                {
                    MessageBox.Show("Create user failed");
                    return;
                }

                // success
                MessageBox.Show("User Created");
                this.txtEmail.Text = string.Empty;
                this.txtIsxName.Text = string.Empty;
                this.txtKey.Text = string.Empty;
                this.txtPassword.Text = string.Empty;
                this.txtUsername.Text = string.Empty;
                this.chkPaid.Checked = false;
                this.chkBeta.Checked = false;
                this.txtUsername.Focus();
                CreatePassword();
            }

            catch (Exception excep)
            {
                ISXBotHelper.clsError.ShowError(excep, "Create User");
            }            
#endif
        }

        private void ISXCreateUser_Load(object sender, EventArgs e)
        {
            CreatePassword();
        }

        /// <summary>
        /// Create a random password for the new user
        /// </summary>
        private void CreatePassword()
        {
            StringBuilder sb = new StringBuilder();
            Random rand = new Random(DateTime.Now.Millisecond);

            try
            {
                // loop through and randomly create the password
                for (int i = 0; i < 7; i++)
                {
                    // random number 1-3
                    switch (rand.Next(1, 4))
                    {
                        case 1: // a-z (97-122)
                            sb.Append(Convert.ToChar(rand.Next(97, 123)));
                            break;
                        case 2: // A-Z (65-90)
                            sb.Append(Convert.ToChar(rand.Next(65, 91)));
                            break;
                        case 3: // 0-9 (48-57)
                            sb.Append(Convert.ToChar(rand.Next(48, 58)));
                            break;
                    }
                }

                // pop text and tooltip
                this.txtPassword.Text = sb.ToString();
                this.toolTip1.SetToolTip(this.txtPassword, sb.ToString());
            }

            catch (Exception excep)
            {
                ISXBotHelper.clsError.ShowError(excep, "Create Password");
                this.txtPassword.Text = string.Empty;
                this.toolTip1.SetToolTip(this.txtPassword, string.Empty);
            }            
        }
    }
}