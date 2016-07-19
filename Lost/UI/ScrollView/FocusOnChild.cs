//-----------------------------------------------------------------------
// <copyright file="FocusOnChild.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace IdioWords
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(ScrollRect))]
    public class FocusOnChild : MonoBehaviour
    {
        #pragma warning disable 0649
        [Header("Scroller Movement")]
        [SerializeField] private float scrollerSpeed = 2.0f;
        [SerializeField] private float kickInVelocity = 200.0f;

        [Header("Dot Properties")]
        [SerializeField] private Transform dotContainer;
        [SerializeField] private GameObject dotPrefab;
        [SerializeField] private float maxScale = 1.8f;
        [SerializeField] private float minScale = 1.0f;
        [SerializeField] private float dotScaleSpeed = 3.0f;
        #pragma warning restore 0649

        private ScrollRect scrollRect;

        public int ClosestChildIndex
        {
            get
            {
                int closestIndex = 0;
                float closestDistance = this.GetDistanceVector(0).sqrMagnitude;

                for (int i = 1; i < this.ChildCount; i++)
                {
                    float distance = this.GetDistanceVector(i).sqrMagnitude;

                    if (distance < closestDistance)
                    {
                        closestIndex = i;
                        closestDistance = distance;
                    }
                }

                return closestIndex;
            }
        }

        public int ChildCount
        {
            get
            {
                if (this.scrollRect == null)
                {
                    this.scrollRect = this.GetComponent<ScrollRect>();
                }

                return this.scrollRect.content.childCount;
            }
        }

        public void InitializeDots()
        {
            // making sure it hasn't already been initialized
            if (this.dotContainer.transform.childCount > 0)
            {
                return;
            }

            // initializing the dots
            if (this.dotPrefab != null && this.dotContainer != null)
            {
                for (int i = 0; i < this.ChildCount; i++)
                {
                    var newDot = GameObject.Instantiate<GameObject>(this.dotPrefab);
                    newDot.transform.SetParent(this.dotContainer);
                    newDot.transform.localScale = new Vector3(this.minScale, this.minScale, this.minScale);
                    newDot.transform.localPosition = Vector3.zero;
                }
            }
        }

        private void Awake()
        {
            this.scrollRect = this.GetComponent<ScrollRect>();
        }

        private void Start()
        {
            this.InitializeDots();
        }     

        private Vector3 GetDistanceVector(int childIndex)
        {
            return this.scrollRect.transform.position - this.scrollRect.content.transform.GetChild(childIndex).position;
        }
        
        private void Update()
        {
            // making sure we have something to scroll
            if (this.scrollRect.content.transform.childCount == 0)
            {
                return;
            }

            // checking if the minimum speed is met before we start moving it ourselves
            if (this.scrollRect.velocity.sqrMagnitude < (this.kickInVelocity * this.kickInVelocity))
            {
                float distance = this.GetDistanceVector(this.ClosestChildIndex).x;
                this.scrollRect.content.transform.position += new Vector3(distance, 0, 0) * (Time.deltaTime * this.scrollerSpeed);
            }

            // updating the dots
            if (this.dotPrefab != null && this.dotContainer != null)
            {
                Vector3 maxScale = new Vector3(this.maxScale, this.maxScale, this.maxScale);
                Vector3 minScale = new Vector3(this.minScale, this.minScale, this.minScale);

                for (int i = 0; i < this.ChildCount; i++)
                {
                    if (this.ClosestChildIndex == i)
                    {
                        this.dotContainer.GetChild(i).localScale = this.GrowTo(this.dotContainer.GetChild(i).localScale, maxScale);
                    }
                    else
                    {
                        this.dotContainer.GetChild(i).localScale = this.ShrinkTo(this.dotContainer.GetChild(i).localScale, minScale);
                    }
                }
            }
        }

        private Vector3 GrowTo(Vector3 current, Vector3 desired)
        {
            return Vector3.Min(current + (Vector3.one * (Time.deltaTime * this.dotScaleSpeed)), desired);
        }

        private Vector3 ShrinkTo(Vector3 current, Vector3 desired)
        {
            return Vector3.Max(current - (Vector3.one * (Time.deltaTime * this.dotScaleSpeed)), desired);
        }
    }
}
