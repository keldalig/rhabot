using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ISXBotHelper.Combat;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;
using System.Linq;

namespace ISXBotHelper
{
    /// <summary>
    /// General Combat functions
    /// </summary>
    public class clsCombat
    {
        #region Variables

        private int BugCounter = 0;
        private DateTime LastBugCheck = DateTime.MinValue;
        private WoWUnit m_UnitToAttack = null;

        /// <summary>
        /// Set to true when we are casting the attempts to run away spell
        /// </summary>
        public static bool Casting_RunAwaySpell = false;

        // Variables
        #endregion

        #region Enums

        public enum ECurrentStep
        {
            PrePull = 0,
            Pull,
            PostPull,
            InCombat,
            PostCombat
        }

        // Enums
        #endregion

        #region Functions

        #region Combat Status

        /// <summary>
        /// Returns true if we are in combat (attacking or being attacked)
        /// </summary>
        public static bool IsInCombat()
        {
            using (new clsFrameLock.LockBuffer())
                return clsSettings.isxwow.Me.IsInCombat || clsSettings.isxwow.Me.Attacking || (NumUnitsAttackingMe() > 0);
        }

        /// <summary>
        /// Returns true if we are in combat (attacking or being attacked)
        /// </summary>
        public static bool IsPetInCombat()
        {
            using (new clsFrameLock.LockBuffer())
                return (clsSettings.isxwow.Me.Pet.AttackingUnit != null) && (clsSettings.isxwow.Me.Pet.AttackingUnit.IsValid);
        }

        /// <summary>
        /// Returns true if the pet is targeting my target
        /// </summary>
        public static bool IsPetTargettingTarget(WoWUnit CurrentTarget)
        {
            using (new clsFrameLock.LockBuffer())
                return (clsSettings.isxwow.Me.Pet.AttackingUnit != null) && 
                    (clsSettings.isxwow.Me.Pet.AttackingUnit.IsValid) && 
                    (clsSettings.isxwow.Me.Pet.AttackingUnit.GUID == CurrentTarget.GUID);
        }

        /// <summary>
        /// returns the number of units attacking me
        /// </summary>
        public static int NumUnitsAttackingMe()
        {
            // get the list of attackers
            using (new clsFrameLock.LockBuffer())
            {
                GuidList myAttackers = new GuidList();
                myAttackers.Search("-units", "-targetingme", "-aggro");
                return (int)myAttackers.Count;
            }
        }

        /// <summary>
        /// returns a list of units attacking me
        /// </summary>
        public static List<WoWUnit> UnitsAttackingMe()
        {
            GuidList myAttackers;
            WoWUnit tempUnit;
            List<WoWUnit> retList = new List<WoWUnit>();

            // get the list of attackers
            // log it
            if (clsSettings.VerboseLogging)
                clsSettings.Logging.AddToLog(Resources.SearchingForUnitsAttackingMe);

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // get the list
                    myAttackers = new GuidList();
                    myAttackers.Search("-units", "-targetingme", "-aggro", "-alive");

                    int SearchCount = (int)myAttackers.Count;

                    // loop through and add to return list
                    for (int UnitCounter = 0; UnitCounter < SearchCount; UnitCounter++)
                    {
                        // get the unit
                        tempUnit = myAttackers.Object((uint)UnitCounter).GetUnit();

                        // check if in blacklist
                        if (clsBlacklist.IsBlacklisted(clsSettings.BlackList_Combat, tempUnit))
                            continue;

                        // make sure it's valid
                        if ((tempUnit == null) || (!tempUnit.IsValid))
                            continue;

                        // add unit to the list 
                        retList.Add(tempUnit);

                        // log it
                        if (clsSettings.VerboseLogging)
                            clsSettings.Logging.AddToLog(string.Format(Resources.FoundXAttackingMe, tempUnit.Name));
                    }
                }

