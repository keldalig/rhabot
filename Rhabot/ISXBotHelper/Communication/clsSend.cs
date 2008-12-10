using System;
using ISXBotHelper.MSNChat;
using System.Net.Mail;
using System.Net;
using System.Threading;

using MerlinEncrypt;

namespace ISXBotHelper.Communication
{
    /// <summary>
    /// Functions for sending messages to MSN, SMS, Email clients
    /// </summary>
    public static class clsSend
    {
        private static string msg = string.Empty;
        private static string subj = string.Empty;

        public static void SendToAll_MSN()
        {
            SendToAll_MSN(msg);
        }

        /// <summary>
        /// Sends message to all MSN clients
        /// </summary>
        /// <param name="message">the message to send</param>
        public static void SendToAll_MSN(string message)
        {
            try
            {
                // loop through all MSN clients
                if ((clsMSNChat.conversateList != null) && (clsMSNChat.conversateList.Count > 0))
                {
                    foreach (clsMSNConversation conversate in clsMSNChat.conversateList)
                        conversate.SendMessage(message);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SendToAll_MSN");
            }            
        }

        public static void SendToAll_Email()
        {
            SendToAll_Email(msg, subj);
        }

        /// <summary>
        /// Sends to all email clients
        /// </summary>
        /// <param name="message">the message to send</param>
        /// <param name="subject">email subject</param>
        public static void SendToAll_Email(string message, string subject)
        {
            try
            {
                // exit if not full version
                if (!clsSettings.IsFullVersion)
                    return;

                // if no addresses, then exit
                if ((clsSettings.gclsGlobalSettings.Comm_Email_List == null) || (clsSettings.gclsGlobalSettings.Comm_Email_List.Count == 0))
                    return;

                // if we don't have server settings, then exit
                if ((string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.Comm_Email_SMTPServer)) || (clsSettings.gclsGlobalSettings.Comm_Email_SMTPPort <= 0))
                    return;

                // append server/character name if valid
                if (clsCharacter.CharacterIsValid)
                {
                    message = string.Format("Server: {0}\r\nCharacter:{1}\r\n\n{2}",
                        clsSettings.isxwow.AccountName, clsCharacter.CharacterName, message);
                }

                MailMessage mailMessage = new MailMessage();

                // To. 
                foreach (string emailAddy in clsSettings.gclsGlobalSettings.Comm_Email_List)
                    mailMessage.To.Add(new MailAddress(emailAddy));

                // pop email
                mailMessage.From = new MailAddress(clsSettings.gclsGlobalSettings.Comm_Email_List[0]);
                mailMessage.Body = message;
                mailMessage.IsBodyHtml = false;
                mailMessage.Subject = subject;
                mailMessage.Priority = MailPriority.Normal;

                // setup the smtp client, servername, and port
                SmtpClient client = new SmtpClient();
                client.Host = clsSettings.gclsGlobalSettings.Comm_Email_SMTPServer;
                client.Port = clsSettings.gclsGlobalSettings.Comm_Email_SMTPPort;

                // if we have a username and password, set authentication to true
                if ((!string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.Comm_Email_Usename)) && (!string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.Comm_Email_Password)))
                {
                    // decrypt the password
                    string decPswd = new Crypt().DecryptString(clsSettings.gclsGlobalSettings.Comm_Email_Password);

                    // set credentials
                    client.Credentials = new NetworkCredential(clsSettings.gclsGlobalSettings.Comm_Email_Usename, decPswd);
                }

                // send the email
                client.Send(mailMessage);
            }

            catch (Exception excep)
            {
                clsSettings.Logging.AddToLogFormatted("SendToAll", "Error in sending email: {0}", excep.Message);
            }
        }

        /// <summary>
        /// Sends to all connected MSN, SMS, Email, Log, and Debug clients
        /// </summary>
        /// <param name="message">the message to send</param>
        /// <param name="subject">the email subject</param>
        public static void SendToAll(string message, string subject)
        {
            try
            {
                // exit if no message
                if (string.IsNullOrEmpty(message))
                    return;

                // log it first
                clsSettings.Logging.AddToLog("SendToAll", message);

                // exit if not full version
                if (!clsSettings.IsFullVersion)
                    return;
                
                // set variables
                msg = message;
                subj = subject;

                // send to all clients
                new Thread(SendToAll_MSN).Start();
                new Thread(SendToAll_Email).Start();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SendToAll");
            }
        }
    }
}
