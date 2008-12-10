using System;
using System.Threading;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;

namespace ISXBotHelper.Combat
{
    internal class clsCombat_Shaman
    {
        #region Variables

        private WoWUnit UnitToAttack = null;
        private bool CastingEarthShock = false;
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
                CastingEarthShock = false;

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

                // hook the monster cast spell event
                clsWowEvents.MonsterSpellCast += clsWowEvents_MonsterSpellCast;

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
                if (! clsCharacter.IsDead)
                    return combat.HandlePostCombat(UnitToAttack);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Combat - Shaman");
            }

            // shouldn't be here, but for safety...
            return clsGlobals.AttackOutcome.Success;
        }

        #region Functions

        private bool CastSpell(string SpellName)
        {
            // wait until earth shock is cast (or run away spell is cast)
            while ((CastingEarthShock) || (clsCombat.Casting_RunAwaySpell))
                Thread.Sleep(300);

            // cast the spell
            return clsCombat.CastSpell(SpellName);
        }

        /// <summary>
        /// fires when a monster is casting a spell
        /// </summary>
        void clsWowEvents_MonsterSpellCast()
        {
            try
            {
                // try to cast earth shock to disrupt the monster
                CastingEarthShock = true;
                clsCombat.CastSpell("Earth Shock");
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Monster Cast Spell");
            }

            finally
            {
                // reset so we're not stuck in infinite wait loop
                CastingEarthShock = false;
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

                // buff our weapons
                if (! string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_MainHandBuff))
                    clsCombat_Helper.ApplyWeaponBuff(clsCharacter.EquipedItem(WoWEquipSlot.MainHand), clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_MainHandBuff, true);
                if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_OffhandBuff))
                    clsCombat_Helper.ApplyWeaponBuff(clsCharacter.EquipedItem(WoWEquipSlot.OffHand), clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_OffhandBuff, false);

                // cast pre-combat buff list
                clsCombat.CastSpellList(clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Shaman - PrePull");
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

                        // wait half a sec
                        Thread.Sleep(250);
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Shaman - DoPull");
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

                // drop totems
                if (! string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_EarthTotem))
                    clsCombat_Helper.DropTotem(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_EarthTotem);
                if (! string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_FireTotem))
                    clsCombat_Helper.DropTotem(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_FireTotem);
                if (! string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_WaterTotem))
                    clsCombat_Helper.DropTotem(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_WaterTotem);
                if (! string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_AirTotem))
                    clsCombat_Helper.DropTotem(clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_AirTotem);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Shaman - DoPostPull");
            }            
        }

        /// <summary>
        /// Performs one round of combat
        /// </summary>
        private void DoCombat()
        {
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

                // make sure we are close
                combat.MoveAndFace(UnitToAttack);

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // let our toon whack on the mob a bit
                using (new clsFrameLock.LockBuffer())
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.Combat, Resources.WhackingOnX,
                        UnitToAttack.Name, clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
                }

                Thread.Sleep(clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Shaman - DoCombat");
            }            
        }

        // Combat Round Functions
        #endregion
    }
}
