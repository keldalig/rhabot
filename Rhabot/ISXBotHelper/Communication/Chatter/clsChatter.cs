using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ISXBotHelper.Communication;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXBotHelper.XProfile;

namespace ISXBotHelper.Chatter
{
    /// <summary>
    /// This class will use the Verbot 4 SDK to talk with users in wow.
    /// For now, we are just going to log chats and suggested responses
    /// See the VConsole app in the Verbot SDK folder for a working example
    /// http://www.codeproject.com/useritems/VerbotSDK.asp
    /// </summary>
    public class clsChatter : ThreadBase
    {
        #region Variables

        private clsSettings.ThreadItem threadItem;
        private const string section = "Chatter";
        private const string attribute = "Output_";
        private const string entryName = "Input_";

        private string m_ChatterKBFile = string.Empty;
        /// <summary>
        /// The location of the chatter knowledge base file
        /// </summary>
        public string ChatterKBFile
        {
            get { return m_ChatterKBFile; }
            set { m_ChatterKBFile = value; }
        }

        private List<clsChatterRuleInfo> RuleList = new List<clsChatterRuleInfo>();

        // Variables
        #endregion

        #region Public Functions

        /// <summary>
        /// Starts the chatter bot
        /// </summary>
        public void StartChatter()
        {
            // exit if not valid
            if (!clsSettings.GuidValid)
                return;

            // exit if we have no kb file
            if (!File.Exists(ChatterKBFile))
            {
                clsError.ShowError(new Exception(string.Format(Resources.ChatterScriptFileNotFound, ChatterKBFile)), Resources.Chatter, true, new StackFrame(0, true), false);
                return;
            }

            // start in new thread
            Thread thread = new Thread(StartChatter_Thread);
            thread.Name = "Chatter";
            threadItem = new clsSettings.ThreadItem(thread, this);
            clsSettings.ThreadList.Add(threadItem);
            thread.Start();
        }

        /// <summary>
        /// Shuts down the chatter bot
        /// </summary>
        public void DoShutdown()
        {
            try
            {
                Shutdown = true;

                // shutdown the thread
                clsSettings.KillThread(threadItem, Resources.ChatterShuttingdown);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Chatter.Shutdown");
            }
        }

