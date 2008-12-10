using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.AutoNav;
using ISXBotHelper.Explore;
using ISXBotHelper.Properties;
using ISXBotHelper.XProfile;

namespace Rhabot
{
    public partial class uscAutoNav : UserControl
    {
        #region Variables

        private readonly ListViewGroup GrpTop = new ListViewGroup(Resources.TopLevel, Resources.TopLevelActions);
        private readonly ListViewGroup GrpSub = new ListViewGroup(Resources.SubGroup, Resources.SubGroupActions);
        private readonly ListViewGroup GrpConditions = new ListViewGroup(Resources.Conditions, Resources.Conditions);
        private readonly Color ColorAllow = Color.Black;
        private readonly Color ColorNotAllow = Color.Red;

        // Variables
        #endregion

        #region Init/Load

        public uscAutoNav()
        {
            InitializeComponent();
        }

        private void uscAutoNav_Load(object sender, EventArgs e)
        {
            // hide the controls
            ShowNPC(false);
            ShowQuantity(false);
            ShowZone(false);
            ShowLocation(false);
            ShowPathFile(false);
            ShowFlying(false);

            // pop the lists
            PopZoneList();
            PopTopList();
        }

        // Init/Load
        #endregion

        #region Pop Functions

        /// <summary>
        /// Pops the left list
        /// </summary>
        private void PopTopList()
        {
            try
            {
                // clear the list
                this.lstvItems.Items.Clear();
                this.lstvItems.Groups.Clear();

                // add the groups
                this.lstvItems.Groups.Add(GrpTop);
                this.lstvItems.Groups.Add(GrpSub);
                this.lstvItems.Groups.Add(GrpConditions);

                // add each item

                #region Top Level

                AddItemToTopList(Resources.GoToRunRhabotPath, GrpTop, clsBlockListItem.ENodeType.Run_Rhabot_Path, Resources.RunsapremadeRhabotpath);
                AddItemToTopList(Resources.GotoVendorXinZoneY, GrpTop, clsBlockListItem.ENodeType.Go_To_Vendor_X_City_Y, Resources.Buildsapathtothevendor);
                AddItemToTopList(Resources.GoToFlightmasterXinZoneY, GrpTop, clsBlockListItem.ENodeType.Go_To_Flightmaster_X_City_Y, Resources.Buildsapathtotheflightmaster);
                AddItemToTopList(Resources.GoToTrainerXinZoneY, GrpTop, clsBlockListItem.ENodeType.Go_To_Trainer_X_City_Y, Resources.Buildsapathtothetrainer);
                AddItemToTopList(Resources.GoToTakeBoattoZoneX, GrpTop, clsBlockListItem.ENodeType.Take_Boat_To_City_X, Resources.Buildsapathtotheboatdock);
                AddItemToTopList(Resources.GoToInnkeeperinZoneX, GrpTop, clsBlockListItem.ENodeType.Go_To_Innkeeper_X_City_Y, Resources.BuildsapathtotheInnkeeper);
                AddItemToTopList(Resources.GoToRepairVendorXinZoneY, GrpTop, clsBlockListItem.ENodeType.Go_To_Repair_Vendor_X_City_Y, Resources.Buildsapathtotherepairvendor);
                AddItemToTopList(Resources.GoToMailboxinZoneX, GrpTop, clsBlockListItem.ENodeType.Go_To_Mailbox_X_City_Y, Resources.Buildsapathtothemailbox);
                AddItemToTopList(Resources.GoToNPCXinZoneY, GrpTop, clsBlockListItem.ENodeType.Speak_To_Person_X_At_Y, Resources.BuildsapathtotheNPC);
                AddItemToTopList(Resources.GoToQuestgiverXinZoneY, GrpTop, clsBlockListItem.ENodeType.Go_To_QuestGiver_X_At_Y, Resources.Buildsapaththequestgiver);
                AddItemToTopList(Resources.GoToRunRhabotPath, GrpTop, clsBlockListItem.ENodeType.Run_Rhabot_Path, Resources.RunsapremadeRhabotpath);
                AddItemToTopList(Resources.GoToStonehome, GrpTop, clsBlockListItem.ENodeType.Stone_Home, Resources.HearthstonestoyourhomeInn);
                AddItemToTopList(Resources.GoToXYZ, GrpTop, clsBlockListItem.ENodeType.Go_To_XYZ, Resources.Buildsapathfromthecurrentlocation_toXYZ);
                AddItemToTopList(Resources.FishinZoneX, GrpTop, clsBlockListItem.ENodeType.Fish_At_XYZ, Resources.Buildsapathtothespecifiedschooloffish);
                AddItemToTopList(Resources.BlockNewBlock, GrpTop, clsBlockListItem.ENodeType.NEW_BLOCK, Resources.Createanewblock);
                AddItemToTopList(Resources.RunFullRhabotPath, GrpTop, clsBlockListItem.ENodeType.Run_Rhabot_Path_Full, Resources.RunsacompleteRhabotpath);

                // Top Level
                #endregion

                #region Sub Actions

                AddItemToTopList(Resources.PurchaseXofYitems, GrpSub, clsBlockListItem.ENodeType.Purchase_X_of_Y, Resources.PurchasesYitemuntil);
                AddItemToTopList(Resources.TakeflighttoX, GrpSub, clsBlockListItem.ENodeType.Take_Flight_X_To_Y, Resources.Takesflightfromyourcurrentlocation);
                AddItemToTopList(Resources.MakeInnyourhome, GrpSub, clsBlockListItem.ENodeType.Make_Inn_Home, Resources.MakesthisInnyournewhome);
                AddItemToTopList(Resources.Pickupquest, GrpSub, clsBlockListItem.ENodeType.Pick_Up_Quest_X, Resources.PicksupthequestnamedX);
                AddItemToTopList(Resources.Turninquest, GrpSub, clsBlockListItem.ENodeType.Turn_In_Quest_Y, Resources.TurnsinquestnamedX);
                AddItemToTopList(Resources.FightmobsinZoneX, GrpSub, clsBlockListItem.ENodeType.Fight_Mobs_Zone_X, Resources.Buildsapathtofightmobs);
                AddItemToTopList(Resources.FightmobsinZoneXnamedY, GrpSub, clsBlockListItem.ENodeType.Fight_Mobs_Zone_X_Named_Y, Resources.BuildsapathtofightmobsNamedX);
                AddItemToTopList(Resources.FightelitemobsinZoneX, GrpSub, clsBlockListItem.ENodeType.Fight_Elite_Mobs_Zone_X, Resources.BuildsapathtofightelitemobsinZoneX);
                AddItemToTopList(Resources.FightelitemobsinZoneXnamedY, GrpSub, clsBlockListItem.ENodeType.Fight_Elite_Mobs_Zone_X_Named_Y, Resources.BuildsapathtofightelitemobsinZoneXNamedY);
                AddItemToTopList(Resources.FindHerbsinZoneX, GrpSub, clsBlockListItem.ENodeType.Find_Herbs_Zone_X, Resources.Buildsapathtoallknownherbs);
                AddItemToTopList(Resources.FindChestsinZoneX, GrpSub, clsBlockListItem.ENodeType.Find_Mines_Zone_X, Resources.BuildsapathtoallknownMines);
                AddItemToTopList(Resources.FindChestsinZoneXEx, GrpSub, clsBlockListItem.ENodeType.Find_Chests_Zone_X, Resources.BuildsapathtoallknownChests);
                AddItemToTopList(Resources.FindobjectsnamedX, GrpSub, clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y_MULTI, Resources.BuildsapathtoallknownObjects);
                AddItemToTopList(Resources.UseIteminbackpack, GrpSub, clsBlockListItem.ENodeType.Use_Item_X, Resources.UsestheitemnamedX);
                AddItemToTopList(Resources.GoToXYZ, GrpSub, clsBlockListItem.ENodeType.Go_To_XYZ, Resources.Buildsapathfromthecurrentlocation_toXYZ);
                AddItemToTopList(Resources.GoToRunRhabotPath, GrpSub, clsBlockListItem.ENodeType.Run_Rhabot_Path, Resources.RunsapremadeRhabotpath);
                AddItemToTopList(Resources.ChooseGossipnumberX, GrpSub, clsBlockListItem.ENodeType.Choose_Gossip_Option_X, Resources.ChoosesthegossipatindexX);
                AddItemToTopList(Resources.SpecialConditionReturntovendor, GrpConditions, clsBlockListItem.ENodeType.Return_To_Vendor_When_X_Has_Quantity_Y, Resources.Return_To_Vendor_When_X_Has_Quantity_Y);

                // Sub Actions
                #endregion

                #region Conditions

                AddItemToTopList(Resources.ContinueuntilyouhavekilledXofY, GrpConditions, clsBlockListItem.ENodeType.Continue_Until_X_Mob_Y_Killed, Resources.ContinuethisblockofactionsuntilyoukillX);
                AddItemToTopList(Resources.ContinueuntilyouhaveXquantity, GrpConditions, clsBlockListItem.ENodeType.Continue_Until_X_Item_Y_Quantity, Resources.ContinuethisblockofactionsuntilyourcharacterhasXnumber);
                AddItemToTopList(Resources.ContinueuntilCharacterislevelX, GrpConditions, clsBlockListItem.ENodeType.Continue_Until_Character_Is_Level_X, Resources.untilyourcharactersreachesthespecifiedlevel);
                AddItemToTopList(Resources.ContinueuntilDurabilityisXpercent, GrpConditions, clsBlockListItem.ENodeType.Continue_Durability_X_Percent, Resources.untilyourdurabilityisXpercent);
                AddItemToTopList(Resources.ContinueuntilBagsarefull, GrpConditions, clsBlockListItem.ENodeType.Continue_Until_Bags_Full, Resources.untilyourbagsarefull);
                AddItemToTopList(Resources.FishuntilXtime, GrpConditions, clsBlockListItem.ENodeType.Fish_Until_X_Time, Resources.FishesuntilXminutes);
                AddItemToTopList(Resources.FishuntilyouhaveXquantity, GrpConditions, clsBlockListItem.ENodeType.Fish_Capture_X_of_Y, Resources.FishesuntilyouhaveXnumberofYfish);
                
                // Conditions
                #endregion
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoNav, Resources.PopLeftList);
            }            
        }

