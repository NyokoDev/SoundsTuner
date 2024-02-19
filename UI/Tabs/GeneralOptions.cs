using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using AlgernonCommons.UI;
using ColossalFramework.UI;
using UnityEngine;
using System.Diagnostics;
using POAIDBOX.Structure;
using System.Diagnostics.Eventing.Reader;
using POAIDBOX;
using AmbientSoundsTuner2.SoundPack;
using AmbientSoundsTuner2.Sounds;

namespace POAIDBOX
{

    /// <summary>
    /// The mod's settings options panel.
    /// </summary>
    public sealed class OptionsPanel : OptionsPanelBase
    {
        public bool Background;
        // Layout constants.
        private const float Margin = 5f;
        float currentY = Margin;
        public static float TabWidth = 200f;

        /// <summary>
        /// Performs on-demand panel setup.
        /// </summary> 
        /// 
        
    protected override void Setup()
        {
            m_BackgroundSprite = "UnlockingPanel";
            Background = true;
            Tabstrip();
        }

        readonly UpdatedTab UpdatedTab;
        readonly SoundPacksManager SoundPacksManager;
        readonly SoundManager SoundManager;

        /// <summary>
        /// Creates the tabstrips.
        /// </summary>
        public void Tabstrip()
        {
            if (Background)
            {
                UITabstrip tabStrip = UITabstrips.AddTabstrip(this, 0f, 0f, OptionsPanelManager<OptionsPanel>.PanelWidth, OptionsPanelManager<OptionsPanel>.PanelHeight, out UITabContainer _);
                tabStrip.clipChildren = false;
                UpdatedTab updatedTab = new UpdatedTab(tabStrip, 0);
                LegacyTab legacyTab = new LegacyTab(tabStrip, 1);

                // Select the first tab.
                tabStrip.selectedIndex = -1;
                UpdatedTab.PopulateDropdown();
                SoundManager.InitializeSounds();
                SoundPacksManager.InitSoundPacks();
            }
            else
            {
                UnityEngine.Debug.Log("Background is false");
            }
        }
    }
}
