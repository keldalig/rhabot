// http://www.isxwow.net/forums/viewtopic.php?f=15&t=1014&p=11159#p11159

using System;
using LavishScriptAPI;

namespace ISXBotHelper
{
    /// <summary>
    /// All wow raised events are hooked here
    /// </summary>
    internal class clsWowEvents
    {
        #region Variables

        private static int CurrentXP = 0;

        private static EventHandler<LSEventArgs> dGuildInvite = null;
        private static EventHandler<LSEventArgs> dGroupInvite = null;
        private static EventHandler<LSEventArgs> dDuelInvite = null;
        private static EventHandler<LSEventArgs> dCharLevel = null;
        private static EventHandler<LSEventArgs> dSay = null;
        private static EventHandler<LSEventArgs> dPrivate = null;
        private static EventHandler<LSEventArgs> dTargetChange = null;
        private static EventHandler<LSEventArgs> dQuestDetailFrame = null;
        private static EventHandler<LSEventArgs> dQuestCompleteFrame = null;
        private static EventHandler<LSEventArgs> dQuestProgressFrame = null;
        private static EventHandler<LSEventArgs> dMonsterCastSpell = null;
        private static EventHandler<LSEventArgs> dUnitCombat = null;
        private static EventHandler<LSEventArgs> dPlayerDead = null;
        private static EventHandler<LSEventArgs> dMonsterDead = null;
        private static EventHandler<LSEventArgs> dUIErrorMessage = null;
        private static EventHandler<LSEventArgs> dChatMonsterEmote = null;

        // Variables
        #endregion

        #region Start / Stop

        public static void Start(object objState)
        {
            Start();
        }

        public static void Start()
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // hook guild invite
                    if (dGuildInvite == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Guild Invite");
                        dGuildInvite = HGuildInvite;
                        LavishScript.Events.AttachEventTarget("GUILD_INVITE_REQUEST", dGuildInvite);
                    }

                    // hook group invite
                    if (dGroupInvite == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Group Invite");
                        dGroupInvite = HGroupInvite;
                        LavishScript.Events.AttachEventTarget("PARTY_INVITE_REQUEST", dGroupInvite);
                    }

                    // hook duel invite
                    if (dDuelInvite == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Duel Request");
                        dDuelInvite = HDuelInvite;
                        LavishScript.Events.AttachEventTarget("DUEL_REQUESTED", dDuelInvite);
                    }

                    // character level
                    if (dCharLevel == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Player Level Up");
                        dCharLevel = HCharLevel;
                        LavishScript.Events.AttachEventTarget("PLAYER_LEVEL_UP", dCharLevel);
                    }

                    // Say Chat
                    if (dSay == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Say Chat");
                        dSay = HSayChat;
                        LavishScript.Events.AttachEventTarget("CHAT_MSG_SAY", dSay);
                    }

                    // private chat
                    if (dPrivate == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Private Chat");
                        dPrivate = HPrivateChat;
                        LavishScript.Events.AttachEventTarget("CHAT_MSG_WHISPER", dPrivate);
                    }

                    // target change
                    if (dTargetChange == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Target Changed");
                        dTargetChange = HTargetChange;
                        LavishScript.Events.AttachEventTarget("PLAYER_TARGET_CHANGED", dTargetChange);
                    }

                    // quest detail frame shown
                    if (dQuestDetailFrame == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Quest Detail");
                        dQuestDetailFrame = HQuestDetailAlert;
                        LavishScript.Events.AttachEventTarget("QUEST_DETAIL", dQuestDetailFrame);
                    }

                    // quest complete
                    if (dQuestCompleteFrame == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Quest Complete");
                        dQuestCompleteFrame = HQuestCompleteFrameAlert;
                        LavishScript.Events.AttachEventTarget("QUEST_COMPLETE", dQuestCompleteFrame);
                    }

                    // quest progress
                    if (dQuestProgressFrame == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Quest Progress");
                        dQuestProgressFrame = HQuestProgressFrameAlert;
                        LavishScript.Events.AttachEventTarget("QUEST_PROGRESS", dQuestProgressFrame);
                    }

                    // hook the monster cast spell event
                    if (dMonsterCastSpell == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Monster Cast Spell");
                        dMonsterCastSpell = MonsterCastSpell;
                        LavishScript.Events.AttachEventTarget("CHAT_MSG_SPELL_HOSTILEPLAYER_DAMAGE", dMonsterCastSpell);
                    }

