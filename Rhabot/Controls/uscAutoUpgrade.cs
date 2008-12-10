using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Items;
using ISXBotHelper.Properties;
using ISXWoW;

namespace Rhabot
{
    public partial class uscAutoUpgrade : UserControl
    {
        #region Init

        public uscAutoUpgrade()
        {
            InitializeComponent();
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;
        }

        private void uscAutoUpgrade_Load(object sender, EventArgs e)
        {
            // load the settings
            LoadSettings();
        }

        // Init
        #endregion

        #region Load

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                // pop the combo's first

                // material
                this.clmnMaterial.Items.Clear();
                this.clmnMaterial.Items.Add(ISXRhabotGlobal.clsGlobals.EEquipItemMaterialType.None.ToString());
                this.clmnMaterial.Items.Add(ISXRhabotGlobal.clsGlobals.EEquipItemMaterialType.Cloth.ToString());
                this.clmnMaterial.Items.Add(ISXRhabotGlobal.clsGlobals.EEquipItemMaterialType.Leather.ToString());
                this.clmnMaterial.Items.Add(ISXRhabotGlobal.clsGlobals.EEquipItemMaterialType.Mail.ToString());
                this.clmnMaterial.Items.Add(ISXRhabotGlobal.clsGlobals.EEquipItemMaterialType.Plate.ToString());

                // stats
                this.clmnStat.Items.Clear();
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.None.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.Armor.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.AttackPower.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.DPS.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.Agility.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.Intellect.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.Spirit.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.Stamina.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.Strength.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.ArcaneResist.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.FireResist.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.FrostResist.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.HolyResist.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.NatureResist.ToString());
                this.clmnStat.Items.Add(ISXRhabotGlobal.clsGlobals.ENeedEquipStat.ShadowResist.ToString());

                // create rows
                this.gridEquip.Rows.Clear();
                this.gridEquip.Rows.Add(WoWEquipSlot.Head.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Neck.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Shoulders.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Back.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Chest.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Wrists.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Hands.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Waist.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Legs.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Feet.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Finger1.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Finger2.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Trinket1.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Trinket2.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.MainHand.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.OffHand.ToString(), Resources.None, Resources.None);
                this.gridEquip.Rows.Add(WoWEquipSlot.Ranged.ToString(), Resources.None, Resources.None);

                // load settings - just select the correct combo stuff
                if ((clsSettings.gclsGlobalSettings.AutoEquipList != null) && (clsSettings.gclsGlobalSettings.AutoEquipList.Count > 0))
                {
                    int rows = this.gridEquip.RowCount;
                    foreach (clsAutoEquipItem item in clsSettings.gclsGlobalSettings.AutoEquipList)
                    {
                        for (int i = 0; i < rows; i++)
                        {
                            // skip if not the correct slot
                            if (this.gridEquip[0, i].Value.ToString() != item.EquipSlot.ToString())
                                continue;

                            // found the slot, update the dropdown's
                            this.gridEquip[1, i].Value = item.MaterialType.ToString();
                            this.gridEquip[2, i].Value = item.EquipStat.ToString();
                            this.gridEquip.UpdateCellValue(1, i);
                            this.gridEquip.UpdateCellValue(2, i);
                            break;
                        }
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoEquip, Resources.LoadAutoEquipError);
            }
        }

        // Load
        #endregion

        #region Save

        private void cmdSave_Click(object sender, EventArgs e)
        {
            int j = this.uscStartEndLevel1.EndLevel + 1, rows = this.gridEquip.RowCount;

            try
            {
                // set wait cursor
                this.Cursor = Cursors.WaitCursor;

                // clear the existing list
                clsSettings.gclsGlobalSettings.AutoEquipList = new List<clsAutoEquipItem>();

                // loop through and add each item
                int i;
                for (i = 0; i < rows; i++)
                {
                    // if weapons, rings, etc, not save material type
                    if ((i == 1) || (i >= 10))
                    {
                        clsSettings.gclsGlobalSettings.AutoEquipList.Add(new clsAutoEquipItem(
                            this.gridEquip[0, i].Value.ToString().Trim(),
                            Resources.None,
                            this.gridEquip[2, i].Value.ToString().Trim()));
                    }

                    else
                    {
                        clsSettings.gclsGlobalSettings.AutoEquipList.Add(new clsAutoEquipItem(
                            this.gridEquip[0, i].Value.ToString().Trim(),
                            this.gridEquip[1, i].Value.ToString().Trim(),
                            this.gridEquip[2, i].Value.ToString().Trim()));
                    }
                }

                // save the file
                for (i = this.uscStartEndLevel1.StartLevel; i < j; i++)
                    clsSettings.SaveGlobalSettings();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.AutoEquip, Resources.SaveAutoEquipError);
            }            
            finally
            {
                // set cursor
                this.Cursor = Cursors.Default;
            }
        }

        // Save
        #endregion
    }
}
