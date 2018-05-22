//-----------------------------------------------------------------------
// <copyright file="RectTransformContextMenus.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// http://answers.unity3d.com/questions/782478/unity-46-beta-anchor-snap-to-button-new-ui-system.html
    /// </summary>
    public static class RectTransformContextMenus
    {
        [MenuItem("CONTEXT/RectTransform/Anchors to Corners %[")]
        public static void AnchorsToCorners()
        {
            foreach (var selectedObject in Selection.objects.OfType<GameObject>())
            {
                RectTransform rectTransform = selectedObject.GetComponent<RectTransform>();
                RectTransform parentTransform = rectTransform.parent as RectTransform;

                if (rectTransform == null || parentTransform == null)
                {
                    continue;
                }

                Vector2 newAnchorsMin = new Vector2(rectTransform.anchorMin.x + rectTransform.offsetMin.x / parentTransform.rect.width,
                                                    rectTransform.anchorMin.y + rectTransform.offsetMin.y / parentTransform.rect.height);

                Vector2 newAnchorsMax = new Vector2(rectTransform.anchorMax.x + rectTransform.offsetMax.x / parentTransform.rect.width,
                                                    rectTransform.anchorMax.y + rectTransform.offsetMax.y / parentTransform.rect.height);

                rectTransform.anchorMin = newAnchorsMin;
                rectTransform.anchorMax = newAnchorsMax;
                rectTransform.offsetMin = rectTransform.offsetMax = new Vector2(0, 0);
            }
        }
    }
}
