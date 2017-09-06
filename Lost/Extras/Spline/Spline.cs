
namespace Lost
{
    using System.Linq;
    using UnityEngine;

    [ExecuteInEditMode]
    public class Spline : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private bool isLooping;
        #pragma warning restore 0649

        // used to calculate the length of the spline
        private SplinePoint[] children;
        private float splineLength;

        // used for caching
        private float lastDesiredLength = float.MaxValue;
        private float currentSplineLength = 0;
        private int cachedIndex = 0;

        public bool IsLooping
        {
            get { return this.isLooping; }
        }

        public float SplineLength
        {
            get { return this.splineLength; }
        }

        public static Vector3 Interpolate(SplinePoint p1, SplinePoint p2, float percentage)
        {
            Vector3 v1 = p1.transform.position;
            Vector3 v2 = p1.transform.position + (p1.transform.rotation * p1.OutHandle);
            Vector3 v3 = p2.transform.position + (p2.transform.rotation * p2.InHandle);
            Vector3 v4 = p2.transform.position;

            return new Vector3(
                Interpolate(v1.x, v2.x, v3.x, v4.x, percentage),
                Interpolate(v1.y, v2.y, v3.y, v4.y, percentage),
                Interpolate(v1.z, v2.z, v3.z, v4.z, percentage));
        }

        public Vector3 Evaluate(float desiredLength)
        {
            // early out if we've reached the end
            if (desiredLength > this.splineLength)
            {
                return this.isLooping ? this.children.First().transform.position : this.children.Last().transform.position;
            }

            // if this desiredLength isn't greater than the last, then start from beginning and don't used cached values
            if (desiredLength < lastDesiredLength)
            {
                this.cachedIndex = 0;
                this.currentSplineLength = 0.0f;
            }

            // cache the desired length to test against for next time
            this.lastDesiredLength = desiredLength;

            for (int i = cachedIndex; i < this.children.Length; i++)
            {
                float currentLength = desiredLength - this.currentSplineLength;
                float childLength = this.children[i].Length;

                if (currentLength <= childLength)
                {
                    return Interpolate(this.children[i], this.children[i].Next, currentLength / childLength);
                }
                else
                {
                    this.currentSplineLength += childLength;
                    this.cachedIndex++;
                }
            }

            Debug.LogError("Unable to correctly evaluate spline", this);
            return this.children[0].transform.position;
        }

        private void Awake()
        {
            if (Application.isPlaying == false)
            {
                // creating two spline point children if none exist in editor mode
                if (this.GetComponentsInChildren<SplinePoint>().Length == 0)
                {
                    var p1 = new GameObject("SplinePoint (0)", typeof(SplinePoint));
                    p1.transform.SetParent(this.transform);
                    p1.transform.localPosition = Vector3.zero;

                    var p2 = new GameObject("SplinePoint (1)", typeof(SplinePoint));
                    p2.transform.SetParent(this.transform);
                    p2.transform.localPosition = Vector3.one;
                }

                return;
            }

            this.children = this.GetComponentsInChildren<SplinePoint>();

            Debug.Assert(this.children.Length > 1, "Spline doesn't have enough child nodes.  Must have at least 2 spline points.", this);
            Debug.Assert(this.children.Length == this.transform.childCount, "Spline contains unknown children.  All children must be of type SplinePoint.", this);

            // getting the total length by summing the children
            for (int i = 0; i < this.children.Length; i++)
            {
                this.children[i].Initialize();
                this.splineLength += this.children[i].Length;
            }
        }

        private static float Interpolate(float p0, float p1, float p2, float p3, float t)
        {
            // formula from "Cubic Bézier curves" section on http://en.wikipedia.org/wiki/B%C3%A9zier_curve
            return (1.0f - t) * (1.0f - t) * (1.0f - t) * p0 +
                   3 * (1.0f - t) * (1.0f - t) * t * p1 +
                   3 * (1.0f - t) * t * t * p2 +
                   t * t * t * p3;
        }
        
        #if UNITY_EDITOR
        private void Update()
        {
            this.AutoOrientChildPoints();
        }
        
        private void AutoOrientChildPoints()
        {
            int childCount = this.transform.childCount;

            for (int currentIndex = 0; currentIndex < childCount; currentIndex++)
            {
                SplinePoint currentPoint = this.transform.GetChild(currentIndex).GetComponent<SplinePoint>();

                if (currentPoint.AutoOrient == false)
                {
                    continue;
                }

                int previousIndex;
                int nextIndex;

                if (this.IsLooping)
                {
                    previousIndex = currentIndex == 0 ? childCount - 1 : currentIndex - 1;
                    nextIndex = (currentIndex == (childCount - 1)) ? 0 : currentIndex + 1;
                }
                else
                {
                    previousIndex = currentIndex == 0 ? 0 : currentIndex - 1;
                    nextIndex = (currentIndex == (childCount - 1)) ? childCount - 1 : currentIndex + 1;
                }

                Vector3 previousPosition = this.transform.GetChild(previousIndex).position;
                Vector3 currentPosition = currentPoint.transform.position;
                Vector3 nextPosition = this.transform.GetChild(nextIndex).position;

                // setting the spline points rotation
                Vector3 direction = nextPosition - previousPosition;
                currentPoint.transform.LookAt(currentPosition + direction);

                // setting the In/Out tangent lengths
                currentPoint.InHandle = new Vector3(0, 0, -Mathf.Max(0.25f, Vector3.Magnitude(previousPosition - currentPosition) * 0.3f));
                currentPoint.OutHandle = new Vector3(0, 0, Mathf.Max(0.25f, Vector3.Magnitude(nextPosition - currentPosition) * 0.3f));
            }
        }
        #endif
    }
}
