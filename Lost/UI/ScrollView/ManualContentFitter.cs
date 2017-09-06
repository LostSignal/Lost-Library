//-----------------------------------------------------------------------
// <copyright file="VerticalContentFitter.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    
    [RequireComponent(typeof(RectTransform))]
    public class ManualContentFitter : UIBehaviour
    {
        public enum GrowDirection
        {
            Vertical,
            Horizontal,
            Both,
        }

        #pragma warning disable 0649
        [SerializeField] private GrowDirection growDirection;
        [SerializeField] private float bottomPadding;
        [SerializeField] private float rightPadding;
        #pragma warning restore 0649
        
        private RectTransform rectTransform;

        public void Resize()
        {
            // using a coroutine runner, becuase it has to happen even if the gameobject is disabled
            CoroutineRunner.Instance.ExecuteAtEndOfFrame(this.ResizeInternal);
        }
        
        protected override void Awake()
        {
            base.Awake();
            this.rectTransform = this.GetComponent<RectTransform>();

            if (this.growDirection == GrowDirection.Horizontal || this.growDirection == GrowDirection.Both)
            {
                Debug.LogWarningFormat("ManualContentFitter has not been tested using GrowDirection {0}", this.growDirection.ToString());
            }
        }

        private void ResizeInternal()
        {
            if (!this.rectTransform || this.transform.childCount == 0)
            {
                return;
            }

            Vector3 scale = this.transform.lossyScale;
            Vector3[] corners = new Vector3[4];

            RectTransform rectTransform = (RectTransform)this.transform;
            rectTransform.GetWorldCorners(corners);

            float currentBottomY = corners[0].y / scale.y;
            float currentHeight = (corners[1].y - corners[0].y) / scale.y;

            float currentRightX = corners[3].x / scale.x;
            float currentWidth = (corners[3].x - corners[0].x) / scale.x;

            // setting the min to the first child in the list
            ((RectTransform)this.transform.GetChild(0)).GetWorldCorners(corners);
            float minBottomY = corners[0].y / scale.y;
            float maxRightX = corners[3].x / scale.x;

            // going through the rest of the children to find the actual min
            for (int i = 1; i < this.transform.childCount; i++)
            {
                ((RectTransform)this.transform.GetChild(i)).GetWorldCorners(corners);
                minBottomY = Mathf.Min(minBottomY, corners[0].y / scale.y);
                maxRightX = Mathf.Max(maxRightX, corners[3].x / scale.x);
            }

            float verticalGrowthAmmount = (currentBottomY - minBottomY) + this.bottomPadding;
            float horizontalGrowthAmmount = (maxRightX - currentRightX) + this.rightPadding;

            if (this.growDirection == GrowDirection.Vertical || this.growDirection == GrowDirection.Both)
            {
                this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight + verticalGrowthAmmount);
            }

            if (this.growDirection == GrowDirection.Horizontal || this.growDirection == GrowDirection.Both)
            {
                this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth + horizontalGrowthAmmount);
            }
        }
    }
}
