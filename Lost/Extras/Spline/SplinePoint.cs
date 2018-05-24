
namespace Lost
{
    using UnityEngine;

    public class SplinePoint : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private bool autoOrient = true;
        [SerializeField] private Vector3 inHandle = new Vector3(0, 0, -0.25f);
        [SerializeField] private Vector3 outHandle = new Vector3(0, 0, 0.25f);
        #pragma warning restore 0649

        private bool isIntialized;
        private Spline parent;
        private SplinePoint nextPoint;
        private float length;

        public bool AutoOrient
        {
            get { return this.autoOrient; }
        }

        public Vector3 InHandle
        {
            get { return this.inHandle; }

            #if UNITY_EDITOR
            set { this.inHandle = value; }
            #endif
        }

        public Vector3 OutHandle
        {
            get { return this.outHandle; }

            #if UNITY_EDITOR
            set { this.outHandle = value; }
            #endif
        }

        public SplinePoint Next
        {
            get { return this.nextPoint; }
        }

        public float Length
        {
            get { return this.length; }
        }

        public void Initialize()
        {
            if (this.isIntialized)
            {
                return;
            }

            this.parent = this.GetComponentInParent<Spline>();

            Debug.Assert(parent != null, "SplinePoint must be directly under a Spline object.", this);

            int myIndex = this.transform.GetSiblingIndex();

            // testing if we're the last one in the spline
            if (myIndex == this.parent.transform.childCount - 1)
            {
                this.nextPoint = this.parent.IsLooping ? this.parent.transform.GetChild(0).GetComponent<SplinePoint>() : null;
            }
            else
            {
                this.nextPoint = this.parent.transform.GetChild(myIndex + 1).GetComponent<SplinePoint>();
            }

            if (this.nextPoint != null)
            {
                this.length = this.GetLengthBetweenSplinePoints(this, this.nextPoint);
            }


            this.isIntialized = true;
        }

        private void Awake()
        {
            this.Initialize();
        }

        private float GetLengthBetweenSplinePoints(SplinePoint p1, SplinePoint p2)
        {
            float length = 0.0f;

            for (int i = 0; i < 100; i++)
            {
                Vector3 v1 = Spline.Interpolate(p1, p2, (i + 0) / 100.0f);
                Vector3 v2 = Spline.Interpolate(p1, p2, (i + 1) / 100.0f);
                length += (v2 - v1).magnitude;
            }

            return length;
        }
    }
}
