/*
 * NOTE: you should check this code for TODO: comments. There are LOT
 * 
 * You must keep the following:
 *      UseExternalCombat   (remember to set it to True)
 *      BeginCombat   (must return ISXRhabotGlobal.clsGlobals.AttackOutcome)
 * 
 * NOTE: You can NOT reference Rhabot, as this will create a 
 *  circular reference and Rhabot will not load
 * 
 * This code is not complete. It code needs your personalization.
 * 
 * Rhabot
 *  
*/

using System;
using System.Collections.Generic;
using ISXWoW;
using System.Threading;

using ISXRhabotGlobal;

namespace ISXCombat
{
    /// <summary>
    /// Handles external combat
    /// </summary>
    public partial class clsCombat
    {
        #region Properties

        // TODO: set this to true to so Rhabot will use your routine and
        //  not the internal Rhabot combat routing
        public static bool UseExternalCombat = false;

        // Properties
        #endregion

        #region Enums

        public enum ECombatStep
        {
            PrePull = 0,
            Pull,
            PostPull,
            InCombat
        }

        // Enums
        #endregion

        #region Variables

        /// <summary>
        /// Global instance of the unit to attack
        /// </summary>
        private WoWUnit UnitToAttack = null;

        // Variables
        #endregion

        /// <summary>
        /// Begin combat with the selected unit. Return the combat results
        /// We will already be dismounted and stopped moving
        /// </summary>
        /// <param name="UnitToAttack">the unit to attack. he might already be attacking us</param>
        public ISXRhabotGlobal.clsGlobals.AttackOutcome BeginCombat(WoWUnit unitToAttack)
        {
            // flag to set combat state
            ECombatStep currentStep = ECombatStep.PrePull;

            // TODO: If you can heal yourself, enter the healing spell you use
            // This is used when moving to a target that is too far away. Will
            // heal you if health percent is too low
            string HealSpell = "";

            // used for monitoring stopped script
            clsGlobals RhabotGlobals = new clsGlobals();

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // set global unit to attack
                    unitToAttack = unitToAttack;

                    // exit if dead
                    if (isxwow.Me.Dead)
                        return clsGlobals.AttackOutcome.Dead;
                }

                // create new instance of movement
                using (new clsFrameLock.LockBuffer())
                    movement = WoWMovement.Get();

                // loop through until one of us dies or the script is stopped
                while (true)
                {
                    #region Test for Stop

                    // check for stop
                    if (clsGlobals.ScriptStopped)
                        return clsGlobals.AttackOutcome.Stopped;

                    // check if dead
                    if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        break;

                    // Test for Stop
                    #endregion

                    #region Check Health and Need Panic

                    // TODO: not all classes need to check their mana. comment if you
                    // don't want to check your mana

                    // check if we need to drink a mana potion
                    CheckNeedMana();

                    // check if we need health, drinking potion or casting healing spell
                    CheckNeedHeal();

                    // check if we need to panic (too low health, or too many mobs)
                    if (NeedsPanic())
                        return clsGlobals.AttackOutcome.Panic;

                    //Check Health and Need Panic
                    #endregion

                    #region Target and Face

                    // target the unit
                    using (new clsFrameLock.LockBuffer())
                        UnitToAttack.Target();

                    // face the unit
                    Face(UnitToAttack.Location.X, UnitToAttack.Location.Y);

                    // Target and Face
                    #endregion

                    #region Combat

                    // handle combat based on what step we are on
                    switch (currentStep)
                    {
                        case ECombatStep.PrePull:
                            // do pre pull stuff
                            DoPrePull();
                            currentStep = ECombatStep.Pull;
                            continue;

                        case ECombatStep.Pull:
                            // do pull actions
                            DoPull();
                            currentStep = ECombatStep.PostPull;
                            continue;

                        case ECombatStep.PostPull:
                            // post pull actions
                            DoPostPull();
                            currentStep = ECombatStep.InCombat;
                            continue;

                        case ECombatStep.InCombat:
                            // combat actions
                            DoCombat();

                            // restart loop
                            continue;
                    }

                    // Combat
                    #endregion
                }

                // get the result of combat
                clsGlobals.DeadUnitEnum due = CheckDead(UnitToAttack);

                // exit if dead
                if (due == clsGlobals.DeadUnitEnum.Character)
                    return clsGlobals.AttackOutcome.Dead; // DEAD! :(

                // NOTE: this event needs to be raised for the AutoNav bots
                // raise the event this mob was killed
                clsGlobals.Raise_MobKilled(UnitToAttack.Name, UnitToAttack.Level);

                // check if we have aggro, if so, fight it
                if (NumUnitsAttackingMe() > 0)
                    return BeginCombat(GetUnitAttackingMe());

                // not being attacked, return result
                return clsGlobals.AttackOutcome.Success;
            }

            catch (Exception excep)
            {
                // TODO: you need to handle this error however you want. You might
                // want to create a form to display to the user. 
                // I do NOT suggest using MessageBox because it could have
                // adverse affects on Rhabot due to threading issues and 
                // form modality

                clsGlobals.AddStringToLog(string.Format("BeginCombat: ERROR:\r\n{0}", excep.Message));
            }

