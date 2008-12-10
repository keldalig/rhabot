using System;
using System.Collections.Generic;
using System.Threading;
using ISXBotHelper.Properties;
using ISXWoW;
using ISXBotHelper.Settings.Settings;

namespace ISXBotHelper
{
    public static class clsMail
    {
        // From: SendMail in misc.iss of WowCUF
        // Thanx for Tenshi for his great code

        /// <summary>
        /// Sends items that are in the ItemMail list to the specified character. Returns false on error
        /// </summary>
        public static bool SendMailList()
        {
            bool rVal = false;
            List<WoWItem> itemList;

            try
            {
                // log it
                clsSettings.Logging.AddToLog(Resources.SendMailList);

                // exit if not valid
                if ((!clsSettings.GuidValid) || (!clsSettings.IsFullVersion))
                    return false;

                // exit if no player name
                if (string.IsNullOrEmpty(clsSettings.gclsGlobalSettings.Character_MailTo))
                    return true;

                // stop moving
                clsPath.StopMoving();

                // get the mailbox
                GuidList gl = GuidList.New("-mailbox");

                // if nothing returned, exit
                if (gl.Count == 0)
                {
                    clsSettings.Logging.AddToLog(Resources.SendMailList, "No Mailbox Found");
                    return false;
                }

                // get the object
                WoWObject mailbox = gl.Object(0);

                // if we are too far away, move closer
                PathListInfo.PathPoint mPoint = new PathListInfo.PathPoint(mailbox.Location);
                if (mPoint.Distance(clsCharacter.MyLocation) > 4)
                {
                    // move to the mailbox, exit if not successful
                    clsPath cPath = new clsPath();
                    if (cPath.MoveToPoint(mPoint) != clsPath.EMovementResult.Success)
                        return false;
                }

                // try to use it
                if (!mailbox.Use())
                {
                    clsSettings.Logging.AddToLog(Resources.SendMailList, "Could not open mailbox");
                    return false;
                }

                // mail according to color
                clsSettings.Logging.AddToLog(Resources.SendMailList, "Mailing according to color");
                if (clsSettings.gclsGlobalSettings.ItemMailColors.Grey)
                    MailColor(clsGlobalSettings.clsItemColorInfo.GreyFilter);
                if (clsSettings.gclsGlobalSettings.ItemMailColors.White)
                    MailColor(clsGlobalSettings.clsItemColorInfo.WhiteFilter);
                if (clsSettings.gclsGlobalSettings.ItemMailColors.Green)
                    MailColor(clsGlobalSettings.clsItemColorInfo.GreenFilter);
                if (clsSettings.gclsGlobalSettings.ItemMailColors.Blue)
                    MailColor(clsGlobalSettings.clsItemColorInfo.BlueFilter);
                if (clsSettings.gclsGlobalSettings.ItemMailColors.Purple)
                    MailColor(clsGlobalSettings.clsItemColorInfo.PurpleFilter);

                // loop through the mail list and send items that are found
                clsSettings.Logging.AddToLog(Resources.SendMailList, "Looping through the mail list and send items that are found");
                foreach (string item in clsSettings.gclsGlobalSettings.ItemMailList)
                {
                    // get the item from our bags if we have it
                    itemList = clsSearch.Search_Item(string.Format("-inventory, {0}", item));

                    // if nothing returned, skip
                    if ((itemList == null) || (itemList.Count == 0))
                        continue;

                    // loop through and mail returned items
                    foreach (WoWItem mailItem in itemList)
                        MailItem(mailItem);
                }

                // set return value
                rVal = true;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "SendMailList");
            }

            // return value
            return rVal;
        }

        /// <summary>
        /// Mails this item
        /// </summary>
        /// <param name="mailItem"></param>
        private static void MailItem(WoWItem mailItem)
        {
            bool DidPickup = false;

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    // exit if for some reason the item is invalid
                    if ((mailItem == null) || (!mailItem.IsValid))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.Mail, Resources.CanNotMailX, mailItem.FullName);
                        return;
                    }

                    // check if the item stack has too few items (only mail if stack is 85% full or more)
                    if ((mailItem.MaxStackCount > 1) && (mailItem.StackCount < (mailItem.MaxStackCount * 0.85)))
                    {
                        clsSettings.Logging.AddToLogFormatted(Resources.SendMailList, "Skipping item '{0}'. Not enough in stack to mail, yet", mailItem.Name);
                        return; // don't mail this item
                    }

                    // log it
                    clsSettings.Logging.AddToLogFormatted(Resources.SendMailList, "Attempting to mail item '{0}'", mailItem.Name);

                    // pickup the item
                    DidPickup = mailItem.PickUp();
                }

                // if we got the item, mail it
                if (DidPickup)
                {
                    // click the send mail button
                    //clsSettings.ExecuteWoWAPI("ClickSendMailItemButton()");

                    // wait half a second
                    Thread.Sleep(500);

                    // send it
                    clsSettings.ExecuteWoWAPI(string.Format("SendMail(\"{0}\", \"{1}\", \"\")", clsSettings.gclsGlobalSettings.Character_MailTo, mailItem.FullName));

                    // wait one second
                    Thread.Sleep(1000);
                }
                else
                {
                    using (new clsFrameLock.LockBuffer())
                        clsSettings.Logging.AddToLogFormatted(Resources.Mail, Resources.CouldNotPickUpX, mailItem.FullName);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Mail Item");
            }            
        }

        /// <summary>
        /// Mails items of this color
        /// </summary>
        /// <param name="ItemColorFlag"></param>
        private static void MailColor(string ItemColorFlag)
        {
            List<WoWItem> itemList;

            try
            {
                // get the list of items in your bag of this type
                itemList = clsSearch.Search_Item(string.Format("-items,-notsoulbound,-inventory,{0}", ItemColorFlag));

                // exit if no list
                if ((itemList == null) || (itemList.Count == 0))
                    return;

                // loop through and mail each item
                foreach (WoWItem item in itemList)
                    MailItem(item);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, string.Format("Mail Color Items: {0}", ItemColorFlag));
            }            
        }
    }
}
