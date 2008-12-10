using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;

namespace ISXBotHelper.Combat
{
    public class clsCombat_Hunter
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
                if (! clsCharacter.IsDead)
                    return combat.HandlePostCombat(UnitToAttack);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Combat - Hunter");
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

        /// <summary>
        /// Returns true if our pet is dead, or the mob is too close
        /// </summary>
        private bool PetDeadOrTooClose
        {
            get
            {
                using (new clsFrameLock.LockBuffer())
                    return (! clsCharacter.HunterHasPet) || (clsCharacter.IsPetDead) || clsPath.DistanceToTarget(UnitToAttack) < 10;
            }
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
                // call pet
                clsCharacter.CallPet();

                // exit if we are already in combat
                if (clsCombat.IsInCombat())
                    return;

                // cast pre-combat buff list
                clsCombat.CastSpellList(clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Hunter - PrePull");
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

                // send in the pet
                // the AttackingUnit will already be set to our current target
                // if the pet is not already in combat, so compare guid's
                if (clsCombat.IsPetTargettingTarget(UnitToAttack))
                    clsSettings.ExecuteWoWAPI("PetAttack()");
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Hunter - DoPull");
            }            
        }

        /// <summary>
        /// Performs one round of combat
        /// </summary>
        private void DoCombat()
        {
            List<string> SpellList;

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // if we have a mob that is attacking us, and is closer, 
                    // then target that one instead. Don't do this if our pet is dead,
                    // or we are already targetting something close
                    if ((!PetDeadOrTooClose) && (clsCombat.NumUnitsAttackingMe() > 1))
                    {
                        // send in the pet
                        // the AttackingUnit will already be set to our current target
                        // if the pet is not already in combat, so compare guid's
                        if (clsCombat.IsPetTargettingTarget(UnitToAttack))
                            clsSettings.ExecuteWoWAPI("PetAttack()");

                        // get UnitToAttack's distance
                        double UADist = clsPath.DistanceToTarget(UnitToAttack);

                        // loop through the list of mobs to see if one is closer
                        List<WoWUnit> unitList = clsCombat.UnitsAttackingMe();
                        foreach (WoWUnit aUnit in unitList)
                        {
                            // skip if invalid
                            if ((aUnit == null) || (!aUnit.IsValid))
                                continue;

                            // if closer, target this one
                            if (clsPath.DistanceToTarget(aUnit) < UADist)
                            {
                                UnitToAttack = aUnit;
                                UnitToAttack.Target();
                            }   
                        }
                    }
                }

                // toggle attack
                using (new clsFrameLock.LockBuffer())
                {
                    if ((!clsSettings.isxwow.Me.Attacking) && (!clsSettings.isxwow.Me.AttackingUnit.Dead))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.TogglingAttack, clsSettings.isxwow.Me.IsInCombat.ToString(), clsSettings.isxwow.Me.Attacking.ToString());
                        clsSettings.isxwow.Me.ToggleAttack();
                    }
                }

                // if our pet is dead, move closer if too far away
                if (PetDeadOrTooClose)
                    combat.MoveAndFace(UnitToAttack);

                // cast Healing Over Time if we need it
                combat.NeedsHOT();

                // get dots to cast
                if (PetDeadOrTooClose)
                    SpellList = clsSettings.gclsLevelSettings.Combat_DOT_List;
                else
                    SpellList = clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Ranged_DOT;

                // cast dots
                foreach (string dot in SpellList)
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
                if (PetDeadOrTooClose)
                    SpellList = clsSettings.gclsLevelSettings.Combat_SpamSpells_List;
                else
                    SpellList = clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Ranged_SpamSpells;

                // cast close range spells
                foreach (string spell in SpellList)
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

                // make sure we are close
                if (PetDeadOrTooClose)
                    combat.MoveAndFace(UnitToAttack);

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // let our toon whack (shoot) on the mob a bit
                using (new clsFrameLock.LockBuffer())
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.WhackingOnX,
                        UnitToAttack.Name, clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
                }

                Thread.Sleep(clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Hunter - DoCombat");
            }            
        }

        // Combat Round Functions
        #endregion
    }
}
