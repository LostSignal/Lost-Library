//-----------------------------------------------------------------------
// <copyright file="UIUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public static class UIUtil
    {
        public static void InsureCanvasExists()
        {
            // making sure a canvas exists
            var canvasComponent = GameObject.FindObjectOfType<Canvas>();

            if (canvasComponent == null)
            {
                var canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }

            // making sure the event system exists
            var eventSystemComponent = GameObject.FindObjectOfType<EventSystem>();

            if (eventSystemComponent == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }
    }
}
