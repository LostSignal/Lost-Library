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

        private static Dictionary<System.Type, DialogLogic> dialogTypes = new Dictionary<System.Type, DialogLogic>();

        public static void RegisterDialog(DialogLogic dialogLogic)
        {
            dialogTypes.Add(dialogLogic.GetType(), dialogLogic);
        }

        public static void UnregisterDialog(DialogLogic dialogLogic)
        {
            dialogTypes.Remove(dialogLogic.GetType());
        }

        public static T GetDialog<T>() where T : DialogLogic
        {
            if (dialogTypes.TryGetValue(typeof(T), out DialogLogic dialogLogic))
            {
                return (T)dialogLogic;
            }

            return null;
        }

        public bool IsTopMostDialog(Dialog dialog)
        {
            return this.dialogs.Last != null && this.dialogs.Last.Value == dialog;
        }

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
                this.dialogs.Last.Value.BackButtonPressed();
            }
        }

        #if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
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
