//-----------------------------------------------------------------------
// <copyright file="GizmosUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR

namespace Lost
{
    using UnityEngine;

    public static class GizmosUtil
    {
        public static void DrawWireCube(Transform transform)
        {
            Vector3 top1 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(+0.5f, 0.5f, +0.5f));
            Vector3 top2 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(+0.5f, 0.5f, -0.5f));
            Vector3 top3 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));
            Vector3 top4 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-0.5f, 0.5f, +0.5f));

            Vector3 bottom1 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(+0.5f, -0.5f, +0.5f));
            Vector3 bottom2 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(+0.5f, -0.5f, -0.5f));
            Vector3 bottom3 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));
            Vector3 bottom4 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-0.5f, -0.5f, +0.5f));

            Gizmos.DrawLine(top1, top2);
            Gizmos.DrawLine(top2, top3);
            Gizmos.DrawLine(top3, top4);
            Gizmos.DrawLine(top4, top1);

            Gizmos.DrawLine(bottom1, bottom2);
            Gizmos.DrawLine(bottom2, bottom3);
            Gizmos.DrawLine(bottom3, bottom4);
            Gizmos.DrawLine(bottom4, bottom1);

            Gizmos.DrawLine(bottom1, top1);
            Gizmos.DrawLine(bottom2, top2);
            Gizmos.DrawLine(bottom3, top3);
            Gizmos.DrawLine(bottom4, top4);
        }

        public static void DrawWireCylinder(Transform transform, int segments = 25)
        {
            Vector3 centerTop = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, 0.5f, 0));
            Vector3 centerBottom = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, -0.5f, 0));

            for (int i = 0; i < segments; i++)
            {
                float point1Theta = (360.0f / (float)segments) * i;
                Vector3 point1 = Quaternion.Euler(0, point1Theta, 0) * new Vector3(0.5f, 0, 0);

                float point2Theta = (360.0f / (float)segments) * ((i + 1) % segments);
                Vector3 point2 = Quaternion.Euler(0, point2Theta, 0) * new Vector3(0.5f, 0, 0);

                Vector3 topP1 = transform.localToWorldMatrix.MultiplyPoint(point1 + new Vector3(0, 0.5f, 0));
                Vector3 topP2 = transform.localToWorldMatrix.MultiplyPoint(point2 + new Vector3(0, 0.5f, 0));
                Vector3 bottomP1 = transform.localToWorldMatrix.MultiplyPoint(point1 + new Vector3(0, -0.5f, 0));
                Vector3 bottomP2 = transform.localToWorldMatrix.MultiplyPoint(point2 + new Vector3(0, -0.5f, 0));

                Gizmos.DrawLine(topP1, topP2);
                Gizmos.DrawLine(bottomP1, bottomP2);
                Gizmos.DrawLine(topP1, bottomP1);
                Gizmos.DrawLine(topP1, centerTop);
                Gizmos.DrawLine(bottomP1, centerBottom);
            }
        }

        public static void DrawWireCylinder(Vector3 position, float radius, float height, int segments = 25)
        {
            float halfHeight = height / 2.0f;
            Vector3 centerTop = position.AddToY(halfHeight);
            Vector3 centerBottom = position.AddToY(-halfHeight);

            for (int i = 0; i < segments; i++)
            {
                float point1Theta = (360.0f / (float)segments) * i;
                Vector3 point1 = Quaternion.Euler(0, point1Theta, 0) * new Vector3(radius, 0, 0);

                float point2Theta = (360.0f / (float)segments) * ((i + 1) % segments);
                Vector3 point2 = Quaternion.Euler(0, point2Theta, 0) * new Vector3(radius, 0, 0);

                Vector3 topP1 = (point1 + position).AddToY(halfHeight);
                Vector3 topP2 = (point2 + position).AddToY(halfHeight);
                Vector3 bottomP1 = (point1 + position).AddToY(-halfHeight);
                Vector3 bottomP2 = (point2 + position).AddToY(-halfHeight);

                Gizmos.DrawLine(topP1, topP2);
                Gizmos.DrawLine(bottomP1, bottomP2);
                Gizmos.DrawLine(topP1, bottomP1);
                Gizmos.DrawLine(topP1, centerTop);
                Gizmos.DrawLine(bottomP1, centerBottom);
            }
        }

        public static void DrawWireCylinder(Vector3 position, Cylinder cylinder, int segments = 25)
        {
            float halfHeight = cylinder.Height / 2.0f;
            Vector3 centerTop = position.AddToY(halfHeight);
            Vector3 centerBottom = position.AddToY(-halfHeight);

            for (int i = 0; i < segments; i++)
            {
                float point1Theta = (360.0f / (float)segments) * i;
                Vector3 point1 = Quaternion.Euler(0, point1Theta, 0) * new Vector3(cylinder.Radius, 0, 0);

                float point2Theta = (360.0f / (float)segments) * ((i + 1) % segments);
                Vector3 point2 = Quaternion.Euler(0, point2Theta, 0) * new Vector3(cylinder.Radius, 0, 0);

                Vector3 topP1 = (point1 + position).AddToY(halfHeight);
                Vector3 topP2 = (point2 + position).AddToY(halfHeight);
                Vector3 bottomP1 = (point1 + position).AddToY(-halfHeight);
                Vector3 bottomP2 = (point2 + position).AddToY(-halfHeight);

                Gizmos.DrawLine(topP1, topP2);
                Gizmos.DrawLine(bottomP1, bottomP2);
                Gizmos.DrawLine(topP1, bottomP1);
                Gizmos.DrawLine(topP1, centerTop);
                Gizmos.DrawLine(bottomP1, centerBottom);
            }
        }

        public static void DrawWireCircle(Vector3 position, float radius, int segments = 25)
        {
            for (int i = 0; i < segments; i++)
            {
                float point1Theta = (360.0f / (float)segments) * i;
                Vector3 point1 = Quaternion.Euler(0, point1Theta, 0) * new Vector3(radius, 0, 0);

                float point2Theta = (360.0f / (float)segments) * ((i + 1) % segments);
                Vector3 point2 = Quaternion.Euler(0, point2Theta, 0) * new Vector3(radius, 0, 0);

                Gizmos.DrawLine(point1 + position, point2 + position);
                Gizmos.DrawLine(point1 + position, position);
            }
        }
    }
}

#endif
