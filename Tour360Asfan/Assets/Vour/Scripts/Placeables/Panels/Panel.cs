using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrizGames.Vour
{
    public abstract class Panel : MonoBehaviour
    {
        [SerializeField] protected Transform _panel = null;
        public virtual Transform panel
        {
            get
            {
                if (_panel == null || !Application.isPlaying)
                    _panel = FindPanel();
                return _panel;
            }
        }

        public virtual Transform panelParent => panel.parent;

        /// <summary>
        /// Start
        /// </summary>
        protected virtual void Start()
        {
            InitPanel();
        }

        /// <summary>
        /// InitPanel
        /// </summary>
        public abstract void InitPanel();

        /// <summary>
        /// FindPanel
        /// </summary>
        /// <returns>Transform of panel</returns>
        protected abstract Transform FindPanel();
    }
}
