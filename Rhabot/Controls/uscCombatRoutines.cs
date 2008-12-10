using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXWoW;
using ISXBotHelper.Settings.Settings;

namespace Rhabot.Controls
{
    public partial class uscCombatRoutines : UserControl
    {
        #region Init

        public uscCombatRoutines()
        {
            InitializeComponent();
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void uscCombatRoutines_Load(object sender, EventArgs e)
        {
            // load settings
            LoadSettings();
        }

        // Init
        #endregion

        #region Load / Save

        /// <summary>
        /// Load the settings
        /// </summary>
        private void LoadSettings()
        {
            GroupBox grpB = null;

            try
            {
                using (new clsFrameLock.LockBuffer())
                {
                    if (clsSettings.isxwow.Me.Class != WoWClass.NULL)
                        this.lblClass.Text = clsSettings.isxwow.Me.Class.ToString();
                    else
                        this.lblClass.Text = Resources.NoClassNull;

                    // get the proper group box
                    switch (clsSettings.isxwow.Me.Class)
                    {
                        case WoWClass.Warrior:
                            grpB = this.grpWarrior;

                            // pull type
                            switch (clsSettings.gclsLevelSettings.Combat_ClassSettings.Warrior_PullType)
                            {
                                case clsCombatSettings.EWarriorPullType.Charge:
                                    this.optCharge.Checked = true;
                                    break;
                                case clsCombatSettings.EWarriorPullType.Shoot:
                                    this.optShoot.Checked = true;
                                    break;
                                case clsCombatSettings.EWarriorPullType.Throw:
                                    this.optThrow.Checked = true;
                                    break;
                            }

                            // checkboxes
                            this.chkWarrior_Execute.Checked = clsSettings.gclsLevelSettings.Combat_ClassSettings.Warrior_UseExecute;
                            this.chkWarrior_Overpower.Checked = clsSettings.gclsLevelSettings.Combat_ClassSettings.Warrior_UseOverPower;
                            this.chkWarrior_Rampage.Checked = clsSettings.gclsLevelSettings.Combat_ClassSettings.Warrior_UseRampage;

                            break;
                        case WoWClass.Paladin:
                            grpB = this.grpPaladin;
                            this.chkPaladin_UseAvengeShield.Checked = clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_UseAvengersShield;
                            this.chkPaladin_UseConsecration.Checked = clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_UseConsecration;
                            this.txtPaladin_SealOne.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_SealOne;
                            this.txtPaladin_SealTwo.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Paladin_SealTwo;

                            break;

                        case WoWClass.Hunter:
                            grpB = this.grpHunter;
                            
                            // pop pet happiness
                            this.txtHunterPetHappiness.Value = clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Pet_FeedAtHappinessPercent;

                            // pop the food bag list
                            PopHunterBagList();
                            PopHunterFeedList();

                            // pop listboxes
                            this.txtHunterRangedDOT.Text = clsSettings.BuildListString(clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Ranged_DOT);
                            this.txtHunterRangedSpamSpells.Text = clsSettings.BuildListString(clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Ranged_SpamSpells);

                            break;

                        case WoWClass.Warlock:
                            grpB = this.grpWarlock;
                            if (clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_PullWithPet)
                                this.optWarlockPetPull.Checked = true;
                            else
                                this.optWarlockSpellPull.Checked = true;
                            this.txtWarlockCastWaitTime.Value = clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_PullWaitTime;
                            this.txtWarlockMobHealthDrain.Value = clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_DrainSoulOnHealthPercent;
                            this.txtWarlockSoulshards.Value = clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_SoulShardCount;
                            this.cmbWarlockPet.SelectedIndex = Convert.ToInt32(clsSettings.gclsLevelSettings.Combat_ClassSettings.Warlock_Pet);
                            break;

                        case WoWClass.Druid:
                            grpB = this.grpDruid;
                            switch (clsSettings.gclsLevelSettings.Combat_ClassSettings.Druid_CombatForm)
                            {
                                case ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.Humanoid:
                                    this.optHumanoid.Checked = true;
                                    break;
                                case ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.Bear:
                                    this.optBear.Checked = true;
                                    break;
                                case ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.Cat:
                                    this.optCat.Checked = true;
                                    break;
                                case ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.DireBear:
                                    this.optDireBear.Checked = true;
                                    break;
                                case ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.MoonKin:
                                    this.optMoonkin.Checked = true;
                                    break;
                            }
                            this.chkDruidBearOnAggro.Checked = clsSettings.gclsLevelSettings.Combat_ClassSettings.Druid_ChangeToBearOnAggro;
                            break;
                        case WoWClass.Shaman:
                            grpB = this.grpShaman;

                            this.txtShaman_AirTotem.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_AirTotem;
                            this.txtShaman_EarthTotem.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_EarthTotem;
                            this.txtShaman_FireTotem.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_FireTotem;
                            this.txtShaman_MainWeapBuff.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_MainHandBuff;
                            this.txtShaman_OffWeaponBuff.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_OffhandBuff;
                            this.txtShaman_WaterTotem.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Shaman_WaterTotem;

                            break;

                        case WoWClass.Priest:
                            break;

                        case WoWClass.Rogue:
                            break;
                    }

                    // debuffs
                    this.txtDebuffCurse.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Curse;
                    this.txtDebuffDisease.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Disease;
                    this.txtDebuffPoison.Text = clsSettings.gclsLevelSettings.Combat_ClassSettings.Debuff_Poison;

                    // show the group box
                    if (grpB != null)
                    {
                        grpB.Location = new Point(3, 36);
                        grpB.Visible = true;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatRoutines, Resources.LoadSettings);
            }
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                // set wait cursor
                this.Cursor = Cursors.WaitCursor;

                // save the settings files
                int j = this.uscStartEndLevel1.EndLevel + 1;
                for (int i = this.uscStartEndLevel1.StartLevel; i < j; i++)
                {
                    // load settings for this level
                    clsLevelSettings LevelSettings = clsSettings.LoadPanelSettings(i);

                    using (new clsFrameLock.LockBuffer())
                    {
                        // get the proper group box
                        switch (clsSettings.isxwow.Me.Class)
                        {
                            case WoWClass.Warrior:
                                // pull type
                                if (this.optShoot.Checked)
                                    LevelSettings.Combat_ClassSettings.Warrior_PullType = clsCombatSettings.EWarriorPullType.Shoot;
                                else if (this.optThrow.Checked)
                                    LevelSettings.Combat_ClassSettings.Warrior_PullType = clsCombatSettings.EWarriorPullType.Throw;
                                else
                                    LevelSettings.Combat_ClassSettings.Warrior_PullType = clsCombatSettings.EWarriorPullType.Charge;

                                // checkboxes
                                LevelSettings.Combat_ClassSettings.Warrior_UseExecute = this.chkWarrior_Execute.Checked;
                                LevelSettings.Combat_ClassSettings.Warrior_UseOverPower = this.chkWarrior_Overpower.Checked;
                                LevelSettings.Combat_ClassSettings.Warrior_UseRampage = this.chkWarrior_Rampage.Checked;
                                break;

                            case WoWClass.Paladin:
                                LevelSettings.Combat_ClassSettings.Paladin_SealOne = this.txtPaladin_SealOne.Text;
                                LevelSettings.Combat_ClassSettings.Paladin_SealTwo = this.txtPaladin_SealTwo.Text;
                                LevelSettings.Combat_ClassSettings.Paladin_UseAvengersShield = this.chkPaladin_UseAvengeShield.Checked;
                                LevelSettings.Combat_ClassSettings.Paladin_UseConsecration = this.chkPaladin_UseConsecration.Checked;
                                break;

                            case WoWClass.Hunter:
                                LevelSettings.Combat_ClassSettings.Hunter_Ranged_DOT = clsSettings.BuildSaveList(this.txtHunterRangedDOT);
                                LevelSettings.Combat_ClassSettings.Hunter_Ranged_SpamSpells = clsSettings.BuildSaveList(this.txtHunterRangedSpamSpells);
                                LevelSettings.Combat_ClassSettings.Hunter_Pet_FoodList = SaveHunterFeedList();
                                LevelSettings.Combat_ClassSettings.Hunter_Pet_FeedAtHappinessPercent = (int)this.txtHunterPetHappiness.Value;
                                break;

                            case WoWClass.Warlock:
                                LevelSettings.Combat_ClassSettings.Warlock_PullWithPet = this.optWarlockPetPull.Checked;
                                if (this.txtWarlockCastWaitTime.Visible)
                                    LevelSettings.Combat_ClassSettings.Warlock_PullWaitTime = (int)this.txtWarlockCastWaitTime.Value;
                                else
                                    LevelSettings.Combat_ClassSettings.Warlock_PullWaitTime = 0;
                                LevelSettings.Combat_ClassSettings.Warlock_DrainSoulOnHealthPercent = (int)this.txtWarlockMobHealthDrain.Value;
                                LevelSettings.Combat_ClassSettings.Warlock_SoulShardCount = (int)this.txtWarlockSoulshards.Value;
                                LevelSettings.Combat_ClassSettings.Warlock_Pet = (ISXRhabotGlobal.clsGlobals.ECombatClass_Warlock_Pet)Enum.Parse(typeof(ISXRhabotGlobal.clsGlobals.ECombatClass_Warlock_Pet), this.cmbWarlockPet.Text);

                                break;

                            case WoWClass.Druid:
                                // form
                                if (this.optHumanoid.Checked)
                                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.Humanoid;
                                else if (this.optMoonkin.Checked)
                                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.MoonKin;
                                else if (this.optDireBear.Checked)
                                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.DireBear;
                                else if (this.optCat.Checked)
                                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.Cat;
                                else if (this.optBear.Checked)
                                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = ISXRhabotGlobal.clsGlobals.ECombatClass_Druid_Form.Bear;

                                LevelSettings.Combat_ClassSettings.Druid_ChangeToBearOnAggro = this.chkDruidBearOnAggro.Checked;
                                break;

                            case WoWClass.Shaman:
                                LevelSettings.Combat_ClassSettings.Shaman_AirTotem = this.txtShaman_AirTotem.Text;
                                LevelSettings.Combat_ClassSettings.Shaman_EarthTotem = this.txtShaman_EarthTotem.Text;
                                LevelSettings.Combat_ClassSettings.Shaman_FireTotem = this.txtShaman_FireTotem.Text;
                                LevelSettings.Combat_ClassSettings.Shaman_MainHandBuff = this.txtShaman_MainWeapBuff.Text;
                                LevelSettings.Combat_ClassSettings.Shaman_OffhandBuff = this.txtShaman_OffWeaponBuff.Text;
                                LevelSettings.Combat_ClassSettings.Shaman_WaterTotem = this.txtShaman_WaterTotem.Text;

                                break;

                            case WoWClass.Rogue:
                                break;
                            case WoWClass.Priest:
                                break;
                        }

                        // debufs
                        LevelSettings.Combat_ClassSettings.Debuff_Curse = this.txtDebuffCurse.Text;
                        LevelSettings.Combat_ClassSettings.Debuff_Disease = this.txtDebuffDisease.Text;
                        LevelSettings.Combat_ClassSettings.Debuff_Poison = this.txtDebuffPoison.Text;
                    }

                    // save the settings to the class
                    clsSettings.SavePanelSettings(i, LevelSettings);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatRoutines, Resources.SaveSettings);
            }

            finally
            {
                // set cursor
                this.Cursor = Cursors.Default;
            }
        }

        // Load / Save
        #endregion

        #region Hunter Button Clicks

        private void cmdHunterAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                if (this.lstHunterFoodInBag.SelectedIndex < 0)
                    return;

                // get the item
                string item = this.lstHunterFoodInBag.SelectedItem.ToString();

                // see if it exists already
                foreach (ListViewItem lvi in this.lstHunterFeedWithThis.Items)
                {
                    // exit if already exists
                    if (string.Compare(lvi.Text, item, true) == 0)
                        return;
                }

                // add it
                this.lstHunterFeedWithThis.Items.Add(item);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatRoutines, Resources.AddToFeedList);
            }            
        }

        private void cmdHunterDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // exit if nothing selected
                if (this.lstHunterFeedWithThis.SelectedIndex < 0)
                    return;

                // remove from the list
                this.lstHunterFeedWithThis.Items.RemoveAt(this.lstHunterFeedWithThis.SelectedIndex);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatRoutines, Resources.RemoveFromFeedList);
            }            
        }

        private void cmdHunterRefreshBag_Click(object sender, EventArgs e)
        {
            PopHunterBagList();
        }

        /// <summary>
        /// Pops the hunter bag list with list of edible foods for pet
        /// </summary>
        private void PopHunterBagList()
        {
            try
            {
                // clear the list
                this.lstHunterFoodInBag.Items.Clear();
                this.lstHunterFoodInBag.SuspendLayout();
                this.cmdHunterRefreshBag.Enabled = false;

                using (new clsFrameLock.LockBuffer())
                {
                    // get the list of food items
                    List<WoWItem> FoodList = clsSearch.Search_Item("-items,-inventory,-food");

                    // pop the listbox
                    foreach (WoWItem item in FoodList)
                        this.lstHunterFoodInBag.Items.Add(item.Name);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatRoutines, Resources.PopHunterBagList);
            }            

            finally
            {
                this.lstHunterFoodInBag.ResumeLayout();
                this.cmdHunterRefreshBag.Enabled = true;
            }
        }

        /// <summary>
        /// Pops the list of items to feed pet
        /// </summary>
        private void PopHunterFeedList()
        {
            try
            {
                this.lstHunterFeedWithThis.Items.Clear();
                this.lstHunterFeedWithThis.SuspendLayout();

                // exit if no items in list
                if ((clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Pet_FoodList == null) || (clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Pet_FoodList.Count == 0))
                    return;

                // pop the listbox
                foreach (string item in clsSettings.gclsLevelSettings.Combat_ClassSettings.Hunter_Pet_FoodList)
                    this.lstHunterFeedWithThis.Items.Add(item);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatRoutines, Resources.PopHunterFeedList);
            }            

            finally
            {
                this.lstHunterFeedWithThis.ResumeLayout();
            }
        }

        private List<string> SaveHunterFeedList()
        {
            List<string> retList = new List<string>();

            try
            {
                // if the list is empty, exit
                if (this.lstHunterFeedWithThis.Items.Count == 0)
                    return retList;

                // pop the list
                foreach (ListViewItem item in this.lstHunterFeedWithThis.Items)
                {
                    if (!string.IsNullOrEmpty(item.Text))
                        retList.Add(item.Text);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatRoutines, Resources.SaveHunterFeedList);
            }

            return retList;
        }

        // Hunter Button Clicks
        #endregion

        #region Warlock_Clicks

        private void optWarlockPetPull_CheckedChanged(object sender, EventArgs e)
        {
            this.lblWarlockPullWaitTime.Visible = this.optWarlockPetPull.Checked;
            this.txtWarlockCastWaitTime.Visible = this.optWarlockPetPull.Checked;
        }

        // Warlock_Clicks
        #endregion
    }
}

