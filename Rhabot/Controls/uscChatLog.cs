using System;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using Rhabot.Threaded_Controls;

namespace Rhabot
{
    public partial class uscChatLog : UserControl
    {
        public uscChatLog()
        {
            InitializeComponent();

            // hook the chat events
            clsWowEvents.SayChat += clsWowEvents_SayChat;
            clsWowEvents.PrivateChat += clsWowEvents_PrivateChat;
        }

        void clsWowEvents_PrivateChat(string author, string message)
        {
            clsChat_CommunicationReceieved(clsChat.EChannel.Private, author, message);
        }

        void clsWowEvents_SayChat(string author, string message)
        {
            clsChat_CommunicationReceieved(clsChat.EChannel.Say, author, message);
        }

        /// <summary>
        /// Raised when we get a message
        /// </summary>
        /// <param name="Channel">channel received on</param>
        /// <param name="FromName">who from</param>
        /// <param name="Message">the message</param>
        void clsChat_CommunicationReceieved(clsChat.EChannel Channel, string FromName, string Message)
        {
            try
            {
                clsSettings.Logging.AddToLog(Resources.uscChatLog, Resources.CommReceivedProcessing);

                string msg = string.Format("{0}: {1}\r\n", FromName, Message);

                // add to the correct text box
                ThreadTextbox txtBox;
                if (Channel == clsChat.EChannel.Say)
                    txtBox = this.txtSayChat;
                else
                    txtBox = this.txtWhispers;

                // insert and scroll
                txtBox.InsertAndScroll(msg);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscChatLog, Resources.CommunicationReceived);
            }
        }
    }
}
