using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;

namespace Rhabot
{
    public partial class uscMainInfo : UserControl
    {
        public uscMainInfo()
        {
            InitializeComponent();
        }

        #region Variables

        private bool Stopped = false;

        DateTime StartTime = DateTime.Now;

        // My last seen experience
        int LastXP = 0;

        // list of xp gained
        List<int> XPList = new List<int>();

        // list of gold gained/lost
        List<int> GoldList = new List<int>();

        int XPDiff = 0, GoldDiff = 0;
        int XPEarned = 0, GoldEarned = 0;

        // Variables
        #endregion

        #region Start / Stop

        /// <summary>
        /// Starts monitoring activity
        /// </summary>
        public void Start()
        {
            Stopped = false;
            ThreadPool.QueueUserWorkItem(Start_Thread);
        }

        /// <summary>
        /// Stops monitoring activity
        /// </summary>
        public void Stop()
        {
            // kill thread
            Stopped = true;
        }

        // Start / Stop
        #endregion

        #region Thread

        private void Start_Thread(object objState)
        {
            try
            {
                // rest variables
                StartTime = DateTime.Now;
                LastXP = clsCharacter.MyXP;
                XPList = new List<int>();
                int LastGold = clsCharacter.MyCopper;
                GoldList = new List<int>();
                XPDiff = 0;
                GoldDiff = 0;
                XPEarned = 0;
                GoldEarned = 0;

                // loop until we stop
                while (!Stopped)
                {
                    // exit if shutdown
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                    {
                        clsSettings.Logging.AddToLog(Resources.ExitingDueToScriptStop);
                        return;
                    }
                    using (new clsFrameLock.LockBuffer())
                    {
                        #region XP

                        // if we lost xp, it means we've leveled, reset xp stuff
                        if (clsCharacter.MyXP < LastXP)
                        {
                            LastXP = clsCharacter.MyXP;
                            XPList.Clear();
                            XPEarned = 0;
                        }

                        // get the xp difference
                        XPDiff = clsCharacter.MyXP - LastXP;

                        // inc xp earned
                        XPEarned += XPDiff;

                        // add to the xp list
                        XPList.Add(XPDiff);

                        // if we have more than 60 items in the xplist, remove the first (oldest) one
                        if (XPList.Count > 59)
                            XPList.RemoveAt(0);

                        // update last xp
                        LastXP = clsCharacter.MyXP;

                        // XP
                        #endregion

                        #region Gold

                        // get gold diff
                        GoldDiff = clsCharacter.MyCopper - LastGold;

                        // inc gold earned
                        GoldEarned += GoldDiff;

                        // add to gold list
                        GoldList.Add(GoldDiff);

                        // if more than 60 items in gold list, remove the first (oldest) one
                        if (GoldList.Count > 59)
                            GoldList.RemoveAt(0);

                        // update last gold
                        LastGold = clsCharacter.MyCopper;

                        // Gold
                        #endregion
                    }

                    #region Update Labels

                    // exit if stopped
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                        return;

                    // gold / hour
                    if (this.lblGoldHour.InvokeRequired)
                        this.lblGoldHour.Invoke(new dUpdateLblGoldAVG(UpdateLblGoldAVG));
                    else
                        UpdateLblGoldAVG();

                    // exit if stopped
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                        return;

                    // time played
                    if (this.lblTimePlayed.InvokeRequired)
                        this.lblTimePlayed.Invoke(new dUpdateLblTimePlayed(UpdateLblTimePlayed));
                    else
                        UpdateLblTimePlayed();

                    // exit if stopped
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                        return;

                    // total gold
                    if (this.lblTotalGold.InvokeRequired)
                        this.lblTotalGold.Invoke(new dUpdateLblGold(UpdateLblGold));
                    else
                        UpdateLblGold();

                    // exit if stopped
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                        return;

                    // total xp
                    if (this.lblTotalXP.InvokeRequired)
                        this.lblTotalXP.Invoke(new dUpdateLblXP(UpdateLblXP));
                    else
                        UpdateLblXP();

                    // exit if stopped
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                        return;

                    // xp / hour
                    if (this.lblXPHour.InvokeRequired)
                        this.lblXPHour.Invoke(new dUpdateLblXPAVG(UpdateLblXPAVG));
                    else
                        UpdateLblXPAVG();

                    // exit if stopped
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                        return;

                    // zone
                    if (this.lblZone.InvokeRequired)
                        this.lblZone.Invoke(new dUpdateLblZone(UpdateLblZone));
                    else
                        UpdateLblZone();

                    // exit if stopped
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                        return;

                    // sub zone
                    if (this.lblSubZone.InvokeRequired)
                        this.lblSubZone.Invoke(new dUpdateLblSubZone(UpdateLblSubZone));
                    else
                        UpdateLblSubZone();

                    // exit if stopped
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                        return;

                    // location
                    if (this.lblLocation.InvokeRequired)
                        this.lblLocation.Invoke(new dUpdateLblLocation(UpdateLblLocation));
                    else
                        UpdateLblLocation();

                    // exit if stopped
                    if ((Stopped) || (clsSettings.IsShuttingDown))
                        return;

                    // Update Labels
                    #endregion

                    // sleep for one second then do it again
                    Thread.Sleep(1000);
                }
            }

            catch (ThreadAbortException) { }

            catch (Exception excep)
            {
                // skip, probably thread abort problem
                //clsError.ShowError(excep, Resources.RhabotGoldXPInfoLoop);
            }
        }

