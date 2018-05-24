//-----------------------------------------------------------------------
// <copyright file="KeyboardManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class KeyboardManager
    {
        private Dictionary<KeyCode, KeyboardButton> buttons = new Dictionary<KeyCode, KeyboardButton>();

        public void Update()
        {
            foreach (KeyValuePair<KeyCode, KeyboardButton> pair in this.buttons)
            {
                bool pressed = UnityEngine.Input.GetKey(pair.Key);
                pair.Value.Update(pressed);
            }
        }

        public KeyboardButton GetButton(KeyCode keyCode)
        {
            KeyboardButton button;

            if (this.buttons.TryGetValue(keyCode, out button) == false)
            {
                button = new KeyboardButton();
                button.Update(UnityEngine.Input.GetKey(keyCode));
                this.buttons.Add(keyCode, button);
            }

            return button;
        }
    }
}
