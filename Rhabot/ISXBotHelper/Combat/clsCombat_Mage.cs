using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;

namespace ISXBotHelper.Combat
{
    internal class clsCombat_Mage
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
                            currentStep = clsCombat.ECurrentStep.PostPull;
                            continue;

                        case clsCombat.ECurrentStep.PostPull:
                            DoPostPull();
                            currentStep = clsCombat.ECurrentStep.InCombat;
                            continue;

                        case clsCombat.ECurrentStep.InCombat:
                            // combat actions
                            DoCombat();

                            // get any death
                            clsGlobals.DeadUnitEnum due = combat.CheckDead(UnitToAttack);

                            // check if bugged
                            if (combat.IsBugged(UnitToAttack))
                            {
                                // if we are dead, return that instead
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
                if (! clsCharacter.IsDead)
                    return combat.HandlePostCombat(UnitToAttack);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Combat - Mage");
            }

            // shouldn't be here, but for safety...
            return clsGlobals.AttackOutcome.Success;
        }

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
                clsError.ShowError(excep, "Mage - PrePull");
            }            
        }

        /// <summary>
        /// Performs the pull if we can
        /// </summary>
        private void DoPull()
        {
            int pDistance = 3;

            try
            {
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

                // pull if far enough away
                if (clsPath.DistanceToTarget(UnitToAttack) > 9)
                {
                    // get the distance we should be from unit
                    if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_PullSpell))
                        pDistance = clsSettings.gclsLevelSettings.TargetRange;

                    // move close if too far away
                    clsPath.MoveToTarget(UnitToAttack, pDistance);

                    // if we have a pull spell, use it
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
                clsError.ShowError(excep, "Mage - DoPull");
            }            
        }

        /// <summary>
        /// Do post pull actions
        /// </summary>
        private void DoPostPull()
        {
            try
            {
                // cast protection spell
                if (! string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ProtectionSpell))
                    CastSpell(clsSettings.gclsLevelSettings.Combat_ProtectionSpell);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Mage - DoPostPull");
            }            
        }

        /// <summary>
        /// Performs one round of combat
        /// </summary>
        private void DoCombat()
        {
            bool PolymorphCast = false;

            try
            {
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

                // if we have more than one mob on us, sheep the other mob
                if (clsCombat.NumUnitsAttackingMe() > 1)
                {
                    // loop through mobs
                    using (new clsFrameLock.LockBuffer())
                    {
                        List<WoWUnit> mobList = clsCombat.UnitsAttackingMe();
                        foreach (WoWUnit mob in mobList)
                        {
                            // skip if our targetted mob
                            if (mob.GUID == UnitToAttack.GUID)
                                continue;

                            // if this mob is already sheeped, then exit this block
                            if (clsCharacter.BuffExists(mob, "Polymorph"))
                                break;

                            // if this mob is beast or human, cast the spell
                            if ((mob.CreatureType == WoWCreatureType.Beast) || (mob.CreatureType == WoWCreatureType.Humanoid))
                            {
                                // cast the spell
                                PolymorphCast = true;
                                if (! clsCombat.Casting_RunAwaySpell)
                                    clsCombat.CastSpell("Polymorph", mob);
                                break;
                            }
                        }
                    }

                    // retarget our mob
                    if (PolymorphCast)
                        using (new clsFrameLock.LockBuffer())
                            UnitToAttack.Target();
                }

                // cast dots
                foreach (string dot in clsSettings.gclsLevelSettings.Combat_DOT_List)
                {
                    // check for death
                    if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        return;

                    // cast the dot if it doesn't exists on mob
                    if (! clsCharacter.BuffExists(UnitToAttack, dot))
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

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // blink away if the mob is too close
                Blink();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Mage - DoCombat");
            }            
        }

        /// <summary>
        /// Blinks away if we are too close to the mob
        /// </summary>
        private void Blink()
        {
            try
            {
                PathListInfo.PathPoint testPoint;
                using (new clsFrameLock.LockBuffer())
                {
                    // exit if the mob is far enough away or we are not high enough level
                    if ((clsCharacter.CurrentLevel < 20) || (clsPath.DistanceToTarget(UnitToAttack) > 10))
                        return;

                    // get heading and location
                    double heading = clsSettings.isxwow.Me.Heading;
                    PathListInfo.PathPoint myLoc = clsCharacter.MyLocation;

                    // check if we can blink straight ahead
                    testPoint = TestBlinkLocation(myLoc, heading);
                    
                    // if no testpoint, check behind us
                    if (testPoint == null)
                        testPoint = TestBlinkLocation(myLoc, heading - 180);

                    // if no testpoint, check to our sides
                    if (testPoint == null)
                        testPoint = TestBlinkLocation(myLoc, heading - 90);
                    if (testPoint == null)
                        testPoint = TestBlinkLocation(myLoc, heading + 90);
                }

                // if we don't have a point, then we can't blink, so just exit here
                if (testPoint == null)
                    return;

                // log it
                clsSettings.Logging.AddToLogFormatted("Blink", Resources.BlinkingToX, testPoint);

                // face the new point
                clsFace.FacePointExCombat(testPoint);

                // blink
                CastSpell("Blink");

                // face our mob
                clsFace.FacePointExCombat(clsPath.GetUnitLocation(UnitToAttack));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Mage - Blink");
            }            
        }

        private PathListInfo.PathPoint TestBlinkLocation(PathListInfo.PathPoint MyLocation, double HeadingToSearch)
        {
            // fix heading if necessary
            if (HeadingToSearch < 0)
                HeadingToSearch += 360;
            if (HeadingToSearch > 360)
                HeadingToSearch -= 360;

            // get the point in front of us
            PathListInfo.PathPoint testPoint = new PathListInfo.PathPoint(
                (MyLocation.X + (20 * Math.Cos(-1 * HeadingToSearch))),
                (MyLocation.Y + (20 * Math.Sin(-1 * HeadingToSearch))),
                MyLocation.Z);

            // test for nearby mobs, if nothing found, do the blink
            clsSettings.Logging.AddToLogFormatted(Resources.BlinkTestingPointX, testPoint.ToString());
            if (clsSearch.FindSurroundingTarget(testPoint, clsCharacter.MobLowLevel, clsCharacter.MobHighLevel, 15, UnitToAttack) != null)
                return testPoint;
            else
                return null;
        }

        // Combat Round Functions
        #endregion

        #region Functions

        /// <summary>
        /// Casts a spell
        /// </summary>
        /// <param name="spellName">the spell to cast</param>
        private bool CastSpell(string spellName)
        {
            // don't do anything while casting run away spell
            while (clsCombat.Casting_RunAwaySpell)
                Thread.Sleep(300);

            // cast the spell
            return clsCombat.CastSpell(spellName);
        }

        // Functions
        #endregion
    }
}
