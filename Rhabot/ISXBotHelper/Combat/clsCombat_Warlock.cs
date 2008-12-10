using System;
using System.Threading;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;
using LavishVMAPI;
using System.Linq;

namespace ISXBotHelper.Combat
{
    internal class clsCombat_Warlock
    {
        #region Variables

        private WoWUnit UnitToAttack = null;
        private readonly clsCombat combat = new clsCombat();

        // Variables
        #endregion

        /// <summary>
        /// Begins combat on the selected unit
        /// </summary>
        public clsGlobals.AttackOutcome BeginCombat(WoWUnit AttackUnit)
        {
            clsCombat.ECurrentStep currentStep = clsCombat.ECurrentStep.PrePull;

            try
            {
                #region Setup and Validation

                // set our module unit to attack
                using (new clsFrameLock.LockBuffer())
                {
                    UnitToAttack = AttackUnit;

                    // exit if no unit or character
                    if ((!clsCharacter.CharacterIsValid) || (UnitToAttack == null) || (!UnitToAttack.IsValid) || (UnitToAttack.Dead))
                        return clsGlobals.AttackOutcome.Success;

                    // exit if dead
                    if (clsCharacter.IsDead)
                        return clsGlobals.AttackOutcome.Dead;
                }

                // Setup and Validation
                #endregion

                // loop through until one of us dies or the script is stopped
                while (true)
                {
                    #region Test for Stop

                    // check for stop
                    if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsGlobals.AttackOutcome.Stopped;

                    // check if dead
                    if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        break;

                    // Test for Stop
                    #endregion

                    #region Check Health and Need Panic

                    // check if we need to drink a mana potion
                    combat.CheckNeedMana();

                    // check if we need health, drinking potion or casting healing spell
                    combat.CheckNeedHeal();

                    // check if we need to panic (too low health, or too many mobs)
                    if (combat.NeedsPanic())
                        return clsGlobals.AttackOutcome.Panic;

                    //Check Health and Need Panic
                    #endregion

                    #region Target and Face

                    // target the unit
                    using (new clsFrameLock.LockBuffer())
                        UnitToAttack.Target();

                    // face the unit
                    clsFace.FacePointExCombat(clsPath.GetUnitLocation(UnitToAttack));

                    // Target and Face
                    #endregion

                    #region Combat

                    // handle combat based on what step we are on
                    switch (currentStep)
                    {
                        case clsCombat.ECurrentStep.PrePull:
                            // do pre pull stuff
                            DoPrePull();
                            currentStep = clsCombat.ECurrentStep.Pull;
                            continue;

                        case clsCombat.ECurrentStep.Pull:
                            // do pull actions
                            DoPull();
                            currentStep = clsCombat.ECurrentStep.InCombat;
                            continue;

                        case clsCombat.ECurrentStep.InCombat:
                            // combat actions
                            DoCombat();

                            // check if bugged
                            if (combat.IsBugged(UnitToAttack))
                            {
                                // if we are dead, return that instead
                                clsGlobals.DeadUnitEnum due = combat.CheckDead(UnitToAttack);
                                if (due == clsGlobals.DeadUnitEnum.Character)
                                    return clsGlobals.AttackOutcome.Dead;
                                else
                                    return clsGlobals.AttackOutcome.Bugged;
                            }

                            // restart loop
                            continue;
                    }

                    // Combat
                    #endregion
                }

                // handle post combat stuff
                if (!clsCharacter.IsDead)
                    return combat.HandlePostCombat(UnitToAttack);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Combat - Warlock");
            }

            // shouldn't be here, but for safety...
            return clsGlobals.AttackOutcome.Success;
        }

        #region Functions

        private bool CastSpell(string SpellName)
        {
            // wait until earth shock is cast (or run away spell is cast)
            while (clsCombat.Casting_RunAwaySpell)
                Thread.Sleep(300);

            // cast the spell
            return clsCombat.CastSpell(SpellName);
        }

        // Functions
        #endregion

        #region Combat Round Funtions

