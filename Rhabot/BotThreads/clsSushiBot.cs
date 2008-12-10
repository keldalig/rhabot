/*
 * Fishing code by Undrgrnd59
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXBotHelper.Threading;
using ISXWoW;
using LavishVMAPI;

namespace Rhabot.BotThreads
{
    internal class clsSushiBot : ThreadBase
    {
        #region Variables

        private clsSettings.ThreadItem threadItem;
        private bool IsStopped;
        private WoWItem EquipedPole;

        /// <summary>
        /// List of fishing poles, with the best pole first
        /// </summary>
        private readonly List<string> FishingPoles = new List<string>() { "Arcanite Fishing Pole", "Nat Pagle's Extreme Angler FC-5000", "Seth's Graphite Fishing Pole", "Big Iron Fishing Pole", "Darkwood Fishing Pole", "Strong Fishing Pole", "Blump Family Fishing Pole", "Fishing Pole" };

        // Variables
        #endregion

        #region Start / Stop

        /// <summary>
        /// Starts fishing. You must already be at a fishing spot. Equips a pole if you don't have one on
        /// </summary>
        public void StartSushi()
        {
            IsStopped = false;
            Thread thread = new Thread(Sushi_Thread);
            thread.Name = "Sushi Bot";
            threadItem = new clsSettings.ThreadItem(thread, this);
            clsSettings.ThreadList.Add(threadItem);
            thread.Start();
        }

        /// <summary>
        /// Stops fishing. Does not stop other threads, etc.
        /// </summary>
        public void StopSushi()
        {
            IsStopped = true;

            // kill the thread
            clsSettings.KillThread(threadItem, Resources.ExitingDueToScriptStop);
        }

        // Start / Stop
        #endregion

        #region Run Sushi Bot

        private void Sushi_Thread()
        {
            List<WoWGameObject> bobberList;
            WoWSpell fishSpell;

            try
            {
                clsSettings.Logging.AddToLog(Resources.SushiBot, Resources.Starting);

                // loop until shutdown
                while ((! Shutdown) && (!IsStopped) && (clsSettings.TestPauseStop(Resources.SushiBot, Resources.ExitingDueToScriptStop)))
                {
                    // set the equipped pole
                    EquipedPole = FindPole();

                    // put on a pole
                    if ((EquipedPole == null) || (!EquipedPole.IsValid))
                        EquipedPole = EquipPole();

                    // exit if no pole found
                    if ((EquipedPole == null) || (!EquipedPole.IsValid))
                    {
                        clsError.ShowError(new Exception(Resources.SushiWhatAreYouThinking), Resources.SushiBot, string.Empty, true, new StackFrame(0, true), false);
                        return;
                    }

                    // buff pole if it needs the buff
                    if (!EquipedPole.Enchantment(Resources.FishingLure).IsValid)
                        BuffPole();

                    // check if we can buff with a drink
                    BuffSelf();

                    // get the spell
                    using (new clsFrameLock.LockBuffer())
                        fishSpell = WoWSpell.Get(Resources.Fishing);

                    // cast the pole
                    if (!fishSpell.Cast())
                    {
                        // couldn't cast. log it and sleep 2 seconds
                        clsSettings.Logging.AddToLog(Resources.SushiBot, Resources.SushiFishSpellError);
                        Thread.Sleep(2000);
                        continue;
                    }

                    Thread.Sleep(500);

                    // check for bites while casting
                    while (clsCharacter.IsCasting)
                    {
                        // sleep a tic
                        Thread.Sleep(300);

                        // get the bobber
                        bobberList = clsSearch.Search_GameObject("-gameobjects,Fishing Bobber,-exact,-usable");

                        // sleep 1/3 second if no bobber found
                        if ((bobberList == null) || (bobberList.Count == 0))
                        {
                            Thread.Sleep(100);
                            clsSettings.Logging.AddToLog(Resources.SushiBot, Resources.SushiBobberError);
                            continue;
                        }

                        // check if the bobber has a bite
                        if (bobberList[0].GotBite)
                        {
                            // open it
                            using (new clsFrameLock.LockBuffer())
                                bobberList[0].Use();

                            // wait a tick
                            Thread.Sleep(300);

                            // loot it
                            clsLoot.GetLoot();

                            // open items (such as clams)
                            clsVendor.OpenItems();

                            // delete junk items
                            clsVendor.DeleteJunkItems();

                            // disnchant items
                            clsVendor.DisenchantItems();

                            break;
                        }

                        // exit if stopped
                        if ((!IsStopped) && (! clsSettings.TestPauseStop(Resources.SushiBot, Resources.ExitingDueToScriptStop)))
                            return;
                    }

                    // wait one second
                    clsSettings.Logging.AddToLog(Resources.SushiBot, Resources.WaitingBeforeRecast);
                    Thread.Sleep(1000);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.SushiBot);
                clsSettings.Stop = true;
            }            

            finally
            {
                clsSettings.Logging.AddToLog(Resources.SushiBot, Resources.Exiting);
            }
        }

        // Run Sushi Bot
        #endregion

        #region Functions

        /// <summary>
        /// Buffs ourself by drinking or taking other actions
        /// </summary>
        private void BuffSelf()
        {
            // check if we have the buff
            if (clsCharacter.BuffExists("Captain Rumsey's Lager")) 
                return;

            // check if we have the drink
            List<WoWItem> itemList = clsSearch.Search_BagItem("Captain Rumsey's Lager");
            if ((itemList == null) || (itemList.Count <= 0)) 
                return;
            
            // use the item
            itemList[0].Use();

            // wait a tick
            Frame.Wait(false);
            Frame.Wait(false);
            Frame.Wait(false);
        }

        /// <summary>
        /// Buffs the pole with the an available fishing lure
        /// </summary>
        private void BuffPole()
        {
            // get our fishing skill
            int Skill = clsSettings.isxwow.Me.Skill(Resources.Fishing);

            List<WoWItem> itemList = null;

            // get a bobber
            if (Skill >= 100)
            {
                itemList = clsSearch.Search_BagItem("Sharpened Fishing Hooks");
                if ((itemList == null) || (itemList.Count == 0))
                    itemList = clsSearch.Search_BagItem("Aquadynamic Fish Attractor");
                if ((itemList == null) || (itemList.Count == 0))
                    itemList = clsSearch.Search_BagItem("Bright Baubles");
                if ((itemList == null) || (itemList.Count == 0))
                    itemList = clsSearch.Search_BagItem("Flesh Eating Worm");
            }
            if ((Skill >= 50) && ((itemList == null) || (itemList.Count == 0)))
            {
                itemList = clsSearch.Search_BagItem("Aquadynamic Fish Lens");
                if ((itemList == null) || (itemList.Count == 0))
                    itemList = clsSearch.Search_BagItem("Nightcrawlers");
            }
            if ((itemList == null) || (itemList.Count == 0))
                itemList = clsSearch.Search_BagItem("Shiny Bauble");

            // exit if nothing found
            if ((itemList == null) || (itemList.Count == 0))
                return;

            // buff the pole
            using (FrameLock fl = new FrameLock(true))
            {
                itemList[0].Use();
                fl.Unlock();
                
                fl.Lock();
                EquipedPole.PickUp();
            }

            while (clsCharacter.IsCasting)
                Thread.Sleep(1000);
        }

        /// <summary>
        /// Returns the equipped fishing pole
        /// </summary>
        /// <returns></returns>
        private WoWItem FindPole()
        {
            const string searchStr = "-worn,-items,{0},-exact";
            // get the list of equiped fishing poles
            List<WoWItem> poleList = null;

            using (new clsFrameLock.LockBuffer())
            {
                // try to find the best pole
                foreach (string pole in FishingPoles)
                {
                    poleList = clsSearch.Search_Item(string.Format(searchStr, pole));
                    if ((poleList != null) && (poleList.Count > 0) && (poleList[0].IsValid))
                        break;
                }
            }

            // return the first item found
            if ((poleList != null) && (poleList.Count > 0))
                return poleList[0];

            return null;
        }

        /// <summary>
        /// Returns the equipped fishing pole
        /// </summary>
        /// <returns></returns>
        private WoWItem EquipPole()
        {
            List<WoWItem> poleList = null;
            WoWItem tempItem = null;

            // try to find the best pole in the bag
            using (new clsFrameLock.LockBuffer())
            {
                foreach (string pole in FishingPoles)
                {
                    // try the best pole
                    poleList = clsSearch.Search_BagItem(pole);
                    if ((poleList != null) && (poleList.Count > 0) && (poleList[0].IsValid))
                        break;
                }
            }

            // found something, use it
            if ((poleList != null) && (poleList.Count > 0) && (poleList[0].IsValid))
            {
                using (new clsFrameLock.LockBuffer())
                {
                    tempItem = poleList[0];
                    tempItem.Use();
                }
                Thread.Sleep(1000);
            }

            // return the pole
            return tempItem;
        }

        // Functions
        #endregion
    }
}
