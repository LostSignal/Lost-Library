//-----------------------------------------------------------------------
// <copyright file="TimespanText.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    #if USE_TEXTMESH_PRO
    using Text = TMPro.TextMeshProUGUI;
    #else
    using Text = UnityEngine.UI.Text;
    #endif

    [RequireComponent(typeof(Text))]
    public class TimespanText : MonoBehaviour
    {
        private Text text;
        private int seconds;

        public int Seconds
        {
            get
            {
                return this.seconds;
            }

            set
            {
                if (this.seconds != value)
                {
                    this.seconds = value;
                    this.UpdateText();
                }
            }
        }

        private void Awake()
        {
            this.text = this.GetComponent<Text>();
        }

        private void OnEnable()
        {
            this.UpdateText();
        }
        
        private void UpdateText()
        {
            // update text can be called before Awake is called, so this is very necessary, but this will get called again OnEnable
            if (this.text == null)
            {
                return;
            }

            int hours = this.seconds / 60 / 60;
            int minutes = this.seconds / 60;
            int seconds = this.seconds % 60;

            if (hours > 0)
            {
                minutes -= hours * 60;
                this.text.text = string.Format("{0}:{1:D2}:{2:D2}", hours, minutes, seconds);
            }
            else
            {
                this.text.text = string.Format("{0}:{1:D2}", minutes, seconds);
            }
        }
    }
}
