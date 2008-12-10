using System;
using System.Collections.Generic;
using ISXBotHelper.Settings.Settings;
using ISXBotHelper.Items;
using ISXWoW;
using ISXRhabotGlobal;

namespace ISXBotHelper
{
    internal static class clsCreateProfile
    {
        /// <summary>
        /// Creates default profiles
        /// </summary>
        public static void CreateDefaultProfiles()
        {
            try
            {
                // clear the dictionary
                clsSettings.DefaultSettingsList.Clear();

                // create settings for each class
                CreateDruidSettings();
                System.Windows.Forms.Application.DoEvents();
                CreateWarlockSettings();
                System.Windows.Forms.Application.DoEvents();
                CreateShamanSettings();
                System.Windows.Forms.Application.DoEvents();
                CreateMageSettings();
                System.Windows.Forms.Application.DoEvents();
                CreatePriestSettings();
                System.Windows.Forms.Application.DoEvents();
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "CreateDefaultProfiles");
            }            
        }


        #region Druid

        /// <summary>
        /// Create settings for druids
        /// </summary>
        private static void CreateDruidSettings()
        {
            clsGlobalSettings GlobalSettings = new clsGlobalSettings();
            clsLevelSettings LevelSettings = new clsLevelSettings();
            string SettingsName = "Default - Druid";
            int i;

            try
            {
                // set global settings
                GlobalSettings.AutoBuff_BuffList.Add("Thorns");
                GlobalSettings.AutoBuff_BuffList.Add("Mark of the Wild");
                GlobalSettings.AutoBuff_Heal = "Healing Touch";
                GlobalSettings.AutoBuff_HealPercent = 65;
                GlobalSettings.DeclineDuelInvite = true;
                GlobalSettings.DeclineGroupInvite = true;
                GlobalSettings.DeclineGuildInvite = true;
                GlobalSettings.ItemSellColors.Grey = true;

                // set auto equip list
                GlobalSettings.AutoEquipList = AutoCreate_Leather();

                #region TalentList

                for (i = 10; i < 15; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Mark of the Wild", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Furor", 15));
                for (i = 16; i <= 20; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Naturalist", i));
                for (i = 21; i <= 23; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Natural Shapeshifter", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Omen of Clarity", 24));
                for (i = 25; i <= 29; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Ferocity", i));
                for (i = 30; i <= 32; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Feral Instinct", i));
                for (i = 33; i <= 34; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Brutal Impact", i));
                for (i = 35; i <= 37; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Thick Hide", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Feral Charge", 38));
                for (i = 39; i <= 41; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Sharpened Claws", i));
                for (i = 42; i <= 43; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Shredding Attacks", i));
                for (i = 44; i <= 46; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Predatory Strikes", i));
                for (i = 47; i <= 48; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Primal Fury", i));
                for (i = 49; i <= 50; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Savage Fury", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Faerie Fire (Feral)", 51));
                for (i = 52; i <= 53; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Nurturing Instinct", i));
                for (i = 54; i <= 58; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Heart of the Wild", i));
                for (i = 59; i <= 61; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Survival of the Fittest", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Leader of the Pack", 62));
                for (i = 63; i <= 64; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Leader of the Pack", i));
                for (i = 69; i <= 69; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Predatory Instincts", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Mangle", 70));

                // TalentList
                #endregion

                // save the global settings
                // clsSettings.Logging.AddToLog("CreateDruidSettings", "Saving Global DRUID Settings");
                SaveGlobalSettings(GlobalSettings, SettingsName);

                #region 1_3

                for (i = 1; i <= 3; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_HealSpell = "Healing Touch";
                    LevelSettings.Combat_PullSpell = "Wrath";
                    LevelSettings.Combat_SpamSpells_List.Add("Wrath");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateDruidSettings", "Saving DRUID Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 1_3
                #endregion

                #region 4_9

                for (i = 4; i <= 9; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_PreBuffSpells_List.Add("Mark of the Wild");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Thorns");
                    LevelSettings.Combat_HealSpell = "Healing Touch";
                    LevelSettings.Combat_HealingOT = "Rejuvenation";
                    LevelSettings.Combat_PullSpell = "Wrath";
                    LevelSettings.Combat_SpamSpells_List.Add("Wrath");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateDruidSettings", "Saving DRUID Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 4_9
                #endregion

                #region 10_14

                for (i = 10; i <= 14; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();

                    LevelSettings.Combat_ClassSettings.Druid_ChangeToBearOnAggro = true;
                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = clsGlobals.ECombatClass_Druid_Form.Bear;

                    LevelSettings.Combat_PreBuffSpells_List.Add("Mark of the Wild");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Thorns");
                    LevelSettings.Combat_HealSpell = "Healing Touch";
                    LevelSettings.Combat_HealingOT = "Rejuvenation";
                    LevelSettings.Combat_PullSpell = "Maul";
                    LevelSettings.Combat_DOT_List.Add("Demoralizing Roar");
                    LevelSettings.Combat_SpamSpells_List.Add("Maul");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateDruidSettings", "Saving DRUID Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 10_14
                #endregion

                #region 15_20

                for (i = 15; i <= 20; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();

                    LevelSettings.Combat_ClassSettings.Druid_ChangeToBearOnAggro = true;
                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = clsGlobals.ECombatClass_Druid_Form.Bear;

                    LevelSettings.Combat_PreBuffSpells_List.Add("Mark of the Wild");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Thorns");
                    LevelSettings.Combat_HealSpell = "Healing Touch";
                    LevelSettings.Combat_HealingOT = "Rejuvenation";
                    LevelSettings.Combat_PullSpell = "Maul";
                    LevelSettings.Combat_DOT_List.Add("Demoralizing Roar");
                    LevelSettings.Combat_SpamSpells_List.Add("Maul");
                    LevelSettings.Combat_SpamSpells_List.Add("Maul");
                    LevelSettings.Combat_SpamSpells_List.Add("Bash");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateDruidSettings", "Saving DRUID Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 15_20
                #endregion

                #region 21_26

                for (i = 21; i <= 26; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();

                    LevelSettings.Combat_ClassSettings.Druid_ChangeToBearOnAggro = true;
                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = clsGlobals.ECombatClass_Druid_Form.Cat;

                    LevelSettings.Combat_PreBuffSpells_List.Add("Mark of the Wild");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Thorns");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Prowl");
                    LevelSettings.Combat_HealSpell = "Healing Touch";
                    LevelSettings.Combat_HealingOT = "Rejuvenation";
                    LevelSettings.Combat_PullSpell = "Claw";
                    LevelSettings.Combat_SpamSpells_List.Add("Claw");
                    LevelSettings.Combat_SpamSpells_List.Add("Claw");
                    LevelSettings.Combat_SpamSpells_List.Add("Rip");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateDruidSettings", "Saving DRUID Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 21_26
                #endregion

                #region 27_32

                for (i = 27; i <= 32; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();

                    LevelSettings.Combat_ClassSettings.Druid_ChangeToBearOnAggro = true;
                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = clsGlobals.ECombatClass_Druid_Form.Cat;

                    LevelSettings.Combat_PreBuffSpells_List.Add("Mark of the Wild");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Thorns");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Prowl");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Tiger's Fury");
                    LevelSettings.Combat_HealSpell = "Healing Touch";
                    LevelSettings.Combat_HealingOT = "Rejuvenation";
                    LevelSettings.Combat_PullSpell = "Claw";
                    LevelSettings.Combat_SpamSpells_List.Add("Faerie Fire (Feral)");
                    LevelSettings.Combat_SpamSpells_List.Add("Claw");
                    LevelSettings.Combat_SpamSpells_List.Add("Claw");
                    LevelSettings.Combat_SpamSpells_List.Add("Rip");
                    LevelSettings.Combat_SpamSpells_List.Add("Claw");

                    LevelSettings.Combat_DOT_List.Add("Rake");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateDruidSettings", "Saving DRUID Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 27_32
                #endregion

                #region 33_50

                for (i = 33; i <= 50; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();

                    LevelSettings.Combat_ClassSettings.Druid_ChangeToBearOnAggro = true;
                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = clsGlobals.ECombatClass_Druid_Form.Cat;

                    LevelSettings.Combat_PreBuffSpells_List.Add("Mark of the Wild");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Thorns");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Prowl");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Tiger's Fury");
                    LevelSettings.Combat_HealSpell = "Healing Touch";
                    LevelSettings.Combat_HealingOT = "Rejuvenation";
                    LevelSettings.Combat_PullSpell = "Claw";
                    LevelSettings.Combat_SpamSpells_List.Add("Faerie Fire (Feral)");
                    LevelSettings.Combat_SpamSpells_List.Add("Claw");
                    LevelSettings.Combat_SpamSpells_List.Add("Claw");
                    LevelSettings.Combat_SpamSpells_List.Add("Ferocious Bite");
                    LevelSettings.Combat_SpamSpells_List.Add("Claw");

                    LevelSettings.Combat_DOT_List.Add("Rake");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateDruidSettings", "Saving DRUID Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 33_50
                #endregion

                #region 51_70

                for (i = 51; i <= 70; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();

                    LevelSettings.Combat_ClassSettings.Druid_ChangeToBearOnAggro = true;
                    LevelSettings.Combat_ClassSettings.Druid_CombatForm = clsGlobals.ECombatClass_Druid_Form.Cat;

                    LevelSettings.Combat_PreBuffSpells_List.Add("Mark of the Wild");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Thorns");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Prowl");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Tiger's Fury");
                    LevelSettings.Combat_HealSpell = "Healing Touch";
                    LevelSettings.Combat_HealingOT = "Rejuvenation";
                    LevelSettings.Combat_PullSpell = "Ravage";
                    LevelSettings.Combat_SpamSpells_List.Add("Faerie Fire (Feral)");
                    LevelSettings.Combat_SpamSpells_List.Add("Ferocious Bite");

                    LevelSettings.Combat_DOT_List.Add("Rake");
                    LevelSettings.Combat_DOT_List.Add("Mangle (Cat)");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateDruidSettings", "Saving DRUID Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 51_70
                #endregion
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "CreateDruidSettings");
            }            

            finally
            {
                // clsSettings.Logging.AddToLog("Finished DRUID settings");
            }
        }

        // Druid
        #endregion

        #region Warlock

        private static void CreateWarlockSettings()
        {
            clsGlobalSettings GlobalSettings = new clsGlobalSettings();
            clsLevelSettings LevelSettings = new clsLevelSettings();
            string SettingsName = "Default - Warlock";
            int i;

            try
            {
                // set global settings
                GlobalSettings.DeclineDuelInvite = true;
                GlobalSettings.DeclineGroupInvite = true;
                GlobalSettings.DeclineGuildInvite = true;
                GlobalSettings.ItemSellColors.Grey = true;

                // set auto equip list
                GlobalSettings.AutoEquipList = AutoCreate_Cloth();

                #region TalentList

                for (i = 10; i < 15; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Corruption", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Drain Soul", 15));
                for (i = 16; i <= 17; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Life Tap", i));
                for (i = 18; i <= 19; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Soul Siphon", i));
                for (i = 20; i <= 24; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Fel Concentration", i));
                for (i = 25; i <= 26; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Nightfall", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Empowered Corruption", 27));
                for (i = 28; i <= 32; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Demonic Embrace", i));
                for (i = 33; i <= 35; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Voidwalker", i));
                for (i = 36; i <= 38; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Fel Intellect", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Fel Domination", 39));
                for (i = 40; i <= 42; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Fel Stamina", i));
                for (i = 43; i <= 45; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Demonic Aegis", i));
                for (i = 46; i <= 47; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Master Summoner", i));
                for (i = 48; i <= 52; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Unholy Power", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Demonic Sacrifice", 53));
                for (i = 54; i <= 55; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Mana Feed", i));
                for (i = 56; i <= 60; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Master Demonologist", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Soul Link", 61));
                for (i = 62; i <= 64; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Demonic Knowledge", i));
                for (i = 65; i <= 69; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Demonic Tactics", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Summon Felguard", 70));

                // TalentList
                #endregion

                // save the global settings
                // clsSettings.Logging.AddToLog("CreateWarlockSettings", "Saving Global WARLOCK Settings");
                SaveGlobalSettings(GlobalSettings, SettingsName);

                #region 1_3

                for (i = 1; i <= 3; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_DowntimePercent = 55;
                    LevelSettings.Combat_ClassSettings.Warlock_Pet = clsGlobals.ECombatClass_Warlock_Pet.Imp;
                    LevelSettings.Combat_ClassSettings.Warlock_PullWithPet = false;
                    LevelSettings.Combat_PreBuffSpells_List.Add("Demon Skin");
                    LevelSettings.Combat_PullSpell = "Shadow Bolt";
                    LevelSettings.Combat_SpamSpells_List.Add("Shadow Bolt");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateWarlockSettings", "Saving WARLOCK Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 1_3
                #endregion

                #region 4_8

                for (i = 4; i <= 8; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_DowntimePercent = 55;
                    LevelSettings.Combat_ClassSettings.Warlock_Pet = clsGlobals.ECombatClass_Warlock_Pet.Imp;
                    LevelSettings.Combat_ClassSettings.Warlock_PullWithPet = false;
                    LevelSettings.Combat_PreBuffSpells_List.Add("Demon Skin");
                    LevelSettings.Combat_PullSpell = "Immolate";
                    LevelSettings.Combat_SpamSpells_List.Add("Shadow Bolt");
                    LevelSettings.Combat_DOT_List.Add("Immolate");
                    LevelSettings.Combat_DOT_List.Add("Corruption");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateWarlockSettings", "Saving WARLOCK Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 4_8
                #endregion

                #region 9_18

                for (i = 9; i <= 18; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_DowntimePercent = 55;
                    LevelSettings.Combat_ClassSettings.Warlock_Pet = clsGlobals.ECombatClass_Warlock_Pet.Voidwalker;
                    LevelSettings.Combat_ClassSettings.Warlock_PullWithPet = true;
                    LevelSettings.Combat_PreBuffSpells_List.Add("Demon Skin");
                    LevelSettings.Combat_PullSpell = "Immolate";
                    LevelSettings.Combat_SpamSpells_List.Add("Shadow Bolt");
                    LevelSettings.Combat_DOT_List.Add("Immolate");
                    LevelSettings.Combat_DOT_List.Add("Corruption");
                    LevelSettings.Combat_DOT_List.Add("Curse of Agony");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateWarlockSettings", "Saving WARLOCK Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 9_18
                #endregion

                #region 19_50

                for (i = 19; i <= 50; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_DowntimePercent = 55;
                    LevelSettings.Combat_ClassSettings.Warlock_Pet = clsGlobals.ECombatClass_Warlock_Pet.Voidwalker;
                    LevelSettings.Combat_ClassSettings.Warlock_PullWithPet = true;
                    LevelSettings.Combat_PreBuffSpells_List.Add("Demon Armory");
                    LevelSettings.Combat_PullSpell = "Immolate";
                    LevelSettings.Combat_SpamSpells_List.Add("Shadow Bolt");
                    LevelSettings.Combat_DOT_List.Add("Immolate");
                    LevelSettings.Combat_DOT_List.Add("Corruption");
                    LevelSettings.Combat_DOT_List.Add("Curse of Agony");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateWarlockSettings", "Saving WARLOCK Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 19_50
                #endregion

                #region 51_70

                for (i = 51; i <= 70; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_DowntimePercent = 55;
                    LevelSettings.Combat_ClassSettings.Warlock_Pet = clsGlobals.ECombatClass_Warlock_Pet.Felguard;
                    LevelSettings.Combat_ClassSettings.Warlock_PullWithPet = true;
                    LevelSettings.Combat_PreBuffSpells_List.Add("Demon Skin");
                    LevelSettings.Combat_PullSpell = "Immolate";
                    LevelSettings.Combat_SpamSpells_List.Add("Shadow Bolt");
                    LevelSettings.Combat_DOT_List.Add("Immolate");
                    LevelSettings.Combat_DOT_List.Add("Corruption");
                    LevelSettings.Combat_DOT_List.Add("Curse of Agony");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateWarlockSettings", "Saving WARLOCK Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 51_70
                #endregion
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "CreateWarlockSettings");
            }

            finally
            {
                // clsSettings.Logging.AddToLog("Finished WARLOCK settings");
            }
        }

        // Warlock
        #endregion

        #region Shaman

        /// <summary>
        /// Create settings for shamans
        /// </summary>
        private static void CreateShamanSettings()
        {
            clsGlobalSettings GlobalSettings = new clsGlobalSettings();
            clsLevelSettings LevelSettings = new clsLevelSettings();
            string SettingsName = "Default - Shaman";
            int i;

            try
            {
                // set global settings
                GlobalSettings.DeclineDuelInvite = true;
                GlobalSettings.DeclineGroupInvite = true;
                GlobalSettings.DeclineGuildInvite = true;
                GlobalSettings.ItemSellColors.Grey = true;

                // set auto equip list
                GlobalSettings.AutoEquipList = AutoCreate_Leather();

                #region Talents

                for (i = 10; i < 15; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Concussion", i));
                for (i = 15; i < 20; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Convection", i));
                for (i = 20; i < 25; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Totemic Focus", i));
                for (i = 25; i < 30; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Healing Wave", i));
                for (i = 30; i < 35; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Ancestral Knowledge", i));
                for (i = 35; i < 37; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Guardian Totems", i));
                for (i = 37; i < 42; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Thundering Strikes", i));
                for (i = 42; i < 45; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Lightning Shield", i));
                for (i = 45; i < 50; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Anticipation", i));
                for (i = 50; i < 55; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Flurry", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Spirit Weapons", 55));
                for (i = 56; i < 59; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Elemental Weapons", i));
                for (i = 59; i < 63; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Weapon Mastery", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Dual Wield", 63));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Stormstrike", 64));
                for (i = 65; i < 70; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Unleashed Rage", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Shamanistic Rage", 70));

                // Talents
                #endregion

                // save the global settings
                // clsSettings.Logging.AddToLog("CreateShamanSettings", "Saving Global SHAMAN Settings");
                SaveGlobalSettings(GlobalSettings, SettingsName);

                #region 1_3

                for (i = 1; i <= 3; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_HealSpell = "Healing Wave";
                    LevelSettings.Combat_PullSpell = "Lightning Bolt";
                    LevelSettings.Combat_SpamSpells_List.Add("Lightning Bolt");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateShamanSettings", "Saving SHAMAN Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 1_3
                #endregion

                #region 4_9

                for (i = 4; i <= 9; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_HealSpell = "Healing Wave";
                    LevelSettings.Combat_PullSpell = "Lightning Bolt";
                    LevelSettings.Combat_SpamSpells_List.Add("Earth Shock");

                    // totem
                    LevelSettings.Combat_ClassSettings.Shaman_EarthTotem = "Stoneskin Totem";

                    // weapon buff
                    LevelSettings.Combat_ClassSettings.Shaman_MainHandBuff = "Rockbiter Weapon";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateShamanSettings", "Saving SHAMAN Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 4_9
                #endregion

                #region 10_14

                for (i = 10; i <= 14; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_PreBuffSpells_List.Add("Lightning Shield");
                    LevelSettings.Combat_HealSpell = "Healing Wave";
                    LevelSettings.Combat_PullSpell = "Lightning Bolt";
                    //LevelSettings.Combat_SpamSpells_List.Add("Lightning Bolt");
                    LevelSettings.Combat_SpamSpells_List.Add("Earth Shock");
                    LevelSettings.Combat_DOT_List.Add("Flame Shock");

                    // totem
                    LevelSettings.Combat_ClassSettings.Shaman_EarthTotem = "Stoneskin Totem";

                    // weapon buff
                    LevelSettings.Combat_ClassSettings.Shaman_MainHandBuff = "Flametongue Weapon";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateShamanSettings", "Saving SHAMAN Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 10_14
                #endregion

                #region 15_20

                for (i = 15; i <= 20; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_PreBuffSpells_List.Add("Lightning Shield");
                    LevelSettings.Combat_HealSpell = "Healing Wave";
                    LevelSettings.Combat_PullSpell = "Lightning Bolt";
                    LevelSettings.Combat_DOT_List.Add("Flame Shock");

                    // totem
                    LevelSettings.Combat_ClassSettings.Shaman_EarthTotem = "Stoneskin Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_FireTotem = "Searing Totem";

                    // weapon buff
                    LevelSettings.Combat_ClassSettings.Shaman_MainHandBuff = "Flametongue Weapon";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateShamanSettings", "Saving SHAMAN Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 15_20
                #endregion

                #region 21_26

                for (i = 21; i <= 26; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_PreBuffSpells_List.Add("Lightning Shield");
                    LevelSettings.Combat_HealSpell = "Healing Wave";
                    LevelSettings.Combat_PullSpell = "Lightning Bolt";
                    LevelSettings.Combat_DOT_List.Add("Flame Shock");

                    // totem
                    LevelSettings.Combat_ClassSettings.Shaman_EarthTotem = "Stoneskin Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_FireTotem = "Searing Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_WaterTotem = "Healing Stream Totem";

                    // weapon buff
                    LevelSettings.Combat_ClassSettings.Shaman_MainHandBuff = "Flametongue Weapon";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateShamanSettings", "Saving SHAMAN Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 21_26
                #endregion

                #region 27_30

                for (i = 27; i <= 30; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_PreBuffSpells_List.Add("Lightning Shield");
                    LevelSettings.Combat_HealSpell = "Healing Wave";
                    LevelSettings.Combat_PullSpell = "Lightning Bolt";
                    LevelSettings.Combat_DOT_List.Add("Flame Shock");

                    // totem
                    LevelSettings.Combat_ClassSettings.Shaman_EarthTotem = "Stoneskin Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_FireTotem = "Searing Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_WaterTotem = "Mana Spring Totem";


                    // weapon buff
                    LevelSettings.Combat_ClassSettings.Shaman_MainHandBuff = "Flametongue Weapon";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateShamanSettings", "Saving SHAMAN Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 27_30
                #endregion

                #region 31_63

                for (i = 31; i <= 63; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_PreBuffSpells_List.Add("Lightning Shield");
                    LevelSettings.Combat_HealSpell = "Healing Wave";
                    LevelSettings.Combat_PullSpell = "Lightning Bolt";
                    LevelSettings.Combat_DOT_List.Add("Flame Shock");

                    // totem
                    LevelSettings.Combat_ClassSettings.Shaman_EarthTotem = "Stoneskin Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_FireTotem = "Searing Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_WaterTotem = "Mana Spring Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_AirTotem = "Grounding Totem";

                    // weapon buff
                    LevelSettings.Combat_ClassSettings.Shaman_MainHandBuff = "Flametongue Weapon";
                    LevelSettings.Combat_ClassSettings.Shaman_OffhandBuff = "Rockbiter Weapon";

                    // post combat
                    LevelSettings.Combat_PostCombatSpells.Add("Totemic Call");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateShamanSettings", "Saving SHAMAN Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 31_63
                #endregion

                #region 64_69

                for (i = 64; i <= 69; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_PreBuffSpells_List.Add("Lightning Shield");
                    LevelSettings.Combat_HealSpell = "Healing Wave";
                    LevelSettings.Combat_PullSpell = "Lightning Bolt";

                    LevelSettings.Combat_DOT_List.Add("Flame Shock");
                    LevelSettings.Combat_DOT_List.Add("Stormstrike");

                    // totem
                    LevelSettings.Combat_ClassSettings.Shaman_EarthTotem = "Stoneskin Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_FireTotem = "Searing Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_WaterTotem = "Mana Spring Totem";
                    LevelSettings.Combat_ClassSettings.Shaman_AirTotem = "Grounding Totem";

                    // weapon buff
                    LevelSettings.Combat_ClassSettings.Shaman_MainHandBuff = "Flametongue Weapon";
                    LevelSettings.Combat_ClassSettings.Shaman_OffhandBuff = "Rockbiter Weapon";

                    // post combat
                    LevelSettings.Combat_PostCombatSpells.Add("Totemic Call");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateShamanSettings", "Saving SHAMAN Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 64_69
                #endregion

                #region 70

                i = 70;
                // create settings
                LevelSettings = DefaultLevelSettings();
                
                LevelSettings.Combat_PreBuffSpells_List.Add("Lightning Shield");
                LevelSettings.Combat_PreBuffSpells_List.Add("Shamanistic Rage");

                LevelSettings.Combat_HealSpell = "Healing Wave";
                LevelSettings.Combat_PullSpell = "Lightning Bolt";

                LevelSettings.Combat_DOT_List.Add("Flame Shock");
                LevelSettings.Combat_DOT_List.Add("Stormstrike");

                // totem
                LevelSettings.Combat_ClassSettings.Shaman_EarthTotem = "Stoneskin Totem";
                LevelSettings.Combat_ClassSettings.Shaman_FireTotem = "Searing Totem";
                LevelSettings.Combat_ClassSettings.Shaman_WaterTotem = "Mana Spring Totem";
                LevelSettings.Combat_ClassSettings.Shaman_AirTotem = "Grounding Totem";

                // weapon buff
                LevelSettings.Combat_ClassSettings.Shaman_MainHandBuff = "Flametongue Weapon";
                LevelSettings.Combat_ClassSettings.Shaman_OffhandBuff = "Rockbiter Weapon";

                // post combat
                LevelSettings.Combat_PostCombatSpells.Add("Totemic Call");

                // save settings
                // clsSettings.Logging.AddToLogFormatted("CreateShamanSettings", "Saving SHAMAN Settings for Level {0}", i);
                SaveLevelSettings(LevelSettings, i, SettingsName);

                // 70
                #endregion
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "CreateShamanSettings");
            }

            finally
            {
                // clsSettings.Logging.AddToLog("Finished SHAMAN settings");
            }
        }

        // Shaman
        #endregion

        #region Mage

        /// <summary>
        /// Create settings for Mages
        /// </summary>
        private static void CreateMageSettings()
        {
            clsGlobalSettings GlobalSettings = new clsGlobalSettings();
            clsLevelSettings LevelSettings = new clsLevelSettings();
            string SettingsName = "Default - Mage";
            int i;

            try
            {
                // set global settings
                GlobalSettings.DeclineDuelInvite = true;
                GlobalSettings.DeclineGroupInvite = true;
                GlobalSettings.DeclineGuildInvite = true;
                GlobalSettings.ItemSellColors.Grey = true;

                // set auto equip list
                GlobalSettings.AutoEquipList = AutoCreate_Cloth();

                #region Talents

                for (i = 10; i < 12; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Frost Warding", i));
                for (i = 12; i < 17; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Fireball", i));
                for (i = 17; i < 22; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Impact", i));
                for (i = 22; i < 24; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Arcane Subtlety", i));
                for (i = 24; i < 29; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Arcane Focus", i));
                for (i = 29; i < 34; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Arcane Missiles", i));
                for (i = 34; i < 36; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Wand Specialization", i));
                for (i = 36; i < 41; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Magic Absorption", i));
                for (i = 41; i < 46; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Arcane Concentration", i));
                for (i = 46; i < 49; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Arcane Impact", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Arcane Fortitude", 49));
                for (i = 50; i < 53; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Arcane Meditation", i));
                for (i = 53; i < 58; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Arcane Mind", i));
                for (i = 58; i < 60; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Prismatic Cloak", i));
                for (i = 60; i < 63; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Arcane Potency", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Empowered Arcane Missiles", 63));
                for (i = 64; i < 66; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Spell Power", i));
                for (i = 66; i < 70; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Mind Mastery", i));

                // Talents
                #endregion

                // save the global settings
                // clsSettings.Logging.AddToLog("CreateMageSettings", "Saving Global MAGE Settings");
                SaveGlobalSettings(GlobalSettings, SettingsName);

                #region 1_4

                for (i = 1; i <= 4; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Frost Armor");

                    // pull
                    LevelSettings.Combat_PullSpell = "Fireball";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Fireball");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateMageSettings", "Saving MAGE Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 1_4
                #endregion

                #region 5_8

                for (i = 5; i <= 8; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Frost Armor");

                    // pull
                    LevelSettings.Combat_PullSpell = "Frostbolt";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Fireball");
                    LevelSettings.Combat_SpamSpells_List.Add("Fire Blast");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateMageSettings", "Saving MAGE Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 5_8
                #endregion

                #region 9_14

                for (i = 9; i <= 14; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Frost Armor");

                    // pull
                    LevelSettings.Combat_PullSpell = "Frostbolt";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Arcane Missiles");
                    LevelSettings.Combat_SpamSpells_List.Add("Fire Blast");
                    LevelSettings.Combat_SpamSpells_List.Add("Frostbolt");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateMageSettings", "Saving MAGE Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 9_14
                #endregion

                #region 15_30

                for (i = 15; i <= 30; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Frost Armor");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Arcane Intellect");

                    // pull
                    LevelSettings.Combat_PullSpell = "Frostbolt";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Fireball");
                    LevelSettings.Combat_SpamSpells_List.Add("Arcane Missiles");
                    LevelSettings.Combat_SpamSpells_List.Add("Fire Blast");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateMageSettings", "Saving MAGE Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 15_30
                #endregion

                #region 31_62

                for (i = 31; i <= 62; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Ice Armor");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Arcane Intellect");

                    // pull
                    LevelSettings.Combat_PullSpell = "Frostbolt";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Fireball");
                    LevelSettings.Combat_SpamSpells_List.Add("Arcane Missiles");
                    LevelSettings.Combat_SpamSpells_List.Add("Fire Blast");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateMageSettings", "Saving MAGE Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 31_62
                #endregion

                #region 63_70

                for (i = 63; i <= 70; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Molten Armor");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Arcane Intellect");

                    // pull
                    LevelSettings.Combat_PullSpell = "Frostbolt";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Fireball");
                    LevelSettings.Combat_SpamSpells_List.Add("Arcane Missiles");
                    LevelSettings.Combat_SpamSpells_List.Add("Fire Blast");

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreateMageSettings", "Saving MAGE Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 63_70
                #endregion
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "CreateMageSettings");
            }

            finally
            {
                // clsSettings.Logging.AddToLog("Finished MAGE settings");
            }
        }

        // Mage
        #endregion

        #region Priest

        /// <summary>
        /// Create settings for Mages
        /// </summary>
        private static void CreatePriestSettings()
        {
            clsGlobalSettings GlobalSettings = new clsGlobalSettings();
            clsLevelSettings LevelSettings = new clsLevelSettings();
            string SettingsName = "Default - Shadow Priest";
            int i;

            try
            {
                // set global settings
                GlobalSettings.DeclineDuelInvite = true;
                GlobalSettings.DeclineGroupInvite = true;
                GlobalSettings.DeclineGuildInvite = true;
                GlobalSettings.ItemSellColors.Grey = true;

                // set auto equip list
                GlobalSettings.AutoEquipList = AutoCreate_Cloth();

                #region Talents

                for (i = 10; i < 12; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Healing Focus", i));
                for (i = 12; i < 14; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Power Word: Fortitude", i));
                for (i = 14; i < 19; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Unbreakable Will", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Power Word: Shield", 19));
                for (i = 20; i < 22; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Martyrdom", i));
                for (i = 22; i < 25; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Meditation", i));
                for (i = 25; i < 30; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Spirit Tap", i));
                for (i = 30; i < 35; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Blackout", i));
                for (i = 35; i < 37; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Shadow Word: Pain", i));
                for (i = 37; i < 42; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Shadow Focus", i));
                for (i = 42; i < 44; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Shadow Reach", i));
                for (i = 44; i < 49; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Shadow Weaving", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Vampiric Embrace", 49));
                for (i = 50; i < 52; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Improved Vampiric Embrace", i));
                for (i = 52; i < 54; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Shadow Resilience", i));
                for (i = 54; i < 59; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Darkness", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Shadowform", 59));
                for (i = 60; i < 65; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Shadow Power", i));
                for (i = 65; i < 70; i++)
                    GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Misery", i));
                GlobalSettings.TalentList.Add(new ISXBotHelper.Talents.clsPTalent("Vampiric Touch", 70));

                // Talents
                #endregion

                // save the global settings
                // clsSettings.Logging.AddToLog("CreatePriestSettings", "Saving Global PRIEST Settings");
                SaveGlobalSettings(GlobalSettings, SettingsName);

                #region 1_4

                for (i = 1; i <= 4; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Power Word: Fortitude");

                    // pull
                    LevelSettings.Combat_PullSpell = "Smite";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Smite");

                    // healing
                    LevelSettings.Combat_HealSpell = "Lesser Heal";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreatePriestSettings", "Saving PRIEST Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 1_4
                #endregion

                #region 5_10

                for (i = 5; i <= 10; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Power Word: Fortitude");

                    // pull
                    LevelSettings.Combat_PullSpell = "Smite";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Smite");

                    // dots
                    LevelSettings.Combat_DOT_List.Add("Shadow Word: Pain");

                    // healing
                    LevelSettings.Combat_HealSpell = "Lesser Heal";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreatePriestSettings", "Saving PRIEST Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 5_10
                #endregion

                #region 11_16

                for (i = 11; i <= 16; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;
                    LevelSettings.Combat_CastSpamRandomly = true;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Power Word: Fortitude");

                    // pull
                    LevelSettings.Combat_PullSpell = "Smite";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Smite");
                    LevelSettings.Combat_SpamSpells_List.Add("Mind Blast");

                    // dots
                    LevelSettings.Combat_DOT_List.Add("Shadow Word: Pain");

                    // healing
                    LevelSettings.Combat_HealSpell = "Lesser Heal";
                    LevelSettings.Combat_HealingOT = "Renew";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreatePriestSettings", "Saving PRIEST Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 11_16
                #endregion

                #region 17_20

                for (i = 17; i <= 20; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;
                    LevelSettings.Combat_CastSpamRandomly = true;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Power Word: Fortitude");

                    // pull
                    LevelSettings.Combat_PullSpell = "Smite";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Smite");
                    LevelSettings.Combat_SpamSpells_List.Add("Mind Blast");

                    // dots
                    LevelSettings.Combat_DOT_List.Add("Shadow Word: Pain");

                    // healing
                    LevelSettings.Combat_HealSpell = "Heal";
                    LevelSettings.Combat_HealingOT = "Renew";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreatePriestSettings", "Saving PRIEST Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 17_20
                #endregion

                #region 21_50

                for (i = 21; i <= 50; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;
                    LevelSettings.Combat_CastSpamRandomly = true;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Power Word: Fortitude");

                    // pull
                    LevelSettings.Combat_PullSpell = "Smite";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Smite");
                    LevelSettings.Combat_SpamSpells_List.Add("Mind Blast");

                    // dots
                    LevelSettings.Combat_DOT_List.Add("Shadow Word: Pain");
                    LevelSettings.Combat_DOT_List.Add("Holy Fire");

                    // healing
                    LevelSettings.Combat_HealSpell = "Heal";
                    LevelSettings.Combat_HealingOT = "Renew";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreatePriestSettings", "Saving PRIEST Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 21_50
                #endregion

                #region 51_60

                for (i = 51; i <= 60; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;
                    LevelSettings.Combat_CastSpamRandomly = true;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Power Word: Fortitude");

                    // pull
                    LevelSettings.Combat_PullSpell = "Smite";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Smite");
                    LevelSettings.Combat_SpamSpells_List.Add("Mind Blast");

                    // dots
                    LevelSettings.Combat_DOT_List.Add("Shadow Word: Pain");
                    LevelSettings.Combat_DOT_List.Add("Holy Fire");
                    LevelSettings.Combat_DOT_List.Add("Vampiric Embrace");

                    // healing
                    LevelSettings.Combat_HealSpell = "Heal";
                    LevelSettings.Combat_HealingOT = "Renew";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreatePriestSettings", "Saving PRIEST Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 51_60
                #endregion

                #region 61_69

                for (i = 61; i <= 69; i++)
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;
                    LevelSettings.Combat_CastSpamRandomly = true;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Power Word: Fortitude");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Shadowform");

                    // pull
                    LevelSettings.Combat_PullSpell = "Smite";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Smite");
                    LevelSettings.Combat_SpamSpells_List.Add("Mind Blast");

                    // dots
                    LevelSettings.Combat_DOT_List.Add("Shadow Word: Pain");
                    LevelSettings.Combat_DOT_List.Add("Vampiric Embrace");

                    // healing
                    LevelSettings.Combat_HealSpell = "Heal";
                    LevelSettings.Combat_HealingOT = "Renew";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreatePriestSettings", "Saving PRIEST Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 61_69
                #endregion

                #region 70

                i = 70;
                {
                    // create settings
                    LevelSettings = DefaultLevelSettings();
                    LevelSettings.Combat_ManaSpam = 25;
                    LevelSettings.Combat_HealthPercent = 45;
                    LevelSettings.Combat_WaitTime_ms = 1000;
                    LevelSettings.Combat_CastSpamRandomly = true;

                    // buffs
                    LevelSettings.Combat_PreBuffSpells_List.Add("Power Word: Fortitude");
                    LevelSettings.Combat_PreBuffSpells_List.Add("Shadowform");

                    // cast the shadow fiend as a buff (so it is cast every time)
                    // Creates a shadowy fiend to attack the target. Caster receives mana when the Shadowfiend deals damage.
                    LevelSettings.Combat_PreBuffSpells_List.Add("Shadowfiend");

                    // pull
                    LevelSettings.Combat_PullSpell = "Smite";

                    // spam spells
                    LevelSettings.Combat_SpamSpells_List.Add("Smite");
                    LevelSettings.Combat_SpamSpells_List.Add("Mind Blast");

                    // dots
                    LevelSettings.Combat_DOT_List.Add("Shadow Word: Pain");
                    LevelSettings.Combat_DOT_List.Add("Vampiric Embrace");
                    LevelSettings.Combat_DOT_List.Add("Vampiric Touch");

                    // healing
                    LevelSettings.Combat_HealSpell = "Heal";
                    LevelSettings.Combat_HealingOT = "Renew";

                    // save settings
                    // clsSettings.Logging.AddToLogFormatted("CreatePriestSettings", "Saving PRIEST Settings for Level {0}", i);
                    SaveLevelSettings(LevelSettings, i, SettingsName);
                }

                // 70
                #endregion
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "CreatePriestSettings");
            }

            finally
            {
                // clsSettings.Logging.AddToLog("Finished PRIEST settings");
            }
        }

        // Priest
        #endregion

        #region Helpers

        private static clsLevelSettings DefaultLevelSettings()
        {
            clsLevelSettings rSettings = new clsLevelSettings();
            rSettings.Combat_DowntimePercent = 55;
            rSettings.Combat_HealthPercent = 45;
            rSettings.Combat_ManaSpam = 45;
            rSettings.Combat_PanicThreshold = 3;
            rSettings.HighLevelAttack = 4;
            rSettings.LowLevelAttack = 1;
            rSettings.Search_Chest = true;
            rSettings.SearchRange = 30;
            rSettings.TargetElites = false;
            rSettings.TargetRange = 20;
            return rSettings;
        }

        /// <summary>
        /// Auto equip for cloth wearers (favors intellect and spirit)
        /// </summary>
        /// <returns></returns>
        private static List<ISXBotHelper.Items.clsAutoEquipItem> AutoCreate_Cloth()
        {
            List<clsAutoEquipItem> aList = new List<clsAutoEquipItem>();

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Back,
                clsGlobals.EEquipItemMaterialType.Cloth,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Chest,
                clsGlobals.EEquipItemMaterialType.Cloth,
                clsGlobals.ENeedEquipStat.Stamina));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Feet,
                clsGlobals.EEquipItemMaterialType.Cloth,
                clsGlobals.ENeedEquipStat.Stamina));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Finger1,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Finger2,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Spirit));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Hands,
                clsGlobals.EEquipItemMaterialType.Cloth,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Head,
                clsGlobals.EEquipItemMaterialType.Cloth,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Legs,
                clsGlobals.EEquipItemMaterialType.Cloth,
                clsGlobals.ENeedEquipStat.Spirit));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.MainHand,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Neck,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Ranged,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Shoulders,
                clsGlobals.EEquipItemMaterialType.Cloth,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Waist,
                clsGlobals.EEquipItemMaterialType.Cloth,
                clsGlobals.ENeedEquipStat.Spirit));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Wrists,
                clsGlobals.EEquipItemMaterialType.Cloth,
                clsGlobals.ENeedEquipStat.Intellect));

            // return the list
            return aList;
        }

        /// <summary>
        /// Creates and AutoEqip list for leather wearers
        /// </summary>
        /// <returns></returns>
        private static List<clsAutoEquipItem> AutoCreate_Leather()
        {
            List<clsAutoEquipItem> aList = new List<clsAutoEquipItem>();

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Back,
                clsGlobals.EEquipItemMaterialType.Leather,
                clsGlobals.ENeedEquipStat.Agility));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Chest,
                clsGlobals.EEquipItemMaterialType.Leather,
                clsGlobals.ENeedEquipStat.Agility));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Feet,
                clsGlobals.EEquipItemMaterialType.Leather,
                clsGlobals.ENeedEquipStat.Agility));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Finger1,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Finger2,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Hands,
                clsGlobals.EEquipItemMaterialType.Leather,
                clsGlobals.ENeedEquipStat.Strength));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Head,
                clsGlobals.EEquipItemMaterialType.Leather,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Legs,
                clsGlobals.EEquipItemMaterialType.Leather,
                clsGlobals.ENeedEquipStat.Agility));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.MainHand,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Neck,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Ranged,
                clsGlobals.EEquipItemMaterialType.None,
                clsGlobals.ENeedEquipStat.Agility));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Shoulders,
                clsGlobals.EEquipItemMaterialType.Leather,
                clsGlobals.ENeedEquipStat.Intellect));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Waist,
                clsGlobals.EEquipItemMaterialType.Leather,
                clsGlobals.ENeedEquipStat.Agility));

            aList.Add(new clsAutoEquipItem(
                WoWEquipSlot.Wrists,
                clsGlobals.EEquipItemMaterialType.Leather,
                clsGlobals.ENeedEquipStat.Intellect));

            // return the list
            return aList;
        }

        // Helpers
        #endregion

        #region Saves

        /// <summary>
        /// Save the global settings
        /// </summary>
        private static void SaveGlobalSettings(clsGlobalSettings GlobalSettings, string SettingsName)
        {
            try
            {
                // add to the dictionary
                GlobalSettings.MSN_Username = SettingsName;
                clsSettings.DefaultSettingsList.Add(GlobalSettings, new Dictionary<int, clsLevelSettings>());
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to save global settings");
            }
        }

        /// <summary>
        /// Saves the settings to the settings file
        /// </summary>
        private static void SaveLevelSettings(clsLevelSettings LevelSettings, int Level, string SettingsName)
        {
            try
            {
                // find the key
                foreach (clsGlobalSettings gs in clsSettings.DefaultSettingsList.Keys)
                {
                    // if we found our match, add this item
                    if (gs.MSN_Username == SettingsName)
                    {
                        clsSettings.DefaultSettingsList[gs].Add(Level, LevelSettings);
                        return;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, "Unable to save level settings");
            }
        }

        // Saves
        #endregion
    }
}
