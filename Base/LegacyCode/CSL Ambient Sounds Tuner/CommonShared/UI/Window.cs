using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace AmbientSoundsTuner2.CommonShared.UI
{
    /// <summary>
    /// A default options window.
    /// </summary>
    public class Window : UIPanel
    {
        /// <summary>
        /// The title bar height.
        /// </summary>
        protected const float TitleBarHeight = 40;

        /// <summary>
        /// Starts this window.
        /// </summary>
        public override void Start()
        {
            base.Start();
            this.Hide();
            this.backgroundSprite = "UnlockingPanel2";
            UITextureAtlas atlas = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(a => a.name == "Ingame");
            if (atlas != null)
            {
                this.atlas = atlas;
            }
            this.color = new Color32(58, 88, 104, 255);
            this.canFocus = true;
            this.isInteractive = true;
            this.autoSize = true;

            this.CreateTitle();
            this.CreateDragHandle();
            this.CreateCloseButton();
            this.CreateContentPanel();
        }

        /// <summary>
        /// Closes this window.
        /// </summary>
        public virtual void Close()
        {
            this.Hide();
            if (this.parent != null)
            {
                this.parent.Focus();
            }
        }

        private string title = "";
        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        /// <value>
        /// The window title.
        /// </value>
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                if (this.TitleObject != null)
                {
                    this.TitleObject.GetComponent<UILabel>().text = value;
                }
            }
        }

        /// <summary>
        /// Gets the content panel.
        /// </summary>
        /// <value>
        /// The content panel.
        /// </value>
        public UIPanel ContentPanel
        {
            get
            {
                return this.ContentPanelObject.GetComponent<UIPanel>();
            }
        }

        /// <summary>
        /// Gets the title object.
        /// </summary>
        /// <value>
        /// The title object.
        /// </value>
        protected GameObject TitleObject { get; private set; }
        /// <summary>
        /// Gets the drag handle object.
        /// </summary>
        /// <value>
        /// The drag handle object.
        /// </value>
        protected GameObject DragHandleObject { get; private set; }
        /// <summary>
        /// Gets the close button object.
        /// </summary>
        /// <value>
        /// The close button object.
        /// </value>
        protected GameObject CloseButtonObject { get; private set; }
        /// <summary>
        /// Gets the content panel object.
        /// </summary>
        /// <value>
        /// The content panel object.
        /// </value>
        protected GameObject ContentPanelObject { get; private set; }

        /// <summary>
        /// Creates the title.
        /// </summary>
        protected virtual void CreateTitle()
        {
            this.TitleObject = new GameObject("Title");
            this.TitleObject.transform.parent = this.transform;
            this.TitleObject.transform.localPosition = Vector3.zero;

            UILabel title = this.TitleObject.AddComponent<UILabel>();
            title.text = this.title;
            title.textAlignment = UIHorizontalAlignment.Center;
            title.position = new Vector3(this.width / 2 - title.width / 2, -TitleBarHeight / 2 + title.height / 2).RoundToInt();
            title.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
            title.atlas = this.atlas;
        }

        /// <summary>
        /// Creates the drag handle.
        /// </summary>
        protected virtual void CreateDragHandle()
        {
            this.DragHandleObject = new GameObject("DragHandler");
            this.DragHandleObject.transform.parent = this.transform;
            this.DragHandleObject.transform.localPosition = Vector3.zero;

            UIDragHandle dragHandle = this.DragHandleObject.AddComponent<UIDragHandle>();
            dragHandle.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
            dragHandle.size = new Vector2(this.width, TitleBarHeight);
        }

        /// <summary>
        /// Creates the close button.
        /// </summary>
        protected virtual void CreateCloseButton()
        {
            this.CloseButtonObject = new GameObject("CloseButton");
            this.CloseButtonObject.transform.parent = this.transform;
            this.CloseButtonObject.transform.localPosition = Vector3.zero;

            UIButton closeButton = this.CloseButtonObject.AddComponent<UIButton>();
            closeButton.anchor = UIAnchorStyle.Top | UIAnchorStyle.Right;
            closeButton.size = new Vector2(32, 32);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.relativePosition = new Vector3(this.width - closeButton.width - 4, 4);
            closeButton.playAudioEvents = true;
            closeButton.eventClick += (component, eventParam) => this.Close();
            closeButton.atlas = this.atlas;
        }

        /// <summary>
        /// Creates the content panel.
        /// </summary>
        protected virtual void CreateContentPanel()
        {
            this.ContentPanelObject = new GameObject("ContentPanel");
            this.ContentPanelObject.transform.parent = this.transform;
            this.ContentPanelObject.transform.localPosition = Vector3.zero;

            UIPanel contentPanel = this.ContentPanelObject.AddComponent<UIPanel>();
            contentPanel.anchor = UIAnchorStyle.Bottom | UIAnchorStyle.Left | UIAnchorStyle.Right;
            contentPanel.position = new Vector3(0, -TitleBarHeight);
            contentPanel.size = new Vector2(this.width, Mathf.Max(0, this.height - TitleBarHeight));
            contentPanel.atlas = this.atlas;
        }

        /// <summary>
        /// Called when a key is pressed.
        /// </summary>
        /// <param name="p">The event parameter.</param>
        protected override void OnKeyDown(UIKeyEventParameter p)
        {
            if (!p.used && p.keycode == KeyCode.Escape)
            {
                this.Close();
                p.Use();
            }
            base.OnKeyDown(p);
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void ShowWindow(Window window)
        {
            window.CenterToParent();
            window.Show(true);
            window.Focus();
        }
    }
}
