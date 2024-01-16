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

            UISprite image2Sprite = this.AddUIComponent<UISprite>();
            image2Sprite.height = 1000f;
            image2Sprite.relativePosition = new Vector3(0f, -50f);
            image2Sprite.width = 1000f;
            image2Sprite.atlas = UITextures.LoadSingleSpriteAtlas("..\\Resources\\bck");
            image2Sprite.spriteName = "normal";
            image2Sprite.zOrder = 1;
            Background = true;


            Tabstrip();
        }


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
            }
            else
            {
                UnityEngine.Debug.Log("Background is false");
            }
        }
    }
}