        private void AddItemToTopList(string text, ListViewGroup Group, clsBlockListItem.ENodeType NodeType, string Tooltip)
        {
            ListViewItem lvi = new ListViewItem(text, Group);
            lvi.Tag = NodeType;
            lvi.ToolTipText = Tooltip;
            // disallow sub groups
            if (lvi.Group != GrpTop)
                lvi.ForeColor = ColorNotAllow;
            this.lstvItems.Items.Add(lvi);
        }

        // Pop Functions
        #endregion

        #region Button Clicks

        #region Save

        /// <summary>
        /// Save the action plan
        /// </summary>
        private void cmdSave_Click(object sender, EventArgs e)
        {
            List<clsBlockList> autoNavList = new List<clsBlockList>();
            clsBlockList bl;
            TreeNode node;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                #region Validation

                // make sure we have action items
                if (this.trvActionPlan.Nodes.Count == 0)
                {
                    MessageBox.Show(Resources.Pleaseaddoneormoreactionitems);
                    return;
                }

                // make sure we have a plan name
                if (string.IsNullOrEmpty(this.txtPlanName.Text))
                {
                    MessageBox.Show(Resources.Pleaseentertheplanename);
                    this.txtPlanName.Focus();
                    return;
                }

                // Validation
                #endregion

                // get the node count
                int nodeCount = this.trvActionPlan.GetNodeCount(false);

                // loop through top level nodes and add them to the list
                for (int i = 0; i < nodeCount; i++)
                {
                    // get the top item
                    bl = new clsBlockList();
                    node = this.trvActionPlan.Nodes[i];
                    bl.BlockItem = (clsBlockListItem)node.Tag;

                    // pop children
                    if (node.Nodes.Count > 0)
                        bl.SubBlocks = SaveNodeChildren(node);

                    // add to the list
                    autoNavList.Add(bl);
                }

                // save the list
                clsSettings.SaveAutoNav(new clsAutoNavSaveList(autoNavList, this.txtPlanName.Text.Trim()));

                // notify of save
                this.lblMessage.Text = Resources.AutoNavPlanSaved;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.SaveActionPlan);
            }            

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Builds a list of children block list items for saving
        /// </summary>
        /// <param name="node">the parent node</param>
        private List<clsBlockList> SaveNodeChildren(TreeNode node)
        {
            TreeNode childNode;
            List<clsBlockList> childList = new List<clsBlockList>();
            clsBlockList bl;
            int j = node.Nodes.Count;

            try
            {
                // loop through all children of this node
                for (int i = 0; i < j; i++)
                {
                    // get the node
                    bl = new clsBlockList();
                    childNode = node.Nodes[i];
                    bl.BlockItem = (clsBlockListItem)childNode.Tag;

                    // if this node has children, pop them here
                    if (childNode.Nodes.Count > 0)
                        bl.SubBlocks = SaveNodeChildren(childNode);

                    // add to the list
                    childList.Add(bl);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.SaveNodeChildren);
            }

            return childList;
        }

        // Save
        #endregion

        #region Load

        /// <summary>
        /// Load the action plan
        /// </summary>
        private void cmdLoad_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                this.lblMessage.Text = string.Empty;

                // ask user what to load
                List<string> PlanList = clsSettings.LoadAutoNavList();

                this.Cursor = Cursors.Default;

                // if no list, tell user
                if ((PlanList == null) || (PlanList.Count == 0))
                {
                    clsError.ShowError(new Exception(Resources.YoudonothaveanysavedAutoNavplans), Resources.LoadAutoNavPlans, true, new StackFrame(0, true), false);
                    return;
                }

                // build the data for the select list
                List<string> columns = new List<string>();
                columns.Add(Resources.PlanName);
                List<List<string>> PlanData = new List<List<string>>();
                List<string> pItem;
                foreach (string pName in PlanList)
                {
                    pItem = new List<string>();
                    pItem.Add(pName);
                    PlanData.Add(pItem);
                }

                // show the planlist in a new window to the user. let them select one item
                frmSelect fSelect = new frmSelect();
                fSelect.Show(this);
                string PlanName = fSelect.ShowForm(Resources.AutoNavPlanSelect, columns, PlanData, 0);
                fSelect.Close();
                fSelect = null;

                // exit if nothing selected
                if (string.IsNullOrEmpty(PlanName))
                    return;

                // clear everything
                this.Cursor = Cursors.WaitCursor;
                ClearControls();
                this.trvActionPlan.Nodes.Clear();
                PopZoneList();
                PopTopList();

                // open the file
                List<clsBlockList> AutoNavList = clsSettings.LoadAutoNav(PlanName).AutoNavList;

                // loop through and add all the nodes
                foreach (clsBlockList bl in AutoNavList)
                    this.trvActionPlan.Nodes.Add(LoadChildNode(bl));

                // expand all nodes
                this.trvActionPlan.ExpandAll();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.LoadActionPlan);
            }            

            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Loads a node into the treeview
        /// </summary>
        /// <param name="bl">the item to load</param>
        private TreeNode LoadChildNode(clsBlockList bl)
        {
            TreeNode node = null;

            try
            {
                // create a new node
                node = new TreeNode(bl.BlockItem.NodeType.ToString());

                // set it's tag
                node.Tag = bl.BlockItem;

                // pop any kids it might have
                foreach (clsBlockList subBL in bl.SubBlocks)
                    node.Nodes.Add(LoadChildNode(subBL));
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.LoadChildNode);
            }

            return node;
        }

        // Load
        #endregion

        #region New

        /// <summary>
        /// Create new plan
        /// </summary>
        private void cmdNewPlan_Click(object sender, EventArgs e)
        {
            // clear the list
            this.trvActionPlan.Nodes.Clear();
        }

        // New
        #endregion

        /// <summary>
        /// Browse for the path file
        /// </summary>
        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            Xml xml;

            try
            {
                this.lblMessage.Text = string.Empty;

                // exit if nothing selected
                if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;

                // get filename
                string filename = this.openFileDialog1.FileName;

                // set the text
                this.txtPathFile.Text = filename;

                // clear the list
                this.cmbPathName.Items.Clear();

                // load the paths
                xml = new Xml(filename);
                string[] sections;
                using (xml.Buffer(true))
                {
                    sections = xml.GetSectionNames();
                }

                // pop the combo
                foreach (string section in sections)
                    this.cmbPathName.Items.Add(section);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.BrowseforRhabotPaths);
            }
            finally
            {
                xml = null;
            }
        }

        /// <summary>
        /// Add from the action list to the plan list
        /// </summary>
        private void cmdAdd_Click(object sender, EventArgs e)
        {
            this.lblMessage.Text = string.Empty;
            AddNewItem();
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblMessage.Text = string.Empty;
                TreeNode node = this.trvActionPlan.SelectedNode;

                // exit if nothing selected
                if (node == null)
                    return;

                // remove from parent if a child
                if (node.Parent != null)
                    node.Parent.Nodes.Remove(node);
                else
                    this.trvActionPlan.Nodes.Remove(node);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.RemoveActionItem);
            }
        }

        /// <summary>
        /// user selected a node, we need to color the top list
        /// </summary>
        private void trvActionPlan_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                this.lblMessage.Text = string.Empty;
                // exit if nothing selected
                if (e.Node == null)
                {
                    ClearControls();
                    EnableZoneControls(false);
                    ShowNPC(false);
                    ShowFlying(false);
                    ShowLocation(false);
                    ShowPathFile(false);
                    ShowQuantity(false);
                    ShowZone(false);

                    return;
                }

                clsBlockListItem bli = (clsBlockListItem)e.Node.Tag;
                clsBlockListItem.ENodeType nodeType = bli.NodeType;

                // clear the text controls
                ClearControls();

                // enable the save button
                this.cmdSaveItem.Enabled = true;

                // if not a parent node, get the parent node
                TreeNode node = e.Node;
                if (e.Node.Parent != null)
                    node = e.Node.Parent;

                // loop through the top list and color each item
                foreach (ListViewItem lvi in this.lstvItems.Items)
                    ColorItem(node, lvi);

                // enable controls and pop the npc list
                EnableZoneControls(true);
                //PopNPCList(nodeType);
                this.lblQuantity.Text = Resources.Quantity;
                this.lblPathFile.Text = Resources.PathFile;
                ShowPathFile(false);
                ShowFlying(false);

                #region Show/Hide controls according to selected action

                switch (nodeType)
                {
                    case clsBlockListItem.ENodeType.Go_To_Vendor_X_City_Y:
                    case clsBlockListItem.ENodeType.Go_To_Flightmaster_X_City_Y:
                    case clsBlockListItem.ENodeType.Go_To_Trainer_X_City_Y:
                    case clsBlockListItem.ENodeType.Go_To_Innkeeper_X_City_Y:
                    case clsBlockListItem.ENodeType.Go_To_Repair_Vendor_X_City_Y:
                    case clsBlockListItem.ENodeType.Speak_To_Person_X_At_Y:
                    case clsBlockListItem.ENodeType.Go_To_QuestGiver_X_At_Y:
                    case clsBlockListItem.ENodeType.Fish_At_XYZ:
                        ShowZone(true);
                        ShowNPC(true);
                        ShowQuantity(false);
                        ShowLocation(true);
                        ShowFlying(true);
                        break;

                    case clsBlockListItem.ENodeType.Take_Boat_To_City_X:
                        ShowZone(true);
                        ShowNPC(true);
                        ShowLocation(true);
                        break;
                    case clsBlockListItem.ENodeType.Run_Rhabot_Path:
                        // show rhabot path options
                        ShowZone(true);
                        ShowFlying(true);
                        ShowPathFile(true);
                        break;
                    case clsBlockListItem.ENodeType.Go_To_Mailbox_X_City_Y:
                        ShowZone(true);
                        ShowFlying(true);
                        break;

                    case clsBlockListItem.ENodeType.Stone_Home:
                    case clsBlockListItem.ENodeType.Make_Inn_Home: 
                        ShowZone(false);
                        ShowNPC(false);
                        ShowQuantity(false);
                        ShowLocation(false);
                        break;

                    case clsBlockListItem.ENodeType.Fish_Until_X_Time:
                    case clsBlockListItem.ENodeType.Fish_Capture_X_of_Y: 
                        ShowZone(false);
                        ShowNPC(false);
                        ShowQuantity(true);
                        ShowLocation(false);

                        if (nodeType == clsBlockListItem.ENodeType.Fish_Until_X_Time)
                            this.lblQuantity.Text = Resources.Timeminutes;
                        else if (nodeType == clsBlockListItem.ENodeType.Fish_Capture_X_of_Y)
                            ShowNPC(true);

                        break;

                    case clsBlockListItem.ENodeType.NEW_BLOCK:
                        ShowZone(true);
                        ShowNPC(false);
                        ShowQuantity(false);
                        ShowLocation(false);
                        break;

                    case clsBlockListItem.ENodeType.Purchase_X_of_Y:
                        ShowZone(false);
                        ShowNPC(false);
                        ShowQuantity(true);
                        ShowLocation(false);
                        ShowPathFileOnly(true);
                        this.lblPathFile.Text = Resources.PurchaseItem;
                        break;

                    case clsBlockListItem.ENodeType.Take_Flight_X_To_Y:
                        ShowZone(false);
                        ShowNPC(true);
                        ShowQuantity(false);
                        ShowLocation(false);
                        break;

                    case clsBlockListItem.ENodeType.Pick_Up_Quest_X:
                    case clsBlockListItem.ENodeType.Turn_In_Quest_Y: 
                        ShowZone(false);
                        ShowNPC(false);
                        ShowQuantity(false);
                        ShowLocation(false);
                        ShowPathFileOnly(true);
                        this.lblPathFile.Text = Resources.QuestName;                        
                        break;

                    case clsBlockListItem.ENodeType.Fight_Mobs_Zone_X_Named_Y:
                    case clsBlockListItem.ENodeType.Fight_Elite_Mobs_Zone_X_Named_Y:
                        ShowZone(true);
                        ShowNPC(true);
                        ShowQuantity(false);
                        ShowLocation(false);
                        ShowFlying(true);
                        break;

                    case clsBlockListItem.ENodeType.Fight_Mobs_Zone_X:
                    case clsBlockListItem.ENodeType.Fight_Elite_Mobs_Zone_X:
                    case clsBlockListItem.ENodeType.Find_Herbs_Zone_X:
                    case clsBlockListItem.ENodeType.Find_Mines_Zone_X:
                    case clsBlockListItem.ENodeType.Find_Chests_Zone_X:
                        ShowZone(true);
                        ShowNPC(false);
                        ShowQuantity(false);
                        ShowLocation(false);
                        ShowFlying(true);
                        
                        break;
                    case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y:
                    case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y_MULTI:
                        ShowZone(true);
                        ShowNPC(false);
                        ShowQuantity(false);
                        ShowLocation(false);
                        break;

                    case clsBlockListItem.ENodeType.Use_Item_X:
                        ShowZone(false);
                        ShowNPC(false);
                        ShowQuantity(false);
                        ShowLocation(false);
                        ShowPathFileOnly(true);
                        this.lblPathFile.Text = Resources.ItemName;
                        break;

                    case clsBlockListItem.ENodeType.Go_To_XYZ:
                        ShowZone(true);
                        ShowNPC(false);
                        ShowQuantity(false);
                        ShowLocation(true);
                        ShowFlying(true);
                        break;

                    case clsBlockListItem.ENodeType.Choose_Gossip_Option_X:
                        ShowZone(false);
                        ShowNPC(false);
                        ShowQuantity(true);
                        ShowLocation(false);
                        this.lblQuantity.Text = Resources.GossipNum;
                        break;

                    case clsBlockListItem.ENodeType.Return_To_Vendor_When_X_Has_Quantity_Y:
                        ShowZone(true);
                        ShowNPC(true);
                        ShowQuantity(false);
                        ShowLocation(true);
                        ShowFlying(true);
                        break;

                    case clsBlockListItem.ENodeType.Continue_Until_Bags_Full:
                        ShowZone(false);
                        ShowNPC(false);
                        ShowQuantity(false);
                        ShowLocation(false);
                        break;

                    case clsBlockListItem.ENodeType.Continue_Durability_X_Percent:
                        ShowZone(false);
                        ShowNPC(false);
                        ShowQuantity(true);
                        ShowLocation(false);
                        this.lblQuantity.Text = Resources.DurabilityPct;
                        break;

                    case clsBlockListItem.ENodeType.Continue_Until_X_Item_Y_Quantity:
                        ShowZone(false);
                        ShowNPC(true);
                        ShowQuantity(true);
                        ShowLocation(false);
                        ShowPathFileOnly(true);
                        this.lblPathFile.Text = Resources.ItemName;
                        break;

                    case clsBlockListItem.ENodeType.Continue_Until_X_Mob_Y_Killed:
                        ShowZone(false);
                        ShowNPC(true);
                        ShowQuantity(true);
                        ShowLocation(false);
                        break;

                    case clsBlockListItem.ENodeType.Continue_Until_Character_Is_Level_X:
                        ShowZone(false);
                        ShowNPC(false);
                        ShowQuantity(true);
                        ShowLocation(false);
                        this.lblQuantity.Text = Resources.Level;
                        break;
                }

                // Show/Hide controls according to selected action
                #endregion

                // pop controls
                EnableZoneControls(false);
                if (!string.IsNullOrEmpty(bli.ZoneName))
                    this.cmbZoneName.SelectedItem = bli.ZoneName;
                else
                {
                    // get the parent if it has one
                    if ((e.Node.Parent != null) && (e.Node.Parent.Tag != null))
                    {
                        string pZone = ((clsBlockListItem)e.Node.Parent.Tag).ZoneName;
                        if (!string.IsNullOrEmpty(pZone))
                            this.cmbZoneName.SelectedItem = pZone;
                    }
                }
                if (! string.IsNullOrEmpty(bli.DestName))
                    this.cmbNPCName.SelectedItem = bli.DestName;
                this.txtQuantity.Value = bli.Quantity;
                if (bli.DestinationPoint != null)
                {
                    this.txtX.Text = bli.DestinationPoint.X.ToString();
                    this.txtY.Text = bli.DestinationPoint.Y.ToString();
                    this.txtZ.Text = bli.DestinationPoint.Z.ToString();
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.ActionItemSelect);
            }
        }

        private void ClearControls()
        {
            this.lblMessage.Text = string.Empty;
            this.cmbNPCName.Items.Clear();
            this.txtQuantity.Value = 0;
            this.txtPathFile.Text = string.Empty;
            this.txtX.Text = string.Empty;
            this.txtY.Text = string.Empty;
            this.txtZ.Text = string.Empty;
            this.cmbPathName.Items.Clear();
        }

        /// <summary>
        /// save these settings back into the selected node's tag
        /// </summary>
        private void cmdSaveItem_Click(object sender, EventArgs e)
        {
            TreeNode node;
            clsBlockListItem bli;

            try
            {
                this.lblMessage.Text = string.Empty;
                // exit if nothing selected
                node = this.trvActionPlan.SelectedNode;
                if (node == null)
                    return;

                // get the tag
                bli = (clsBlockListItem)node.Tag;

                // update bli
                bli.ZoneName = this.cmbZoneName.Text;
                bli.Quantity = (int)this.txtQuantity.Value;
                bli.PathCanFly = this.chkCanFly.Checked;

                if ((clsSettings.IsNumeric(this.txtX.Text)) &&
                   (clsSettings.IsNumeric(this.txtY.Text)) &&
                   (clsSettings.IsNumeric(this.txtZ.Text)))
                {
                    bli.DestinationPoint = new clsPathPoint(
                        Convert.ToDouble(this.txtX.Text),
                        Convert.ToDouble(this.txtY.Text),
                        Convert.ToDouble(this.txtZ.Text));
                }
                
                // get the destination name
                if (this.cmbNPCName.Visible)
                    bli.DestName = this.cmbNPCName.Text;

                else if (bli.NodeType == clsBlockListItem.ENodeType.Run_Rhabot_Path)
                {
                    if ((!string.IsNullOrEmpty(this.txtPathFile.Text)) && (!string.IsNullOrEmpty(this.cmbPathName.Text)))
                        bli.DestName = string.Format("{0}:::{1}", this.txtPathFile.Text, this.cmbPathName.Text);
                }

                else if (this.txtPathFile.Visible)
                    bli.DestName = this.txtPathFile.Text;

                // save back to the node tag
                node.Tag = bli;

                // clear controls
                this.trvActionPlan.SelectedNode = null;
                ClearControls();
                EnableZoneControls(false);
                ShowNPC(false);
                ShowFlying(false);
                ShowLocation(false);
                ShowPathFile(false);
                ShowQuantity(false);
                ShowZone(false);
                PopTopList();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.SaveActionItemOptions);
            }            
        }

        // Button Clicks
        #endregion

        #region Functions

        /// <summary>
        /// Returns the zone of the node's parent (or this node if no parent)
        /// </summary>
        /// <param name="treeNode"></param>
        private string GetParentZone(TreeNode treeNode)
        {
            TreeNode pNode = null;

            try
            {
                this.lblMessage.Text = string.Empty;
                // get the node's parent
                if (treeNode.Parent == null)
                    pNode = treeNode;
                else
                    pNode = treeNode.Parent;

                // get the parent's tag, if none, then exit
                if (pNode.Tag == null)
                    return string.Empty;

                // return the zone
                return ((clsBlockListItem)pNode.Tag).ZoneName;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.GetParentZone);
            }            
            return string.Empty;
        }

        /// <summary>
        /// Color the listview item
        /// </summary>
        private void ColorItem(TreeNode node, ListViewItem lvi)
        {
            clsBlockListItem.ENodeType lviTag = (clsBlockListItem.ENodeType)lvi.Tag;
            clsBlockListItem.ENodeType NodeTag = ((clsBlockListItem)node.Tag).NodeType;
            Color fColor = ColorNotAllow;

            try
            {
                this.lblMessage.Text = string.Empty;
                // for top, always allow
                if (lvi.Group == GrpTop)
                {
                    lvi.ForeColor = ColorAllow;
                    return;
                }

                switch (NodeTag)
                {
                    case clsBlockListItem.ENodeType.NEW_BLOCK:
                    case clsBlockListItem.ENodeType.NONE:
                    case clsBlockListItem.ENodeType.Go_To_XYZ:
                    case clsBlockListItem.ENodeType.Run_Rhabot_Path:
                        fColor = ColorAllow;
                        break;
                    case clsBlockListItem.ENodeType.Go_To_Vendor_X_City_Y:
                    case clsBlockListItem.ENodeType.Go_To_Repair_Vendor_X_City_Y:
                    case clsBlockListItem.ENodeType.Return_To_Vendor_When_X_Has_Quantity_Y:
                        if (lviTag == clsBlockListItem.ENodeType.Purchase_X_of_Y)
                            fColor = ColorAllow;
                        break;
                    case clsBlockListItem.ENodeType.Go_To_Flightmaster_X_City_Y:
                        if (lviTag == clsBlockListItem.ENodeType.Take_Flight_X_To_Y)
                            fColor = ColorAllow;
                        break;
                    case clsBlockListItem.ENodeType.Go_To_Innkeeper_X_City_Y:
                        if (lviTag == clsBlockListItem.ENodeType.Make_Inn_Home)
                            fColor = ColorAllow;
                        break;
                    case clsBlockListItem.ENodeType.Speak_To_Person_X_At_Y:
                        if (lviTag == clsBlockListItem.ENodeType.Choose_Gossip_Option_X)
                            fColor = ColorAllow;
                        break;
                    case clsBlockListItem.ENodeType.Go_To_QuestGiver_X_At_Y:
                        switch (lviTag)
                        {
                            case clsBlockListItem.ENodeType.Pick_Up_Quest_X:
                            case clsBlockListItem.ENodeType.Turn_In_Quest_Y:
                            case clsBlockListItem.ENodeType.Choose_Gossip_Option_X:
                                fColor = ColorAllow;
                                break;
                        }
                        break;
                    case clsBlockListItem.ENodeType.Fish_At_XYZ:
                        if ((lviTag == clsBlockListItem.ENodeType.Fish_Capture_X_of_Y) || 
                            (lviTag == clsBlockListItem.ENodeType.Fish_Until_X_Time))
                                fColor = ColorAllow;
                        break;
                    case clsBlockListItem.ENodeType.Fight_Mobs_Zone_X:
                    case clsBlockListItem.ENodeType.Fight_Mobs_Zone_X_Named_Y:
                    case clsBlockListItem.ENodeType.Fight_Elite_Mobs_Zone_X:
                    case clsBlockListItem.ENodeType.Fight_Elite_Mobs_Zone_X_Named_Y:
                    case clsBlockListItem.ENodeType.Find_Herbs_Zone_X:
                    case clsBlockListItem.ENodeType.Find_Mines_Zone_X:
                    case clsBlockListItem.ENodeType.Find_Chests_Zone_X:
                    case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y:
                    case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y_MULTI:
                    case clsBlockListItem.ENodeType.Use_Item_X:
                    case clsBlockListItem.ENodeType.Choose_Gossip_Option_X:
                    case clsBlockListItem.ENodeType.Continue_Until_Bags_Full:
                    case clsBlockListItem.ENodeType.Continue_Durability_X_Percent:
                    case clsBlockListItem.ENodeType.Continue_Until_X_Item_Y_Quantity:
                    case clsBlockListItem.ENodeType.Continue_Until_X_Mob_Y_Killed:
                    case clsBlockListItem.ENodeType.Continue_Until_Character_Is_Level_X:
                        if (lvi.Group == GrpConditions)
                            fColor = ColorAllow;
                        break;
                }

                // change color based on parent's type
                lvi.ForeColor = fColor;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.ColorItem);
            }            
        }

        /// <summary>
        /// Adds a new item to the list
        /// </summary>
        private void AddNewItem()
        {
            TreeNode ParentNode, SelectedNode, InsertNode;

            try
            {
                // exit if nothing selected
                if (this.lstvItems.SelectedIndices.Count == 0)
                    return;

                // get the select list item
                ListViewItem lvi = this.lstvItems.SelectedItems[0];

                // warn if selected item can not be added
                if (lvi.ForeColor == ColorNotAllow)
                {
                    clsError.ShowError(new Exception(Resources.Youcannotaddthisactionitem), string.Empty, false, new StackFrame(0, true), false);
                    return;
                }

                // build the insert node
                InsertNode = new TreeNode(lvi.Text);
                clsBlockListItem bli = new clsBlockListItem();
                bli.NodeType = (clsBlockListItem.ENodeType)lvi.Tag;
                InsertNode.Tag = bli;

                // if item is a top or condition, then add it after the selected parent node
                SelectedNode = this.trvActionPlan.SelectedNode;
                if (SelectedNode == null)
                {
                    this.trvActionPlan.Nodes.Add(InsertNode);
                    return;
                }

                // get parent node
                ParentNode = SelectedNode.Parent;

                // if no parent and top item, add new top item
                if ((ParentNode == null) && (lvi.Group == GrpTop))
                {
                    // create new top node
                    this.trvActionPlan.Nodes.Insert(this.trvActionPlan.Nodes.IndexOf(SelectedNode) + 1, InsertNode);
                    return;
                }

                // if no parent, and sub item, add it to the selected node
                if ((ParentNode == null) && (lvi.Group != GrpTop))
                {
                    // create new sub node
                    SelectedNode.Nodes.Add(InsertNode);
                    return;
                }

                // if parent, and condition or sub, add it
                if ((ParentNode != null) && ((lvi.Group == GrpSub) || (lvi.Group == GrpConditions)))
                {
                    // create new sub node
                    InsertNode = new TreeNode(lvi.Text);
                    InsertNode.Tag = lvi.Tag;
                    ParentNode.Nodes.Add(InsertNode);
                    return;
                }

                // if parent, and top, add it
                if ((ParentNode != null) && (lvi.Group == GrpTop))
                {
                    // create new top node
                    this.trvActionPlan.Nodes.Insert(this.trvActionPlan.Nodes.IndexOf(ParentNode) + 1, InsertNode);
                    return;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AddActionItem);
            }

            finally
            {
                this.trvActionPlan.ExpandAll();
            }
        }

        private void lstvItems_DoubleClick(object sender, EventArgs e)
        {
            this.lblMessage.Text = string.Empty;
            AddNewItem();
        }

        private clsBlockListItem GetItemFromXML(string section, string entry, Xml xml, clsBlockListItem.ENodeType NodeType)
        {
            // TODO: get item
            clsBlockListItem bli = new clsBlockListItem();

            /*
            bli.DestinationPoint = new clsPathPoint(
                Convert.ToDouble(xml.GetValue_Attribute(section, entry, clsProcessExplore.attr_X)),
                Convert.ToDouble(xml.GetValue_Attribute(section, entry, clsProcessExplore.attr_Y)),
                Convert.ToDouble(xml.GetValue_Attribute(section, entry, clsProcessExplore.attr_Z)));
            bli.DestName = clsSettings.XMLGet_String(section, entry, xml);
            bli.NodeType = NodeType;
            bli.ZoneName = section;
            bli.Quantity = 0;
            */

            return bli;
       }

        // Functions
        #endregion

        #region Show/Hide Controls

        private void ShowNPC(bool Show)
        {
            this.lblNPCName.Visible = Show;
            this.cmbNPCName.Visible = Show;
            this.cmbNPCName.SelectedIndex = -1;
        }

        private void ShowQuantity(bool Show)
        {
            this.lblQuantity.Visible = Show;
            this.txtQuantity.Visible = Show;
            this.txtQuantity.Value = 0;
        }

        private void ShowLocation(bool Show)
        {
            this.lblX.Visible = Show;
            this.lblY.Visible = Show;
            this.lblZ.Visible = Show;
            this.txtX.Visible = Show;
            this.txtY.Visible = Show;
            this.txtZ.Visible = Show;

            this.txtX.Text = string.Empty;
            this.txtY.Text = string.Empty;
            this.txtZ.Text = string.Empty;
        }

        private void ShowZone(bool Show)
        {
            this.lblZoneName.Visible = Show;
            this.cmbZoneName.Visible = Show;
            this.cmbZoneName.SelectedIndex = -1;
        }

        /// <summary>
        /// Pop location if an NPC is selected
        /// </summary>
        private void cmbNPCName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // skip if nothing selected
            int index = this.cmbNPCName.SelectedIndex;
            if (index < 0)
                return;

            // skip if no location
            clsBlockListItem bli = (clsBlockListItem)this.cmbNPCName.SelectedItem;
            if (bli.DestinationPoint == null)
                return;

            // pop location
            this.txtX.Text = bli.DestinationPoint.X.ToString().Trim();
            this.txtY.Text = bli.DestinationPoint.Y.ToString().Trim();
            this.txtZ.Text = bli.DestinationPoint.Z.ToString().Trim();
        }

        /// <summary>
        /// Enable controls and pop npc list
        /// </summary>
        private void cmbZoneName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // enable zone controls
                if (this.cmbZoneName.SelectedIndex > -1)
                {
                    EnableZoneControls(true);

                    // pop npc list
                    clsBlockListItem bli = (clsBlockListItem)this.trvActionPlan.SelectedNode.Tag;
                    PopNPCList(bli.NodeType);
                    int j = this.cmbNPCName.Items.Count;
                    
                    // select npc if one found
                    if (!string.IsNullOrEmpty(bli.DestName))
                    {
                        // loop through the npc combo until we find the correct dest name
                        int i;
                        for (i = 0; i < j; i++)
                        {
                            // skip if not found
                            if (((clsBlockListItem)this.cmbNPCName.Items[i]).DestName != bli.DestName)
                                continue;

                            // select this item
                            this.cmbNPCName.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.SelectingZone);
            }
        }

        /// <summary>
        /// Enables/Disables controls after a zone has been selected
        /// </summary>
        private void EnableZoneControls(bool Enable)
        {
//            this.cmbNPCName.Enabled = Enable;
//            this.txtQuantity.Enabled = Enable;
//            this.txtX.Enabled = Enable;
//            this.txtY.Enabled = Enable;
//            this.txtZ.Enabled = Enable;
        }

        /// <summary>
        /// Shows/Hides the path file information
        /// </summary>
        private void ShowPathFile(bool Show)
        {
            this.lblPathFile.Visible = Show;
            this.lblPathName.Visible = Show;
            this.txtPathFile.Visible = Show;
            this.cmbPathName.Visible = Show;
            this.cmdBrowse.Visible = Show;
            this.txtPathFile.ReadOnly = true;
        }

        /// <summary>
        /// Shows/Hides the flying mount checkbox
        /// </summary>
        private void ShowFlying(bool Show)
        {
            this.chkCanFly.Visible = Show;
        }

        /// <summary>
        /// Shows the path file label and textbox
        /// </summary>
        private void ShowPathFileOnly(bool Show)
        {
            this.lblPathFile.Visible = Show;
            this.txtPathFile.Visible = Show;
            if (Show)
                this.txtPathFile.ReadOnly = false;
        }

        // Show/Hide Controls
        #endregion

        #region PopList

        /// <summary>
        /// Pops the Zone combo box
        /// </summary>
        private void PopZoneList()
        {
            Xml xml;

            try
            {
                // exit if the file doesn't exist
                if (!File.Exists(clsSettings.ExploreReadPath))
                    return;

                // open the file
                xml = new Xml(clsSettings.ExploreReadPath);
                string[] sections;
                using (xml.Buffer(true))
                {
                    // get the list of sections
                    sections = xml.GetSectionNames();
                }

                // pop the list
                this.cmbZoneName.Items.Clear();
                foreach (string zone in sections)
                    this.cmbZoneName.Items.Add(zone.Trim());

                // sort the list
                this.cmbZoneName.Sorted = true;
            }
            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PopZoneList);
            }            
            finally
            {
                xml = null;
            }
        }

        /// <summary>
        /// Populates the NPC combo box
        /// </summary>
        /// <param name="NodeType">action type</param>
        private void PopNPCList(clsBlockListItem.ENodeType NodeType)
        {
            List<clsUnitInfo.EUnitType> TypeList = new List<clsUnitInfo.EUnitType>();
            List<string> NameList = new List<string>();
            string labelName = Resources.NPCName;
            Xml xml;
            clsBlockListItem bli;

            try
            {
                // clear the list
                this.cmbNPCName.Items.Clear();

                // exit if no zone selected
                if (this.cmbZoneName.SelectedIndex < 0)
                    return;

                // get the zone name
                string section = this.cmbZoneName.Text.Trim();

                // get the NPC Type
                switch (NodeType)
                {
                    case clsBlockListItem.ENodeType.Go_To_Vendor_X_City_Y:
                    case clsBlockListItem.ENodeType.Return_To_Vendor_When_X_Has_Quantity_Y:
                        TypeList.Add(clsUnitInfo.EUnitType.Vendor);
                        TypeList.Add(clsUnitInfo.EUnitType.RepairVendor);
                        break;
                    case clsBlockListItem.ENodeType.Go_To_Flightmaster_X_City_Y:
                        TypeList.Add(clsUnitInfo.EUnitType.Flightmaster);
                        break;
                    case clsBlockListItem.ENodeType.Go_To_Trainer_X_City_Y:
                        TypeList.Add(clsUnitInfo.EUnitType.Trainer);
                        break;
                    case clsBlockListItem.ENodeType.Go_To_Innkeeper_X_City_Y:
                        TypeList.Add(clsUnitInfo.EUnitType.Innkeeper);
                        break;
                    case clsBlockListItem.ENodeType.Go_To_Repair_Vendor_X_City_Y:
                        TypeList.Add(clsUnitInfo.EUnitType.RepairVendor);
                        break;
                    case clsBlockListItem.ENodeType.Go_To_QuestGiver_X_At_Y:
                    case clsBlockListItem.ENodeType.Speak_To_Person_X_At_Y:
                        TypeList.Add(clsUnitInfo.EUnitType.NPC);
                        TypeList.Add(clsUnitInfo.EUnitType.RepairVendor);
                        TypeList.Add(clsUnitInfo.EUnitType.Vendor);
                        TypeList.Add(clsUnitInfo.EUnitType.Innkeeper);
                        break;
                    case clsBlockListItem.ENodeType.Fish_At_XYZ:
                    case clsBlockListItem.ENodeType.Fish_Capture_X_of_Y:
                        TypeList.Add(clsUnitInfo.EUnitType.Other);
                        labelName = Resources.FishSchools;
                        break;
                    case clsBlockListItem.ENodeType.Go_To_Boat_X_City_Y:
                        // we need to pop the list as a vendor list, then exit
                        PopNPC_As_Boat();
                        return;
                    case clsBlockListItem.ENodeType.Fight_Mobs_Zone_X_Named_Y:
                    case clsBlockListItem.ENodeType.Fight_Elite_Mobs_Zone_X_Named_Y:
                        TypeList.Add(clsUnitInfo.EUnitType.Mob);
                        TypeList.Add(clsUnitInfo.EUnitType.EliteMob);
                        labelName = Resources.MobName;
                        break;
                    case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y:
                    case clsBlockListItem.ENodeType.Pickup_Item_Named_X_Zone_Y_MULTI:
                    case clsBlockListItem.ENodeType.Continue_Until_X_Mob_Y_Killed:
                        TypeList.Add(clsUnitInfo.EUnitType.Other);
                        TypeList.Add(clsUnitInfo.EUnitType.Other);
                        labelName = Resources.OtherItems;
                        break;
                    default:
                        return;
                }

                this.lblNPCName.Text = labelName;

                // open the read file
                xml = new Xml(clsSettings.ExploreReadPath);
                using (xml.Buffer(true))
                {
                    // get the list of entries for this zone
                    string[] entries = xml.GetEntryNames(section);

                    // loop through and pop entries
                    foreach (string entry in entries)
                    {
                        // get the unit type
                        // TODO: fix this
                        //unitType = xml.GetValue_Attribute(section, entry, clsProcessExplore.attr_Type);
                        string unitType = string.Empty;

                        // skip if no type
                        if ((string.IsNullOrEmpty(unitType)) || (unitType == clsUnitInfo.EUnitType.NONE.ToString()))
                            continue;

                        // if a quest giver, skip if no quest
                        // TODO: fix this
                        /*
                        if ((NodeType == clsBlockListItem.ENodeType.Go_To_QuestGiver_X_At_Y) &&
                            (!XML_GetBoolAttribute(xml.GetValue_Attribute(section, entry, clsProcessExplore.attr_Quest), false)))
                                continue;
                        */

                        // if fishing and not a school of fish, exit
                        if (((NodeType == clsBlockListItem.ENodeType.Fish_At_XYZ) || 
                            (NodeType == clsBlockListItem.ENodeType.Fish_Capture_X_of_Y)) &&
                                (!entry.ToLower().Contains(Resources.school)))
                                    continue;

                        // skip if the unit type does not exist in the list
                        if (!TypeList.Contains((clsUnitInfo.EUnitType)Enum.Parse(typeof(clsUnitInfo.EUnitType), unitType)))
                            continue;

                        // we have something, create a new block list item
                        bli = GetItemFromXML(section, entry, xml, NodeType);
                        
                        // add it to the list if it's not already there
                        if (!NameList.Contains(bli.DestName))
                        {
                            this.cmbNPCName.Items.Add(bli);
                            NameList.Add(bli.DestName);
                        }
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PopNPCList);
            }
            finally
            {
                xml = null;
            }
        }

        /// <summary>
        /// Pops the NPC list with the list of zones
        /// </summary>
        private void PopNPC_As_Boat()
        {
            try
            {
                // clear the npc list
                this.cmbNPCName.Items.Clear();

                this.lblNPCName.Text = Resources.Destination;
                // TODO: look in runningman to pop npc combo with destination cities
                // TODO: the zone will be the start city, so we may need to decide
                //  which dock to be on, for zones with more than one boat (like Auberdine)
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.PopNPCAsBoat);
            }
        }

        private bool XML_GetBoolAttribute(string value, bool DefaultVal)
        {
            if (string.IsNullOrEmpty(value))
                return DefaultVal;

            return Convert.ToBoolean(value);
        }

        // PopList
        #endregion
    }
}
