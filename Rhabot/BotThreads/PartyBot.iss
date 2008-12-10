Looting Events (pb\utils\events.iss)


LavishScript:RegisterEvent[START_LOOT_ROLL]
LavishScript:RegisterEvent[LOOT_BIND_CONFIRM]


;----------------------------------------------------------------------------------
;| Atom Name: STARTLOOTROLL
;| Returns: 
;| Description: This event rolls on loot drops and confirms the bop dialog but only when partied
;|
;----------------------------------------------------------------------------------
atom(script) STARTLOOTROLL(string eventid, string stuff, int rollID)
{
    if ${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_LOOT_ROLL](exists)}
    {
        if ${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_LOOT_ROLL]} < 3
        {
            WoWScript RollOnLoot(${rollID},${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_LOOT_ROLL]})
            WoWScript ConfirmLootRoll(${rollID},${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_LOOT_ROLL]})
        }
    }
}

;----------------------------------------------------------------------------------
;| Atom Name: LOOTBINDCONFIRM
;| Returns: 
;| Description: This event comes up when a bop confirmation comes up (fndude)
;|
;----------------------------------------------------------------------------------
atom(script) LOOTBINDCONFIRM(string eventid, string stuff, string Params)
{
    if ${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_AUTO_CONFIRM_BOP]}
    {
        WowScript ConfirmLootSlot("1")
        WowScript StaticPopup_Hide("LOOT_BIND")
    }
}


;========================================================================================================


Slap a unit (used to reset the party)

LavishScript:RegisterEvent[CHAT_MSG_TEXT_EMOTE]

;----------------------------------------------------------------------------------
;| Atom Name: CHATMSGTEXTEMOTE
;| Returns: 
;| Description: The CHATMSGTEXTEMOTE event used to reset the bots if they get stuck
;|
;----------------------------------------------------------------------------------
atom(script) CHATMSGTEXTEMOTE(string eventid, string eventstring)
{
    ;call Debug "CHATMSGTEXTEMOTE:"
    ;[Event:250:CHAT_MSG_TEXT_EMOTE]("Name slaps you across the face. Ouch!","Name","","","","",0,0,"",0)
    ;I used to force the slap to come from the MT but it's easier to allow anyone to slap him
    ;if ${eventstring.Find["${MT.TName} slaps you across the face"]}
    if ${eventstring.Find["slaps you across the face"]}
    {
        BOT.BOT_STATE:Set[READY]
        BOT.BOT_STATE_VISIBLE:Set[FOLLOWINGMT]
        FT:Initialize
        FT.TGUID:Set[${MT.TGUID}]
        FT:UpdateLoc[${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_FOLLOW_DISTANCE]}, ${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_FOLLOW_POLL_PRECISION]},0]
        wowpress moveforward
    }
}

;=========================================================================================================


Quest Accept / Confirm (when shared ?)

LavishScript:RegisterEvent[QUEST_DETAIL]
LavishScript:RegisterEvent[QUEST_ACCEPT_CONFIRM]

;----------------------------------------------------------------------------------
;| Atom Name: QUESTACCEPTCONFIRM
;| Returns: 
;| Description: The QUESTACCEPTCONFIRM event used to auto accept escort type quests
;|
;----------------------------------------------------------------------------------
atom(script) QUESTACCEPTCONFIRM(string eventid, string eventstring)
{
    ;call Debug "QUESTACCEPTCONFIRM:"
    ;[Event:389:QUEST_ACCEPT_CONFIRM]("Playername","Questname")
    if ${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_AUTO_ACCEPT_QUESTS]}
    {
        WoWScript AcceptQuest()
        WoWScript ConfirmAcceptQuest()
    }
}

;----------------------------------------------------------------------------------
;| Atom Name: QUESTDETAIL
;| Returns: 
;| Description: The QUESTDETAIL event used to auto accept quests
;|
;----------------------------------------------------------------------------------
atom(script) QUESTDETAIL(string eventid, string eventstring)
{
    ;call Debug "RESURRECTREQUEST:"
    ;[Event:342:QUEST_DETAIL]
    if ${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_AUTO_ACCEPT_QUESTS]}
    {
        WoWScript AcceptQuest()
        WoWScript ConfirmAcceptQuest()
    }
}


;=========================================================================================================

Resurrection Request (when someone wants to rez you)


LavishScript:RegisterEvent[RESURRECT_REQUEST]


;----------------------------------------------------------------------------------
;| Atom Name: RESURRECTREQUEST
;| Returns: 
;| Description: The RESURRECTREQUEST event used to auto res
;|
;----------------------------------------------------------------------------------
atom(script) RESURRECTREQUEST(string eventid, string eventstring)
{
    ;call Debug "RESURRECTREQUEST:"
    ;[Event:318:RESURRECT_REQUEST]("Playername")
    if ${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_AUTO_ACCEPT_RES]}
    {
        WoWScript AcceptResurrect()
    }
}

;=========================================================================================================


Combat 
	- non-healers and not tank
		- check if healers have aggro. if so, attack the healer's aggro
		- check if other party members have more than one aggro. if so, target non-targetted unit
	- healers
		- check if other members need heal, decurse/debuff
		

;=========================================================================================================

Party Invite

LavishScript:RegisterEvent[PARTY_INVITE_REQUEST]


;----------------------------------------------------------------------------------
;| Atom Name: PARTYINVITEREQUEST
;| Returns: 
;| Description: The PARTYINVITEREQUEST event used to auto accept party invite
;|
;----------------------------------------------------------------------------------
atom(script) PARTYINVITEREQUEST(string eventid, string eventstring)
{
    ;call Debug "PARTYINVITEREQUEST:"
    ;[Event:319:PARTY_INVITE_REQUEST]("Playername")
    if ${LavishSettings[PB].FindSet["General Settings"].FindSetting[SETTING_AUTO_ACCEPT_INVITE]}
    {
        WoWScript AcceptGroup()
    }
}

;=========================================================================================================