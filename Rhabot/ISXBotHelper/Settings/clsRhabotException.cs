using System;
using System.Diagnostics;

namespace ISXBotHelper
{
    internal class clsRhabotException : ApplicationException
    {
        private Exception m_excep;
        private string m_msg = string.Empty;
        private bool m_WriteToLog = false;
        private bool m_WriteToService = false;
        private StackFrame m_stackframe = null;

        public clsRhabotException(Exception excep, string msg, bool WriteToLog, bool WriteToService, StackFrame stackframe)
        {
            m_excep = excep;
            m_WriteToLog = WriteToLog;
            m_WriteToService = WriteToService;
            m_stackframe = stackframe;
            m_msg = msg;
        }

        public clsRhabotException(string excepMsg, string msg)
        {
            m_excep = new Exception(excepMsg);
            m_msg = msg;
            m_WriteToLog = true;
            m_WriteToService = false;
            m_stackframe = new StackFrame(1, true);
        }

        /// <summary>
        /// Calls clsError.ShowError using the save data
        /// </summary>
        public void ShowError()
        {
            clsError.ShowError(m_excep, m_msg, m_WriteToLog, m_stackframe, m_WriteToService);
        }
    }
}
