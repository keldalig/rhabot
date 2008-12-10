using System;
using System.Collections.Generic;
using System.Text;
using ISXWoW;
using LavishVMAPI;
using LavishScriptAPI;

namespace CodeSnippets
{
    class clsAuction
    {
        /* ==========================================
         * By: UndRgrnD59 (UndRgrnD59@gmail.com)
         * ==========================================
         * This is my auction object, it will provide
         * you with most of the tools you need to
         * write C# programs that will interface with
         * the auction house. Has the code necessary
         * to search for specific items and sell them.
         * ========================================== */

        #region Complex Auction Interaction

        /* ============= Get Auction List ============
         * Returns a list of items currently in the AH
         * window.
         * =========================================== */
        public List<AuctionItem> GetAuctionList()
        {
            List<AuctionItem> returnList = new List<AuctionItem>();
            int numOfItems = GetNumAuctionItems(); // number of items currently on screen
            int totalNumOfItems = GetTotalNumAuctionItems(); // total number of items
            if (numOfItems == 0) // if there are no results just return a blank file
            { return returnList; }

            for (int i = 1; i < numOfItems + 1; i++) // no such item at index 0, start with 1
            {
                AuctionItem temp = new AuctionItem(GetName(i), i, GetCount(i), GetLevel(i), GetMinBid(i),
                    GetMinIncrement(i), GetBuyoutPrice(i), GetBidAmmount(i), GetOwner(i)); // make a temporary item to hold the data
                returnList.Add(temp); // add the item to the return list
            }

            return returnList;
        }

        #endregion

        #region Basic Auction House Interaction

        public bool CanQuery()
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();
            // Can we send a query or is it busy with the last one...
            if (isxwow.WoWScript<bool>("CanSendAuctionQuery()"))
            {
                return true;
            }
            return false;
        }

        public void Query(string Name, int Page)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();
            while (!(CanQuery())) // wait for us to be able to query again
            {
            }
            // query the AH window for the name and page
            isxwow.WoWScript("QueryAuctionItems(\"" + Name + "\",\"\",\"\",nil,nil,nil," + Page + ",nil,nil)");
            Thread.Sleep(3000); // this solved a lot of problems... gotta slow our program down so wow can catch up
        }

        public void QueryForItem(string type, int startLevel, int endLevel, int Page)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();
            // for information on what item type you can search for
            // go here: http://www.wowwiki.com/API_QueryAuctionItems
            isxwow.WoWScript("QueryAuctionItems(\"\"," + startLevel + "," + endLevel + "," + type + ",nil,nil," + Page + ",nil,nil)");
        }

        public void PlaceBid(int index, int bid) //added this in last minute.. should work, not sure bid the buyout to buyout
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();
            isxwow.WoWScript("PlaceAuctionBid(\"list\"," + index + "," + bid + ")");
        }

        public void StartAuction(int startPrice, int boPrice, int time)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            isxwow.WoWScript("StartAuction(" + startPrice + ", " + boPrice + ", " + time + ")");
        }

        public void PlaceAuctionBid(int index, int bid)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            isxwow.WoWScript("PlaceAuctionBid(\"list\"," + index + "," + bid + ")");
        }

        public bool FoundAuctionResults()
        {
            Auction ah = new Auction();
            int ahItems = ah.GetNumAuctionItems();
            if (ahItems != 0)
            {
                return true;
            }
            return false;
        }

        public int GetNumAuctionItems()
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<int>("GetNumAuctionItems(\"list\")");
        }

        public int GetTotalNumAuctionItems()
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<int>("GetNumAuctionItems(\"list\")", 2);
        }

        public int GetNumberOfPages()
        {
            int auctionItems = GetNumAuctionItems();
            int totalAuctionItems = GetTotalNumAuctionItems();
            if (auctionItems == 0 || totalAuctionItems == 0) // To prevent the no "dividing by 0" error.
            { return 0; }

            int pages = totalAuctionItems / auctionItems;
            int remainder = totalAuctionItems % auctionItems;
            if (remainder > 0)
            {
                pages = pages + 1;
            }

            return pages;
        }

        public string GetWoWAuctionItemLink(int index)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<string>("GetAuctionItemLink(\"list\"," + index + ")");
        }

        public string DisplayFormattedPrice(int price)
        {
            // TODO: see format gold function on uscMainInfo

            int gold = (int)(price / 10000);
            int silver = (int)((price / 100) % 100);
            int copper = (int)(price % 100);

            return (gold.ToString() + "g " + silver.ToString() + "s " + copper.ToString() + "c");
        }

        #endregion

        #region Auction Item Properties

        public string GetName(int index)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<string>("GetAuctionItemInfo(\"list\"," + index + ")", 1);
        }

        public int GetCount(int index)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<int>("GetAuctionItemInfo(\"list\"," + index + ")", 3);
        }

        public int GetLevel(int index)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<int>("GetAuctionItemInfo(\"list\"," + index + ")", 6);
        }

        public int GetMinBid(int index)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<int>("GetAuctionItemInfo(\"list\"," + index + ")", 7);
        }

        public int GetMinIncrement(int index)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<int>("GetAuctionItemInfo(\"list\"," + index + ")", 8);
        }

        public int GetBuyoutPrice(int index)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<int>("GetAuctionItemInfo(\"list\"," + index + ")", 9);
        }

        public int GetBidAmmount(int index)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<int>("GetAuctionItemInfo(\"list\"," + index + ")", 10);
        }

        public string GetOwner(int index)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            return isxwow.WoWScript<string>("GetAuctionItemInfo(\"list\"," + index + ")", 12);
        }

        #endregion
    }
}
