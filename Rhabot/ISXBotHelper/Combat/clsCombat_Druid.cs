using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;

namespace ISXBotHelper.Combat
{
    // http://www.wowwiki.com/Druid

    public class clsCombat_Druid
    {
        #region Variables

        private WoWUnit UnitToAttack = null;
        private readonly clsCombat combat = new clsCombat();
        private List<string> buffList = new List<string>();

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

                // build the list of buffs to remove
                buffList.Add("Aquatic Form");
                buffList.Add("Bear Form");
                buffList.Add("Cat Form");
                buffList.Add("Dire Bear Form");
                buffList.Add("Flight Form");
                buffList.Add("Moonkin Form");
                buffList.Add("Swift Flight Form");
                buffList.Add("Travel Form");
                buffList.Add("Tree of Life Form");

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
                    CheckNeedHeal();

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
                clsError.ShowError(excep, "Combat - Druid");
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
                clsSettings.Logging.AddToLog(Resources.DruidCombat, Resources.ChangingForm);

                // cast pre-combat buff list
                if (!clsCombat.IsInCombat())
                    clsCombat.CastSpellList(clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List);

                // Change form as required by settings
                ChangeForm();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Druid - PrePull");
            }
        }

        /// <summary>
        /// Change form, if needed, based on user settings
        /// </summary>
        private void ChangeForm()
        {
            // change forms if needed
            switch (clsSettings.gclsLevelSettings.Combat_ClassSettings.Druid_CombatForm)
            {
                case clsGlobals.ECombatClass_Druid_Form.Humanoid:
                    // remove the buff's if they exist
                    clsCharacter.RemoveListOfBuffs(buffList);
                    break;

                case clsGlobals.ECombatClass_Druid_Form.Bear:
                    if (!clsCharacter.BuffExists("Bear Form"))
                        clsCombat.CastSpell_NoCheck("Bear Form");
                    break;

                case clsGlobals.ECombatClass_Druid_Form.Cat:
                    if (!clsCharacter.BuffExists("Cat Form"))
                        clsCombat.CastSpell_NoCheck("Cat Form");
                    break;

                case clsGlobals.ECombatClass_Druid_Form.DireBear:
                    if (!clsCharacter.BuffExists("Dire Bear Form"))
                        clsCombat.CastSpell_NoCheck("Dire Bear Form");
                    break;

                case clsGlobals.ECombatClass_Druid_Form.MoonKin:
                    if (!clsCharacter.BuffExists("Moonkin Form"))
                        clsCombat.CastSpell_NoCheck("Moonkin Form");
                    break;
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
                clsError.ShowError(excep, "Druid - DoPull");
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
                clsError.ShowError(excep, "Druid - DoPostPull");
            }
        }

        /// <summary>
        /// Performs one round of combat
        /// </summary>
        private void DoCombat()
        {
            bool NeedsBearForm = false;

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

                // check if we need to mor[
                if (clsSettings.gclsLevelSettings.Combat_ClassSettings.Druid_ChangeToBearOnAggro)
                {
                    using (new clsFrameLock.LockBuffer())
                    {
                        // if we have more than one unit attacking,
                        // and we are level 10,
                        // and we are not already morphed into bear form
                        NeedsBearForm = ((clsCombat.NumUnitsAttackingMe() > 1) &&
                                        (clsCharacter.CurrentLevel > 9) &&
                                        (!clsCharacter.BuffExists("Bear Form")) &&
                                        (!clsCharacter.BuffExists("Dire Bear Form")));
                    }
                }

                // if we can morph on aggro, check for aggro and morph accordingly
                if (NeedsBearForm)
                {
                    clsSettings.Logging.AddToLog(Resources.DruidCombat, Resources.TransformingToBear);

                    // direbear form
                    if (clsCharacter.CurrentLevel > 39)
                    {
                        // cast the direbear spell. if it fails, cast bear
                        if (! clsCombat.CastSpell_NoCheck("Dire Bear Form"))
                            clsCombat.CastSpell_NoCheck("Bear Form");
                    }

                    else // bear form
                    {
                        clsCombat.CastSpell_NoCheck("Bear Form");
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
                CheckNeedHeal();

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
                CheckNeedHeal();

                // make sure we are close
                combat.MoveAndFace(UnitToAttack);

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // let our toon whack on the mob a bit
                using (new clsFrameLock.LockBuffer())
                    clsSettings.Logging.AddToLogFormatted(Resources.DruidCombat, Resources.WhackingOnX,
                        UnitToAttack.Name, clsSettings.gclsLevelSettings.Combat_WaitTime_ms);

                Thread.Sleep(clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Druid - DoCombat");
            }
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

        /// <summary>
        /// Checks if we need to heal. if so, removes the shapeshift, then reapplies
        /// after healing
        /// </summary>
        private void CheckNeedHeal()
        {
            // if we are above the setting, then exit
            if (clsCharacter.HealthPercent > clsSettings.gclsLevelSettings.Combat_HealthPercent)
                return;

            // we need to heal
            // remove the buff's if they exist
            clsCharacter.RemoveListOfBuffs(buffList);

            // heal
            combat.CheckNeedHeal();

            // reapply shapeshift
            ChangeForm();
        }

        // Functions
        #endregion
    }
}