                    // combat text update
                    if (dUnitCombat == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Unit_Combat");
                        dUnitCombat = UnitCombat;
                        LavishScript.Events.AttachEventTarget("UNIT_COMBAT", dUnitCombat);
                    }

                    // player dead
                    if (dPlayerDead == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking PLAYER_DEAD");
                        dPlayerDead = PlayerDead;
                        LavishScript.Events.AttachEventTarget("PLAYER_DEAD", dPlayerDead);
                    }

                    // monster dead
                    if (dMonsterDead == null)
                    {
                        // get xp
                        CurrentXP = clsCharacter.MyXP;

                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Monster Dies");
                        dMonsterDead = MonsterDead;
                        LavishScript.Events.AttachEventTarget("CHAT_MSG_COMBAT_HOSTILE_DEATH", dMonsterDead);
                    }

                    // ui error message
                    if (dUIErrorMessage == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking UI Error Message");
                        dUIErrorMessage = UIErrorMessage;
                        LavishScript.Events.AttachEventTarget("UI_ERROR_MESSAGE", dUIErrorMessage);
                    }

                    // chat monster emote
                    if (dChatMonsterEmote == null)
                    {
                        clsSettings.Logging.AddToLog("\tWoWEvents: Hooking Chat Monster Emote");
                        dChatMonsterEmote = fChatMonsterEmote;
                        LavishScript.Events.AttachEventTarget("CHAT_MSG_MONSTER_EMOTE", dChatMonsterEmote);
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "clsWoWEvents.Start");
            }
        }

        public static void Shutdown()
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // detach events
                    if (dDuelInvite != null)
                    {
                        LavishScript.Events.DetachEventTarget("DUEL_REQUESTED", dDuelInvite);
                        dDuelInvite = null;
                    }
                    if (dGroupInvite != null)
                    {
                        LavishScript.Events.DetachEventTarget("PARTY_INVITE_REQUEST", dGroupInvite);
                        dGroupInvite = null;
                    }
                    if (dGuildInvite != null)
                    {
                        LavishScript.Events.DetachEventTarget("GUILD_INVITE_REQUEST", dGuildInvite);
                        dGuildInvite = null;
                    }
                    // detach level up
                    if (dCharLevel != null)
                    {
                        LavishScript.Events.DetachEventTarget("PLAYER_LEVEL_UP", dCharLevel);
                        dCharLevel = null;
                    }

                    // say chat
                    if (dSay != null)
                    {
                        LavishScript.Events.DetachEventTarget("CHAT_MSG_SAY", dSay);
                        dSay = null;
                    }

                    // private chat
                    if (dPrivate != null)
                    {
                        LavishScript.Events.DetachEventTarget("CHAT_MSG_WHISPER", dPrivate);
                        dPrivate = null;
                    }

                    // detach target change
                    if (dTargetChange != null)
                    {
                        LavishScript.Events.DetachEventTarget("PLAYER_TARGET_CHANGED", dTargetChange);
                        dTargetChange = null;
                    }

                    // quests
                    if (dQuestDetailFrame != null)
                    {
                        LavishScript.Events.DetachEventTarget("QUEST_DETAIL", dQuestDetailFrame);
                        dQuestDetailFrame = null;
                    }
                    if (dQuestCompleteFrame != null)
                    {
                        LavishScript.Events.DetachEventTarget("QUEST_COMPLETE", dQuestCompleteFrame);
                        dQuestCompleteFrame = null;
                    }
                    if (dQuestProgressFrame != null)
                    {
                        LavishScript.Events.DetachEventTarget("QUEST_PROGRESS", dQuestProgressFrame);
                        dQuestProgressFrame = null;
                    }
                    if (dMonsterCastSpell != null)
                    {
                        LavishScript.Events.DetachEventTarget("CHAT_MSG_SPELL_HOSTILEPLAYER_DAMAGE", dMonsterCastSpell);
                        dMonsterCastSpell = null;
                    }
                    if (dUnitCombat != null)
                    {
                        LavishScript.Events.DetachEventTarget("UNIT_COMBAT", dUnitCombat);
                        dUnitCombat = null;
                    }
                    if (dPlayerDead != null)
                    {
                        LavishScript.Events.DetachEventTarget("PLAYER_DEAD", dPlayerDead);
                        dPlayerDead = null;
                    }
                    if (dMonsterDead != null)
                    {
                        LavishScript.Events.DetachEventTarget("CHAT_MSG_COMBAT_HOSTILE_DEATH", dMonsterDead);
                        dMonsterDead = null;
                    }
                    if (dUIErrorMessage != null)
                    {
                        LavishScript.Events.DetachEventTarget("UI_ERROR_MESSAGE", dUIErrorMessage);
                        dUIErrorMessage = null;
                    }
                    if (dChatMonsterEmote != null)
                    {
                        LavishScript.Events.DetachEventTarget("CHAT_MSG_MONSTER_EMOTE", dChatMonsterEmote);
                        dChatMonsterEmote = null;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "clsWoWEvents.Shutdown");
            }
        }

        // Start / Stop
        #endregion

        #region Events

        #region Guild Invite

        public delegate void GuildInviteHandler(string InviterName, string GuildName);
        public static event GuildInviteHandler GuildInvite;

        private static void HGuildInvite(object sender, LSEventArgs e)
        {
            if (GuildInvite != null)
                GuildInvite(e.Args[0], e.Args[1]);
        }

        // Guild Invite
        #endregion

        #region Group Invite

        public delegate void GroupInviteHandler(string GroupLeader);
        public static event GroupInviteHandler GroupInvite;

        private static void HGroupInvite(object sender, LSEventArgs e)
        {
            if (GroupInvite != null)
                GroupInvite(e.Args[0]);
        }

        // Group Invite
        #endregion

        #region Duel Invite

        public delegate void DuelInviteHandler(string OpponentName);
        public static event DuelInviteHandler DuelInvite;

        private static void HDuelInvite(object sender, LSEventArgs e)
        {
            if (DuelInvite != null)
                DuelInvite(e.Args[0]);
        }

        // Duel Invite
        #endregion

        #region Character Level Up

        public delegate void CharLevelHandler(int Level);
        public static event CharLevelHandler CharLevel;

        /// <summary>
        /// Raised by WoW when the character levels
        /// </summary>
        private static void HCharLevel(object sender, LSEventArgs e)
        {
            if (CharLevel != null)
                CharLevel(Convert.ToInt32(e.Args[0]));
        }

        // Character Level Up
        #endregion

        #region Say Chat

        public delegate void SayChatHandler(string author, string message);
        public static event SayChatHandler SayChat;

        /// <summary>
        /// Incoming Say message
        /// </summary>
        private static void HSayChat(object sender, LSEventArgs e)
        {
            try
            {
                // get author and message
                string author = e.Args[3];
                string msg = e.Args[2];

                // exit if no auther or mesasge
                if ((string.IsNullOrEmpty(author)) || (string.IsNullOrEmpty(msg)))
                    return;

                // raise it
                if (SayChat != null)
                    SayChat(author, msg);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "HSayChat");
            }
        }

        // Say Chat
        #endregion

        #region Private Chat

        public delegate void PrivateChatHandler(string author, string message);
        public static event PrivateChatHandler PrivateChat;

        /// <summary>
        /// Incoming private message
        /// </summary>
        private static void HPrivateChat(object sender, LSEventArgs e)
        {
            try
            {
                // get author and message
                string author = e.Args[3];
                string msg = e.Args[2];

                // exit if no auther or mesasge
                if ((string.IsNullOrEmpty(author)) || (string.IsNullOrEmpty(msg)))
                    return;

                // raise it
                if (PrivateChat != null)
                    PrivateChat(author, msg);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "HPrivateChat");
            }
        }

        // Private Chat
        #endregion

        #region Target Changed

        public delegate void TargetChangeHandler();
        public static event TargetChangeHandler TargetChanged;

        /// <summary>
        /// Raised by WoW when the target changes
        /// </summary>
        private static void HTargetChange(object sender, LSEventArgs e)
        {
            if (TargetChanged != null)
                TargetChanged();
        }

        // Target Changed
        #endregion

        #region Quest Detail

        public delegate void QuestDetailsHandler();
        public static event QuestDetailsHandler QuestDetails;

        /// <summary>
        /// Fired when you get to the frame that lets you press "Accept"
        /// Fired when the player is given a more detailed view of his quest.
        /// </summary>
        public static void HQuestDetailAlert(object sender, LSEventArgs e)
        {
            if (QuestDetails != null)
                QuestDetails();
        }

        // Quest Detail
        #endregion

        #region Quest Complete

        public delegate void QuestCompleteHandler();
        public static event QuestCompleteHandler QuestComplete;

        /// <summary>
        /// Fired after the player hits the "Continue" button in the quest-information page, before the "Complete Quest" button.
        /// In other words it fires when you are given the option to complete a quest, but just before you actually complete the quest (as stated above). 
        /// </summary>
        public static void HQuestCompleteFrameAlert(object sender, LSEventArgs e)
        {
            if (QuestComplete != null)
                QuestComplete();
        }

        // Quest Complete
        #endregion

        #region Quest Progress

        public delegate void QuestProgressHandler();
        public static event QuestProgressHandler QuestProgress;

        /// <summary>
        /// Fired when a player is talking to an NPC about the status of a quest and has not yet clicked the complete button.
        /// </summary>
        public static void HQuestProgressFrameAlert(object sender, LSEventArgs e)
        {
            if (QuestProgress != null)
                QuestProgress();
        }

        // Quest Progress
        #endregion

        #region Monster Cast Spell

        public delegate void MonsterCastSpellHandler();
        public static event MonsterCastSpellHandler MonsterSpellCast;

        /// <summary>
        /// Fired when an opponent casts a spell
        /// </summary>
        public static void MonsterCastSpell(object sender, LSEventArgs e)
        {
            if (MonsterSpellCast != null)
                MonsterSpellCast();
        }

        // Monster Cast Spell
        #endregion

        #region Unit Combat

        public delegate void UnitCombatHandler(string unitid, string arg2, string arg3, int dmg, int dmgType);
        public static event UnitCombatHandler UnitCombat_Update;

        /// <summary>
        /// Fired when an opponent casts a spell
        /// </summary>
        public static void UnitCombat(object sender, LSEventArgs e)
        {
            if (UnitCombat_Update != null)
                UnitCombat_Update(
                    e.Args[0],
                    e.Args[1],
                    e.Args[2],
                    string.IsNullOrEmpty(e.Args[3]) ? 0 : Convert.ToInt32(e.Args[3]),
                    string.IsNullOrEmpty(e.Args[4]) ? 0 : Convert.ToInt32(e.Args[4]));
        }

        // Unit Combat
        #endregion

        #region Player Dead

        public delegate void PlayerDeadHandler();
        public static event PlayerDeadHandler PlayerDied;

        /// <summary>
        /// Fired when you die
        /// </summary>
        public static void PlayerDead(object sender, LSEventArgs e)
        {
            if (PlayerDied != null)
                PlayerDied();
        }

        // Player Dead
        #endregion

        #region Monster Dead

        /// <summary>
        /// Monster dead
        /// </summary>
        /// <param name="XPGained">xp gained on monster death</param>
        /// <param name="XPToLevel">xp needed to level</param>
        /// <param name="KillsToLevel">estimated kills to level</param>
        public delegate void MonsterDeadHandler(int XPGained, int XPToLevel, int KillsToLevel);
        public static event MonsterDeadHandler MonsterDied;

        /// <summary>
        /// Fired when you die
        /// </summary>
        public static void MonsterDead(object sender, LSEventArgs e)
        {
            if (MonsterDied != null)
            {
                int XPGained;
                int ktl, XPLevel;

                // get the xp we gained
                using (new clsFrameLock.LockBuffer())
                {
                    XPGained = clsSettings.isxwow.Me.Exp;
                    if (XPGained > CurrentXP)
                        XPGained = XPGained - CurrentXP;
                    CurrentXP = clsSettings.isxwow.Me.Exp;

                    // exit if xpgained is less than 0
                    if ((XPGained <= 0) || (CurrentXP <= 0))
                        return;

                    // get kills to level
                    XPLevel = clsSettings.isxwow.Me.NextLevelExp;
                    ktl = (XPLevel - clsSettings.isxwow.Me.Exp) / XPGained;
                }

                // raise the event
                MonsterDied(XPGained, XPLevel, ktl);
            }
        }

        // Monster Dead
        #endregion

        #region UIErrorMessage

        public delegate void UIErrorMessageHandler(string ErrorMessage);
        public static event UIErrorMessageHandler UIErrorMsg;

        private static void UIErrorMessage(object sender, LSEventArgs e)
        {
            if (UIErrorMsg != null)
                UIErrorMsg(e.Args[0]);
        }

        // UIErrorMessage
        #endregion

        #region Chat Monster Emote

        public delegate void ChatMonsterEmoteHandler(string EmoteBody, string MonsterName);
        public static event ChatMonsterEmoteHandler ChatMonsterEmote;

        private static void fChatMonsterEmote(object sender, LSEventArgs e)
        {
            if (ChatMonsterEmote != null)
                ChatMonsterEmote(e.Args[0], e.Args[1]);
        }

        // Chat Monster Emote
        #endregion

        // Events
        #endregion
    }
}
