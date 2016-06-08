//-----------------------------------------------------------------------
// <copyright file="DialogManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class DialogManager : SingletonGameObject<DialogManager>
    {
        private DialogStatus currentDialog;

        public DialogStatus ShowDialog(Dialog dialog)
        {
            // TODO this is place holder till we have queue working, and also passive vs interactive queues
            if (this.currentDialog != null)
            {
                return null;
            }

            if (this.currentDialog != null && object.ReferenceEquals(dialog, this.currentDialog.DialogPrefab))
            {
                Logger.LogError(dialog, "Same Dialog was shows twice in a row!");
                return this.currentDialog;
            }

            this.currentDialog = new DialogStatus();
            this.currentDialog.CurrentState = DialogStatus.State.Running;
            this.currentDialog.DialogPrefab = dialog;
            this.currentDialog.DialogInstance = GameObject.Instantiate<Dialog>(dialog);

            GameObject.DontDestroyOnLoad(this.currentDialog.DialogInstance);

            return this.currentDialog;
        }
                
        public void DialogFinished()
        {
            this.currentDialog.CurrentState = DialogStatus.State.Done;
        }

        public void Update()
        {
            if (this.currentDialog != null)
            {
                if (this.currentDialog.CurrentState == DialogStatus.State.Done)
                {
                    GameObject.DestroyObject(this.currentDialog.DialogInstance.gameObject);
                    this.currentDialog = null;
                }
            }
        }
    }
}