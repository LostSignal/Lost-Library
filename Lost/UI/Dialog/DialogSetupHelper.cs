//-----------------------------------------------------------------------
// <copyright file="DialogSetupHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class DialogSetupHelper : MonoBehaviour
    {
        #if UNITY_EDITOR

        private void Update()
        {
            Dialog dialog = this.GetComponent<Dialog>();

            if (dialog == null)
            {
                return;
            }

            bool needsBlocker = dialog.BlockInput || dialog.TapOutsideToDismiss;
            Transform blockerTransform = dialog.transform.Find("Blocker");

            // making sure the Blocker exists if it needs it
            if (needsBlocker && blockerTransform == null)
            {
                var blockerGameObject = dialog.gameObject.GetOrCreateChild("Blocker", typeof(InputBlocker), typeof(CanvasGroup));

                var image = blockerGameObject.GetComponent<Image>();
                image.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);

                var rectTransform = blockerGameObject.GetComponent<RectTransform>();
                rectTransform.SetAsFirstSibling();
                rectTransform.FitToParent();
            }
            else if (needsBlocker == false && blockerTransform != null)
            {
                GameObject.DestroyImmediate(blockerTransform.gameObject);
            }

            // making sure Content exists
            if (dialog.transform.Find("Content") == null)
            {
                var content = dialog.gameObject.GetOrCreateChild("Content", typeof(RectTransform));
                var contentRectTransform = content.GetComponent<RectTransform>();
                contentRectTransform.FitToParent();
            }
        }

        #endif
    }
}
