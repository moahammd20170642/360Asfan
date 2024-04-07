using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CrizGames.Vour
{
    public class InfoPanel : Panel
    {
        public bool CustomPanel = false;
        public GameObject CustomPanelObject;

        public string Title = "Awesome Title";

        public Sprite Image;
        public InfoPoint.InfoPanelImageType PanelType;

        [TextArea(5, 10)]
        public string Text = "Interesting text.";

        public override Transform panelParent => transform;

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            if (CustomPanel && CustomPanelObject == null)
            {
                Debug.LogError($"Info Point \"{gameObject.name}\" has no Custom Panel Object set!");
            }

            base.Start();
        }

        /// <summary>
        /// InitPanel
        /// </summary>
        public override void InitPanel()
        {
            if (CustomPanel)
                return;

            name = $"Info Panel ({Title})";
            panel.GetChild(1).GetComponent<TextMeshProUGUI>().text = Title;
            panel.GetChild(2).GetComponent<TextMeshProUGUI>().text = Text;
            if (panel.childCount > 3)
                panel.GetChild(3).GetComponent<UnityEngine.UI.Image>().sprite = Image;
        }

        /// <summary>
        /// FindPanel
        /// </summary>
        /// <returns>Transform of panel</returns>
        protected override Transform FindPanel()
        {
            if (transform.childCount > 0)
                return transform.GetChild(0);
            else 
                return null;
        }
    }
}
