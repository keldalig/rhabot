// Code by Undrgrnd59 - Apr 2007

namespace ISXBotHelper.Talents
{
    public class clsTalentInfo
    {
        public string name;
        public int tier;
        public int column;
        public int currentRank;
        public int maxRank;
        public int treeNum;
        public int talentIndex;

        public clsTalentInfo(string name, int tier, int column, int currentRank, int maxRank, int treeNum, int talentIndex)
        {
            this.name = name;
            this.tier = tier;
            this.column = column;
            this.currentRank = currentRank;
            this.maxRank = maxRank;
            this.treeNum = treeNum;
            this.talentIndex = talentIndex;
        }
    }
}
