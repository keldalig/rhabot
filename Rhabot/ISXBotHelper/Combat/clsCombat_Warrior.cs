using System;
using System.Threading;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;

namespace ISXBotHelper.Combat
{
    // http://www.worldofwarcraft.com/info/classes/warrior/spells.html
    // http://www.worldofwarcraft.com/info/classes/warrior/talents.html

    public class clsCombat_Warrior
    {
        #region Variables

        private WoWUnit UnitToAttack = null;
        private bool CastingOverpower = false;
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

                // reset earthshock value
                CastingOverpower = false;

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

                // hook the monster dodge event
                if (clsSettings.gclsLevelSettings.Combat_ClassSettings.Warrior_UseOverPower)
                    clsWowEvents.UnitCombat_Update += clsWowEvents_UnitCombat_Update;

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
                            // post pull actions
                            DoPostPull();
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
                return combat.HandlePostCombat(UnitToAttack);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Combat - Warrior");
            }

            // shouldn't be here, but for safety...
            return clsGlobals.AttackOutcome.Success;
        }

        #region Functions

        private bool CastSpell(string SpellName)
        {
            // wait until earth shock is cast (or run away spell is cast)
            while ((CastingOverpower) || (clsCombat.Casting_RunAwaySpell))
                Thread.Sleep(300);

            // cast the spell
            return clsCombat.CastSpell(SpellName);
        }

        /// <summary>
        /// Fires when the monster 
        /// </summary>
        void clsWowEvents_UnitCombat_Update(string unitid, string arg2, string arg3, int dmg, int dmgType)
        {
            // exit if not our target and not dodging
            if ((string.Compare(unitid, "target", true) != 0) || 
                ((string.Compare(arg2, "dodge", true) != 0) && (string.Compare(arg2, "critical", true) != 0)))
                    return;

            try
            {
                string spellname = "Overpower";

                // if critical, see if we can cast rampage
                if (arg2.ToLower() == "critical")
                {
                    // exit, no rampage
                    if (!clsSettings.gclsLevelSettings.Combat_ClassSettings.Warrior_UseRampage)
                        return;

                    // use rampage
                    spellname = "Rampage";
                }

                // try to cast overpower/rampage
                CastingOverpower = true;
                clsCombat.CastSpell(spellname);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unit Combat Update");
            }

            finally
            {
                // reset so we're not stuck in infinite wait loop
                CastingOverpower = false;
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
                // exit if we are already in combat
                if (clsCombat.IsInCombat())
                    return;

                // cast pre-combat buff list
                clsCombat.CastSpellList(clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Warrior - PrePull");
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
                    //if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_PullSpell))
                    //    pDistance = clsSettings.gclsLevelSettings.TargetRange;

                    // decide how to pull
                    switch (clsSettings.gclsLevelSettings.Combat_ClassSettings.Warrior_PullType)
                    {
                        case clsCombatSettings.EWarriorPullType.Charge:
                            clsSettings.Logging.AddToLogFormatted(Resources.WarriorCombat, Resources.Charging);
                            CastSpell("Charge");
                            break;

                        case clsCombatSettings.EWarriorPullType.Shoot:
                            clsSettings.Logging.AddToLogFormatted(Resources.WarriorCombat, Resources.Shooting);
                            CastSpell("Shoot");
                            break;

                        case clsCombatSettings.EWarriorPullType.Throw:
                            clsSettings.Logging.AddToLogFormatted(Resources.WarriorCombat, Resources.Throwing);
                            CastSpell("Throw");
                            break;
                    }
                }

                else // move close if too far away                    
                    clsPath.MoveToTarget(UnitToAttack, pDistance);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Warrior - DoPull");
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
                if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ProtectionSpell))
                    CastSpell(clsSettings.gclsLevelSettings.Combat_ProtectionSpell);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Warrior - DoPostPull");
            }
        }

        /// <summary>
        /// Performs one round of combat
        /// </summary>
        private void DoCombat()
        {
            // use execute ? (unit must be 20% or less health, and we need to have 20 rage)
            // use Rampage ? (fury spec, bottom) (must check at start of each loop if usable)

            try
            {
                // move closer if too far away
                combat.MoveAndFace(UnitToAttack);

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

                // check if we need to execute
                if (clsSettings.gclsLevelSettings.Combat_ClassSettings.Warrior_UseExecute)
                    NeedsExecute();

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

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

                // check if we need to execute
                if (clsSettings.gclsLevelSettings.Combat_ClassSettings.Warrior_UseExecute)
                    NeedsExecute();

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // heal if we need it
                combat.CheckNeedHeal();

                // cast the spell list
                foreach (string spell in clsSettings.gclsLevelSettings.Combat_SpamSpells_List)
                {
                    // check for death
                    if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        return;

                    CastSpell(spell);
                }

                // heal if we need it
                combat.CheckNeedHeal();

                // make sure we are close
                combat.MoveAndFace(UnitToAttack);

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // Cleave if aggro'd
                if (clsCombat.NumUnitsAttackingMe() > 1)
                    CastSpell("Cleave");

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // let our toon whack on the mob a bit
                using (new clsFrameLock.LockBuffer())
                    clsSettings.Logging.AddToLogFormatted(Resources.WarriorCombat, Resources.WhackingOnX,
                        UnitToAttack.Name, clsSettings.gclsLevelSettings.Combat_WaitTime_ms);

                Thread.Sleep(clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Warrior - DoCombat");
            }
        }

        // Combat Round Functions
        #endregion

        #region Helpers

        /// <summary>
        /// Checks for and performs Execute
        /// </summary>
        private void NeedsExecute()
        {
            using (new clsFrameLock.LockBuffer())
            {
                // exit if conditions are not right
                if ((clsSettings.isxwow.Me.Rage < 20) || (UnitToAttack.HealthPercent > 20))
                    return;
            }

            // kill him
            CastSpell("Execute");
        }

        // Helpers
        #endregion
    }
}