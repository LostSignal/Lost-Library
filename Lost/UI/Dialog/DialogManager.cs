//-----------------------------------------------------------------------
// <copyright file="DialogManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class DialogManager : SingletonGameObject<DialogManager>
    {
        private LinkedList<Dialog> dialogs = new LinkedList<Dialog>();

        public void AddDialog(Dialog dialog)
        {
            if (dialog != null && dialog.RegisterForBackButton && this.dialogs.Contains(dialog) == false)
            {
                this.dialogs.AddLast(dialog);
            }
        }

        public void RemoveDialog(Dialog dialog)
        {
            if (dialog != null && dialog.RegisterForBackButton && this.dialogs.Contains(dialog))
            {
                this.dialogs.Remove(dialog);
            }
        }

        public void BackButtonPressed()
        {
            if (this.dialogs.Count > 0)
            {
                this.dialogs.Last.Value.OnBackButtonPressed();
            }
        }
        
        #if UNITY_ANDROID || UNITY_EDITOR
        private void Update()
        {
            // NOTE [bgish]: this catches the Android Back Button
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                this.BackButtonPressed();
            }
        }
        #endif
    }
}
