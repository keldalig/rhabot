using System;
using System.Windows.Forms;
using ISXBotHelper;
using ISXBotHelper.Properties;
using ISXWoW;
using ISXBotHelper.Settings.Settings;
using System.Collections.Generic;

namespace Rhabot
{
    public partial class uscCombatSettings : UserControl
    {
        public uscCombatSettings()
        {
            InitializeComponent();
        }

        private void uscCombatSettings_Load(object sender, EventArgs e)
        {
            // exit if in designer
            if (DesignMode)
                return;

            // hook the settings changed event
            clsGlobals.SettingsLoaded += clsGlobals_SettingsLoaded;

            // load the settings
            LoadSettings();

            // hide depending on class
            HideControls();
        }

        void clsGlobals_SettingsLoaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            // pop the controls
            this.chkDoPanic.Checked = clsSettings.gclsLevelSettings.Combat_DoPanic;
            this.txtCombatDOT.Text = clsSettings.BuildListString(clsSettings.gclsLevelSettings.Combat_DOT_List);
            this.txtCombatSpam.Text = clsSettings.BuildListString(clsSettings.gclsLevelSettings.Combat_SpamSpells_List);
            this.txtHealingOverTime.Text = clsSettings.gclsLevelSettings.Combat_HealingOT;
            this.txtHealingSpell.Text = clsSettings.gclsLevelSettings.Combat_HealSpell;
            this.txtPreCombatBuff.Text = clsSettings.BuildListString(clsSettings.gclsLevelSettings.Combat_PreBuffSpells_List);
            this.txtProtectionSpell.Text = clsSettings.gclsLevelSettings.Combat_ProtectionSpell;
            this.txtPullSpell.Text = clsSettings.gclsLevelSettings.Combat_PullSpell;
            this.txtCombatWait.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.Combat_WaitTime_ms, this.txtCombatWait);
            this.txtDowntime.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.Combat_DowntimePercent, this.txtDowntime);
            this.txtPanicThreshold.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.Combat_PanicThreshold, this.txtPanicThreshold);
            this.txtHealPct.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.Combat_HealthPercent, this.txtHealPct);
            this.txtManaSpam.Value = clsGlobals.SetNumericValue(clsSettings.gclsLevelSettings.Combat_ManaSpam, this.txtManaSpam);
            this.chkSpamRandom.Checked = clsSettings.gclsLevelSettings.Combat_CastSpamRandomly;
            this.txtPostCombatSpells.Text = clsSettings.BuildListString(clsSettings.gclsLevelSettings.Combat_PostCombatSpells);
            this.txtStopRunAwayAttempt.Text = clsSettings.gclsLevelSettings.Combat_StopRunawayAttemptSpell;

            // pop spell list
            PopSpellList();
        }

        /// <summary>
        /// Pops the spell list based on class
        /// </summary>
        private void PopSpellList()
        {
            this.lstSPellList.Items.Clear();

            // pop based on class
            switch (clsCharacter.MyClass)
            {
                case ISXWoW.WoWClass.Warrior:
                    #region Warrior

                    this.lstSPellList.Items.Add("Heroic Strike");
                    this.lstSPellList.Items.Add("Battle Shout");
                    this.lstSPellList.Items.Add("Charge");
                    this.lstSPellList.Items.Add("Rend");
                    this.lstSPellList.Items.Add("Thunder Clap");
                    this.lstSPellList.Items.Add("Hamstring");
                    this.lstSPellList.Items.Add("Berserker");
                    this.lstSPellList.Items.Add("Bloodrage");
                    this.lstSPellList.Items.Add("Sunder Armor");
                    this.lstSPellList.Items.Add("Taunt");
                    this.lstSPellList.Items.Add("Shield Bash");
                    this.lstSPellList.Items.Add("Overpower");
                    this.lstSPellList.Items.Add("Demoralizing Shout");
                    this.lstSPellList.Items.Add("Revenge");
                    this.lstSPellList.Items.Add("Mocking Blow");
                    this.lstSPellList.Items.Add("Shield Block");
                    this.lstSPellList.Items.Add("Disarm");
                    this.lstSPellList.Items.Add("Cleave");
                    this.lstSPellList.Items.Add("Retaliation");
                    this.lstSPellList.Items.Add("Intimidating Shout");
                    this.lstSPellList.Items.Add("Execute");
                    this.lstSPellList.Items.Add("Berserker");
                    this.lstSPellList.Items.Add("Challenging Shout");
                    this.lstSPellList.Items.Add("Shield Wall");
                    this.lstSPellList.Items.Add("Intercept");
                    this.lstSPellList.Items.Add("Slam");
                    this.lstSPellList.Items.Add("Berserker Rage");
                    this.lstSPellList.Items.Add("Whirlwind");
                    this.lstSPellList.Items.Add("Pummel");
                    this.lstSPellList.Items.Add("Recklessness");
                    this.lstSPellList.Items.Add("Victory Rush");
                    this.lstSPellList.Items.Add("Spell Reflection");
                    this.lstSPellList.Items.Add("Commanding Shout");
                    this.lstSPellList.Items.Add("Intervene");
                    this.lstSPellList.Items.Add("Piercing Howl");
                    this.lstSPellList.Items.Add("Sweeping Strikes");
                    this.lstSPellList.Items.Add("Bloodthirst");
                    this.lstSPellList.Items.Add("Fury");
                    this.lstSPellList.Items.Add("Rampage");
                    this.lstSPellList.Items.Add("Death Wish");
                    this.lstSPellList.Items.Add("Mortal Strike");
                    this.lstSPellList.Items.Add("Last Stand");
                    this.lstSPellList.Items.Add("Concussion Blow");
                    this.lstSPellList.Items.Add("Shield Slam");
                    this.lstSPellList.Items.Add("Devastate");

                    // Warrior
                    #endregion
                    break;

                case ISXWoW.WoWClass.Paladin:
                    #region Pally

                    this.lstSPellList.Items.Add("Seal of Righteousness");
                    this.lstSPellList.Items.Add("Devotion Aura");
                    this.lstSPellList.Items.Add("Holy Light");
                    this.lstSPellList.Items.Add("Blessing of Might");
                    this.lstSPellList.Items.Add("Judgement");
                    this.lstSPellList.Items.Add("Divine Protection");
                    this.lstSPellList.Items.Add("Seal of the Crusader");
                    this.lstSPellList.Items.Add("Hammer of Justice");
                    this.lstSPellList.Items.Add("Purify");
                    this.lstSPellList.Items.Add("Blessing of Protection");
                    this.lstSPellList.Items.Add("Lay On Hands");
                    this.lstSPellList.Items.Add("Redemption");
                    this.lstSPellList.Items.Add("Blessing of Wisdom");
                    this.lstSPellList.Items.Add("Righteous Defense");
                    this.lstSPellList.Items.Add("Retribution Aura");
                    this.lstSPellList.Items.Add("Righteous Fury");
                    this.lstSPellList.Items.Add("Blessing of Freedom");
                    this.lstSPellList.Items.Add("Spiritual Attunement");
                    this.lstSPellList.Items.Add("Exorcism");
                    this.lstSPellList.Items.Add("Flash of Light");
                    this.lstSPellList.Items.Add("Sense Undead");
                    this.lstSPellList.Items.Add("Consecration");
                    this.lstSPellList.Items.Add("Concentration Aura");
                    this.lstSPellList.Items.Add("Seal of Justice");
                    this.lstSPellList.Items.Add("Turn Undead");
                    this.lstSPellList.Items.Add("Blessing of Salvation");
                    this.lstSPellList.Items.Add("Shadow Resistance Aura");
                    this.lstSPellList.Items.Add("Divine Intervention");
                    this.lstSPellList.Items.Add("Seal of Light");
                    this.lstSPellList.Items.Add("Frost Resistance Aura");
                    this.lstSPellList.Items.Add("Divine Shield");
                    this.lstSPellList.Items.Add("Fire Resistance Aura");
                    this.lstSPellList.Items.Add("Seal of Wisdom");
                    this.lstSPellList.Items.Add("Blessing of Light");
                    this.lstSPellList.Items.Add("Cleanse");
                    this.lstSPellList.Items.Add("Hammer of Wrath");
                    this.lstSPellList.Items.Add("Blessing of Sacrifice");
                    this.lstSPellList.Items.Add("Holy Wrath");
                    this.lstSPellList.Items.Add("Greater Blessing of Might");
                    this.lstSPellList.Items.Add("Greater Blessing of Wisdom");
                    this.lstSPellList.Items.Add("Greater Blessing of Light");
                    this.lstSPellList.Items.Add("Greater Blessing of Salvation");
                    this.lstSPellList.Items.Add("Crusader Aura");
                    this.lstSPellList.Items.Add("Seal of Blood");
                    this.lstSPellList.Items.Add("Seal of Vengeance");
                    this.lstSPellList.Items.Add("Avenging Wrath");
                    this.lstSPellList.Items.Add("Divine Favor");
                    this.lstSPellList.Items.Add("Illumination");
                    this.lstSPellList.Items.Add("Holy Shock");
                    this.lstSPellList.Items.Add("Divine Favor");
                    this.lstSPellList.Items.Add("Divine Illumination");
                    this.lstSPellList.Items.Add("Blessing of Kings");
                    this.lstSPellList.Items.Add("Greater Blessing of Kings");
                    this.lstSPellList.Items.Add("Blessing of Sanctuary");
                    this.lstSPellList.Items.Add("Greater Blessing of Sanctuary");
                    this.lstSPellList.Items.Add("Holy Shield");
                    this.lstSPellList.Items.Add("Blessing of Sanctuary");
                    this.lstSPellList.Items.Add("Avenger's Shield");
                    this.lstSPellList.Items.Add("Holy Shield");
                    this.lstSPellList.Items.Add("Seal of Command");
                    this.lstSPellList.Items.Add("Sanctity Aura");
                    this.lstSPellList.Items.Add("Repentance");
                    this.lstSPellList.Items.Add("Crusader Strike");

                    // Pally
                    #endregion
                    break;

                case ISXWoW.WoWClass.Rogue:
                    #region Rogue

                    this.lstSPellList.Items.Add("Sinister Strike");
                    this.lstSPellList.Items.Add("Eviscerate");
                    this.lstSPellList.Items.Add("Stealth");
                    this.lstSPellList.Items.Add("Backstab");
                    this.lstSPellList.Items.Add("Pickpocket");
                    this.lstSPellList.Items.Add("Gouge");
                    this.lstSPellList.Items.Add("Evasion");
                    this.lstSPellList.Items.Add("Sprint");
                    this.lstSPellList.Items.Add("Sap");
                    this.lstSPellList.Items.Add("Slice and Dice");
                    this.lstSPellList.Items.Add("Kick");
                    this.lstSPellList.Items.Add("Expose Armor");
                    this.lstSPellList.Items.Add("Garrote");
                    this.lstSPellList.Items.Add("Feint");
                    this.lstSPellList.Items.Add("Ambush");
                    this.lstSPellList.Items.Add("Rupture");
                    this.lstSPellList.Items.Add("Detect Traps");
                    this.lstSPellList.Items.Add("Distract");
                    this.lstSPellList.Items.Add("Vanish");
                    this.lstSPellList.Items.Add("Cheap Shot");
                    this.lstSPellList.Items.Add("Disarm Trap");
                    this.lstSPellList.Items.Add("Kidney Shot");
                    this.lstSPellList.Items.Add("Blind");
                    this.lstSPellList.Items.Add("Envenom");
                    this.lstSPellList.Items.Add("Deadly Throw");
                    this.lstSPellList.Items.Add("Cloak of Shadows");
                    this.lstSPellList.Items.Add("Shiv");
                    this.lstSPellList.Items.Add("Ghostly Strike");
                    this.lstSPellList.Items.Add("Riposte");
                    this.lstSPellList.Items.Add("Blade Flurry");
                    this.lstSPellList.Items.Add("Cold Blood");
                    this.lstSPellList.Items.Add("Preparation");
                    this.lstSPellList.Items.Add("Hemorrhage");
                    this.lstSPellList.Items.Add("Adrenaline Rush");
                    this.lstSPellList.Items.Add("Premeditation");
                    this.lstSPellList.Items.Add("Mutilate");
                    this.lstSPellList.Items.Add("Shadowstep");

                    // Rogue
                    #endregion
                    break;

                case ISXWoW.WoWClass.Hunter:
                    #region Hunter

                    this.lstSPellList.Items.Add("Raptor Strike");
                    this.lstSPellList.Items.Add("Track Beasts");
                    this.lstSPellList.Items.Add("Aspect of the Monkey");
                    this.lstSPellList.Items.Add("Serpent Sting");
                    this.lstSPellList.Items.Add("Hunter's Mark");
                    this.lstSPellList.Items.Add("Arcane Shot");
                    this.lstSPellList.Items.Add("Concussive Shot");
                    this.lstSPellList.Items.Add("Aspect of the Hawk");
                    this.lstSPellList.Items.Add("Track Humanoids");
                    this.lstSPellList.Items.Add("Mend Pet");
                    this.lstSPellList.Items.Add("Wing Clip");
                    this.lstSPellList.Items.Add("Distracting Shot");
                    this.lstSPellList.Items.Add("Eagle Eye");
                    this.lstSPellList.Items.Add("Eyes of the Beast");
                    this.lstSPellList.Items.Add("Scare Beast");
                    this.lstSPellList.Items.Add("Mongoose Bite");
                    this.lstSPellList.Items.Add("Immolation Trap");
                    this.lstSPellList.Items.Add("Track Undead");
                    this.lstSPellList.Items.Add("Multi-Shot");
                    this.lstSPellList.Items.Add("Aspect of the Cheetah");
                    this.lstSPellList.Items.Add("Disengage");
                    this.lstSPellList.Items.Add("Deterrence");
                    this.lstSPellList.Items.Add("Freezing Trap");
                    this.lstSPellList.Items.Add("Scorpid Sting");
                    this.lstSPellList.Items.Add("Beast Lore");
                    this.lstSPellList.Items.Add("Track Hidden");
                    this.lstSPellList.Items.Add("Track Elementals");
                    this.lstSPellList.Items.Add("Rapid Fire");
                    this.lstSPellList.Items.Add("Frost Trap");
                    this.lstSPellList.Items.Add("Aspect of the Beast");
                    this.lstSPellList.Items.Add("Feign Death");
                    this.lstSPellList.Items.Add("Intimidation");
                    this.lstSPellList.Items.Add("Counterattack");
                    this.lstSPellList.Items.Add("Scatter Shot");
                    this.lstSPellList.Items.Add("Spirit Bond");
                    this.lstSPellList.Items.Add("Track Demons");
                    this.lstSPellList.Items.Add("Flare");
                    this.lstSPellList.Items.Add("Explosive Trap");
                    this.lstSPellList.Items.Add("Viper Sting");
                    this.lstSPellList.Items.Add("Bestial Wrath");
                    this.lstSPellList.Items.Add("Wyvern Sting");
                    this.lstSPellList.Items.Add("Trueshot Aura");
                    this.lstSPellList.Items.Add("Aspect of the Pack");
                    this.lstSPellList.Items.Add("Track Giants");
                    this.lstSPellList.Items.Add("Volley 	Marksmanship");
                    this.lstSPellList.Items.Add("Aspect of the Wild");
                    this.lstSPellList.Items.Add("Track Dragonkin");
                    this.lstSPellList.Items.Add("Tranquilizing Shot");
                    this.lstSPellList.Items.Add("Steady Shot");
                    this.lstSPellList.Items.Add("Aspect of the Viper");
                    this.lstSPellList.Items.Add("Kill Command");
                    this.lstSPellList.Items.Add("Snake Trap");
                    this.lstSPellList.Items.Add("Misdirection");

                    // Hunter
                    #endregion
                    break;

                case ISXWoW.WoWClass.Mage:
                    #region Mage

                    this.lstSPellList.Items.Add("Amplify Magic");
                    this.lstSPellList.Items.Add("Arcane Blast");
                    this.lstSPellList.Items.Add("Arcane Brilliance");
                    this.lstSPellList.Items.Add("Arcane Explosion");
                    this.lstSPellList.Items.Add("Arcane Intellect");
                    this.lstSPellList.Items.Add("Arcane Power");
                    this.lstSPellList.Items.Add("Arcane Missiles");
                    this.lstSPellList.Items.Add("Blink");
                    this.lstSPellList.Items.Add("Conjure Food");
                    this.lstSPellList.Items.Add("Conjure Water");
                    this.lstSPellList.Items.Add("Counterspell");
                    this.lstSPellList.Items.Add("Dampen Magic");
                    this.lstSPellList.Items.Add("Evocation");
                    this.lstSPellList.Items.Add("Invisibility");
                    this.lstSPellList.Items.Add("Mage Armor");
                    this.lstSPellList.Items.Add("Mana Shield");
                    this.lstSPellList.Items.Add("Polymorph");
                    this.lstSPellList.Items.Add("Presence of Mind");
                    this.lstSPellList.Items.Add("Remove Lesser Curse");
                    this.lstSPellList.Items.Add("Slow");
                    this.lstSPellList.Items.Add("Spellsteal");
                    this.lstSPellList.Items.Add("Blast Wave");
                    this.lstSPellList.Items.Add("Combustion");
                    this.lstSPellList.Items.Add("Dragon's Breath");
                    this.lstSPellList.Items.Add("Fire Blast");
                    this.lstSPellList.Items.Add("Fire Ward");
                    this.lstSPellList.Items.Add("Fireball");
                    this.lstSPellList.Items.Add("Flamestrike");
                    this.lstSPellList.Items.Add("Molten Armor");
                    this.lstSPellList.Items.Add("Pyroblast");
                    this.lstSPellList.Items.Add("Scorch");
                    this.lstSPellList.Items.Add("Blizzard");
                    this.lstSPellList.Items.Add("Cold Snap");
                    this.lstSPellList.Items.Add("Cone of Cold");
                    this.lstSPellList.Items.Add("Frost Armor");
                    this.lstSPellList.Items.Add("Frost Nova");
                    this.lstSPellList.Items.Add("Frost Ward");
                    this.lstSPellList.Items.Add("Frostbolt");
                    this.lstSPellList.Items.Add("Ice Armor");
                    this.lstSPellList.Items.Add("Ice Barrier");
                    this.lstSPellList.Items.Add("Ice Block");
                    this.lstSPellList.Items.Add("Icy Veins");
                    this.lstSPellList.Items.Add("Ice Lance");
                    this.lstSPellList.Items.Add("Summon Water Elemental");

                    // Mage
                    #endregion
                    break;

                case ISXWoW.WoWClass.Priest:
                    #region Priest

                    this.lstSPellList.Items.Add("Lesser Heal");
                    this.lstSPellList.Items.Add("Smite");
                    this.lstSPellList.Items.Add("Power Word: Fortitude");
                    this.lstSPellList.Items.Add("Shadow Word: Pain");
                    this.lstSPellList.Items.Add("Power Word: Shield");
                    this.lstSPellList.Items.Add("Fade");
                    this.lstSPellList.Items.Add("Renew");
                    this.lstSPellList.Items.Add("Mind Blast");
                    this.lstSPellList.Items.Add("Inner Fire");
                    this.lstSPellList.Items.Add("Cure Disease");
                    this.lstSPellList.Items.Add("Psychic Scream");
                    this.lstSPellList.Items.Add("Heal");
                    this.lstSPellList.Items.Add("Dispel Magic");
                    this.lstSPellList.Items.Add("Fear Ward");
                    this.lstSPellList.Items.Add("Flash Heal");
                    this.lstSPellList.Items.Add("Holy Fire");
                    this.lstSPellList.Items.Add("Mind Soothe");
                    this.lstSPellList.Items.Add("Shackle Undead");
                    this.lstSPellList.Items.Add("Mind Vision");
                    this.lstSPellList.Items.Add("Mana Burn");
                    this.lstSPellList.Items.Add("Mind Control");
                    this.lstSPellList.Items.Add("Prayer of Healing");
                    this.lstSPellList.Items.Add("Abolish Disease");
                    this.lstSPellList.Items.Add("Levitate");
                    this.lstSPellList.Items.Add("Greater Heal");
                    this.lstSPellList.Items.Add("Shadow Word: Death");
                    this.lstSPellList.Items.Add("Binding Heal");
                    this.lstSPellList.Items.Add("Shadowfiend");
                    this.lstSPellList.Items.Add("Prayer of Mending");
                    this.lstSPellList.Items.Add("Mass Dispel");
                    this.lstSPellList.Items.Add("Desperate Prayer");
                    this.lstSPellList.Items.Add("Starshards");
                    this.lstSPellList.Items.Add("Touch of Weakness");
                    this.lstSPellList.Items.Add("Hex of Weakness");
                    this.lstSPellList.Items.Add("Symbol of Hope");
                    this.lstSPellList.Items.Add("Chastise");
                    this.lstSPellList.Items.Add("Feedback");
                    this.lstSPellList.Items.Add("Elune's Grace");
                    this.lstSPellList.Items.Add("Devouring Plague");
                    this.lstSPellList.Items.Add("Shadowguard");
                    this.lstSPellList.Items.Add("Consume Magic");
                    this.lstSPellList.Items.Add("Mind Flay");
                    this.lstSPellList.Items.Add("Inner Focus");
                    this.lstSPellList.Items.Add("Spirit of Redemption");
                    this.lstSPellList.Items.Add("Silence");
                    this.lstSPellList.Items.Add("Vampiric Embrace");
                    this.lstSPellList.Items.Add("Divine Spirit");
                    this.lstSPellList.Items.Add("Prayer of Spirit");
                    this.lstSPellList.Items.Add("Holy Nova");
                    this.lstSPellList.Items.Add("Shadowform");
                    this.lstSPellList.Items.Add("Lightwell");
                    this.lstSPellList.Items.Add("Power Infusion");
                    this.lstSPellList.Items.Add("Vampiric Touch");
                    this.lstSPellList.Items.Add("Pain Suppression");
                    this.lstSPellList.Items.Add("Circle of Healing");
                    this.lstSPellList.Items.Add("Prayer of Fortitude");
                    this.lstSPellList.Items.Add("Prayer of Fortitude II");
                    this.lstSPellList.Items.Add("Prayer of Fortitude III");
                    this.lstSPellList.Items.Add("Prayer of Shadow Protection");
                    this.lstSPellList.Items.Add("Prayer of Shadow Protection II");

                    // Priest
                    #endregion
                    break;

                case ISXWoW.WoWClass.Warlock:
                    #region Warlock

                    this.lstSPellList.Items.Add("Demon Skin");
                    this.lstSPellList.Items.Add("Shadow Bolt");
                    this.lstSPellList.Items.Add("Immolate");
                    this.lstSPellList.Items.Add("Corruption");
                    this.lstSPellList.Items.Add("Curse of Weakness");
                    this.lstSPellList.Items.Add("Life Tap");
                    this.lstSPellList.Items.Add("Fear");
                    this.lstSPellList.Items.Add("Curse of Agony");
                    this.lstSPellList.Items.Add("Create Healthstone");
                    this.lstSPellList.Items.Add("Drain Soul");
                    this.lstSPellList.Items.Add("Health Funnel");
                    this.lstSPellList.Items.Add("Curse of Recklessness");
                    this.lstSPellList.Items.Add("Drain Life");
                    this.lstSPellList.Items.Add("Unending Breath");
                    this.lstSPellList.Items.Add("Create Soulstone");
                    this.lstSPellList.Items.Add("Searing Pain");
                    this.lstSPellList.Items.Add("Demon Armor");
                    this.lstSPellList.Items.Add("Rain of Fire");
                    this.lstSPellList.Items.Add("Drain Mana");
                    this.lstSPellList.Items.Add("Sense Demons");
                    this.lstSPellList.Items.Add("Curse of Tongues");
                    this.lstSPellList.Items.Add("Detect Invisibility");
                    this.lstSPellList.Items.Add("Banish");
                    this.lstSPellList.Items.Add("Create Firestone");
                    this.lstSPellList.Items.Add("Hellfire");
                    this.lstSPellList.Items.Add("Curse of the Elements");
                    this.lstSPellList.Items.Add("Shadow Ward");
                    this.lstSPellList.Items.Add("Create Spellstone");
                    this.lstSPellList.Items.Add("Howl of Terror");
                    this.lstSPellList.Items.Add("Death Coil");
                    this.lstSPellList.Items.Add("Curse of Shadow");
                    this.lstSPellList.Items.Add("Soul Fire");
                    this.lstSPellList.Items.Add("Inferno");
                    this.lstSPellList.Items.Add("Fel Armor");
                    this.lstSPellList.Items.Add("Incinerate");
                    this.lstSPellList.Items.Add("Soulshatter");
                    this.lstSPellList.Items.Add("Seed of Corruption");
                    this.lstSPellList.Items.Add("Amplify Curse");
                    this.lstSPellList.Items.Add("Curse of Exhaustion");
                    this.lstSPellList.Items.Add("Affliction");
                    this.lstSPellList.Items.Add("Siphon Life");
                    this.lstSPellList.Items.Add("Dark Pact");
                    this.lstSPellList.Items.Add("Unstable Affliction");
                    this.lstSPellList.Items.Add("Affliction");
                    this.lstSPellList.Items.Add("Fel Domination");
                    this.lstSPellList.Items.Add("Demonic Sacrifice");
                    this.lstSPellList.Items.Add("Soul Link");
                    this.lstSPellList.Items.Add("Shadowburn");
                    this.lstSPellList.Items.Add("Conflagrate");
                    this.lstSPellList.Items.Add("Destruction");
                    this.lstSPellList.Items.Add("Shadowfury");

                    // Warlock
                    #endregion
                    break;

                case ISXWoW.WoWClass.Druid:
                    #region Druid

                    this.lstSPellList.Items.Add("Wrath");
                    this.lstSPellList.Items.Add("Moonfire");
                    this.lstSPellList.Items.Add("Starfire");
                    this.lstSPellList.Items.Add("Thorns");
                    this.lstSPellList.Items.Add("Entangling Roots");
                    this.lstSPellList.Items.Add("Nature's Grasp");
                    this.lstSPellList.Items.Add("Faerie Fire");
                    this.lstSPellList.Items.Add("Barkskin");
                    this.lstSPellList.Items.Add("Hibernate");
                    this.lstSPellList.Items.Add("Soothe Animal");
                    this.lstSPellList.Items.Add("Insect Swarm");
                    this.lstSPellList.Items.Add("Hurricane");
                    this.lstSPellList.Items.Add("Innervate");
                    this.lstSPellList.Items.Add("Force of Nature");
                    this.lstSPellList.Items.Add("Cyclone 	");
                    this.lstSPellList.Items.Add("Mark of the Wild");
                    this.lstSPellList.Items.Add("Healing Touch");
                    this.lstSPellList.Items.Add("Rejuvenation");
                    this.lstSPellList.Items.Add("Regrowth");
                    this.lstSPellList.Items.Add("Rebirth");
                    this.lstSPellList.Items.Add("Cure Poison");
                    this.lstSPellList.Items.Add("Remove Curse");
                    this.lstSPellList.Items.Add("Abolish Poison");
                    this.lstSPellList.Items.Add("Omen of Clarity");
                    this.lstSPellList.Items.Add("Tranquility");
                    this.lstSPellList.Items.Add("Nature's Swiftness");
                    this.lstSPellList.Items.Add("Swiftmend");
                    this.lstSPellList.Items.Add("Gift of the Wild");
                    this.lstSPellList.Items.Add("Lifebloom");
                    this.lstSPellList.Items.Add("Growl");
                    this.lstSPellList.Items.Add("Maul");
                    this.lstSPellList.Items.Add("Demoralizing Roar");
                    this.lstSPellList.Items.Add("Enrage");
                    this.lstSPellList.Items.Add("Bash");
                    this.lstSPellList.Items.Add("Swipe");
                    this.lstSPellList.Items.Add("Faerie Fire (Feral)");
                    this.lstSPellList.Items.Add("Feral Charge");
                    this.lstSPellList.Items.Add("Challenging Roar");
                    this.lstSPellList.Items.Add("Frenzied Regeneration");
                    this.lstSPellList.Items.Add("Lacerate	");
                    this.lstSPellList.Items.Add("Claw");
                    this.lstSPellList.Items.Add("Prowl");
                    this.lstSPellList.Items.Add("Rip");
                    this.lstSPellList.Items.Add("Shred");
                    this.lstSPellList.Items.Add("Rake");
                    this.lstSPellList.Items.Add("Tiger's Fury");
                    this.lstSPellList.Items.Add("Dash");
                    this.lstSPellList.Items.Add("Cower");
                    this.lstSPellList.Items.Add("Ferocious Bite");
                    this.lstSPellList.Items.Add("Ravage");
                    this.lstSPellList.Items.Add("Track Humanoid");
                    this.lstSPellList.Items.Add("Pounce");
                    this.lstSPellList.Items.Add("Feline Grace");
                    this.lstSPellList.Items.Add("Maim");

                    // Druid
                    #endregion
                    break;

                case ISXWoW.WoWClass.Shaman:
                    #region Shaman

                    this.lstSPellList.Items.Add("Healing Wave");
                    this.lstSPellList.Items.Add("Lightning Bolt");
                    this.lstSPellList.Items.Add("Rockbiter Weapon");
                    this.lstSPellList.Items.Add("Earth Shock");
                    this.lstSPellList.Items.Add("Stoneskin Totem");
                    this.lstSPellList.Items.Add("Earthbind Totem");
                    this.lstSPellList.Items.Add("Lightning Shield");
                    this.lstSPellList.Items.Add("Stoneclaw Totem");
                    this.lstSPellList.Items.Add("Flame Shock");
                    this.lstSPellList.Items.Add("Flametongue Weapon");
                    this.lstSPellList.Items.Add("Searing Totem");
                    this.lstSPellList.Items.Add("Strength of Earth Totem");
                    this.lstSPellList.Items.Add("Ancestral Spirit");
                    this.lstSPellList.Items.Add("Fire Nova Totem");
                    this.lstSPellList.Items.Add("Purge");
                    this.lstSPellList.Items.Add("Cure Poison");
                    this.lstSPellList.Items.Add("Tremor Totem");
                    this.lstSPellList.Items.Add("Frost Shock");
                    this.lstSPellList.Items.Add("Frostbrand Weapon");
                    this.lstSPellList.Items.Add("Healing Stream Totem");
                    this.lstSPellList.Items.Add("Lesser Healing Wave");
                    this.lstSPellList.Items.Add("Cure Disease");
                    this.lstSPellList.Items.Add("Poison Cleansing Totem");
                    this.lstSPellList.Items.Add("Water Breathing");
                    this.lstSPellList.Items.Add("Frost Resistance Totem");
                    this.lstSPellList.Items.Add("Magma Totem");
                    this.lstSPellList.Items.Add("Mana Spring Totem");
                    this.lstSPellList.Items.Add("Fire Resistance Totem");
                    this.lstSPellList.Items.Add("Flametongue Totem");
                    this.lstSPellList.Items.Add("Water Walking");
                    this.lstSPellList.Items.Add("Grounding Totem");
                    this.lstSPellList.Items.Add("Nature Resistance Totem");
                    this.lstSPellList.Items.Add("Totemic Call");
                    this.lstSPellList.Items.Add("Windfury Weapon");
                    this.lstSPellList.Items.Add("Chain Lightning");
                    this.lstSPellList.Items.Add("Windfury Totem");
                    this.lstSPellList.Items.Add("Sentry Totem");
                    this.lstSPellList.Items.Add("Windwall Totem");
                    this.lstSPellList.Items.Add("Disease Cleansing Totem");
                    this.lstSPellList.Items.Add("Chain Heal");
                    this.lstSPellList.Items.Add("Grace of Air Totem");
                    this.lstSPellList.Items.Add("Tranquil Air Totem");
                    this.lstSPellList.Items.Add("Elemental Mastery");
                    this.lstSPellList.Items.Add("Elemental Fury");
                    this.lstSPellList.Items.Add("Totem of Wrath");
                    this.lstSPellList.Items.Add("Lightning Overload");
                    this.lstSPellList.Items.Add("Stormstrike");
                    this.lstSPellList.Items.Add("Shamanistic Rage");
                    this.lstSPellList.Items.Add("Nature's Swiftness");
                    this.lstSPellList.Items.Add("Mana Tide Totem");
                    this.lstSPellList.Items.Add("Restorative Totems");
                    this.lstSPellList.Items.Add("Earth Shield");
                    this.lstSPellList.Items.Add("Nature's Blessing");
                    this.lstSPellList.Items.Add("Water Shield");
                    this.lstSPellList.Items.Add("Wrath of Air Totem");
                    this.lstSPellList.Items.Add("Earth Elemental Totem");
                    this.lstSPellList.Items.Add("Fire Elemental Totem");
                    this.lstSPellList.Items.Add("Bloodlust");
                    this.lstSPellList.Items.Add("Heroism");

                    // Shaman
                    #endregion
                    break;
            }

            // sort the list
            this.lstSPellList.Sorted = true;
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            // save the settings
            try
            {
                // if we have no level, then exit
                if (clsGlobals.SettingsLevel < 1)
                {
                    MessageBox.Show(Resources.CombatSettingsLevelMsg);
                    return;
                }

                // set wait cursor
                this.Cursor = Cursors.WaitCursor;

                // pop lists
                List<string> CombatDOT = clsSettings.BuildSaveList(this.txtCombatDOT);
                List<string> SpamSpells = clsSettings.BuildSaveList(this.txtCombatSpam);
                List<string> PreCombatBuff = clsSettings.BuildSaveList(this.txtPreCombatBuff);
                List<string> PostCombat = clsSettings.BuildSaveList(this.txtPostCombatSpells);

                // save setting per level
                int j = this.uscStartEndLevel1.EndLevel + 1;
                for (int i = this.uscStartEndLevel1.StartLevel; i < j; i++)
                {
                    // load the settings for this level
                    clsLevelSettings LevelSettings = clsSettings.LoadPanelSettings(i);

                    // update settings
                    LevelSettings.Combat_DoPanic = this.chkDoPanic.Checked;
                    LevelSettings.Combat_DOT_List = CombatDOT;
                    LevelSettings.Combat_SpamSpells_List = SpamSpells;
                    LevelSettings.Combat_HealingOT = this.txtHealingOverTime.Text;
                    LevelSettings.Combat_HealSpell = this.txtHealingSpell.Text;
                    LevelSettings.Combat_HealthPercent = (int)this.txtHealPct.Value;
                    LevelSettings.Combat_ManaSpam = (int)this.txtManaSpam.Value;
                    LevelSettings.Combat_PreBuffSpells_List = PreCombatBuff;
                    LevelSettings.Combat_ProtectionSpell = this.txtProtectionSpell.Text;
                    LevelSettings.Combat_PullSpell = this.txtPullSpell.Text;
                    LevelSettings.Combat_WaitTime_ms = (int)this.txtCombatWait.Value;
                    LevelSettings.Combat_DowntimePercent = (int)this.txtDowntime.Value;
                    LevelSettings.Combat_PanicThreshold = (int)this.txtPanicThreshold.Value;
                    LevelSettings.Combat_CastSpamRandomly = this.chkSpamRandom.Checked;
                    LevelSettings.Combat_PostCombatSpells = PostCombat;
                    LevelSettings.Combat_StopRunawayAttemptSpell = this.txtStopRunAwayAttempt.Text;

                    // save the settings files
                    clsSettings.SavePanelSettings(i, LevelSettings);
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatSettings, Resources.SaveSettings);
            }

            finally
            {
                // set cursor
                this.Cursor = Cursors.Default;
            }
        }

        #region Hide Controls By Class

        /// <summary>
        /// Hide the controls based on current class
        /// </summary>
        private void HideControls()
        {
            try
            {
                // exit if no character
                if (!clsCharacter.CharacterIsValid)
                    return;

                switch (clsCharacter.MyClass)
                {
                    case WoWClass.Druid:
                    case WoWClass.Hunter:
                    case WoWClass.Mage:
                    case WoWClass.Paladin:
                    case WoWClass.Priest:
                    case WoWClass.Rogue:
                    case WoWClass.Warlock:
                        break;

                    case WoWClass.Warrior:
                        this.txtPullSpell.Visible = false;
                        this.lblPullSpell.Visible = false;
                        break;
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatSettings, Resources.HideCombatSettingsErr);
            }            
        }

        // Hide Controls By Class
        #endregion

        #region Drag_and_Drop

        // http://www.codeproject.com/KB/cs/dandtutorial.aspx
        // http://www.codeproject.com/KB/dotnet/csdragndrop01.aspx

        /// <summary>
        /// Raised on mousedown of the spell list box
        /// </summary>
        private void lstSPellList_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                // get the selected index
                int index = this.lstSPellList.IndexFromPoint(e.X, e.Y);

                // exit if nothing selected
                if (index == ListBox.NoMatches)
                    return;

                // start drag/drop effects
                DragDropEffects dde = DoDragDrop(this.lstSPellList.Items[index].ToString(), DragDropEffects.Copy);
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatSettings, "SpellList_MouseDown");
            }            
        }

        /// <summary>
        /// Raised on a mouse over during drag/drop
        /// </summary>
        private void TextBox_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.Text))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatSettings, "DragDrop");
            }
        }

        /// <summary>
        /// Raised on a drop of a multi line textbox
        /// </summary>
        private void MultiTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // make sure this is a string
                if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    // get the data
                    string data = e.Data.GetData(System.Windows.Forms.DataFormats.StringFormat) as string;

                    // update text if we have something
                    if (!string.IsNullOrEmpty(data))
                    {
                        // get the textbox we are using
                        TextBox txtBox = (TextBox)sender;

                        // update text
                        if (string.IsNullOrEmpty(txtBox.Text))
                            txtBox.Text = data;
                        else
                            txtBox.Text = string.Format("{0}\r\n{1}", txtBox.Text, data);
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatSettings, "DragDrop");
            }            
        }

        /// <summary>
        /// Raised on a drop of a single line textbox
        /// </summary>
        private void SingleTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // make sure this is a string
                if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    // get the data
                    string data = e.Data.GetData(System.Windows.Forms.DataFormats.StringFormat) as string;

                    // update text if we have something
                    if (!string.IsNullOrEmpty(data))
                    {
                        // get the textbox we are using and update it
                        TextBox txtBox = (TextBox)sender;
                        txtBox.Text = data;
                    }
                }
            }

            catch (Exception excep)
            {
                clsError.ShowError(excep, Resources.uscCombatSettings, "DragDrop");
            }
        }

        // Drag_and_Drop
        #endregion
    }
}
