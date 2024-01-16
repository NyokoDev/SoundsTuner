using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POAIDBOX
{
    public class UIHelperAid : ICities.UIHelperBase
    {
        private UIComponent m_Root;

      
        public object self => m_Root;
        public UIHelperAid(UIComponent panel)
        {
            m_Root = panel;
        }

        public UIHelperBase AddGroup(string text)
        {
            throw new NotImplementedException();
        }

        public object AddButton(string text, OnButtonClicked eventCallback)
        {
            throw new NotImplementedException();
        }

        public object AddSpace(int height)
        {
            throw new NotImplementedException();
        }

        public object AddCheckbox(string text, bool defaultValue, OnCheckChanged eventCallback)
        {
            throw new NotImplementedException();
        }

        public object AddSlider(string text, float min, float max, float step, float defaultValue, OnValueChanged eventCallback)
        {
            throw new NotImplementedException();
        }

        public object AddDropdown(string text, string[] options, int defaultSelection, OnDropdownSelectionChanged eventCallback)
        {
            throw new NotImplementedException();
        }

        public object AddTextfield(string text, string defaultContent, OnTextChanged eventChangedCallback, OnTextSubmitted eventSubmittedCallback = null)
        {
            throw new NotImplementedException();
        }
    }
}

