using System;
using System.Threading;
using ISXBotHelper.Properties;
using ISXRhabotGlobal;
using ISXWoW;

namespace ISXBotHelper.Combat
{
    internal class clsCombat_Paladin
    {
        #region Variables

        private WoWUnit UnitToAttack = null;
        private bool CastingHammerJustice = false;
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

                // reset hammer justice value
                CastingHammerJustice = false;

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
                    {
                        clsSettings.Logging.AddToLog(Resources.PaladinCombat, Resources.PanicCastingDivineShield);
                        clsCombat.CastSpell("Divine Shield");
                        return clsGlobals.AttackOutcome.Panic;
                    }

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
                            currentStep = clsCombat.ECurrentStep.InCombat;
                            continue;

                        case clsCombat.ECurrentStep.InCombat:
                        default:
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
                clsError.ShowError(excep, "Combat - Paladin");
            }

            // shouldn't be here, but for safety...
            return clsGlobals.AttackOutcome.Success;
        }

        #region Functions

        private bool CastSpell(string SpellName)
        {
            // wait until earth shock is cast (or run away spell is cast)
            while ((CastingHammerJustice) || (clsCombat.Casting_RunAwaySpell))
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
                CastingHammerJustice = true;
                clsCombat.CastSpell("Hammer of Justice");
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Monster Cast Spell");
            }

            finally
            {
                // reset so we're not stuck in infinite wait loop
                CastingHammerJustice = false;
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
                clsError.ShowError(excep, "Paladin - PrePull");
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

                // cast judgement/seals
                if (!string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_SealOne))
                {
                    if (!clsCharacter.BuffExists(UnitToAttack, clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_SealOne))
                    {
                        // pop the first seal
                        CastSpell(clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_SealOne);

                        // judgment to transfer to mob
                        Thread.Sleep(100);
                        CastSpell("Judgement");
                    }
                }

                // cast second seal buff on us
                if ((! string.IsNullOrEmpty(clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_SealTwo)) &&
                    (! clsCharacter.BuffExists(clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_SealTwo)))
                        CastSpell(clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_SealTwo);

                // cast dots
                foreach (string dot in clsSettings.gclsLevelSettings.Combat_DOT_List)
                {
                    // cast the dot if it doesn't exists on mob
                    if (! clsCharacter.BuffExists(UnitToAttack, dot))
                        CastSpell(dot);
                }

                // heal if we need it
                combat.CheckNeedHeal();

                // check for death
                if (combat.CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // aggro spells
                if (clsCombat.NumUnitsAttackingMe() > 1)
                {
                    // cast avenger's shield
                    if (clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_UseAvengersShield)
                        CastSpell("Avenger's Shield");

                    // consecrate
                    if (clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_UseConsecration)
                        CastSpell("Consecration");
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
                    clsSettings.Logging.AddToLogFormatted(Resources.PaladinCombat, Resources.WhackingOnX,
                        UnitToAttack.Name, clsSettings.gclsLevelSettings.Combat_WaitTime_ms);

                Thread.Sleep(clsSettings.gclsLevelSettings.Combat_WaitTime_ms);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Paladin - DoCombat");
            }            
        }

        // Combat Round Functions
        #endregion
    }
}
