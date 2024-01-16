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

namespace POAIDBOX
{
    /// <summary>
    /// The mod's settings options panel.
    /// </summary>
    public sealed class OptionsPanel : OptionsPanelBase
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;
        private const float LabelWidth = 40f;
        private const float TabHeight = 20f;

        public static float TabWidth = 200f;
        
        /// <summary>
        /// Performs on-demand panel setup.
        /// </summary>
        protected override void Setup()
        {
            autoLayout = false;
            float currentY = Margin;
            m_BackgroundSprite = "UnlockingPanel";

            UISprite image2Sprite = this.AddUIComponent<UISprite>();
            image2Sprite.height = 1000f;
            image2Sprite.relativePosition = new Vector3(0f, -50f);
            image2Sprite.width = 1000f;
            image2Sprite.atlas = UITextures.LoadSingleSpriteAtlas("..\\Resources\\bck");
            image2Sprite.spriteName = "normal";
            image2Sprite.zOrder = 1;


            AutoTabstrip tabStrip = AutoTabstrip.AddTabstrip(this, Margin, currentY, 300f, 1f, out _);
            new UpdatedTab(tabStrip, 1);
            new LegacyTab(tabStrip, 2);
            tabStrip.selectedIndex = 1;
            tabStrip.selectedIndex = 2;
         



       
        }
    }
}