        /// <summary>
        /// Performs pre-pull actions, such as weapon buffing and moving closer
        /// </summary>
        private void DoPrePull()
        {
            try
            {
                // exit if we are already in combat
                if (clsCombat.IsInCombat())
                    return;

                // cast pre-combat buff list
                clsCombat.CastSpellList(clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Warlock - PrePull");
            }
        }

        /// <summary>
        /// Performs the pull if we can
        /// </summary>
        private void DoPull()
        {
            try
            {
                // get pet if he is not already here
                if (! clsCharacter.HunterHasPet)
                {
                    // summon the pet
                    CastSpell(string.Format("Summon {0}", clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_Pet.ToString().Trim()));

                    // wait one frame
                    Frame.Wait(false);
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

                // move close if too far away
                if (clsPath.DistanceToTarget(UnitToAttack) > clsSettings.gclsLevelSettings.TargetRange)
                    clsPath.MoveToTarget(UnitToAttack, clsSettings.gclsLevelSettings.TargetRange - 2);

                // handle pull
                if ((clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_PullWithPet) && (!clsCombat.IsPetInCombat()))
                {
                    // pulling with pet
                    clsSettings.Logging.AddToLog(Resources.WarlockUsingPetToPull);
                    clsSettings.ExecuteWoWAPI("PetAttack()");

                    // if we have a wait time, then wait
                    if (clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_PullWaitTime > 0)
                        Thread.Sleep(clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_PullWaitTime);
                }

                else
                {
                    // pulling with spell
                    if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_PullSpell))
                    {
                        // cast the spell
                        clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.CastingPullSpellX, clsSettings.gclsLevelSettings.Combat_PullSpell);
                        CastSpell(clsSettings.gclsLevelSettings.Combat_PullSpell);
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Warlock - DoPull");
            }
        }

        /// <summary>
        /// Performs one round of combat
        /// </summary>
        private void DoCombat()
        {
            string DrainSpell = "Drain Soul";

            try
            {
                // move closer if too far away
                if (clsPath.DistanceToTarget(UnitToAttack) > clsSettings.gclsLevelSettings.TargetRange)
                    clsPath.MoveToTarget(UnitToAttack, clsSettings.gclsLevelSettings.TargetRange - 2);

                // toggle attack
                using (new clsFrameLock.LockBuffer())
                {
                    if ((!clsSettings.isxwow.Me.Attacking) && (!clsSettings.isxwow.Me.AttackingUnit.Dead))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.TogglingAttack, clsSettings.isxwow.Me.IsInCombat.ToString(), clsSettings.isxwow.Me.Attacking.ToString());
                        clsSettings.isxwow.Me.ToggleAttack();
                    }
                }

                // cast Healing Over Time if we need it
                combat.NeedsHOT();

                // cast dots
                foreach (string dot in clsSettings.gclsLevelSettings.Combat_DOT_List)
                {
                    // check for death
                    if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        return;

                    // cast the dot if it doesn't exists on mob
                    if (!clsCharacter.BuffExists(UnitToAttack, dot))
                        CastSpell(dot);
                }

                // heal if we need it
                combat.CheckNeedHeal();

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // cast the spell list
                foreach (string spell in clsSettings.gclsLevelSettings.Combat_SpamSpells_List)
                {
                    // check for death
                    if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        return;

                    // cast the spell
                    CastSpell(spell);

                    // see if we need mana
                    combat.CheckNeedMana();
                }

                // heal if we need it
                combat.CheckNeedHeal();

                // move closer if too far away
                if (clsPath.DistanceToTarget(UnitToAttack) > clsSettings.gclsLevelSettings.TargetRange)
                    clsPath.MoveToTarget(UnitToAttack, clsSettings.gclsLevelSettings.TargetRange - 2);

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // check if needs drain
                bool NeedsDrain;
                using (new clsFrameLock.LockBuffer())
                {
                    NeedsDrain = UnitToAttack.HealthPercent <= clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_DrainSoulOnHealthPercent;

                    // needs drain
                    if (NeedsDrain)
                    {
                        int ShardCount = clsSearch.NumItemsInBag("Soul Shard");
                        double healthP = clsCharacter.HealthPercent;

                        // check drain life
                        if ((clsCharacter.CurrentLevel >= 14) &&
                            (ShardCount >= clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_SoulShardCount))// &&
                            //(healthP <= clsSettings.gclsLevelSettings.Combat_DowntimePercent))
                                DrainSpell = "Drain Life";

                        // check drain mana
                        if ((clsCharacter.CurrentLevel >= 24) &&
                            (ShardCount >= clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_SoulShardCount) &&
                            (healthP > clsSettings.gclsLevelSettings.Combat_DowntimePercent) &&
                            (clsCharacter.ManaPercent <= clsSettings.gclsLevelSettings.Combat_DowntimePercent))
                        {
                            // check for Dark Pact talent
                            if (Talents.clsTalents.MyTalents.Any<Talents.clsTalentInfo>(talent => talent.name.ToLower() == "dark pact"))
                                DrainSpell = "Dark Pact";
                            else
                                DrainSpell = "Drain Mana";
                        }
                    }
                }

                // cast the drain spell if we need to drain
                if (NeedsDrain)
                    CastSpell(DrainSpell);

                // let our toon whack on the mob a bit
                using (new clsFrameLock.LockBuffer())
                {
                    // start shooting with the wand
                    clsCombat.CastSpell_NoWait("Shoot");

                    clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.WhackingOnX,
                        UnitToAttack.Name, clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
                }

                Thread.Sleep(clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Warlock - DoCombat");
            }
        }

        // Combat Round Functions
        #endregion
    }
}
