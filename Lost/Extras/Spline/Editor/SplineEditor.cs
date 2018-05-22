
namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor
    {
        private void OnSceneGUI()
        {
            DrawSpline((Spline)target);
        }

        public static void DrawSpline(Spline spline)
        {
            if (spline == null)
            {
                return;
            }

            int childCount = spline.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                int index1 = i;
                int index2 = i + 1;

                if (index2 >= childCount)
                {
                    if (spline.IsLooping == false)
                    {
                        break;
                    }
                    else
                    {
                        index2 = 0;
                    }
                }

                var point1 = spline.transform.GetChild(index1).GetComponent<SplinePoint>();
                var point2 = spline.transform.GetChild(index2).GetComponent<SplinePoint>();

                DrawInOutTangents(point1);
                DrawInOutTangents(point2);

                Handles.DrawBezier(
                    point1.transform.position,
                    point2.transform.position,
                    point1.transform.position + (point1.transform.rotation * point1.OutHandle),
                    point2.transform.position + (point2.transform.rotation * point2.InHandle),
                    Color.white,
                    null,
                    2.0f);
            }
        }

        private static void DrawInOutTangents(SplinePoint splinePoint)
        {
            Handles.color = Color.blue;
            Handles.DrawLine(splinePoint.transform.position, splinePoint.transform.position + (splinePoint.transform.rotation * splinePoint.InHandle));
            Handles.DrawLine(splinePoint.transform.position, splinePoint.transform.position + (splinePoint.transform.rotation * splinePoint.OutHandle));
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        private static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
        {
            DrawSpline(objectTransform.GetComponent<Spline>());
        }
    }
}
