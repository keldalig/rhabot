using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXWoW;
using XihSolutions.DotMSN;

namespace ISXBotHelper.MSNChat
{
    /// <summary>
    /// Functions for sending/receiving over MSN chat
    /// </summary>
    internal class clsMSNChat : ThreadBase
    {
        #region Variables

        // Create a Messenger object to use DotMSN.
        private Messenger messenger = null;

        /// <summary>
        /// The list of contacts for this user
        /// </summary>
        private readonly List<Contact> contactList = new List<Contact>();

        /// <summary>
        /// List of conversations
        /// </summary>
        public static List<clsMSNConversation> conversateList = new List<clsMSNConversation>();

        // Variables
        #endregion

        #region Events

        internal delegate void ShuttingDownHandler();

        internal static event ShuttingDownHandler ShuttingDown;

        // Events
        #endregion

        #region Connect and Shutdown

        /// <summary>
        /// Connects to MSN and begins monitoring
        /// </summary>
        public void Connect()
        {
            try
            {
                // exit if not valid
                if (!clsSettings.GuidValid)
                    return;

                // if we don't have username/password, raise an error
                if ((string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.MSN_Username)) || (string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.MSN_Password)))
                {
                    clsError.ShowError(new Exception(Resources.MSNChatPleaseProvideUsernamePassword),
                        "MSN Chat", true, new StackFrame(0, true), false);
                    return;
                }

                // create our msn object
                if ((messenger != null) && (messenger.Connected))
                    messenger.Disconnect();
                messenger = new Messenger();

                // You can set proxy settings here
                // for example: messenger.ConnectivitySettings.ProxyHost = "10.0.0.2";

                // by default this example will emulate the official microsoft windows messenger client
                messenger.Credentials.ClientID = "msmsgs@msnmsgr.com";
                messenger.Credentials.ClientCode = "Q1P7W2E4J9R8U3S5";

                // hook events
                messenger.NameserverProcessor.ConnectionEstablished += NameserverProcessor_ConnectionEstablished;
                messenger.Nameserver.SignedIn += Nameserver_SignedIn;
                messenger.Nameserver.SignedOff += Nameserver_SignedOff;
                messenger.NameserverProcessor.ConnectingException += NameserverProcessor_ConnectingException;
                messenger.Nameserver.ExceptionOccurred += Nameserver_ExceptionOccurred;
                messenger.Nameserver.AuthenticationError += Nameserver_AuthenticationError;
                messenger.Nameserver.ServerErrorReceived += Nameserver_ServerErrorReceived;
                messenger.ConversationCreated += messenger_ConversationCreated;

                // set the credentials
                messenger.Credentials.Account = clsSettings.gclsGlobalSettings.MSN_Username;
                messenger.Credentials.Password = clsSettings.DecryptString(clsSettings.gclsGlobalSettings.MSN_Password);

                // inform the user what is happening and try to connecto to the messenger network.			
                clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.ConnectingtoMSNServer);

