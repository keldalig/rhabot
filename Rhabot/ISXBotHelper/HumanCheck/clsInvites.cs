using System;
using System.Threading;
using ISXBotHelper.Properties;

namespace ISXBotHelper
{
    internal static class clsInvites
    {
        #region Start/Stop Functions

        public static void StartHook(object objState)
        {
            StartHook();
        }

        /// <summary>
        /// Hooks appropriate invite events
        /// </summary>
        public static void StartHook()
        {
            // hook guild invite
            if (clsSettings.gclsGlobalSettings.DeclineGuildInvite)
                clsWowEvents.GuildInvite += clsWowEvents_GuildInvite;

            // hook group invite
            if (clsSettings.gclsGlobalSettings.DeclineGroupInvite)
                clsWowEvents.GroupInvite += clsWowEvents_GroupInvite;

            // hook duel invite
            if (clsSettings.gclsGlobalSettings.DeclineDuelInvite)
                clsWowEvents.DuelInvite += clsWowEvents_DuelInvite;
        }

        /// <summary>
        /// Detaches events
        /// </summary>
        public static void Shutdown()
        {
            // unhook guild invite
            if (clsSettings.gclsGlobalSettings.DeclineGuildInvite)
                clsWowEvents.GuildInvite -= clsWowEvents_GuildInvite;

            // unhook group invite
            if (clsSettings.gclsGlobalSettings.DeclineGroupInvite)
                clsWowEvents.GroupInvite -= clsWowEvents_GroupInvite;

            // unhook duel invite
            if (clsSettings.gclsGlobalSettings.DeclineDuelInvite)
                clsWowEvents.DuelInvite -= clsWowEvents_DuelInvite;
        }

        // Start/Stop Functions
        #endregion

        #region Invite Functions

        private static readonly Random rnd = new Random(DateTime.Now.Millisecond);

        static void clsWowEvents_DuelInvite(string OpponentName)
        {
            // cancel in a new thread
            new Thread(clsWowEvents_DuelInvite_Thread).Start();
        }

        static void clsWowEvents_GroupInvite(string GroupLeader)
        {
            // cancel in a new thread
            new Thread(clsWowEvents_GroupInvite_Thread).Start();
        }

        static void clsWowEvents_GuildInvite(string InviterName, string GuildName)
        {
            // cancel in a new thread
            new Thread(clsWowEvents_GuildInvite_Thread).Start();
        }

        static void clsWowEvents_DuelInvite_Thread()
        {
            // wait 2-4 seconds
            Thread.Sleep(rnd.Next(2000, 4000));

            clsSettings.Logging.AddToLog(Resources.DecliningDuelInvite);

            // decline the group invite
            clsSettings.ExecuteWoWAPI("CancelDuel()");
        }

        static void clsWowEvents_GroupInvite_Thread()
        {
            // wait 2-4 seconds
            Thread.Sleep(rnd.Next(2000, 4000));

            clsSettings.Logging.AddToLog(Resources.DecliningGroupInvite);

            // decline the group invite
            clsSettings.ExecuteWoWAPI("DeclineGroup()");
        }

        static void clsWowEvents_GuildInvite_Thread()
        {
            // wait 2-4 seconds
            Thread.Sleep(rnd.Next(2000, 4000));

            clsSettings.Logging.AddToLog(Resources.DecliningGuildInvite);

            // decline the group invite
            clsSettings.ExecuteWoWAPI("DeclineGuild()");
        }

        // Invite Functions
        #endregion
    }
}
