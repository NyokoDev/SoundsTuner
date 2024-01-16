using AlgernonCommons.Translation;
using AlgernonCommons.UI;
using ColossalFramework.UI;

namespace POAIDBOX
{
    public sealed class LegacyTab
    {
        private AutoTabstrip tabStrip;
        private int tabindex;
        private const float Margin = 5f;
        float currentY = Margin;

        internal LegacyTab(AutoTabstrip tabStrip, int tabindex)
        {
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, "Test", tabindex, out UIButton _, 170f);


   



            this.tabStrip = tabStrip;
            this.tabindex = 2;
        }
    }
}