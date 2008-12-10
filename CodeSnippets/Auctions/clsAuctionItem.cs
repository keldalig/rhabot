using System;
using System.Collections.Generic;
using System.Text;
using ISXWoW;
using LavishVMAPI;
using LavishScriptAPI;

namespace CodeSnippets.Auctions
{
    class clsAuctionItem
    {
        /* AuctionItem by UndRgrnD59 (UndRgrnD59@gmail.com)
         * ============================================================
         * The AuctionItem is my auctionhouse item information holder
         * for the items currently being Auctioned. It doesn't store all
         * the information possible, just what I thought was necessary.
         * ============================================================ */

        public string name;
        public int indexInList;
        public int count;
        public int level;
        public int minBid;
        public int minIncrement;
        public int buyoutPrice;
        public int bidAmmount;
        public string owner;
        public int gameTimeHours;
        public int gameTimeMinutes;

        public AuctionItem(string itemName, int indexInList, int count, int level, int minBid, int minIncrement, int buyoutPrice, int bidAmmount, string owner)
        {
            ISXWoW.ISXWoW isxwow = new ISXWoW.ISXWoW();

            this.name = itemName;
            this.indexInList = indexInList;
            this.count = count;
            this.level = level;
            this.minBid = minBid;
            this.minIncrement = minIncrement;
            this.buyoutPrice = buyoutPrice;
            this.bidAmmount = bidAmmount;
            this.owner = owner;
            this.gameTimeHours = isxwow.WoWScript<int>("GetGameTime()");
            this.gameTimeMinutes = isxwow.WoWScript<int>("GetGameTime()", 2);
        }
    }
}
