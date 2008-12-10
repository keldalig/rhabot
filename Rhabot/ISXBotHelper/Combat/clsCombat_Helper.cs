using System;
using System.Threading;
using ISXBotHelper.Properties;
using ISXWoW;

namespace ISXBotHelper
{
    public static class clsCombat_Helper
    {
        /// <summary>
        /// If the totem does not exist, then drops the totem
        /// </summary>
        /// <param name="totem">the totem to drop</param>
        public static void DropTotem(string totem)
        {
            GuidList MyTotems;

            // get the list of totems
            using (new clsFrameLock.LockBuffer())
            {
                MyTotems = new GuidList();
                MyTotems.Search("-units", "-totem", "-mine", totem);

                // if the totem exists, exit
                if (MyTotems.Count > 0)
                    return;
            }

            // totem does not exist, drop it
            clsCombat.CastSpell(totem);
        }

        /// <summary>
        /// Applies a buff to the weapon
        /// </summary>
        /// <param name="weapon">the weapon to buff</param>
        /// <param name="BuffName">the buff name</param>
        /// <param name="IsMainHand">true if this is the mainhand weapon, false if offhand</param>
        public static void ApplyWeaponBuff(WoWItem weapon, string BuffName, bool IsMainHand)
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // exit if the weapon is invalid
                    if ((weapon == null) || (!weapon.IsValid))
                    {
                        clsSettings.Logging.AddToLog(Resources.ApplyWeaponBuff, Resources.WeaponIsInvalid);
                        return;
                    }

                    // exit if the item is not a weapon
                    if (!weapon.Class.ToLower().Contains("weapon"))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.ApplyWeaponBuff, Resources.ItemXIsNotAWeapon, weapon.FullName, weapon.Class);
                        return;
                    }

                    // http://www.wowwiki.com/API_GetWeaponEnchantInfo
                    // if this weapon already has a buff, then exit
                    string hasHandEnchant = clsSettings.isxwow.WoWScript<string>("GetWeaponEnchantInfo()", IsMainHand ? (uint)1 : (uint)4);
                    if ((!string.IsNullOrEmpty(hasHandEnchant)) &&
                        (clsSettings.IsNumeric(hasHandEnchant)) &&
                        (Convert.ToInt32(hasHandEnchant.Trim()) == 1))
                            return;
                }

                // apply the buff now
                if (! clsCombat.CastSpell(BuffName))
                    using (new clsFrameLock.LockBuffer())
                        clsSettings.Logging.AddToLogFormatted(Resources.ApplyWeaponBuff, Resources.CouldNotBuffWeaponWithX, weapon.FullName, BuffName);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "ApplyWeaponBuff");
            }
        }

        #region Panic Run

        /// <summary>
        /// Does a panic run
        /// </summary>
        /// <param name="DrinkHealthPotion">true to drink a health potion before running</param>
        public static clsPath.EMovementResult PanicRun(bool DrinkHealthPotion)
        {
            clsPath.EMovementResult rVal;
            PathListInfo.PathPoint target, myLoc;
            bool CanLoop = true;

            try
            {
                // try to drink a health potion
                if (DrinkHealthPotion)
                    clsPotions.DrinkBestHealingPotion();

                // get heading and my location
                double heading;
                using (new clsFrameLock.LockBuffer())
                {
                	myLoc = new PathListInfo.PathPoint(clsSettings.isxwow.Me.Location);
                    heading = clsSettings.isxwow.Me.Heading;
                }

                // http://www.isxwow.net/forums/viewtopic.php?f=15&t=1550&p=11565#p11565
                // get a point 65 yards from us
                target = new PathListInfo.PathPoint(
                    myLoc.X + 65 * Math.Cos(heading),
                    myLoc.Y + 65 * Math.Sin(heading),
                    myLoc.Z);

                // we are in panic mode, so run until 
                // no mobs are attacking or until we die
                clsStuck.ResetStuck();
                while (CanLoop)
                {
                    // check for forced shutdown
                    if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsPath.EMovementResult.Stopped;

                    // face the point
                    if (!clsFace.FacePointEx(target))
                        clsPath.StartMoving();

                    // sleep 1/3 second
                    Thread.Sleep(300);

                    // check if we need to loop again
                    using (new clsFrameLock.LockBuffer())
                        CanLoop = ((target.Distance(clsCharacter.MyLocation) <= 5) || ((clsCombat.NumUnitsAttackingMe() > 0) && (!clsCharacter.IsDead)));
                }

                // decide how to return
                if (!clsCharacter.IsDead)
                {
                    rVal = clsPath.EMovementResult.Success;

                    // heal if needed
                    new clsCombat().CheckNeedHeal();
                }
                else
                    rVal = clsPath.EMovementResult.Dead;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "PanicRun");
                rVal = clsPath.EMovementResult.Error;
            }

            finally
            {
                clsPath.StopMoving();
            }

            return rVal;
        }

        // Panic Run
        #endregion
    }
}
