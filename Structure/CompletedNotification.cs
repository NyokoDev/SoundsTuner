using AlgernonCommons.Notifications;
using AlgernonCommons.Translation;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POAIDBOX.Structure
{
    public class CompletedNotification : ListNotification
    {
        public UIButton _yesButton;

        /// <summary>
        /// Gets the 'No' button (button 1) instance.
        /// </summary>

        /// <summary>
        /// Gets the 'Yes' button (button 2) instance.
        /// </summary>
        public UIButton YesButton => _yesButton;

        /// <summary>
        /// Gets the number of buttons for this panel (for layout).
        /// </summary>
        protected override int NumButtons => 1;



        /// <summary>
        /// Adds buttons to the message box.
        /// </summary>
        public override void AddButtons()
        {

            // Add yes button.
            _yesButton = AddButton(1, NumButtons, "Understood", Close);

        }
    }
}

