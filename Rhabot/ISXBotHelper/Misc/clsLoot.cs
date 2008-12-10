using System;
using System.Threading;
using ISXBotHelper.Properties;
using ISXWoW;
using LavishVMAPI;

namespace ISXBotHelper
{
    public static class clsLoot
    {
        #region Loot Gameobject

        /// <summary>
        /// Loots a game object (herb/mine/chest). Moves to the unit if we are not already there
        /// </summary>
        /// <param name="LootUnit">the unit to loot</param>
        /// <param name="UnitIsObject">true if the LootUnit is an herb, mine, chest, or other object than can be used/opened</param>
        public static clsPath.EMovementResult LootGameOjbect(WoWUnit LootUnit, bool UnitIsObject)
        {
            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    //// skip if not lootable
                    //if ((LootUnit == null) || (!LootUnit.IsValid) || ((!LootUnit.CanLoot) && (!UnitIsObject)))
                    //    return clsPath.EMovementResult.Success;

                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.LootGameOjbect, Resources.LootingX, LootUnit.Name);
                }

                // stop moving
                clsPath.StopMoving();

                // dismount
                clsMount.Dismount();

                // move to the unit
                clsPath cPath = new clsPath();
                clsPath.EMovementResult eResult = cPath.MoveToPoint(clsPath.GetUnitLocation(LootUnit));
                
                // pause a sec
                Thread.Sleep(750);

