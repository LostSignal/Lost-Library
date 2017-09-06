//-----------------------------------------------------------------------
// <copyright file="WaitForButtonClick.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.UI;

    public class WaitForButtonClick : CustomYieldInstruction
    {
        private Button button;
        private bool isDone;

        public override bool keepWaiting
        {
            get { return this.isDone == false; }
        }

        public WaitForButtonClick(Button button)
        {
            this.isDone = false;
            this.button = button;
            this.button.onClick.AddListener(this.OnClick);
        }

        private void OnClick()
        {
            this.isDone = true;
            this.button.onClick.RemoveListener(this.OnClick);
        }
    }
}