                // note that Messenger.Connect() will run in a seperate thread and return immediately.
                // it will fire events that informs you about the status of the connection attempt. 
                // these events are registered in the constructor.
                messenger.Connect();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "MSNChat.Connect", true, new StackFrame(0, true), false);
            }
        }

        /// <summary>
        /// Shuts down MSN chat
        /// </summary>
        public void DoShutdown()
        {
            try
            {
                // if we are not connected, then just exit
                if ((messenger == null) || (!messenger.Connected))
                    return;

                // raise the shutdown event
                if (ShuttingDown != null)
                {
                    clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.RaisingShutdownEventConversationThreads);
                    ShuttingDown();

                    // wait 5 seconds for everything to shutdown
                    clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.Sleeping5Seconds);
                    Thread.Sleep(5000);

                    // disconnect
                    clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.DisconnectingFromMSN);
                    messenger.Disconnect();
                }

                // shutdown finished
                clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.ShutdownFinished);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.MSNChat, "Shutdown Failure", true, new StackFrame(0, true), false);
            }

            finally
            {
                // try to disconnect
                try
                {
                    if (messenger.Connected)
                        messenger.Disconnect();
                }
                catch { }
            }
        }

        // Connect and Shutdown
        #endregion

        #region MSN Name Server Events

        void NameserverProcessor_ConnectionEstablished(object sender, EventArgs e)
        {
            clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.ConnectedMSN);
        }

        void Nameserver_ServerErrorReceived(object sender, MSNErrorEventArgs e)
        {
            clsError.ShowError(new Exception(e.MSNError.ToString()), "MSN Server Error", true, new StackFrame(0, true), false);
        }

        void Nameserver_AuthenticationError(object sender, ExceptionEventArgs e)
        {
            clsError.ShowError(e.Exception, "MSN Authentication failed, check your account or password", true, new StackFrame(0, true), false);
        }

        void Nameserver_ExceptionOccurred(object sender, ExceptionEventArgs e)
        {
            // ignore the unauthorized exception, since we're handling that error in another method.
            if (e.Exception is UnauthorizedException)
                return;

            // show the error
            clsError.ShowError(e.Exception, "MSN Nameserver Exception", true, new StackFrame(0, true), false);
        }

        void NameserverProcessor_ConnectingException(object sender, ExceptionEventArgs e)
        {
            // show the exception
            clsError.ShowError(e.Exception, "MSN Connection failed", true, new StackFrame(0, true), false);
        }

        void Nameserver_SignedOff(object sender, SignedOffEventArgs e)
        {
            clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.SignedOutMSN);
        }

        void Nameserver_SignedIn(object sender, EventArgs e)
        {
            clsSettings.Logging.AddToLogFormatted(Resources.SignedIntoMSN, clsSettings.gclsGlobalSettings.MSN_Username);

            // set our presence status
            messenger.Owner.Status = PresenceStatus.Online;

            // pop the contact list
            foreach (Contact contact in messenger.ContactList.All)
            {
                // add to the list
                contactList.Add(contact);

                // debug it
                clsSettings.Logging.DebugWrite(Resources.MSNChat, string.Format(Resources.AddingContact, contact.Name));
            }

            // start the conversations
            StartConversation();
        }

        // MSN Name Server Events
        #endregion

        #region Conversation Functions

        /// <summary>
        /// Starts a conversation with all contacts. Opens in a new thread 
        /// </summary>
        private void StartConversation()
        {
            // exit if we have no contacts
            if (contactList.Count == 0)
            {
                clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.StartConversationExiting);
                return;
            }

            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.StartConversation);

                // loop through contacts and start conversations for each
                foreach (Contact contact in contactList)
                {
                    // add to the list
                    conversateList.Add(new clsMSNConversation(messenger.CreateConversation(), contact));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "StartConversation", true, new StackFrame(0, true), false);
            }
        }

        // Conversation Functions
        #endregion

        #region Conversation Events

        /// <summary>
        /// Raised when a conversation has been started (either locally, or remotely)
        /// </summary>
        void messenger_ConversationCreated(object sender, ConversationCreatedEventArgs e)
        {
            try
            {
                // if a remote conversation, add to the conversation list
                if (e.Initiator == null)
                {
                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.MSNChat, Resources.ConversationInitiatedRemotely);

                    // start a conversation thread
                    conversateList.Add(new clsMSNConversation(e.Conversation, null));
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "messenger_ConversationCreated", true, new StackFrame(0, true), false);
            }
        }

        // Conversation Events
        #endregion
    }

    #region clsMSNConversation

    /// <summary>
    /// Handles a conversation
    /// </summary>
    internal class clsMSNConversation : ThreadBase
    {
        private readonly Conversation m_Conversation = null;
        private Contact m_Contact = null;
        private readonly clsSettings.ThreadItem threadItem;
        private clsSettings.ThreadItem threadItemInvite;

        // set for streaming logs to this contact
        public DateTime StreamEndTime = DateTime.MinValue;

        /// <summary>
        /// Creates a new conversation class
        /// </summary>
        /// <param name="conversation"></param>
        /// <param name="contact"></param>
        public clsMSNConversation(Conversation conversation, Contact contact)
        {
            // log it
            clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.NewConversationThreadStarted);

            // assign params
            m_Contact = contact;
            m_Conversation = conversation;

            // hook the shutting down event
            clsMSNChat.ShuttingDown += clsMSNChat_ShuttingDown;

            // launch in new thread
            Thread thread = new Thread(Conversate_Thread);
            thread.Name = Resources.MSNChat;
            threadItem = new clsSettings.ThreadItem(thread, this);
            clsSettings.ThreadList.Add(threadItem);
            thread.Start();
        }

        /// <summary>
        /// Raised when we are shutting down
        /// </summary>
        void clsMSNChat_ShuttingDown()
        {
            try
            {
                clsSettings.Logging.DebugWrite(Resources.MSNChat, string.Format("Shutting Down Conversation with {0}", m_Contact != null ? m_Contact.Name : "unknown"));

                // send shutdown text via chat
                SendMessage(string.Format(Resources.MSNRhabotShuttingDown, clsMSNCmd.cmd_PlainText, DateTime.Now));

                // close the conversation
                Application.DoEvents();
                m_Conversation.Switchboard.Close();

                // kill the threads
                clsSettings.KillThread(threadItem, string.Empty);
                clsSettings.KillThread(threadItemInvite, string.Empty);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "MSN Chat: Shutdown Error", true, new StackFrame(0, true), false);
            }
        }
        /// <summary>
        /// Handles a conversation in a new thread
        /// </summary>
        private void Conversate_Thread()
        {
            // hook the conversation events
            m_Conversation.Switchboard.TextMessageReceived += Switchboard_TextMessageReceived;
            m_Conversation.Switchboard.SessionClosed += Switchboard_SessionClosed;
            m_Conversation.Switchboard.ContactJoined += Switchboard_ContactJoined;
            m_Conversation.Switchboard.ContactLeft += Switchboard_ContactLeft;

            // if we have a contact, invite him
            if ((m_Contact != null) && (m_Contact.Online))
                m_Conversation.Invite(m_Contact);
            else
            {
                Thread thread_Invite = new Thread(Invite_Thread);
                thread_Invite.Name = string.Format("MSN Invite {0}", m_Contact.Name);
                threadItemInvite = new clsSettings.ThreadItem(thread_Invite, this);
                clsSettings.ThreadList.Add(threadItemInvite);
                thread_Invite.Start();
            }
        }

        private void Invite_Thread()
        {
            while (true)
            {
                // exit if shutting down
                if ((Shutdown) || (clsSettings.IsShuttingDown))
                    return;

                // if we have a contact, invite him
                if ((m_Contact != null) && (m_Contact.Online))
                {
                    m_Conversation.Invite(m_Contact);
                    return;
                }

                // sleep for 1 second
                Thread.Sleep(1000);
            }
        }

        #region Conversation Events

        void Switchboard_ContactLeft(object sender, ContactEventArgs e)
        {
            try
            {
                // log contact left
                clsSettings.Logging.AddToLogFormatted(Resources.MSNChat, Resources.XleftConverstation, e.Contact.Name);

                // disconnect
                if (m_Conversation != null)
                    m_Conversation.Switchboard.Close();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Switchboard_ContactLeft", true, new StackFrame(0, true), false);
            }
        }

        void Switchboard_ContactJoined(object sender, ContactEventArgs e)
        {
            try
            {
                // log contact joined
                clsSettings.Logging.AddToLogFormatted(Resources.MSNChat, Resources.XjoinedConversation, e.Contact.Name);

                // set contact
                m_Contact = e.Contact;

                // send the welcome messages
                StringBuilder sb = new StringBuilder();

                // send hello
                //sb.Append(clsMSNCmd.cmd_PlainText);
                sb.AppendFormat(Resources.HelloRhabotMSNVersion, clsSettings.BotVersion);
                SendMessage(sb.ToString());

                // send player name, info
                sb = new StringBuilder();
                //sb.Append(clsMSNCmd.cmd_PlainText);
                using (new clsFrameLock.LockBuffer())
                {
                    sb.AppendFormat(Resources.MSNCharInfo,
                                    clsCharacter.CharacterName,
                                    clsCharacter.CurrentLevel,
                                    clsCharacter.ZoneText,
                                    clsCharacter.MyLocation);
                }
                SendMessage(sb.ToString());

                // send help info
                SendMessage(clsMSNCmd.GetHelp());
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Switchboard_ContactJoined", true, new StackFrame(0, true), false);
            }
        }

        void Switchboard_SessionClosed(object sender, EventArgs e)
        {
            try
            {
                if (m_Contact != null)
                    clsSettings.Logging.AddToLogFormatted(Resources.MSNChat, Resources.SessionXClosed, m_Contact.Name);
                else
                    clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.SessionUnknownClosed);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Switchboard_SessionClosed", true, new StackFrame(0, true), false);
            }
        }

        /// <summary>
        /// We got a message from someone
        /// </summary>
        void Switchboard_TextMessageReceived(object sender, TextMessageEventArgs e)
        {
            string tempStr, msg, nl = "\r\n";

            try
            {
                // get the message
                msg = e.Message.Text;

                // log the message
                clsSettings.Logging.AddToLogFormatted(Resources.MSNChat, Resources.MessageFromX, e.Sender.Name, msg);

                // get the first 10 characters of the message
                if (msg.Length > 10)
                    tempStr = msg.Substring(0, 10);
                else
                    tempStr = msg.Clone().ToString().ToUpper();

                // handle the message by checking for commands
                if (tempStr.Contains(clsMSNCmd.cmd_PlainText)) // plaintext, so ignore
                    return;

                // user asked for help
                if (tempStr.Contains(clsMSNCmd.cmd_Help))
                {
                    // send the help message
                    SendMessage(clsMSNCmd.GetHelp());
                    return;
                }

                // logout
                if (tempStr.Contains(clsMSNCmd.cmd_Logout))
                {
                    // send response
                    SendMessage(Resources.LogoutRequestReceived);

                    // logout and exit
                    clsSettings.Logout(true);
                    clsSettings.Shutdown();
                    return;
                }

                // location
                if (tempStr.Contains(clsMSNCmd.cmd_Location))
                {
                    SendMessage(string.Format("Zone: {0}  -  Location: {1}",
                        clsSettings.isxwow.ZoneText,
                        clsCharacter.MyLocation));
                    return;
                }

                // character info
                if (tempStr.Contains(clsMSNCmd.cmd_CharacterInfo))
                {
                    // Returns the character's information (level, isdead, isincombat, zone, location)
                    SendMessage(string.Format(Resources.MSNCharInfo2, nl,
                        clsCharacter.CurrentLevel.ToString().Trim(),
                        clsCharacter.IsDead,
                        clsCombat.IsInCombat(),
                        clsSettings.isxwow.ZoneText,
                        clsCharacter.MyLocation,
                        clsCombat.IsInCombat() ? string.Format(Resources.MSNCharTarget, nl, clsCombat.GetUnitAttackingMe().Name, clsCombat.GetUnitAttackingMe().GUID) : string.Empty));
                    return;
                }

                // stream logs
                if (tempStr.Contains(clsMSNCmd.cmd_StreamLog))
                {
                    // end stream in 5 minutes
                    StreamEndTime = DateTime.Now.AddMinutes(5);

                    // send response
                    SendMessage(string.Format(Resources.RhabotLogsStream, StreamEndTime.ToShortTimeString()));
                    return;
                }

                // stop streaming logs
                if (tempStr.Contains(clsMSNCmd.cmd_StopStreamLog))
                {
                    // end streaming
                    StreamEndTime = DateTime.MinValue;

                    // send response
                    SendMessage(Resources.RhabotLogStreamStop);
                    return;
                }

                // hook chat events
                if (tempStr.Contains(clsMSNCmd.cmd_HookChat))
                {
                    // tell chat to hook the events
                    clsChat.HookChatChannel(clsChat.EChannel.Say);
                    clsChat.HookChatChannel(clsChat.EChannel.Private);

                    // hook the internal events
                    clsChat.CommunicationReceieved += clsChat_CommunicationReceieved;
                    return;
                }

                // stone home
                if (tempStr.Contains(clsMSNCmd.cmd_StoneHome))
                {
                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.MSNChat, Resources.ReceivedCommandStonehome);

                    // stone home
                    clsCharacter.StoneHome();

                    // send response
                    SendMessage(Resources.StonehomeCompleted);
                    return;
                }

                // hook errors
                if (tempStr.Contains(clsMSNCmd.cmd_HookErrorEvents))
                {
                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.MSNChat, Resources.ReceivedCommandhookErrorEvents);

                    // hook the error event
                    clsError.RhabotError += clsError_RhabotError;

                    // send response and exit
                    SendMessage(Resources.RhabotErrorsHooked);
                    return;
                }

                // logout after X seconds
                if (tempStr.Contains(clsMSNCmd.cmd_LogoutX))
                {
                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.MSNChat, Resources.LogoutXCommandEeceived, tempStr);

                    // get logout time
                    string[] time = Regex.Split(tempStr, " ");
                    
                    // if have no time, then cancel and exit
                    if ((time.Length < 2) || (!clsSettings.IsNumeric(time[1].Trim())))
                    {
                        clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.CancellingLogoutAfterX);
                        clsSettings.LogoutAtTime(DateTime.MinValue);
                    }
                    else
                    {
                        // logout after X time
                        clsSettings.Logging.AddToLogFormatted(Resources.MSNChat, Resources.LoggingOutAfterX, time[1]);
                        clsSettings.LogoutAtTime(DateTime.Now.AddSeconds(Convert.ToInt32(time[1].Trim())));
                    }

                    // send response and exit
                    SendMessage(tempStr);
                    return;
                }

                // pause script
                if (tempStr.Contains(clsMSNCmd.cmd_PauseScript))
                {
                    // log it
                    clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.PausingRhabot);

                    // pause it
                    clsSettings.Pause = true;

                    // send response and exit
                    SendMessage(Resources.RhabotPaused);
                    return;
                }

                // unpause script
                if (tempStr.Contains(clsMSNCmd.cmd_UnpauseScript))
                {
                    // log it
                    clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.UnpausingRhabot);

                    // pause it
                    clsSettings.Pause = false;

                    // send response and exit
                    SendMessage(Resources.RhabotUnpaused);
                    return;
                }

                // stop script
                if (tempStr.Contains(clsMSNCmd.cmd_StopScript))
                {
                    // log it
                    clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.StoppingRhabot);

                    // pause it
                    clsSettings.Stop = true;

                    // send response and exit
                    SendMessage(Resources.RhabotStopped);
                    return;
                }

                // inventory list
                if (tempStr.Contains(clsMSNCmd.cmd_InvetoryList))
                {
                    StringBuilder inventorySB = new StringBuilder();

                    // log it
                    clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.InventoryList);

                    // get the list of items in our bag
                    List<WoWItem> bagItems = clsCharacter.GetBagItems();

                    // if no items, return no items
                    if ((bagItems == null) || (bagItems.Count == 0))
                    {
                        clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.NoItemsInInventory);
                        SendMessage(Resources.NoItemsInInventory);
                        return;
                    }

                    inventorySB.AppendFormat(Resources.InventoryListX, clsCharacter.CharacterName);

                    // loop through all items
                    foreach (WoWItem bItem in bagItems)
                        inventorySB.AppendFormat(Resources.MSNItemInfo,
                            bItem.Name, bItem.Bag.Number, bItem.Slot);

                    // send the list
                    SendMessage(inventorySB.ToString());
                    return;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Switchboard_TextMessageReceived", true, new StackFrame(0, true), false);
            }
        }

        /// <summary>
        /// Raised when rhabot encounters an error
        /// </summary>
        /// <param name="excep"></param>
        /// <param name="Message"></param>
        void clsError_RhabotError(Exception excep, string Message)
        {
            SendMessage(string.Format("Rhabot Error:\r\nMessage: {0}\r\nException: {1}", Message, excep.Message));
        }

        // Conversation Events
        #endregion

        #region Functions

        public void SendMessage(string msg)
        {
            try
            {
                // if there is no switchboard available, request a new switchboard session
                if (! m_Conversation.SwitchboardProcessor.Connected)
                    m_Conversation.Messenger.Nameserver.RequestSwitchboard(m_Conversation.Switchboard, this);

                // if we have no contacts, log it and exit
                if (m_Conversation.Switchboard.Contacts.Count == 0)
                {
                    clsSettings.Logging.AddToLog(Resources.MSNChat, Resources.SendMessageFailed);
                    return;
                }

                // create the message
                TextMessage message = new TextMessage(msg);

                /* You can optionally change the message's font, charset, color here.
                 * For example:
                 * message.Color = Color.Red;
                 * message.Decorations = TextDecorations.Bold;
                 */

                // debug the message
                clsSettings.Logging.DebugWrite(Resources.MSNChat, string.Format("To: {0}. Message: {1}", m_Contact.Name, msg));

                // send the mesage
                m_Conversation.Switchboard.SendTextMessage(message);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SendMessage", true, new StackFrame(0, true), false);
            }
        }

        // Functions
        #endregion

        #region In Game Chat

        /// <summary>
        /// Raised when we get an in game chat message
        /// </summary>
        /// <param name="Channel">the channel received on</param>
        /// <param name="FromName">who sent the message</param>
        /// <param name="Message">who the message is from</param>
        void clsChat_CommunicationReceieved(clsChat.EChannel Channel, string FromName, string Message)
        {
            try
            {
                clsSettings.Logging.DebugWrite(Resources.MSNChat, "WoW Communication Received - Processing");

                string channel = string.Empty;

                // send msn message
                switch (Channel)
                {
                    case clsChat.EChannel.Say:
                        channel = "say";
                        break;
                    case clsChat.EChannel.Private:
                        channel = "whisper";
                        break;
                    case clsChat.EChannel.Trade:
                    case clsChat.EChannel.General:
                    case clsChat.EChannel.Guild:
                    case clsChat.EChannel.Officer:
                    case clsChat.EChannel.LFG:
                        return;
                }

                // send the client the message
                SendMessage(string.Format(Resources.MSNChatFrom,
                    channel, FromName, Message));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.MSNChat, "Communication Received Error", true, new StackFrame(0, true), false);
            }
        }

        // In Game Chat
        #endregion
    }

    // clsMSNConversation
    #endregion
}