                // if the list is empty, add an empty element
                if (retList.Count == 0)
                    retList = new List<WoWUnit>();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "UnitsAttackingMe");
            }

            // return the list
            return retList;
        }

        /// <summary>
        /// Returns one unit attacking me, or null
        /// </summary>
        public static WoWUnit GetUnitAttackingMe()
        {
            try
            {
                // get attacking units
                List<WoWUnit> units = UnitsAttackingMe();

                // if unit found, then return it
                using (new clsFrameLock.LockBuffer())
                {
                    if ((units != null) && (units.Count > 0))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.GetUnitAttackingMeFoundX, units[0].Name);
                        return units[0];
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetUnitAttackingMe");
            }
            

            // no units, or error
            return null;
        }

        // Combat Status
        #endregion

        #region BeginCombat

        /// <summary>
        /// Begins combat. Will attempt to pull if the user is not currently in combat.
        /// </summary>
        public clsGlobals.AttackOutcome BeginCombat(WoWUnit UnitToAttack)
        {
            List<string> SpamSpells = new List<string>();
            Random rnd = new Random(DateTime.Now.Millisecond);

            try
            {
                // exit if no unit or character
                using (new clsFrameLock.LockBuffer())
                {
                    if ((!clsCharacter.CharacterIsValid) || (UnitToAttack == null) || (!UnitToAttack.IsValid) || (UnitToAttack.Dead))
                        return clsGlobals.AttackOutcome.Success;
                }

                // exit if dead
                if (clsCharacter.IsDead)
                    return clsGlobals.AttackOutcome.Dead;

                using (new clsFrameLock.LockBuffer())
                {
                    // set the module unit to attack (for helper functions)
                    m_UnitToAttack = UnitToAttack;

                    // log creature name
                    clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, "{0} Lvl: {1}", UnitToAttack.Name, UnitToAttack.Level);

                }

                // reset the stuck timer
                clsStuck.ResetStuck();

                // stop moving and dismount
                clsPath.StopMoving();
                clsMount.Dismount();

                // stand if sitting
                clsSettings.StandUp();

                // face the unit
                clsFace.FacePointExCombat(clsPath.GetUnitLocation(UnitToAttack));

                // heal if needed
                CheckNeedHeal();

                // move closer (target range + 5)
                if (clsPath.DistanceToTarget(UnitToAttack) > clsSettings.gclsLevelSettings.TargetRange + 5)
                    clsPath.MoveToTarget(UnitToAttack, clsSettings.gclsLevelSettings.TargetRange + 5);

                // if user wants to use the external combat routine, let me do so
                if (ISXCombat.clsCombat.UseExternalCombat)
                {
                    clsSettings.Logging.AddToLog(Resources.BeginCombat, Resources.UsingExternalCombatRoutine);
                    return BeginExternalCombat(UnitToAttack);
                }

                // hook facing wrong way; attempts to run away events
                clsWowEvents.UIErrorMsg += clsWowEvents_UIErrorMsg;
                clsWowEvents.ChatMonsterEmote += clsWowEvents_ChatMonsterEmote;

                // if we have a class routine, use that instead
                switch (clsCharacter.MyClass)
                {
                    case WoWClass.Warrior:
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.UsingXCombatRoutine, Resources.Warrior);
                        return new clsCombat_Warrior().BeginCombat(UnitToAttack);

                    case WoWClass.Paladin:
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.UsingXCombatRoutine, Resources.Paladin);
                        return new clsCombat_Paladin().BeginCombat(UnitToAttack);

                    case WoWClass.Druid:
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.UsingXCombatRoutine, Resources.Druid);
                        return new clsCombat_Druid().BeginCombat(UnitToAttack);

                    case WoWClass.Shaman:
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.UsingXCombatRoutine, Resources.Shaman);
                        return new clsCombat_Shaman().BeginCombat(UnitToAttack);

                    case WoWClass.Mage:
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.UsingXCombatRoutine, Resources.Mage);
                        return new clsCombat_Mage().BeginCombat(UnitToAttack);

                    case WoWClass.Hunter:
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.UsingXCombatRoutine, Resources.Hunter);
                        return new clsCombat_Hunter().BeginCombat(UnitToAttack);

                    case WoWClass.Warlock:
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.UsingXCombatRoutine, Resources.Warlock);
                        return new clsCombat_Warlock().BeginCombat(UnitToAttack);

                    case WoWClass.Priest:
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.UsingXCombatRoutine, "Priest");
                        return new clsCombat_Priest().BeginCombat(UnitToAttack);

                    case WoWClass.Rogue:
                        break;
                }

                // if random, resort the list
                if (clsSettings.gclsLevelSettings.Combat_CastSpamRandomly)
                {
                    clsSettings.Logging.AddToLog(Resources.BeginCombat, Resources.BuildingRandomSpamSpellList);

                    int spamCount = clsSettings.gclsLevelSettings.Combat_SpamSpells_List.Count;
                    List<int> spamIndexList = new List<int>();

                    // build random spell list
                    for (int spellCounter = 0; spellCounter < spamCount; spellCounter++)
                    {
                        // get the next random index
                        int spellIndex = rnd.Next(0, spamCount);

                        // make sure we haven't already used this index
                        while (spamIndexList.Contains(spellIndex))
                            spellIndex = rnd.Next(0, spamCount);

                        // add to the index and spamspells list
                        spamIndexList.Add(spellIndex);
                        SpamSpells.Add(clsSettings.gclsLevelSettings.Combat_SpamSpells_List[spellIndex]);
                    }
                }
                else
                {
                    // add the original list
                    foreach (string spamSpell in clsSettings.gclsLevelSettings.Combat_SpamSpells_List)
                        SpamSpells.Add(spamSpell);
                }

                // if we are not in currently in combat, do pre-combat prep
                if (! IsInCombat())
                {
                    // check if script stopped
                    if (! clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsGlobals.AttackOutcome.Stopped;

                    #region Pre Combat Prep

                    // log it
                    clsSettings.Logging.AddToLog(Resources.PreCombatPrep);

                    #region Pre Buff

                    if ((clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List != null) && (clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List.Count > 0))
                    {
                        clsSettings.Logging.DebugWrite(string.Format(Resources.PreBuffSpellsX, clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List.Count));

                        // cast prebuff spells
                        foreach (string SpellName in clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List)
                        { 
                            // skip if no spell
                            if (string.IsNullOrEmpty(SpellName))
                                continue;

                            // check if script stopped
                            if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                                return clsGlobals.AttackOutcome.Stopped;

                            // see if character has the buff already
                            if (clsCharacter.BuffExists(SpellName))
                            {
                                clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.PreBuffAlreadyHaveBuff, SpellName);
                                continue;
                            }

                            // cast the spell
                            CastSpell(SpellName);
                        }
                    }

                    // Pre Buff
                    #endregion

                    // Pre Combat Prep
                    #endregion
                }

                // target the unit
                using (new clsFrameLock.LockBuffer())
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.TargetingX, UnitToAttack.Name);
                    UnitToAttack.Target();
                }

                // toggle attack
                using (new clsFrameLock.LockBuffer())
                {
                    if (!clsSettings.isxwow.Me.Attacking)
                        clsSettings.isxwow.Me.ToggleAttack();
                }

                // face the unit
                clsFace.FacePointExCombat(clsPath.GetUnitLocation(UnitToAttack));

                // pull, if the mob is not close
                #region Pull

                // check if script stopped
                if (! clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                    return clsGlobals.AttackOutcome.Stopped;

                bool DoPull = clsPath.DistanceToTarget(UnitToAttack) > 9;

                if (DoPull)
                {
                    // we need to move closer if the unit is too far away
                    // get the distance we should be from unit
                    int pDistance;
                    if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_PullSpell))
                        pDistance = clsSettings.gclsLevelSettings.TargetRange;
                    else
                        pDistance = 3;

                    // move close if too far away
                    if (clsPath.DistanceToTarget(UnitToAttack) > pDistance)
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.TargetIsTooFarAwayMovingCloser, (clsCharacter.MyLocation.Distance(clsPath.GetUnitLocation(UnitToAttack))));
                        clsPath.Move(MovementDirection.Forward);
                    }

                    // move to the target
                    clsPath.MoveToTarget(UnitToAttack, pDistance);

                    #region Pull with Spell

                    // if we have a pull spell, use it
                    if (! string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_PullSpell))
                    {
                        // cast the spell
                        clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.CastingPullSpellX, clsSettings.gclsLevelSettings.Combat_PullSpell);
                        CastSpell(clsSettings.gclsLevelSettings.Combat_PullSpell, UnitToAttack);
                    }

                    // Pull with Spell
                    #endregion
                }

                // Pull
                #endregion

                #region Protection Spell

                // protection spell
                if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ProtectionSpell))
                {
                    // check if script stopped
                    if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsGlobals.AttackOutcome.Stopped;

                    clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.CastingProtectionSpellX, clsSettings.gclsLevelSettings.Combat_ProtectionSpell);

                    // cast it
                    CastSpell(clsSettings.gclsLevelSettings.Combat_ProtectionSpell);
                }

                // Protection Spell
                #endregion

                // get the mob's health
                LastBugCheck = DateTime.Now;

                // check if we need the heal over time
                if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_HealingOT))
                {
                    // see if we have more than one mob on us
                    if (NumUnitsAttackingMe() > 1)
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.CastingHealOverTimeX, clsSettings.gclsLevelSettings.Combat_HealingOT);
                        CastSpell(clsSettings.gclsLevelSettings.Combat_HealingOT);
                    }
                }

                // cast combat totem if one exists
                if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_CombatTotem))
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.CastingCombatTotemX, clsSettings.gclsLevelSettings.Combat_CombatTotem);
                    CastSpell(clsSettings.gclsLevelSettings.Combat_CombatTotem);
                }

                // FIGHT
                // while neither of us is dead
                while (CheckDead(UnitToAttack) == clsGlobals.DeadUnitEnum.Neither)
                {
                    // check if script stopped
                    if (! clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsGlobals.AttackOutcome.Stopped;

                    // target, for safety
                    MoveAndFace(UnitToAttack);

                    // toggle attack
                    ToggleAttack();

                    // face the unit
                    MoveAndFace(UnitToAttack);

                    #region Heal and Panic Checks

                    // See if we need to heal
                    CheckNeedHeal();
                    
                    // check for panic
                    if (NeedsPanic())
                        return clsGlobals.AttackOutcome.Panic;

                    // Heal and Panic Checks
                    #endregion

                    // if we need to cast the HOT, then cast it now
                    NeedsHOT();

                    // break if dead
                    if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        break;

                    // check if script stopped
                    if (! clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsGlobals.AttackOutcome.Stopped;

                    // face the unit
                    MoveAndFace(UnitToAttack);

                    // cast dot's
                    if ((!clsCharacter.IsCaster()) ||
                        ((clsCharacter.IsCaster()) && (clsCharacter.ManaPercent > clsSettings.gclsLevelSettings.Combat_ManaSpam)))
                    {
                        if (clsSettings.gclsLevelSettings.Combat_DOT_List.Count > 0)
                        {
                            clsSettings.Logging.AddToLog(Resources.Combat, Resources.CastingDOTs);
                            foreach (string dotSpell in clsSettings.gclsLevelSettings.Combat_DOT_List)
                            {
                                // break if dead
                                if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                                    break;

                                // face the unit
                                MoveAndFace(UnitToAttack);

                                // cast spell
                                CastSpell(dotSpell, UnitToAttack);
                            }
                        }
                    }

                    // break if dead
                    if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        break;

                    #region Heal and Panic Checks

                    // See if we need to heal
                    CheckNeedHeal();

                    // check for panic
                    if (NeedsPanic())
                        return clsGlobals.AttackOutcome.Panic;

                    // Heal and Panic Checks
                    #endregion

                    // if we need to cast the HOT, then cast it now
                    NeedsHOT();
                    MoveAndFace(UnitToAttack);

                    #region Check Dead and Script Stopped

                    // break if dead
                    if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        break;

                    // check if script stopped
                    if (! clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsGlobals.AttackOutcome.Stopped;

                    // Check Dead and Script Stopped
                    #endregion

                    // Cast Combat Spells
                    if ((clsSettings.gclsLevelSettings.Combat_SpamSpells_List.Count > 0) && 
                       (((!clsCharacter.IsCaster()) ||
                       ((clsCharacter.IsCaster()) && (clsCharacter.ManaPercent > clsSettings.gclsLevelSettings.Combat_ManaSpam)))))

                    {
                        clsSettings.Logging.AddToLog(Resources.Combat, Resources.CastingSpamSpells);

                        foreach (string spamSpell in SpamSpells)
                        {
                            // break if dead
                            if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                                break;

                            // face the unit
                            MoveAndFace(UnitToAttack);

                            // cast spell
                            CastSpell(spamSpell, UnitToAttack);
                        }
                    }

                    #region Check Dead and Script Stopped

                    // break if dead
                    if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        break;

                    // check if script stopped
                    if (! clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsGlobals.AttackOutcome.Stopped;

                    // Check Dead and Script Stopped
                    #endregion

                    #region Heal and Panic Checks

                    // See if we need to heal
                    CheckNeedHeal();

                    // check for panic
                    if (NeedsPanic())
                        return clsGlobals.AttackOutcome.Panic;

                    // Heal and Panic Checks
                    #endregion

                    // sleep, to let our character whack on the mob some
                    bool TargetValid;
                    using (new clsFrameLock.LockBuffer())
                        TargetValid = (UnitToAttack != null) && (UnitToAttack.IsValid);
                    if ((clsSettings.gclsLevelSettings.Combat_WaitTime_ms > 0) && TargetValid)
                    {
                        MoveAndFace(UnitToAttack);

                        // start shooting if we need to
                        if ((clsCharacter.IsWandUser()) && ((clsCharacter.ManaPercent <= clsSettings.gclsLevelSettings.Combat_ManaSpam)))
                            CastSpell_NoWait("Shoot");

                        clsSettings.Logging.AddToLog(string.Format(Resources.SleepingToLetOurCharacterWhackOnMob, clsSettings.gclsLevelSettings.Combat_WaitTime_ms));
                        Thread.Sleep(clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
                    }

                    #region Check Dead and Script Stopped

                    // break if dead
                    if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        break;

                    // check if script stopped
                    if (! clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsGlobals.AttackOutcome.Stopped;

                    // Check Dead and Script Stopped
                    #endregion

                    #region Heal and Panic Checks

                    // See if we need to heal
                    CheckNeedHeal();

                    // check for panic
                    if (NeedsPanic())
                        return clsGlobals.AttackOutcome.Panic;

                    // Heal and Panic Checks
                    #endregion

                    // Check if this unit is bugged
                    if (IsBugged(UnitToAttack))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.XIsBugged, UnitToAttack.Name);

                        // if we are dead, return that instead
                        clsGlobals.DeadUnitEnum due = CheckDead(UnitToAttack);
                        if (due == clsGlobals.DeadUnitEnum.Character)
                            return clsGlobals.AttackOutcome.Dead;
                        else
                            return clsGlobals.AttackOutcome.Bugged;
                    }
                }

                // handle post combat
                return HandlePostCombat(UnitToAttack);
            }

            catch (Exception excep)
            {
                // show the error
                clsError.ShowError(excep, "BeginCombat", true, new StackFrame(0, true), true);
            }

            finally
            {
                // unhook facing wrong way event
                clsWowEvents.UIErrorMsg -= clsWowEvents_UIErrorMsg;

                // durability check
                if (clsCharacter.DurabilityPercent <= clsSettings.gclsLevelSettings.DurabilityPercent)
                    clsEvent.Raise_CharacterDurabilityLow(clsCharacter.DurabilityPercent);

                // reset the stuck timer
                clsStuck.ResetStuck();
            }

            // shouldn't be here. for safety, return success
            using (new clsFrameLock.LockBuffer())
            {
                if (UnitToAttack != null)
                    clsSettings.Logging.AddToLogFormatted(Resources.UnitXHasDied, UnitToAttack.Name);
            }
            return clsGlobals.AttackOutcome.Success;
        }

        /// <summary>
        /// Handles post combat stuff
        /// </summary>
        /// <returns></returns>
        public clsGlobals.AttackOutcome HandlePostCombat(WoWUnit UnitToAttack)
        {
            // get the result of combat
            clsGlobals.DeadUnitEnum due = CheckDead(UnitToAttack);

            // exit if dead
            if (due == clsGlobals.DeadUnitEnum.Character)
            {
                clsSettings.Logging.AddToLog(Resources.CharacterDiedInCombat);
                return clsGlobals.AttackOutcome.Dead; // DEAD! :(
            }

            using (new clsFrameLock.LockBuffer())
            {
                // add the unit to the loot list
                clsSettings.gclsLevelSettings.LootList.Add(UnitToAttack);

                // raise the event this mob was killed
                clsGlobals.Raise_MobKilled(UnitToAttack.Name, UnitToAttack.Level);
            }

            // if we are still being attacked, find that unit and exit
            if (IsInCombat())
            {
                // get the attacking target, and fight him, too
                WoWUnit attackUnit = GetUnitAttackingMe();

                // if we found a unit, fight it
                if (attackUnit != null)
                {
                    // log it
                    using (new clsFrameLock.LockBuffer())
                    {
                        clsSettings.Logging.AddToLog(
                            string.Format(Resources.StillBeingAttackedByX,
                                attackUnit.Name, attackUnit.Level,
                                attackUnit.Location.X, attackUnit.Location.Y, attackUnit.Location.Z));
                    }

                    // kill him
                    clsGlobals.AttackOutcome aOut = BeginCombat(attackUnit);
                    switch (aOut)
                    {
                        case clsGlobals.AttackOutcome.Success:
                        case clsGlobals.AttackOutcome.Bugged:
                            break;
                        default:
                            clsSettings.Logging.AddToLogFormatted(Resources.BeginCombat, Resources.ExitWithCodeX, aOut.ToString());
                            return aOut;
                    }
                }

                // check dead status and return if dead
                due = CheckDead(UnitToAttack);
                if (due == clsGlobals.DeadUnitEnum.Character)
                {
                    clsSettings.Logging.AddToLog(Resources.CharacterDiedInCombat);
                    return clsGlobals.AttackOutcome.Dead; // DEAD! :(
                }
            }

            // check if we need healing
            CheckNeedHeal(false);

            // cast post combat spells
            if ((clsSettings.gclsLevelSettings.Combat_PostCombatSpells != null) && (clsSettings.gclsLevelSettings.Combat_PostCombatSpells.Count > 0))
            {
                clsSettings.Logging.AddToLog(Resources.Combat, Resources.CastingPostCombatSpells);
                CastSpellList(clsSettings.gclsLevelSettings.Combat_PostCombatSpells);
            }

            // not being attacked, return result
            using (new clsFrameLock.LockBuffer())
                clsSettings.Logging.AddToLog(string.Format(Resources.UnitXHasDied, UnitToAttack.Name));
            return clsGlobals.AttackOutcome.Success;
        }

        /// <summary>
        /// Faces unit, then moves towards it if too far away
        /// </summary>
        /// <param name="UnitToAttack">the unit to face/attack</param>
        public void MoveAndFace(WoWUnit UnitToAttack)
        {
            int dist;

            using (new clsFrameLock.LockBuffer())
            {
                // target if not alrady targetting
                if (UnitToAttack.GUID != clsCharacter.CurrentTarget.GUID)
                    UnitToAttack.Target();

                // get how close we should be from target
                dist = (UnitToAttack.CurrentTarget.GUID == clsSettings.isxwow.Me.GUID) ? 6 : 4;
            }

            using (new clsFrameLock.LockBuffer())
            {
                PathListInfo.PathPoint uPoint = clsPath.GetUnitLocation(UnitToAttack);
                clsSettings.isxwow.Face(uPoint.X, uPoint.Y, 120);
            }

            // move if more than 8 yards away
            if (clsPath.DistanceToTarget(UnitToAttack) > dist)
                clsPath.MoveToTarget(UnitToAttack, dist);

            // if less than 5 yards, back up
            if (clsPath.DistanceToTarget(UnitToAttack) < 2.5)
            {
                clsSettings.Logging.AddToLogFormatted("MoveAndFace", Resources.TooCloseToTargetBackingUp, clsPath.DistanceToTarget(UnitToAttack));

                while (clsPath.DistanceToTarget(UnitToAttack) < 2)
                {
                    clsFace.FacePointExCombat(clsPath.GetUnitLocation(UnitToAttack));
                    clsPath.Move(MovementDirection.Backward);
                }
                clsPath.StopMoving();
                clsFace.FacePointExCombat(clsPath.GetUnitLocation(UnitToAttack));
            }
        }

        /// <summary>
        /// Toggles use into attack mode if we are not already there
        /// </summary>
        public void ToggleAttack()
        {
            using (new clsFrameLock.LockBuffer())
            {
                if ((!clsSettings.isxwow.Me.Attacking) && (!clsSettings.isxwow.Me.AttackingUnit.Dead))
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.TogglingAttack, clsSettings.isxwow.Me.IsInCombat.ToString(), clsSettings.isxwow.Me.Attacking.ToString());
                    clsSettings.isxwow.Me.ToggleAttack();
                }
            }
        }

        // BeginCombat
        #endregion

        // Functions
        #endregion

        #region Combat Assist

        #region Cast Spell

        /// <summary>
        /// Casts a spell. You must already have something targetted. Returns when spellcast is complete
        /// returns false if spell can't be cast
        /// </summary>
        /// <param name="SpellName">the spell to cast</param>
        public static bool CastSpell(string SpellName)
        {
            bool rVal = false;
            WoWSpell wSpell;
            WoWUnit currTarget;

            try
            {
                // exit if not valid, no spell, or stunned
                if ((!clsSettings.GuidValid) || (string.IsNullOrEmpty(SpellName)) || (CheckStunned()))
                {
                    clsSettings.Logging.AddToLog(Resources.CastSpell, Resources.InvalidGuidNoSpellName);
                    return false;
                }

                // log spell
                clsSettings.Logging.AddToLogFormatted(Resources.CastSpell, Resources.CastingSpellX, SpellName);

                // face the target
                using (new clsFrameLock.LockBuffer())
                {
                    currTarget = clsCharacter.CurrentTarget;
                    if ((currTarget != null) && (currTarget.IsValid) && (currTarget.GUID != clsSettings.isxwow.Me.GUID))
                        clsFace.FacePointExCombat(clsPath.GetUnitLocation(currTarget));
                }

                // get the spell
                using (new clsFrameLock.LockBuffer())
                {
                    wSpell = WoWSpell.Get(SpellName);

                    // check if valid spell
                    if ((wSpell == null) || (!wSpell.IsValid))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.CastSpell, Resources.InvalidSpell, SpellName);
                        return false;
                    }

                    // exit if not usable or cost too high
                    int cPower = CurrentPower();
                    int wCost = wSpell.PowerCost;
                    if (wCost > cPower)
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.CastSpell, Resources.SpellXCostsTooMuch, SpellName, wCost, cPower);
                        return false;
                    }
                }

                return CastSpell(wSpell);
            }

            catch (Exception excep)
            {
                // show the error
                clsError.ShowError(excep, "CastSpell");
            }

            return rVal;
        }

        /// <summary>
        /// Casts a spell while targetting the selected unit
        /// </summary>
        /// <param name="SpellName">spell to cast</param>
        /// <param name="UnitToTarget">unit to target</param>
        public static bool CastSpell(string SpellName, WoWUnit UnitToTarget)
        {
            try
            {
                // face unit
                clsFace.FacePointExCombat(clsPath.GetUnitLocation(UnitToTarget));

                // target unit
                using (new clsFrameLock.LockBuffer())
                    UnitToTarget.Target();

                // cast the spell
                return CastSpell(SpellName);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "CastSpell");
            }

            return false;
        }

        /// <summary>
        /// Casts a spell. Does NOT check mana/rage/energy. Only checks if spell is valid. Returns when spellcast is complete
        /// returns false if spell can't be cast
        /// </summary>
        /// <param name="SpellName">the spell to cast</param>
        public static bool CastSpell_NoCheck(string SpellName)
        {
            bool rVal = false;
            WoWSpell wSpell;

            try
            {
                // exit if not valid
                if ((!clsSettings.GuidValid) || (string.IsNullOrEmpty(SpellName)) || (CheckStunned())) 
                    return false;

                // log spell
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLogFormatted(Resources.CastSpell, Resources.CastingSpellX, SpellName);

                // get the spell
                using (new clsFrameLock.LockBuffer())
                {
                    wSpell = WoWSpell.Get(SpellName);

                    // check if valid spell
                    if ((wSpell == null) || (!wSpell.IsValid))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.CastSpellNoCheck, Resources.InvalidSpell, SpellName);
                        return false;
                    }
                }

                return CastSpell(wSpell);
            }

            catch (Exception excep)
            {
                // show the error
                clsError.ShowError(excep, "CastSpell");
            }

            finally
            {
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLogFormatted(Resources.CastSpellNoCheck, Resources.ReturningX, rVal.ToString());
            }

            return rVal;
        }

        /// <summary>
        /// Casts a list of spells on the target mob
        /// </summary>
        /// <param name="SpellList">the list of spells to cast</param>
        public static void CastSpellList(List<string> SpellList)
        {
            // exit if no spells
            if ((SpellList == null) || (SpellList.Count == 0))
                return;

            // loop through our dot spells and see if the mob has this on it
            foreach (string spellItem in SpellList)
            {
                // exit if out of mana
                if (clsCharacter.ManaPercent <= clsSettings.gclsLevelSettings.Combat_ManaSpam)
                    return;

                // cast the spell
                CastSpell(spellItem);
            }
        }

        /// <summary>
        /// Casts a spell and does not wait for it to finish casting (does wait 100 ms)
        /// </summary>
        /// <param name="SpellName">the spell to cast</param>
        public static bool CastSpell_NoWait(string SpellName)
        {
            bool rVal = false;
            WoWSpell wSpell;

            try
            {
                // exit if not valid
                if ((!clsSettings.GuidValid) || (string.IsNullOrEmpty(SpellName)) || (CheckStunned()))
                    return false;

                // log spell
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLogFormatted(Resources.CastSpell, Resources.CastingSpellX, SpellName);

                // face the target
                using (new clsFrameLock.LockBuffer())
                {
                    WoWUnit currTarget = clsCharacter.CurrentTarget;
                    if ((currTarget != null) && (currTarget.IsValid) && (currTarget.GUID != clsSettings.isxwow.Me.GUID))
                        clsFace.FacePointExCombat(clsPath.GetUnitLocation(currTarget));
                }

                // get the spell
                using (new clsFrameLock.LockBuffer())
                {
                    wSpell = WoWSpell.Get(SpellName);

                    // check if valid spell
                    if ((wSpell == null) || (!wSpell.IsValid))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.CastSpell, Resources.InvalidSpell, SpellName);
                        return false;
                    }

                    // cast
                    rVal = wSpell.Cast();
                }
            }

            catch (Exception excep)
            {
                // show the error
                clsError.ShowError(excep, "CastSpell_NoWait");
            }

            finally
            {
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.AddToLogFormatted(Resources.ReturningX, rVal.ToString());
            }

            return rVal;
        }

        /// <summary>
        /// Casts a prepared spell
        /// </summary>
        /// <param name="wSpell">the spell to cast (MUST NOT BE NULL)</param>
        /// <returns></returns>
        private static bool CastSpell(WoWSpell wSpell)
        {
            int cTime = 0;

            // cast it
            using (new clsFrameLock.LockBuffer())
            {
                if (!wSpell.Cast())
                    return false;
            }

            // loop until spell is cast
            while (clsCharacter.IsCasting)
                Thread.Sleep(200);

            // keep sleeping until we finish the spell cooldown
            using (new clsFrameLock.LockBuffer())
            {
                // get time remaining to wait
                if (wSpell.CastTime <= 1500)
                    cTime = 1500 - wSpell.CastTime;
            }

            if (cTime > 0)
                Thread.Sleep(cTime);

            return true;
        }

        // Cast Spell
        #endregion

        /// <summary>
        /// check if we are stunned/silenced. Returns true if stunned
        /// </summary>
        private static bool CheckStunned()
        {
            using (new clsFrameLock.LockBuffer())
            {
                return ((clsCharacter.BuffExists("Stunned")) ||
                    (clsCharacter.BuffExists("Silenced")) ||
                    (clsCharacter.BuffExists("Petrified")));
            }
        }

        /// <summary>
        /// Checks if we need to heal. If true, will cast heal spell
        /// </summary>
        /// <param name="UsePotions">when false, will not attempt to use potions</param>
        public void CheckNeedHeal(bool UsePotions)
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    if (clsSettings.VerboseLogging)
                        clsSettings.Logging.AddToLogFormatted("CheckNeedHeal", Resources.CheckNeedHealHealthPercent, clsCharacter.HealthPercent, clsSettings.gclsLevelSettings.Combat_HealthPercent);

                    // if we are above the setting, then exit
                    if (clsCharacter.HealthPercent > clsSettings.gclsLevelSettings.Combat_HealthPercent)
                        return;
                }

                // if no healing spell, try a healthstone
                if (string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_HealSpell))
                {
                    // if a warlock, then try Drain Life
                    if (clsCharacter.MyClass == WoWClass.Warlock)
                        CastSpell("Drain Life");

                    // if we are above the setting, then exit
                    if (clsCharacter.HealthPercent > clsSettings.gclsLevelSettings.Combat_HealthPercent)
                        return;

                    WoWItem hStone = clsSearch.Search_BagItemOne("Healthstone");
                    if ((hStone != null) && (hStone.IsValid))
                        hStone.Use();

                    // if we are above the setting, then exit
                    if (clsCharacter.HealthPercent > clsSettings.gclsLevelSettings.Combat_HealthPercent)
                        return;

                    // try to drink a potion
                    clsPotions.DrinkBestHealingPotion();
                    return;
                }

                // heal ourself, drink potion if we fail
                if ((!CastSpell(clsSettings.gclsLevelSettings.Combat_HealSpell)) &&
                    (UsePotions))
                        clsPotions.DrinkBestHealingPotion();
            }

            finally
            {
                // if we are holding our breath, jump jump jump
                if (clsCharacter.IsHoldingBreath)
                {
                    clsPath.DoJump();
                    LavishVMAPI.Frame.Wait(false);
                    clsPath.DoJump();
                    LavishVMAPI.Frame.Wait(false);
                    clsPath.DoJump();
                }

                // check if we need to remove a debuff
                CheckNeedDebuff();
            }
        }

        /// <summary>
        /// Checks if we need to heal. If true, will cast heal spell
        /// </summary>
        public void CheckNeedHeal()
        {
            CheckNeedHeal(true);
        }

        /// <summary>
        /// Checks if we need to drink a mana potion
        /// </summary>
        public void CheckNeedMana()
        {
            List<WoWItem> ManaStoneList;

            using (new clsFrameLock.LockBuffer())
            {
                // exit if not caster
                if (!clsCharacter.IsCaster())
                    return;

                // see if we have enough mana
                if (clsCharacter.ManaPercent > 25)
                    return;

                // we need mana, check if we have a mana stone
                ManaStoneList = clsSearch.Search_BagItem("Mana Emerald");
                if ((ManaStoneList == null) || (ManaStoneList.Count == 0))
                    ManaStoneList = clsSearch.Search_BagItem("Mana Ruby");
                if ((ManaStoneList == null) || (ManaStoneList.Count == 0))
                    ManaStoneList = clsSearch.Search_BagItem("Mana Citrine");
                if ((ManaStoneList == null) || (ManaStoneList.Count == 0))
                    ManaStoneList = clsSearch.Search_BagItem("Mana Jade");
                if ((ManaStoneList == null) || (ManaStoneList.Count == 0))
                    ManaStoneList = clsSearch.Search_BagItem("Mana Agate");
            }

            // use the mana stone if we have one
            if ((ManaStoneList != null) && (ManaStoneList.Count > 0))
            {
                using (new clsFrameLock.LockBuffer())
                {                    
                    // only use if the item does not have a cooldown and is usable
                    if ((ManaStoneList[0].IsValid) && (clsItem.GetItemCooldown(ManaStoneList[0]) == 0))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.UsingManaStone, ManaStoneList[0].Name);
                        ManaStoneList[0].Use();
                    }
                }

                // see if we have enough mana, now
                if (clsCharacter.ManaPercent > 25)
                    return;
            }

            // we need mana
            clsPotions.DrinkBestManaPotion();
        }

        /// <summary>
        /// Returns a result if the character or mob is dead
        /// </summary>
        public clsGlobals.DeadUnitEnum CheckDead(WoWUnit UnitToAttack)
        {
            clsGlobals.DeadUnitEnum due = clsGlobals.DeadUnitEnum.Neither;

            using (new clsFrameLock.LockBuffer())
            {
                if (clsSettings.VerboseLogging)
                    clsSettings.Logging.DebugWrite(string.Format(Resources.CheckDeadAttackingUnit,
                        UnitToAttack.Name,
                        UnitToAttack.Dead,
                        clsCharacter.IsDead));

                // get mob death
                if (UnitToAttack.Dead)
                    due = clsGlobals.DeadUnitEnum.Mob;

                // get character death
                if (clsSettings.isxwow.Me.Dead)
                    due = clsGlobals.DeadUnitEnum.Character;
            }

            // return result
            return due;
        }

        /// <summary>
        /// Checks if we can and need to cast a healing over time spell. Returns true if we cast the HOT
        /// </summary>
        public void NeedsHOT()
        {
            // exit if we can't cast
            if (string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_HealingOT))
                return;

            // exit if have the buff
            using (new clsFrameLock.LockBuffer())
            {
                if ((clsCharacter.BuffExists(clsSettings.gclsLevelSettings.Combat_HealingOT)) || (NumUnitsAttackingMe() <= 1))
                    return;
            }

            // log it
            clsSettings.Logging.AddToLogFormatted(Resources.CastingHealOverTimeX, clsSettings.gclsLevelSettings.Combat_HealingOT);

            // cast the healing over time
            CastSpell(clsSettings.gclsLevelSettings.Combat_HealingOT);
        }

        /// <summary>
        /// Returns true if we need to panic
        /// </summary>
        public bool NeedsPanic()
        {
            double panicPct = 0.15;
            bool rVal;

            // exit if panic mode is turned off
            if (!clsSettings.gclsLevelSettings.Combat_DoPanic)
                return false;

            // check mana/health. if both are less than 15%, panic
            if (clsCharacter.HealthPercent <= panicPct)
            {
                // drink health potion
                clsPotions.DrinkBestHealingPotion();

                // if health is still too low, exit
                if (clsCharacter.HealthPercent <= panicPct)
                {
                    // handle warlock panic
                    if (clsCharacter.MyClass == WoWClass.Warlock)
                        WarlockPanic();

                    // exit
                    return true;
                }
            }

            // check mobs attacking me
            rVal = (NumUnitsAttackingMe() >= clsSettings.gclsLevelSettings.Combat_PanicThreshold);

            // if panic, do warlock panic first
            if (rVal)
                WarlockPanic();

            return rVal;
        }

        /// <summary>
        /// handles panic for warlocks
        /// </summary>
        private void WarlockPanic()
        {
            // cast of howl of terror if we need to
            using (new clsFrameLock.LockBuffer())
            {
                if ((clsCharacter.CurrentLevel >= 40) && (clsCharacter.HealthPercent <= 30) && CastSpell("Howl of Terror"))
                    return;
            }

            // cast fear
            CastSpell("Fear");
        }

        /// <summary>
        /// Checks if we can remove curse/poison/disease
        /// </summary>
        public void CheckNeedDebuff()
        {
            // only process if we have remove spells
            if ((!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Curse)) ||
                (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Disease)) ||
                (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Poison)))
            {
                // loop through all buffs on the toon
                for (int buffCounter = 0; buffCounter < 41; buffCounter++)
                {
                    try
                    {
                        string debuffSPell = string.Empty;

                        // get the buff
                        using (new clsFrameLock.LockBuffer())
                        {
                            WoWBuff wbuff = clsSettings.isxwow.Me.Buff(buffCounter);

                            // skip if no buff or invalid or not harmful
                            if ((wbuff == null) || (!wbuff.IsValid) || (!wbuff.IsHarmful))
                                continue;

                            // log it (HealBotFoundDebuff)
                            clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.FoundDebuff, wbuff.Name);

                            // cure poison
                            if ((wbuff.DispelType == "Poison") && (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Poison)))
                            {
                                clsSettings.Logging.AddToLog(Resources.Combat, Resources.RemovingPoison);
                                debuffSPell = clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Poison;
                            }

                            // cure disease
                            if ((wbuff.DispelType == "Disease") && (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Disease)))
                            {
                                clsSettings.Logging.AddToLog(Resources.Combat, Resources.RemovingDisease);
                                debuffSPell = clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Disease;
                            }

                            // remove curse
                            if ((wbuff.DispelType == "Curse") && (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Curse)))
                            {
                                clsSettings.Logging.AddToLog(Resources.Combat, Resources.RemovingCurse);
                                debuffSPell = clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Curse;
                            }
                        }

                        // cast the spell
                        if (!string.IsNullOrEmpty(debuffSPell))
                            CastSpell(debuffSPell);
                    }

                    catch (Exception excep)
                    {
                        clsError.ShowError(excep, "CheckNeedDebuff");
                    }
                }
            }
        }

        // Combat Assist
        #endregion

        #region External Combat

        /// <summary>
        /// This function is called when the user has specified to use an external
        /// combat routine
        /// </summary>
        /// <param name="UnitToAttack">the unit to attack</param>
        private clsGlobals.AttackOutcome BeginExternalCombat(WoWUnit UnitToAttack)
        {
            // default return
            clsGlobals.AttackOutcome outcome = clsGlobals.AttackOutcome.Success;

            try
            {
                // move close if we are too far away
                if (clsPath.DistanceToTarget(UnitToAttack) > clsSettings.gclsLevelSettings.TargetRange + 5)
                    clsPath.MoveToTarget(UnitToAttack, clsSettings.gclsLevelSettings.TargetRange + 5);

                // start the external routine
                ISXCombat.clsCombat combat = new ISXCombat.clsCombat();
                outcome = combat.BeginCombat(UnitToAttack);

                // handle the returns
                switch (outcome)
                {
                    case clsGlobals.AttackOutcome.Success:
                    default:
                        // add unit to loot list
                        clsSettings.gclsLevelSettings.LootList.Add(UnitToAttack);

                        // check for aggro
                        if (NumUnitsAttackingMe() > 0)
                            return BeginCombat(GetUnitAttackingMe());
                        break;

                    case clsGlobals.AttackOutcome.Bugged:
                        // add to bugged list
                        clsSettings.BlackList_Combat.Add(new clsBlacklist(UnitToAttack));
                        return clsGlobals.AttackOutcome.Bugged;

                    case clsGlobals.AttackOutcome.Panic:
                    case clsGlobals.AttackOutcome.Stopped:
                    case clsGlobals.AttackOutcome.Dead:
                        return outcome;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Begin Combat - External Routine");
            }

            return outcome;
        }

        // External Combat
        #endregion

        #region Downtime

        /// <summary>
        /// Returns true if the character needs downtime
        /// </summary>
        public bool NeedDowntime()
        {
            // skip if aggro'd
            if (IsInCombat())
                return false;

            // get the health percent
            using (new clsFrameLock.LockBuffer())
            {
                return ((clsSettings.isxwow.Me.HealthPercent <= clsSettings.gclsLevelSettings.Combat_DowntimePercent) ||
                    ((clsCharacter.IsCaster()) && (clsSettings.isxwow.Me.ManaPercent <= clsSettings.gclsLevelSettings.Combat_DowntimePercent)));
            }
        }

        /// <summary>
        /// Performs downtime. Returns true if Attacked while resting
        /// </summary>
        public bool DoDowntime()
        {
            WoWItem MyFood = null;
            WoWItem MyDrink = null;

            try
            {
                // log
                clsSettings.Logging.AddToLog(Resources.Downtime);

                // get if caster
                bool IsCaster = clsCharacter.IsCaster();

                using (new clsFrameLock.LockBuffer())
                {
                    // get food that we can eat
                    if (clsCharacter.HealthPercent <= clsSettings.gclsLevelSettings.Combat_DowntimePercent)
                        MyFood = FindInFDList("-food");

                    // if caster, then load drink list
                    if ((IsCaster) && (clsCharacter.ManaPercent <= clsSettings.gclsLevelSettings.Combat_DowntimePercent))
                        MyDrink = FindInFDList("-drink");

                    // sit
                    if (!clsSettings.isxwow.Me.Sitting)
                    {
                        clsSettings.Logging.AddToLog(Resources.SittingForDowntime);
                        clsSettings.isxwow.WoWScript("SitStandOrDescendStart()");
                    }
                }

                // if we found food/drink, then use it
                if ((MyDrink != null) || (MyFood != null))
                {
                    // check if in combat, exit if so
                    if (IsInCombat())
                        return true;

                    using (new clsFrameLock.LockBuffer())
                    {
                        // eat
                        if ((MyFood != null) && (!clsCharacter.BuffExists("Well Fed")))
                        {
                            clsSettings.Logging.AddToLog(string.Format(Resources.EatingX, MyFood.Name));
                            MyFood.Use();
                        }
                        else if (clsCharacter.HealthPercent <= clsSettings.gclsLevelSettings.Combat_DowntimePercent)
                        {
                            // try to use a bandage
                            clsPotions.UseBestBandage();
                        }
                    }

                    // drink
                    if (MyDrink != null)
                    {
                        using (new clsFrameLock.LockBuffer())
                        {
                            clsSettings.Logging.AddToLog(string.Format(Resources.DrinkingX, MyDrink.Name));
                            MyDrink.Use();
                        }
                    }

                    // sleep 16 seconds, this allows us to get any 15 second buffs from the food/drink
                    int sleepCounter = 0;
                    while (sleepCounter < 16)
                    {
                        // check if script stopped
                        if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                            return false;

                        // make sure we're not in combat before we go to sleep
                        if (IsInCombat())
                            return true;

                        // inc counter and sleep 1.1 seconds
                        sleepCounter++;
                        Thread.Sleep(1100);
                    }
                }

                // keep sitting until we have full health/mana
                while (true)
                {
                    // check if script stopped
                    if (! clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return false;

                    // check if in combat, exit if so
                    if (IsInCombat())
                        return true;

                    // log
                    if (clsSettings.VerboseLogging)
                        clsSettings.Logging.AddToLog(Resources.StillNeedDowntimeSleeping);
                    Thread.Sleep(500);

                    using (new clsFrameLock.LockBuffer())
                    {
                        // continue if not full health
                        if (clsCharacter.HealthPercent < 100)
                            continue;

                        // continue if not full mana
                        if ((clsCharacter.IsCaster()) && (clsCharacter.ManaPercent < 100))
                            continue;
                    }

                    // full health, so exit
                    break;
                }

                // handle class specific post combat routines
                switch (clsCharacter.MyClass)
                {
                    case WoWClass.Hunter:
                        // stand
                        clsSettings.StandUp();

                        // do hunter stuff
                        HunterPostCombat();

                        // check if we need to do downtime again
                        if (NeedDowntime())
                        {
                            clsSettings.Logging.AddToLogFormatted(Resources.Downtime, Resources.RedoingDowntimeAfterCastingXSpells, Resources.Hunter);
                            DoDowntime();
                        }
                        break;

                    case WoWClass.Mage:
                        // stand
                        clsSettings.StandUp();

                        // do mage stuff
                        MagePostCombat();

                        // check if we need to do downtime again
                        if (NeedDowntime())
                        {
                            clsSettings.Logging.AddToLogFormatted(Resources.Downtime, Resources.RedoingDowntimeAfterCastingXSpells, Resources.Mage);
                            DoDowntime();
                        }
                        break;

                    case WoWClass.Warlock:
                        // stand
                        clsSettings.StandUp();

                        // do warlock stuff
                        WarlockPostCombat();

                        // check if we need to do downtime again
                        if (NeedDowntime())
                        {
                            clsSettings.Logging.AddToLogFormatted(Resources.Downtime, Resources.RedoingDowntimeAfterCastingXSpells, Resources.Warlock);
                            DoDowntime();
                        }
                        break;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Downtime");
            }

            finally
            {
                // stand up
                clsSettings.StandUp();
            }

            return false;
        }

        /// <summary>
        /// Returns the best food/drink that is available from the food/drink list
        /// </summary>
        /// <param name="ItemFilter">-food for food, -drink for drink</param>
        public WoWItem FindInFDList(string ItemFilter)
        {
            return clsItem.FindItem(string.Format("-items,-inventory,{0}", ItemFilter));
        }

        // Downtime
        #endregion

        #region Looting

        /// <summary>
        /// Loots all objects in the loot list
        /// Returns true if we enter combat
        /// </summary>
        public bool DoLoot()
        {
            List<WoWUnit> RemoveList = new List<WoWUnit>();
            int j = clsSettings.gclsLevelSettings.LootList.Count;
            WoWUnit lootUnit;
            bool InCombat = false;

            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.BeginingLootLoop);

                // if we are sitting, stand up
                clsSettings.StandUp();

                // loop through the lootlist
                bool stopped = false;
                for (int i=0; i<j; i++)
                {
                    // get the unit to loot
                    using (new clsFrameLock.LockBuffer())
                    {
                        lootUnit = clsSettings.gclsLevelSettings.LootList[i];
                        if ((lootUnit == null) || (!lootUnit.IsValid))
                            continue;
                    }

                    // if not stopped, loot the unit
                    if (!stopped)
                    {
                        clsPath.EMovementResult result = clsLoot.LootGameOjbect(lootUnit, false);

                        // add the unit to the remove list
                        if (result != clsPath.EMovementResult.Aggroed)
                            RemoveList.Add(lootUnit);

                        // if error or aggro, return result
                        if (result == clsPath.EMovementResult.Aggroed)
                        {
                            InCombat = true;
                            return true;
                        }

                        // if stopped, set the flag
                        if (result == clsPath.EMovementResult.Stopped)
                            stopped = true;
                    }

                    else // clear the list then exit
                    {
                        clsSettings.Logging.AddToLog(Resources.Stopped);
                        break;
                    }
                }

                // clear the loot list
                clsSettings.gclsLevelSettings.LootList.Clear();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "DoLoot");
            }

            finally
            {
                // any items in the remove list need to be removed. There was a general problem
                // looting this item
                if ((!InCombat) && (clsSettings.gclsLevelSettings.LootList.Count > 0))
                    clsSettings.gclsLevelSettings.LootList.Clear();

                // if we are in combat, then just remove the items we could loot
                if ((InCombat) && (clsSettings.gclsLevelSettings.LootList.Count > 0))
                    clsSettings.gclsLevelSettings.LootList.RemoveAll(x => RemoveList.Contains(x));

                clsSettings.Logging.AddToLog(Resources.LootLoopExiting);
            }

            return false;
        }

        // Looting
        #endregion

        #region Power and Power Type

        // code by Loop - http://www.isxwow.net/forums/viewtopic.php?f=15&t=1465
        public enum powerType { Rage, Energy, Mana };
        private static bool IsPowerTypeSet = false;
        private static powerType m_PowerType = powerType.Mana;
        public static powerType GetPowerType()
        {
            // exit if we already have the power type from a previous call
            if (IsPowerTypeSet)
                return m_PowerType;

            using (new clsFrameLock.LockBuffer())
            {
                IsPowerTypeSet = true;

                // handle standard classes
                switch (clsSettings.isxwow.Me.Class)
                {
                    case WoWClass.Warrior:
                        m_PowerType = powerType.Rage;
                        return m_PowerType;

                    case WoWClass.Paladin:
                    case WoWClass.Hunter:
                    case WoWClass.Mage:
                    case WoWClass.Priest:
                    case WoWClass.Warlock:
                    case WoWClass.Shaman:
                        m_PowerType = powerType.Mana;
                        return m_PowerType;
                    
                    case WoWClass.Rogue:
                        m_PowerType = powerType.Energy;
                        return m_PowerType;
                }


                // handle for druids (regular and shapeshift form)

                int mE = clsSettings.isxwow.Me.MaxEnergy;
                int mR = clsSettings.isxwow.Me.MaxRage;
                int mM = clsSettings.isxwow.Me.MaxMana;

                // debug it
                if (clsSettings.VerboseLogging)
                {
                    clsSettings.Logging.AddToLogFormatted("GetPowerType", "MaxEnergy = {0}; Current = {1}", mE, clsSettings.isxwow.Me.Energy);
                    clsSettings.Logging.AddToLogFormatted("GetPowerType", "MaxRage = {0}; Current = {1}", mR, clsSettings.isxwow.Me.Rage);
                    clsSettings.Logging.AddToLogFormatted("GetPowerType", "MaxMana = {0}; Current = {1}", mM, clsSettings.isxwow.Me.Mana);
                }

                if ((mR >= mE) && (mR >= mM))
                {
                    clsSettings.Logging.AddToLogFormatted("GetPowerType", Resources.PowerTypeX, Resources.Rage);
                    m_PowerType = powerType.Rage;
                    return powerType.Rage;
                }

                if ((mM >= mE) && (mM >= mR))
                {
                    clsSettings.Logging.AddToLogFormatted("GetPowerType", Resources.PowerTypeX, Resources.Mana);
                    m_PowerType = powerType.Mana;
                    return powerType.Mana;
                }

                if ((mE >= mR) && (mE >= mM))
                {
                    clsSettings.Logging.AddToLogFormatted("GetPowerType", Resources.PowerTypeX, Resources.Energy);
                    m_PowerType = powerType.Energy;
                    return powerType.Energy;
                }

                clsSettings.Logging.AddToLogFormatted("GetPowerType", Resources.PowerTypeX, Resources.Mana);
                m_PowerType = powerType.Mana;
                return powerType.Mana;
            }
        }

        /// <summary>
        /// Returns the current amount of power (mana,rage, energy)
        /// </summary>
        public static int CurrentPower()
        {
            using (new clsFrameLock.LockBuffer())
            {
                switch (GetPowerType())
                {
                    case powerType.Energy:
                        return clsSettings.isxwow.Me.Energy;
                    case powerType.Rage:
                        // TODO: this should be fixed sometime, so we don't have to divide by 10
                        // http://www.isxwow.net/forums/viewtopic.php?f=17&t=830&hilit=
                        return clsSettings.isxwow.Me.Rage * 10;
                    case powerType.Mana:
                    default:
                        return clsSettings.isxwow.Me.Mana;
                }
            }
        }

        // Power and Power Type
        #endregion

        #region Bugged

        /// <summary>
        /// Checks if the mob is bugged
        /// </summary>
        public bool IsBugged(WoWUnit UnitToAttack)
        {
            try
            {
                // if the mob health percent is 100 for 5 loops, then mark it as bugged
                // and exit

                // do this check if bugcounter is > -1. if -1, it means the mob
                // lost health, which means he's not bugged
                bool TargetValid;
                using (new clsFrameLock.LockBuffer())
                    TargetValid = (UnitToAttack != null) && (UnitToAttack.IsValid);
                if ((BugCounter > -1) && TargetValid)
                {
                    // inc counter if the mob is 100% or has gained health
                    using (new clsFrameLock.LockBuffer())
                    {
                        if (UnitToAttack.HealthPercent >= 100)
                        {
                            BugCounter++;
                        }
                        else
                        {
                            // reset bugged counter because the mob lost health
                            BugCounter = -1; 
                            LastBugCheck = DateTime.Now;
                        }
                    }

                    // exit if the bug counter is less than 5
                    if (BugCounter < 5)
                        return false;

                    // if bugged for 7 seconds
                    if (new TimeSpan(DateTime.Now.Ticks - LastBugCheck.Ticks).Seconds > 7)
                    {
                        // bugged, log it and exit
                        using (new clsFrameLock.LockBuffer())
                        {
                            clsSettings.Logging.AddToLog(
                                string.Format(Resources.BUGGEDUnitX,
                                    UnitToAttack.Name, UnitToAttack.Level,
                                    UnitToAttack.Location.X, UnitToAttack.Location.Y, UnitToAttack.Location.Z));

                            // blacklist the unit
                            clsSettings.BlackList_Combat.Add(new clsBlacklist(UnitToAttack));
                        }

                        // try to get away from the mob
                        clsCombat_Helper.PanicRun(false);

                        // exit
                        return true;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "IsBugged");
            }

            // shouldn't be here, but for safety...
            return false;
        }

        // Bugged
        #endregion

        #region Events

        /// <summary>
        /// Raised when a UI error is shown
        /// </summary>
        /// <param name="ErrorMessage">the ui error message</param>
        void clsWowEvents_UIErrorMsg(string ErrorMessage)
        {
            try
            {
                // skip if not facing wrong way
                if (!ErrorMessage.ToLower().Contains("facing"))
                    return;

                using (new clsFrameLock.LockBuffer())
                {
                    // skip if target is dead or invalid
                    if ((m_UnitToAttack == null) || (!m_UnitToAttack.IsValid) || (m_UnitToAttack.Dead))
                        return;
                }

                // face the target
                clsFace.FacePointExCombat(clsPath.GetUnitLocation(m_UnitToAttack));

                // start moving backwared
                clsPath.Move(MovementDirection.Backward);

                // back up from the target for 10 yards
                while (clsPath.DistanceToTarget(m_UnitToAttack) < 10)
                {
                    // do nothing
                    Thread.Sleep(50);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Combat);
            }            

            finally
            {
                // stop moving
                clsPath.StopMoving();
            }
        }

        /// <summary>
        /// Raised when the monster emotes something
        /// </summary>
        private void clsWowEvents_ChatMonsterEmote(string EmoteBody, string MonsterName)
        {
            // skip if not running away, or no spell defined
            if ((!EmoteBody.ToLower().Contains("attempts to run away")) || (string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_StopRunawayAttemptSpell)))
                return;

            try
            {
                // log it
                clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.MonsterAttemptingToRunAway, clsSettings.gclsLevelSettings.Combat_StopRunawayAttemptSpell);

                // set the casting flag
                Casting_RunAwaySpell = true;

                // cast the run away spell
                CastSpell(clsSettings.gclsLevelSettings.Combat_StopRunawayAttemptSpell);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.Combat, "ChatMonsterEmote");
            }   
         
            finally
            {
                // reset casting
                Casting_RunAwaySpell = false;
            }
        }

        // Events
        #endregion

        #region PostCombat

        /// <summary>
        /// Handle mage post combat functions
        /// </summary>
        private static void MagePostCombat()
        {
            bool NeedsWater = false, NeedsFood = false, NeedsManaStone = false;

            try
            {
                int level = clsCharacter.CurrentLevel;

                // check if we need water
                using (new clsFrameLock.LockBuffer())
                {
                    if ((level >= 4) &&
                        (clsSearch.Search_BagItem("Conjured Glacier Water").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Mountain Spring Water").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Crystal Water").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Sparkling Water").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Mineral Water").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Spring Water").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Purified Water").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Water").Count == 0))
                    {
                        clsSettings.Logging.AddToLog(Resources.PostCombat, Resources.MageNeedsWater);
                        NeedsWater = true;
                    }
                }

                // check if we need food
                using (new clsFrameLock.LockBuffer())
                {
                    if ((level >= 6) &&
                        (clsSearch.Search_BagItem("Conjured Croissant").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Cinnamon Roll").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Sweet Rolls").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Sourdough").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Pumpernickel").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Rye").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Bread").Count == 0) &&
                        (clsSearch.Search_BagItem("Conjured Muffins").Count == 0))
                    {
                        clsSettings.Logging.AddToLog(Resources.PostCombat, Resources.MageNeedsFood);
                        NeedsFood = true;
                    }
                }

                // check if we need mana stones
                using (new clsFrameLock.LockBuffer())
                {
                    if ((level >= 28) &&
                        (clsSearch.Search_BagItem("Mana Emerald").Count == 0) &&
                        (clsSearch.Search_BagItem("Mana Ruby").Count == 0) &&
                        (clsSearch.Search_BagItem("Mana Citrine").Count == 0) &&
                        (clsSearch.Search_BagItem("Mana Jade").Count == 0) &&
                        (clsSearch.Search_BagItem("Mana Agate").Count == 0))
                    {
                        clsSettings.Logging.AddToLog(Resources.PostCombat, Resources.MageNeedsManaStones);
                        NeedsManaStone = true;
                    }
                }

                // get water, food, and manastones
                if (NeedsWater)
                    CastSpell("Conjure Water");

                // wait a sec
                Thread.Sleep(100);

                if (NeedsFood)
                    CastSpell("Conjure Food");

                // wait a sec
                Thread.Sleep(100);

                if (NeedsManaStone)
                    CastSpell("Conjure Mana Stone");
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Mage - DoPostCombat");
            }
        }

        /// <summary>
        /// Hunter post combat stuff
        /// </summary>
        private static void HunterPostCombat()
        {
            WoWItem food = null;
            bool HasFood = false;

            try
            {
                // exit if we dont' have a pet
                if (!clsCharacter.HunterHasPet)
                {
                    clsSettings.Logging.AddToLog(Resources.HunterPostCombat, Resources.ExitingNoPet);
                    return;
                }

                // check if we need to revive our pet
                if (clsCharacter.IsPetDead)
                {
                    // log it
                    clsSettings.Logging.AddToLog(Resources.HunterPostCombat, Resources.PetDeadReviving);

                    CastSpell("Revive Pet");
                 
                    // wait a second
                    Thread.Sleep(1000);
                }

                // http://www.wowwiki.com/Feed_Pet
                // check if pet needs feeding
                if ((clsCharacter.PetHappiness <= clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Pet_FeedAtHappinessPercent) &&
                    (clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Pet_FoodList.Count > 0))
                {
                    // log it
                    clsSettings.Logging.AddToLog(Resources.HunterPostCombat, Resources.FindFoodForPet);

                    using (new clsFrameLock.LockBuffer())
                    {
                        // find food to feed pet with
                        foreach (string foodStr in clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Pet_FoodList)
                        {
                            // see if the item exists in our bag
                            List<WoWItem> foodList = clsSearch.Search_BagItem(foodStr);

                            // if we found something, exit
                            if ((foodList != null) && (foodList.Count > 0))
                            {
                                food = foodList[0];
                                clsSettings.Logging.AddToLogFormatted(Resources.HunterPostCombat, Resources.FoundFoodX, food.Name);
                                HasFood = true;
                                break;
                            }
                        }
                    }

                    // if we have food, feed the pet
                    if (HasFood)
                    {
                        // log it
                        clsSettings.Logging.AddToLog(Resources.HunterPostCombat, Resources.FeedingPet);

                        // cast feed spell
                        CastSpell_NoCheck("Feed Pet");

                        // wait a sec
                        Thread.Sleep(500);

                        // use the food
                        using (new clsFrameLock.LockBuffer())
                            food.Use();
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Hunter - Post Combat");
            }            
        }

        /// <summary>
        /// Warlock post combat
        /// </summary>
        private static void WarlockPostCombat()
        {
            try
            {
                // create a healthstone if we don't have any
                if (clsSearch.NumItemsInBag("Healthstone") == 0)
                    CastSpell("Create Healthstone");
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Warlock - Post Combat");
            }            
        }

        // PostCombat
        #endregion
    }
}
