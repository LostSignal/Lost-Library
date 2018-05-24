//-----------------------------------------------------------------------
// <copyright file="TintGroup.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.UI;

    public class TintGroup : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Color tintColor = Color.white;
        #pragma warning restore 0649

        private Graphic[] graphics;

        public Color TintColor
        {
            get
            {
                return this.tintColor;
            }

            set
            {
                if (this.tintColor != value)
                {
                    this.tintColor = value;
                    this.SetColor();
                }
            }
        }

        private void Awake()
        {
            this.graphics = this.GetComponentsInChildren<Graphic>();
            this.SetColor();
        }

        private void SetColor()
        {
            if (this.graphics == null)
            {
                return;
            }

            for (int i = 0; i < this.graphics.Length; i++)
            {
                this.graphics[i].color = this.tintColor;
            }
        }
    }
}