            // shouldn't be here, unless you didn't handle your exception (slap yourself!)
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
                if (IsInCombat())
                    return;

                // TODO: this is an example of what you might do in pre-pull
                WoWItem MainHandItem = null;
                using (new clsFrameLock.LockBuffer())
                    MainHandItem = isxwow.Me.Equip(WoWEquipSlot.MainHand);
                ApplyWeaponBuff(MainHandItem, "Rockbiter Weapon");
            }

            catch (Exception excep)
            {
                // TODO: you need to handle this error however you want. You might
                // want to create a form to display to the user. 
                // I do NOT suggest using MessageBox because it could have
                // adverse affects on Rhabot due to threading issues and 
                // form modality

                clsGlobals.AddStringToLog(string.Format("DoPrePull: ERROR:\r\n{0}", excep.Message));
            }
        }

        /// <summary>
        /// Performs the pull if we can
        /// </summary>
        private void DoPull()
        {
            // TODO: change this if you want to pull from a closer/further range
            // distance max distance from target to do a pull
            int pDistance = 20;

            try
            {
                // target the unit
                using (new clsFrameLock.LockBuffer())
                    UnitToAttack.Target();

                // toggle attack
                using (new clsFrameLock.LockBuffer())
                {
                    if (!isxwow.Me.Attacking)
                        isxwow.Me.ToggleAttack();
                }

                // face the unit
                using (new clsFrameLock.LockBuffer())
                    Face(UnitToAttack.Location.X, UnitToAttack.Location.Y);

                // get distance from target
                double distance = 0;
                using (new clsFrameLock.LockBuffer())
                    distance = Distance(isxwow.Me.Location.X, isxwow.Me.Location.Y, UnitToAttack.Location.X, UnitToAttack.Location.Y);

                // pull if far enough away
                if (distance > 9)
                {
                    // move close if too far away
                    MoveToTarget(UnitToAttack, pDistance);

                    // TODO: change the pull spell name
                    // cast the pull spell
                    CastSpell("Lightning Bolt");
                }
            }

            catch (Exception excep)
            {
                // TODO: you need to handle this error however you want. You might
                // want to create a form to display to the user. 
                // I do NOT suggest using MessageBox because it could have
                // adverse affects on Rhabot due to threading issues and 
                // form modality

                clsGlobals.AddStringToLog(string.Format("DoPull: ERROR:\r\n{0}", excep.Message));
            }
        }

        /// <summary>
        /// Do post pull actions
        /// </summary>
        private void DoPostPull()
        {
            try
            {
                // TODO: this is where you will handle post pull procedures
                //          such as dropping totems. This happens once per combat session
                //DropTotem("Stoneskin Totem");
            }

            catch (Exception excep)
            {
                // TODO: you need to handle this error however you want. You might
                // want to create a form to display to the user. 
                // I do NOT suggest using MessageBox because it could have
                // adverse affects on Rhabot due to threading issues and 
                // form modality

                clsGlobals.AddStringToLog(string.Format("DoPostPull: ERROR:\r\n{0}", excep.Message));

            }
        }

        /// <summary>
        /// Performs one round of combat
        /// </summary>
        private void DoCombat()
        {
            // list of DoT's to cast on the mob
            List<string> DoTList = new List<string>();

            // list of spells to cast during combat
            List<string> SpellList = new List<string>();

            try
            {
                // TODO: pop the DoTList with the list of DoT's you want
                //      to maintain on the mob
                //DoTList.Add("Searing Pain");

                // TODO: pop the list of spells to cast during combat
                //SpellList.Add("Fireball");

                // move closer if too far away
                MoveAndFace(UnitToAttack);

                // toggle attack
                using (new clsFrameLock.LockBuffer())
                {
                    if ((!isxwow.Me.Attacking) && (!isxwow.Me.AttackingUnit.Dead))
                        isxwow.Me.ToggleAttack();
                }

                // cast Healing Over Time if we need it
                NeedsHOT();

                // cast dots
                foreach (string dot in DoTList)
                {
                    // check for death
                    if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        return;

                    // cast the dot if it doesn't exists on mob
                    if (!BuffExists(UnitToAttack, dot))
                        CastSpell(dot);
                }

                // heal if we need it
                CheckNeedHeal();

                // check for death
                if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // cast the spell list
                foreach (string spell in SpellList)
                {
                    // check for death
                    if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                        return;

                    // cast the spell
                    CastSpell(spell);

                    // TODO: skip this step if you are not a mana user
                    // see if we need mana
                    CheckNeedMana();
                }

                // heal if we need it
                CheckNeedHeal();

                // make sure we are close
                MoveAndFace(UnitToAttack);

                // check for death
                if (CheckDead(UnitToAttack) != clsGlobals.DeadUnitEnum.Neither)
                    return;

                // TODO: change this sleep time to be the amount of time you want
                //      to allow your toon hit the mob with your weapon(s)
                //      Time is in Milliseconds (ms). 1000ms = 1 second. The default
                //      is 3000ms, or 3 seconds

                // let our toon whack on the mob a bit
                Thread.Sleep(3000);
            }

            catch (Exception excep)
            {
                // TODO: you need to handle this error however you want. You might
                // want to create a form to display to the user. 
                // I do NOT suggest using MessageBox because it could have
                // adverse affects on Rhabot due to threading issues and 
                // form modality

                clsGlobals.AddStringToLog(string.Format("DoCombat: ERROR:\r\n{0}", excep.Message));
            }
        }

        // Combat Round Functions
        #endregion

        #region Helper Functions

        // TODO: this function checks for Panic reasons. By default it checks
        // if health is less than 15%

        // TODO: you may want to change how panic resolves. By default, Rhabot
        // tries to run away. If you have another method of resolving panic,
        // you should handle it here, and return Success in the combat
        // function if you successfully loose aggro.
        
        /// <summary>
        /// Returns true if we need to panic
        /// </summary>
        private bool NeedsPanic()
        {
            bool tempBool = false;

            // check if health less than 15%
            tempBool = (isxwow.Me.HealthPercent <= 15);

            // TODO: if health is less than 15%, drink potion, use healthstone, etc
            //if (tempBool)
                // drink potion, use healthstone, etc

            // check if health less than 15%
            // if we still have no health, panic
            return (isxwow.Me.HealthPercent <= 15);
        }

        /// <summary>
        /// Checks if we need to heal. If true, will try to heal
        /// </summary>
        private void CheckNeedHeal()
        {
            // TODO: you may want to tweak this setting. Currently, it
            // won't heal if health percent is above 75%

            // if we are above the setting, then exit
            if (isxwow.Me.HealthPercent > 75)
                return;

            // TODO: you need to heal yourself. Cast heal, use bandage, drink potion, etc
            //CastSpell("Heal Me");

            // TODO: if you can remove poison, disease, or curses, uncomment this and edit the function
            //DispellBuffs();
        }

        /// <summary>
        /// Checks if we need mana. If true, will try to increase mana
        /// </summary>
        private void CheckNeedMana()
        {
            // TODO: you may want to tweak this setting. Currently, it
            // is set to increase mana if mana is less than 45%

            // if we are above the setting, then exit
            if (isxwow.Me.HealthPercent > 45)
                return;

            // TODO: you need to increase your mana. Drink a potion or something
            //DrinkBestManaPotion();
        }

        /// <summary>
        /// Checks if we can and need to cast a healing over time spell. 
        /// </summary>
        private void NeedsHOT()
        {
            // TODO: rename this to your Healing Over Time (HoT) spell
            string HoTSpell = "Gift of the Naaru";

            // TODO: if you have a Healing Over Time (HoT) spell, use it here
            //          otherwise, just exit
            return;

            using (new clsFrameLock.LockBuffer())
            {
                // exit if we already have the buff
                // or if we only have one mob attacking us
                if ((BuffExists(HoTSpell)) || (NumUnitsAttackingMe() <= 1))
                    return;
            }

            // cast the healing over time
            CastSpell(HoTSpell);
        }

        #region Dispell Buffs

        /// <summary>
        /// Removes poison/disease/curses
        /// </summary>
        private void DispellBuffs()
        {
            WoWBuff wbuff = null;
            string spell = "";

            try
            {
                // loop through all buffs on the toon
                for (int buffCounter = 0; buffCounter < 41; buffCounter++)
                {
                    try
                    {
                        // get the buff
                        using (new clsFrameLock.LockBuffer())
                        {
                            wbuff = isxwow.Me.Buff(buffCounter);

                            // skip if no buff or invalid or not harmful
                            if ((wbuff == null) || (!wbuff.IsValid) || (!wbuff.IsHarmful))
                                continue;
                        }
                    }
                    catch { }

                    // log it
                    using (new clsFrameLock.LockBuffer())
                    {
                        clsGlobals.AddStringToLog(string.Format("DispellBuffs: Found Debuff '{0}'", wbuff.Name));

                        // TODO: uncomment for the type of dispelling you can do
//                        switch (wbuff.DispelType)
//                        {
//                            case "Poison":
//                                clsGlobals.AddStringToLog("DispellBuffs: Removing Poison from Target");
//                                spell = "Cure Poison";
//                                break;
//
//                            case "Disease":
//                                clsGlobals.AddStringToLog("DispellBuffs: Removing Disease from Target");
//                                spell = "Cure Disease";
//                                break;
//
//                            case "Curse":
//                                clsGlobals.AddStringToLog("DispellBuffs: Removing Curse from Target");
//                                spell = "Remove Lesser Curse");
//                                break;
//                        }
                    }

                    // cast the spell if we have one
                    if (!string.IsNullOrEmpty(spell))
                        CastSpell(spell);
                }
            }

            catch (Exception excep)
            {
                clsGlobals.AddStringToLog(string.Format("DispellBuffs: ERROR\r\n{0}", excep.Message));
            }
        }

        // Dispell Buffs
        #endregion

        // Helper Functions
        #endregion
    }
}