                // if we are stuck, blacklist it and return success
                if ((eResult == clsPath.EMovementResult.Stuck) || (eResult == clsPath.EMovementResult.PathObstructed))
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.LootGameOjbect, Resources.LootStuckBugged, eResult.ToString());

                    // blacklist the unit
                    using (new clsFrameLock.LockBuffer())
                        clsSettings.Blacklist_GameObjects.Add(new clsBlacklist(LootUnit));

                    // return success
                    return clsPath.EMovementResult.Success;
                }

                // if not success, return result
                if (eResult != clsPath.EMovementResult.Success)
                    return eResult;

                bool Targetted = false;
                using (new clsFrameLock.LockBuffer())
                {
                    // exit if not lootable
                    // lootable = corpse sparkling
                    // CanLoot = your bag icon on your cursor is not dark (close enough to loot)
                    if (!UnitIsObject) // TESTING: && ((!LootUnit.Lootable) || (!LootUnit.CanLoot)))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.LootGameOjbect, "Could not loot object '{0}'. Lootable is false", LootUnit.Name);
                        return clsPath.EMovementResult.Success;
                    }

                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.LootGameOjbect, Resources.LootingXatY,
                        LootUnit.Name, clsPath.GetUnitLocation(LootUnit).ToString());

                    // target the unit
                    Targetted = LootUnit.Target();
                }

                // if we did not target, try again
                if (! Targetted)
                {
                    // wait a tick
                    LavishVMAPI.Frame.Wait(false);

                    // try it again
                    using (new clsFrameLock.LockBuffer())
                        LootUnit.Target();
                }

                // wait a tick
                LavishVMAPI.Frame.Wait(false);

                // open loot window
                bool used;
                using (new clsFrameLock.LockBuffer())
                    used = LootUnit.Use();

                // if we couldn't open the loot window, wait 2 seconds and try again
                if (!used)
                {
                    clsSettings.Logging.AddToLog(Resources.CoulndtOpenLootWindow);

                    // change the wait time based on what the item is
                    // to allow more time for herbing
                    Thread.Sleep(UnitIsObject ? 5000 : 2000);

                    using (new clsFrameLock.LockBuffer())
                        LootUnit.Use();
                }

                // get the loot
                if (GetLoot() == clsPath.EMovementResult.Error)
                {
                    // blacklist
                    using (new clsFrameLock.LockBuffer())
                        clsSettings.Blacklist_GameObjects.Add(new clsBlacklist(LootUnit));
                    return clsPath.EMovementResult.Success;
                }

                // close the loot window if it is open
                using (new clsFrameLock.LockBuffer())
                {
                    if (UI.LootWindowFrame.IsVisible)
                        UI.LootWindowFrame.Close();
                }
                Thread.Sleep(300);

                // wait a frame (?)
                Frame.Wait(false);

                // TODO: reset skinnable when working
                bool CanSkin;
                using (new clsFrameLock.LockBuffer())
                    CanSkin = ((clsSettings.gclsLevelSettings.IsSkinner) && (LootUnit.IsValid)); // && (LootUnit.Skinnable));

                // skinning if we are skinner
                if (CanSkin)
                {
                    // exit if stopped
                    if (!clsSettings.TestPauseStop(Resources.ExitingDueToScriptStop))
                        return clsPath.EMovementResult.Stopped;

                    // wait 2 seconds for loot window to close
                    if (clsCharacter.LootWindowIsVisible)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            // exit the loot window is gone
                            if (!clsCharacter.LootWindowIsVisible)
                                break;

                            // make sure we're not in combat
                            if (clsCombat.IsInCombat())
                                return clsPath.EMovementResult.Aggroed;

                            // sleep 1/5 second
                            Thread.Sleep(200);
                        }
                    }

                    // sleep 1 sec, then try to skin
                    Thread.Sleep(1000);

                    // make sure we're not in combat
                    if (clsCombat.IsInCombat())
                        return clsPath.EMovementResult.Aggroed;

                    // skin it
                    using (new clsFrameLock.LockBuffer())
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.LootGameOjbect, Resources.SkinningX, LootUnit.Name);
                        used = LootUnit.Use();
                    }

                    // if we couldn't open the loot window, wait 2 seconds and try again
                    if (!used)
                    {
                        clsSettings.Logging.AddToLog(Resources.CoulndtOpenLootWindow);
                        Thread.Sleep(2000);

                        using (new clsFrameLock.LockBuffer())
                            LootUnit.Use();
                    }

                    // sleep half sec
                    Thread.Sleep(500);

                    while (clsCharacter.IsCasting)
                    {
                        // make sure we're not in combat
                        if (clsCombat.IsInCombat())
                            return clsPath.EMovementResult.Aggroed;

                        // wait
                        Thread.Sleep(300);
                    }

                    // get the loot
                    GetLoot();
                }

                // open lockboxes
                clsVendor.OpenLockboxes();

                // open items
                clsVendor.OpenItems();

                // delete junk items
                clsVendor.DeleteJunkItems();

                // disnchant items
                clsVendor.DisenchantItems();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "LootGameObject");
                return clsPath.EMovementResult.Error;
            }

            finally
            {
                // Check if we need to equip any bagged items
                ISXBotHelper.Items.clsAutoEquip.CheckNeedEquipItems();
            }

            // everything should be good at this point
            return clsPath.EMovementResult.Success;
        }

        /// <summary>
        /// Gets loot after the loot window is open
        /// </summary>
        public static clsPath.EMovementResult GetLoot()
        {
            DateTime loopTimer = DateTime.Now;

            try
            {
                // sleep to give loot window time to display
                while (!clsCharacter.LootWindowIsVisible)
                {
                    clsSettings.Logging.AddToLogFormatted(Resources.GetLoot, Resources.LootWaitTimer, new TimeSpan(DateTime.Now.Ticks - loopTimer.Ticks).Seconds);
                    Thread.Sleep(300);
                    if (new TimeSpan(DateTime.Now.Ticks - loopTimer.Ticks).Seconds > 5)
                    {
                        clsSettings.Logging.AddToLog(Resources.GetLoot, Resources.ExitingLootWinNotVisible);
                        return clsPath.EMovementResult.Error;
                    }
                }

                // if frame is not visible, log it
                if (!clsCharacter.LootWindowIsVisible)
                    clsSettings.Logging.AddToLog(Resources.GetLoot, Resources.LootWinNotVisible);

                clsSettings.Logging.AddToLogFormatted(Resources.GetLoot, Resources.XItemsToLoot, clsCharacter.LootWindowItemCount);

                Thread.Sleep(1000);
                // loot everything
                int i;
                for (i = 1; i < clsCharacter.LootWindowItemCount + 1; i++)
                {
                    // check if aggroed
                    if (clsCombat.IsInCombat())
                        return clsPath.EMovementResult.Aggroed;

                    // loot the item
                    using (new clsFrameLock.LockBuffer())
                    {
                        // get the name
                        string lName = UI.LootWindowFrame.Slot((uint)i).Name;
                        clsSettings.Logging.AddToLogFormatted(Resources.GetLoot, Resources.LootingSlotX, i, lName);
                        UI.LootWindowFrame.Slot((uint)i).Loot();
                    }

                    Thread.Sleep(1000);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "GetLoot");
            }

            return clsPath.EMovementResult.Success;
        }

        // Loot Gameobject
        #endregion
    }
}
