// Event Names: 
// http://www.wowwiki.com/Events/Names
// http://www.wowwiki.com/Events/Communication

// Hooking Events:
// http://www.isxwow.net/forums/viewtopic.php?f=17&t=928&e=0
// http://www.isxwow.net/forums/viewtopic.php?f=15&t=611

using System;
using ISXBotHelper.Properties;

namespace ISXBotHelper
{
    /// <summary>
    /// Functions for dealing with chat
    /// </summary>
    public static class clsChat
    {
        #region Enums

        public enum EChannel
        {
            Say = 0,
            Private,

            /// <summary>
            /// Not Implemented
            /// </summary>
            Trade,

            /// <summary>
            /// Not Implemented
            /// </summary>
            General,

            /// <summary>
            /// Not Implemented
            /// </summary>
            Guild,

            /// <summary>
            /// Not Implemented
            /// </summary>
            Officer,

            /// <summary>
            /// Not Implemented
            /// </summary>
            LFG
        }

        // Enums
        #endregion

        #region Events

        public delegate void CommunicationReceievedHandler(EChannel Channel, string FromName, string Message);

        /// <summary>
        /// Raised when a communication is received
        /// </summary>
        public static event CommunicationReceievedHandler CommunicationReceieved;

        // Events
        #endregion

        #region Hook

        /// <summary>
        /// Starts listenning for events on the specified channel
        /// </summary>
        /// <param name="Channel">the channel to listen on</param>
        public static void HookChatChannel(EChannel Channel)
        {
            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.ListeningChatChannelX, Channel.ToString());

                // instantiate event if it is null
                //if (wowEvent == null)
                //    wowEvent = WoWEvents.GetInstance();

                // hook the event
                switch (Channel)
                {
                    case EChannel.Say:
                        // hook
                        clsWowEvents.SayChat += clsWowEvents_SayChat;
                        break;
                    case EChannel.Private:
                        // hook 
                        clsWowEvents.PrivateChat += clsWowEvents_PrivateChat;
                        break;
                    case EChannel.Trade:
                        
                        break;
                    case EChannel.General:
                        
                        break;
                    case EChannel.Guild:
                        
                        break;
                    case EChannel.Officer:
                        
                        break;
                    case EChannel.LFG:
                        
                        break;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Format("HookChatChannel - {0}", Channel));
            }            
        }

        // Hook
        #endregion

        #region Private Chat Functions

        static void clsWowEvents_PrivateChat(string author, string message)
        {
            try
            {
                // raise the event if hooked
                if (CommunicationReceieved != null)
                {
                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.ReceivedWhisperFromX, author, message);

                    // raise it
                    CommunicationReceieved(EChannel.Private, author, message);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PrivateChat");
            }
        }

        static void clsWowEvents_SayChat(string author, string message)
        {
            try
            {
                // raise the event if hooked
                if (CommunicationReceieved != null)
                {
                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.ReceivedSayFromX, author, message);

                    // raise it
                    CommunicationReceieved(EChannel.Say, author, message);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SayChat");
            }
        }

        // Private Chat Functions
        #endregion

        #region Send Message

        /// <summary>
        /// Sends a message on the specified channel
        /// </summary>
        /// <param name="Channel">channel to send on</param>
        /// <param name="Message">the message to send</param>
        /// <param name="MessageTo">To, if a private message</param>
        /// <returns></returns>
        public static bool SendMessage(EChannel Channel, string Message, string MessageTo)
        {
            bool rVal = false;

            try
            {
                clsSettings.Logging.AddToLogFormatted(Resources.ChatSendMessageX,
                    Channel.ToString(), MessageTo, Message);

                // find the chat type
                string chatType;
                switch (Channel)
                {
                    case EChannel.Say:
                        chatType = "SAY";
                        break;
                    case EChannel.Private:
                        chatType = "WHISPER";
                        break;
                    case EChannel.Guild:
                        chatType = "GUILD";
                        break;
                    case EChannel.Officer:
                        chatType = "OFFICER";
                        break;
                    case EChannel.LFG:
                    case EChannel.Trade:
                    case EChannel.General:
                    default:
                        return true;
                }

                // send the message
                clsSettings.Logging.AddToLogFormatted(Resources.ChatSendMessageX2,
                    chatType, Message);
                rVal = SendChatMessage(Message, chatType, MessageTo);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Format("SendMessage: Channel {0} - Message: {1}", Channel, Message));
            }
            
            return rVal;
        }

        // http://www.wowwiki.com/API_SendChatMessage
        /// <summary>
        /// Sends a chat message to the specified channel
        /// </summary>
        /// <param name="msg">the message to send</param>
        /// <param name="chatType">http://www.wowwiki.com/ChatTypeId</param>
        /// <param name="MsgTo">the person receiving the message if chatType is whisper</param>
        private static bool SendChatMessage(string msg, string chatType, string MsgTo)
        {
            bool rVal = false;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.SendingMessageToX,
                    MsgTo, chatType, msg);

                // send the message
                rVal = clsSettings.ExecuteWoWAPI(string.Format("SendChatMessage(\"{0}\", \"{1}\", nil, \"{2}\")",
                    msg, chatType, MsgTo));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SendChatMessage");
            }
            
            // return
            return rVal;
        }

        // Send Message
        #endregion

        #region AFK/DND

        /// <summary>
        /// Sets the AFK message. 
        /// </summary>
        /// <param name="message">the AFK message. Send blank to remove AFK message</param>
        public static bool SetAFKMessage(string message)
        {
            bool rVal = false;
            try
            {
                rVal = SendChatMessage(message, "AFK", string.Empty);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SetAFKMessage");
            }

            return rVal;
        }

        /// <summary>
        /// Sets the DND message. 
        /// </summary>
        /// <param name="message">the DND message. Send blank to remove DND message</param>
        public static bool SetDNDMessage(string message)
        {
            bool rVal = false;
            try
            {
                rVal = SendChatMessage(message, "DND", string.Empty);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SetDNDMessage");
            }

            return rVal;
        }

        // AFK/DND
        #endregion

        #region Shutdown

        /// <summary>
        /// Detaches all events
        /// </summary>
        internal static void Shutdown()
        {
            try
            {
                // unhook events
                clsWowEvents.SayChat -= clsWowEvents_SayChat;
                clsWowEvents.PrivateChat -= clsWowEvents_PrivateChat;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Shutting down clsChat");
            }
        }

        // Shutdown
        #endregion
    }
}