        // Thread
        #endregion

        #region Update Labels

        private delegate void dUpdateLblGold();
        private delegate void dUpdateLblGoldAVG();
        private delegate void dUpdateLblXP();
        private delegate void dUpdateLblXPAVG();
        private delegate void dUpdateLblTimePlayed();
        private delegate void dUpdateLblZone();
        private delegate void dUpdateLblSubZone();
        private delegate void dUpdateLblLocation();

        private void UpdateLblGold()
        {
            try
            {
                // 0 if nothing
                if (GoldEarned == 0)
                {
                    this.lblTotalGold.Text = Resources.GoldFormat;
                    return;
                }

                // update label
                this.lblTotalGold.Text = clsCharacter.ParseCoinage(GoldEarned);
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.Contains(Resources.abort))
                    clsError.ShowError(excep, "UpdateLblGold");
            }
        }

        private void UpdateLblXP()
        {
            try
            {
                // update xp earned
                this.lblTotalXP.Text = XPEarned.ToString().Trim();
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.Contains(Resources.abort))
                    clsError.ShowError(excep, "UpdateLblXP");
            }
        }

        private void UpdateLblGoldAVG()
        {
            try
            {
                // update label with gold average
                this.lblGoldHour.Text = clsCharacter.ParseCoinage(AverageList(GoldList));
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.Contains(Resources.abort))
                    clsError.ShowError(excep, "UpdateLblGoldAVG");
            }
        }

        private void UpdateLblXPAVG()
        {
            try
            {
                // update xp label with xp average
                this.lblXPHour.Text = AverageList(XPList).ToString();
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.Contains(Resources.abort))
                    clsError.ShowError(excep, "UpdateLblXPAVG");
            }
        }

        private void UpdateLblTimePlayed()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                TimeSpan tSpan = new TimeSpan(DateTime.Now.Ticks - StartTime.Ticks);
                clsGlobals.RunningTime = tSpan;

                // get the number of hours played
                if (tSpan.Hours < 10)
                    sb.Append("0");
                sb.AppendFormat("{0}:", tSpan.Hours.ToString().Trim());

                // get minutes played
                tSpan = tSpan.Subtract(new TimeSpan(tSpan.Hours, 0, 0));
                if (tSpan.Minutes < 10)
                    sb.Append("0");
                sb.AppendFormat("{0}:", tSpan.Minutes.ToString().Trim());

                // get seconds played
                tSpan = tSpan.Subtract(new TimeSpan(0, tSpan.Minutes, 0));
                if (tSpan.Seconds < 10)
                    sb.Append("0");
                sb.Append(tSpan.Seconds.ToString().Trim());

                // update label
                this.lblTimePlayed.Text = sb.ToString();
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.Contains(Resources.abort))
                    clsError.ShowError(excep, "UpdateLblTimePlayed");
            }
        }

        /// <summary>
        /// Averages a list and returns the result
        /// </summary>
        /// <param name="AvgList">the list to average</param>
        private int AverageList(List<int> AvgList)
        {
            long avgAmount = 0;

            try
            {
                // return 0 if nothing in list
                if ((AvgList == null) || (AvgList.Count == 0))
                    return 0;

                // if one item in list, return that item
                if (AvgList.Count == 1)
                    return AvgList[0];

                // loop through and add all items
                foreach (int item in AvgList)
                    avgAmount += item;

                // return the average
                return (int)(avgAmount / AvgList.Count);
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.Contains(Resources.abort))
                    clsError.ShowError(excep, "Average List");
            }

            return 0;
        }

        private void UpdateLblZone()
        {
            try
            {
                this.lblZone.Text = clsCharacter.ZoneText;
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.Contains(Resources.abort))
                    clsError.ShowError(excep, "UpdateLblZone");
            }
        }

        private void UpdateLblSubZone()
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                    this.lblSubZone.Text = clsSettings.isxwow.SubZoneText;
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.Contains(Resources.abort))
                    clsError.ShowError(excep, "UpdateLblSubZone");
            }
        }

        private void UpdateLblLocation()
        {
            try
            {
                this.lblLocation.Text = clsCharacter.MyLocation.ToString();
            }

            catch (Exception excep)
            {
                // skip if thread abort
                if (!excep.Message.Contains(Resources.abort))
                    clsError.ShowError(excep, "UpdateLblLocation");
            }
        }

        // Update Labels
        #endregion
    }
}