        /// <summary>
        /// Returns a list of the chatter rules in this kb
        /// </summary>
        public List<clsChatterRuleInfo> GetChatterRules()
        {
            List<clsChatterRuleInfo> retList = new List<clsChatterRuleInfo>();
            clsChatterRuleInfo rInfo;
            Xml xml;
            string[] entries;
            string tempStr;

            try
            {
                // exit if no rule file
                if (string.IsNullOrEmpty(ChatterKBFile))
                    return null;

                // load the kb file
                xml = new Xml(ChatterKBFile);

                using (xml.Buffer(true))
                {
                    // get the entry list
                    entries = xml.GetEntryNames(section);

                    // exit if no entries
                    if ((entries == null) || (entries.Length == 0))
                        return new List<clsChatterRuleInfo>();

                    // loop through and get the list
                    foreach (string entry in entries)
                    {
                        // pop the rule info
                        rInfo = new clsChatterRuleInfo(clsSettings.XMLGet_String(section, entry, xml).Replace("\\'", string.Empty));

                        // loop through attributes
                        for (int i = 0; i < 100; i++)
                        {
                            // get the value
                            tempStr = xml.GetValue_Attribute(section, entry, string.Format("{0}{1}", attribute, i.ToString().Trim()));

                            // exit if no value
                            if (string.IsNullOrEmpty(tempStr))
                                break;

                            // replace changed characters
                            tempStr = tempStr.Replace("\\'", "\"");

                            // add the value
                            rInfo.OutputMessages.Add(tempStr);
                        }

                        // add rinfo to the rule list
                        retList.Add(rInfo);
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetChatterRules");
            }

            return retList;
        }

        /// <summary>
        /// Builds a new knowledgebase file. returns true on success
        /// </summary>
        public bool BuildNewKnowledgeBase(List<clsChatterRuleInfo> ruleList)
        {
            bool rVal = false;
            Xml xml;
            List<Xml.XAttribute> attribList;
            int entryCounter = 0;

            try
            {
                clsSettings.Logging.AddToLogFormatted(Resources.Chatter, Resources.BuildNewKnowledgeBase, ChatterKBFile);

                // open the xml file
                xml = new Xml(ChatterKBFile);

                // buffer
                using (xml.Buffer(true))
                {
                    // loop through the rules and process
                    foreach (clsChatterRuleInfo rInfo in ruleList)
                    {
                        int attribCounter = 0;

                        // build the attribute list
                        attribList = new List<Xml.XAttribute>();

                        // add the outputs
                        foreach (string output in rInfo.OutputMessages)
                            attribList.Add(new Xml.XAttribute(
                                string.Format("{0}{1}", attribute, Convert.ToString(attribCounter++)),
                                output.ToLower()));

                        // save the rule
                        xml.SetValue(
                            section, 
                            string.Format("{0}{1}", entryName, Convert.ToString(entryCounter++).Trim()), 
                            rInfo.InputMessage.ToLower(), attribList);
                    }
                }

                rVal = true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "BuildNewKnowledgeBase");
            }

            return rVal;
        }

        // Public Functions
        #endregion

        #region Private Functions

        /// <summary>
        /// The chatter is started in a new thread
        /// </summary>
        private void StartChatter_Thread()
        {
            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.ChatterStarted);

                // build the rule list
                RuleList = GetChatterRules();

                // hook the communication events
                clsWowEvents.PrivateChat += clsWowEvents_PrivateChat;
                clsWowEvents.SayChat += clsWowEvents_SayChat;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "StartChatter_Thread");
            }
        }

        private void clsWowEvents_SayChat(string author, string message)
        {
            CommunicationReceieved(clsChat.EChannel.Say, author, message);
        }

        void clsWowEvents_PrivateChat(string author, string message)
        {
            CommunicationReceieved(clsChat.EChannel.Private, author, message);
        }

        /// <summary>
        /// Chat messages received
        /// </summary>
        /// <param name="Channel">the channel received on</param>
        /// <param name="FromName">who sent the message</param>
        /// <param name="Message">the message sent</param>
        void CommunicationReceieved(clsChat.EChannel Channel, string FromName, string Message)
        {
            StringBuilder sb = new StringBuilder();
            string MsgBreak = "\r\n------------------------------\r\n", ChatterReply;

            try
            {
                // exit if paused/stopped
                if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                    return;

                // exit if author is myself
                if (FromName.ToLower() == clsSettings.isxwow.Me.Name.ToLower())
                    return;

                clsSettings.Logging.DebugWrite(Resources.Chatter, Resources.CommReceivedProcessing);

                // log it
                sb.Append(MsgBreak); 
                sb.AppendFormat("Chatter: Communication Received\r\n");
                sb.AppendFormat("\tChannel: {0}\r\n", Channel);
                sb.AppendFormat("\tFrom: {0}\r\n", FromName);
                sb.AppendFormat("\tMessage: {0}", Message);
                clsSettings.Logging.AddToLog(sb.ToString());

                // if we have no chatter rules, then exit
                if ((RuleList == null) || (RuleList.Count == 0))
                {
                    clsSettings.Logging.AddToLog(Resources.Chatter, Resources.NoRulesFound);
                    return;
                }
                
                // get the reply of the message
                ChatterReply = GetReply(Message);
                if (string.IsNullOrEmpty(ChatterReply))
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.Chatter, Resources.NoReplyFoundForMessageX, Message);
                    return;
                }

                // get how long to wait before sending message
                int delay = (ChatterReply.Length * 300) + 2000;
                if (delay < 5000)
                    delay = 5000;

                // build the log string
                sb = new StringBuilder();
                sb.AppendFormat("Chatter Response: {0}\r\n", ChatterReply);
                sb.AppendFormat("Delay: {0} ms", delay);
                sb.Append(MsgBreak);
                clsSettings.Logging.AddToLogFormatted(Resources.ChatterResponseX, sb.ToString());

                // send the response
                clsAutoResponder.AddResponse(DateTime.Now.AddMilliseconds(delay), FromName, ChatterReply, Channel);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Chatter - Communication Received");
            }            
        }

        /// <summary>
        /// Returns a random reply or empty string if nothing found
        /// </summary>
        /// <param name="InputMessage">input message to search</param>
        /// <returns></returns>
        private string GetReply(string InputMessage)
        {
            string rVal = string.Empty;

            try
            {
                // low case it
                InputMessage = InputMessage.ToLower();

                // loop through the rule list to see if input message is found somewhere
                foreach (clsChatterRuleInfo rInfo in RuleList)
                {
                    // skip if no output messages or not found
                    if (rInfo.OutputMessages.Count == 0)
                        continue;

                    // convert the input message to a regex string
                    string tempStr = rInfo.InputMessage.Replace(".", "\\.").Replace("*", "\\*").Replace(" ", ".*");
                    if (!Regex.IsMatch(InputMessage, tempStr, RegexOptions.Singleline | RegexOptions.IgnoreCase))
                        continue;

                    // found a match, get a random output
                    rVal = rInfo.OutputMessages[new Random(DateTime.Now.Millisecond).Next(0, rInfo.OutputMessages.Count)];
                    break;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Get Chatter Reply");
            }

            return rVal;
        }

        // Private Functions
        #endregion

    }

    #region clsChatterRuleInfo

    public class clsChatterRuleInfo
    {
        private string m_InputMessage = string.Empty;
        public string InputMessage
        {
            get { return m_InputMessage; }
            set { m_InputMessage = value; }
        }

        private List<string> m_OutputMessages = new List<string>();
        public List<string> OutputMessages
        {
            get { return m_OutputMessages; }
            set { m_OutputMessages = value; }
        }

        /// <summary>
        /// Initializes a new instance of the clsChatterRuleInfo class.
        /// </summary>
        /// <param name="inputMessage"></param>
        public clsChatterRuleInfo(string inputMessage)
        {
            InputMessage = inputMessage;
        }
    }

    // clsChatterRuleInfo
    #endregion
}